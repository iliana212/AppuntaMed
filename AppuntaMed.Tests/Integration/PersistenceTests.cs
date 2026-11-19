using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.Interfaces.Repositories.Models;
using AppuntaMed.Domain.Entities;
using AppuntaMed.Domain.Enums;
using AppuntaMed.Domain.ValueObjects;
using AppuntaMed.Persistence.Seeding;
using AppuntaMed.Tests.Integration.Infrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AppuntaMed.Tests.Integration
{
	/// <summary>
	/// What only a REAL SQL Server can prove: how the queries translate, what the foreign keys forbid
	/// and that the seeder can run again and again.
	/// </summary>
	[Collection(ApiCollection.Name)]
	[Trait("Category", "Integration")]
	public sealed class PersistenceTests(ApiFactory factory) : IntegrationTestBase(factory)
	{
		// ------------------------------------------------------------ helpers

		private Task<Guid> ArrangeDoctorAsync() => InDb(async db =>
		{
			var doctor = new Doctor("Repo", "Doctor", CatalogData.PsychologistId, null,
				new Email($"{Guid.NewGuid():N}@appuntamed.test"), new PhoneNumber("5551111"), Guid.NewGuid().ToString());

			db.Doctors.Add(doctor);
			await db.SaveChangesAsync();

			return doctor.Id;
		});

		private Task<Guid> ArrangeAppointmentAsync(Guid doctorId, DateTime start, DateTime end, bool cancelled = false) => InDb(async db =>
		{
			var appointment = new Appointment(DemoData.AnaPatientId, doctorId, DemoData.Room101Id, new TimeInterval(start, end));

			if (cancelled)
				appointment.CancelAppointment();

			db.Appointments.Add(appointment);
			await db.SaveChangesAsync();

			return appointment.Id;
		});

		private async Task<T> WithRepository<T>(Func<IAppointmentRepository, Task<T>> action)
		{
			await using var scope = Factory.Services.CreateAsyncScope();

			return await action(scope.ServiceProvider.GetRequiredService<IAppointmentRepository>());
		}

		// ------------------------------------------------------------ DuplicateAppointmentTime (overlap rules)

		[Theory]
		[InlineData(0, 60, true)]       // exactly the same interval
		[InlineData(30, 90, true)]      // overlaps the end
		[InlineData(-30, 30, true)]     // overlaps the start
		[InlineData(10, 50, true)]      // inside it
		[InlineData(-10, 70, true)]     // contains it
		[InlineData(59, 61, true)]      // one minute of overlap at the end
		[InlineData(-1, 1, true)]       // one minute of overlap at the start
		[InlineData(60, 120, false)]    // starts exactly when it ends
		[InlineData(-60, 0, false)]     // ends exactly when it starts
		[InlineData(61, 120, false)]    // later
		[InlineData(-120, -61, false)]  // earlier
		public async Task DuplicateAppointmentTime_FollowsTheOverlapRules(int fromMinute, int toMinute, bool expected)
		{
			var doctorId = await ArrangeDoctorAsync();
			var slot = TestSlots.Next(minutes: 60);
			await ArrangeAppointmentAsync(doctorId, slot.Start, slot.End);

			var duplicate = await WithRepository(r => r.DuplicateAppointmentTime(doctorId, slot.Start.AddMinutes(fromMinute), slot.Start.AddMinutes(toMinute)));

			Assert.Equal(expected, duplicate);
		}

		[Fact]
		public async Task DuplicateAppointmentTime_OnlyConsidersTheSameDoctor()
		{
			var busyDoctor = await ArrangeDoctorAsync();
			var otherDoctor = await ArrangeDoctorAsync();
			var slot = TestSlots.Next(minutes: 60);
			await ArrangeAppointmentAsync(busyDoctor, slot.Start, slot.End);

			var duplicate = await WithRepository(r => r.DuplicateAppointmentTime(otherDoctor, slot.Start, slot.End));

			Assert.False(duplicate);
		}

		[Fact]
		public async Task DuplicateAppointmentTime_IgnoresCancelledAppointments()
		{
			var doctorId = await ArrangeDoctorAsync();
			var slot = TestSlots.Next(minutes: 60);
			await ArrangeAppointmentAsync(doctorId, slot.Start, slot.End, cancelled: true);

			var duplicate = await WithRepository(r => r.DuplicateAppointmentTime(doctorId, slot.Start, slot.End));

			Assert.False(duplicate);
		}

		// ------------------------------------------------------------ GetFilterData

		[Fact]
		public async Task GetFilterData_ReturnsTheDoctorsAppointmentsInTheRange_SortedByStart()
		{
			var doctorId = await ArrangeDoctorAsync();
			var third = TestSlots.Next();
			var first = TestSlots.Next();
			var second = TestSlots.Next();

			// Inserted out of order on purpose.
			var ordered = new[] { first, second, third }.OrderBy(s => s.Start).ToArray();
			foreach (var slot in new[] { ordered[2], ordered[0], ordered[1] })
				await ArrangeAppointmentAsync(doctorId, slot.Start, slot.End);

			var result = await WithRepository(r => r.GetFilterData(new AppointmentsFilterDTO
			{
				DoctorId = doctorId,
				Start = ordered[0].Start,
				End = ordered[2].End
			}));

			Assert.Equal(ordered.Select(s => s.Start), result.Select(a => a.TimeInterval.Start));
		}

		[Fact]
		public async Task GetFilterData_ExcludesAppointmentsOutsideTheRange_AndFiltersByStatus()
		{
			var doctorId = await ArrangeDoctorAsync();
			var inside = TestSlots.Next();
			var outside = TestSlots.Next();
			var cancelled = TestSlots.Next();
			await ArrangeAppointmentAsync(doctorId, inside.Start, inside.End);
			await ArrangeAppointmentAsync(doctorId, outside.Start, outside.End);
			await ArrangeAppointmentAsync(doctorId, cancelled.Start, cancelled.End, cancelled: true);

			var scheduledOnly = await WithRepository(r => r.GetFilterData(new AppointmentsFilterDTO
			{
				DoctorId = doctorId,
				Status = AppointmentStatus.Scheduled,
				Start = inside.Start,
				End = inside.End            // inclusive on both ends
			}));

			var cancelledOnly = await WithRepository(r => r.GetFilterData(new AppointmentsFilterDTO
			{
				DoctorId = doctorId,
				Status = AppointmentStatus.Cancelled,
				Start = inside.Start,
				End = cancelled.End
			}));

			Assert.Equal(inside.Start, Assert.Single(scheduledOnly).TimeInterval.Start);
			Assert.Equal(cancelled.Start, Assert.Single(cancelledOnly).TimeInterval.Start);
		}

		[Fact]
		public async Task AnyForDoctor_AnyForPatient_AnyForConsultingRoom_ReflectTheStoredAppointments()
		{
			var doctorWithout = await ArrangeDoctorAsync();
			var slot = TestSlots.Next();
			var doctorWith = await ArrangeDoctorAsync();
			await ArrangeAppointmentAsync(doctorWith, slot.Start, slot.End);

			Assert.True(await WithRepository(r => r.AnyForDoctor(doctorWith)));
			Assert.False(await WithRepository(r => r.AnyForDoctor(doctorWithout)));
			Assert.True(await WithRepository(r => r.AnyForPatient(DemoData.AnaPatientId)));
			Assert.True(await WithRepository(r => r.AnyForConsultingRoom(DemoData.Room101Id)));
			Assert.False(await WithRepository(r => r.AnyForPatient(Guid.NewGuid())));
		}

		// ------------------------------------------------------------ constraints the database enforces by itself

		[Fact]
		public async Task Database_RefusesToDeleteADoctorWithAppointments_NoCascade()
		{
			await Assert.ThrowsAsync<DbUpdateException>(() => InDb(async db =>
			{
				var carlos = await db.Doctors.SingleAsync(d => d.Id == DemoData.DrCarlosId);
				db.Doctors.Remove(carlos);

				return await db.SaveChangesAsync();
			}));

			Assert.True(await InDb(db => db.Appointments.AnyAsync(a => a.Id == DemoData.Appointment1Id)));
		}

		[Fact]
		public async Task Database_RefusesToDeleteAPatientWithAppointments_NoCascade()
		{
			await Assert.ThrowsAsync<DbUpdateException>(() => InDb(async db =>
			{
				db.Patients.Remove(await db.Patients.SingleAsync(p => p.Id == DemoData.AnaPatientId));

				return await db.SaveChangesAsync();
			}));
		}

		[Fact]
		public async Task Database_RefusesToDeleteAConsultingRoomWithAppointments_NoCascade()
		{
			await Assert.ThrowsAsync<DbUpdateException>(() => InDb(async db =>
			{
				db.ConsultingRooms.Remove(await db.ConsultingRooms.SingleAsync(r => r.Id == DemoData.Room101Id));

				return await db.SaveChangesAsync();
			}));
		}

		[Fact]
		public async Task Database_RefusesToDeleteAProfessionInUse()
		{
			await Assert.ThrowsAsync<DbUpdateException>(() => InDb(async db =>
			{
				db.Professions.Remove(await db.Professions.SingleAsync(p => p.Id == CatalogData.PhysicianId));

				return await db.SaveChangesAsync();
			}));
		}

		[Fact]
		public async Task Database_RefusesASpecialityThatBelongsToAnotherProfession_CompositeForeignKey()
		{
			// Carlos is a Physician; Orthodontics is a Dentist speciality. Bypassing the application with raw SQL
			// must still fail: this is what the composite foreign key (ProfessionId, SpecialityId) guarantees.
			await Assert.ThrowsAsync<SqlException>(() => InDb(db => db.Database.ExecuteSqlInterpolatedAsync(
				$"UPDATE Doctors SET SpecialityId = {CatalogData.OrthodonticsId} WHERE Id = {DemoData.DrCarlosId}")));

			var speciality = await InDb(db => db.Doctors.Where(d => d.Id == DemoData.DrCarlosId).Select(d => d.SpecialityId).SingleAsync());
			Assert.Equal(CatalogData.CardiologyId, speciality);
		}

		[Fact]
		public async Task Database_AcceptsADoctorWithoutSpeciality_NullSkipsTheCompositeForeignKey()
		{
			var doctorId = await ArrangeDoctorAsync();   // Psychologist, no speciality

			var speciality = await InDb(db => db.Doctors.Where(d => d.Id == doctorId).Select(d => d.SpecialityId).SingleAsync());

			Assert.Null(speciality);
		}

		[Fact]
		public async Task Database_RefusesADuplicatedProfessionName_UniqueIndex()
		{
			await Assert.ThrowsAsync<DbUpdateException>(() => InDb(async db =>
			{
				db.Professions.Add(new Profession("Dentist"));

				return await db.SaveChangesAsync();
			}));
		}

		[Fact]
		public async Task Database_RefusesTwoProfilesForTheSameAccount_UniqueUserId()
		{
			await Assert.ThrowsAsync<DbUpdateException>(() => InDb(async db =>
			{
				db.Doctors.Add(new Doctor("Dup", "Account", CatalogData.PsychologistId, null,
					new Email($"{Guid.NewGuid():N}@appuntamed.test"), new PhoneNumber("5551111"), DemoData.DrCarlosUserId));

				return await db.SaveChangesAsync();
			}));
		}

		// ------------------------------------------------------------ the seeder can run again and again

		[Fact]
		public async Task Seeder_RunTwice_DoesNotDuplicateAnything()
		{
			var before = await CountSeededRowsAsync();

			await RunSeederAsync();
			await RunSeederAsync();

			Assert.Equal(before, await CountSeededRowsAsync());
		}

		[Fact]
		public async Task Seeder_RestoresADemoAppointmentThatWasDeleted_WithoutTouchingTheOthers()
		{
			await InDb(db => db.Appointments.Where(a => a.Id == DemoData.Appointment4Id).ExecuteDeleteAsync());
			Assert.False(await InDb(db => db.Appointments.AnyAsync(a => a.Id == DemoData.Appointment4Id)));
			var others = await InDb(db => db.Appointments.CountAsync());

			await RunSeederAsync();

			Assert.True(await InDb(db => db.Appointments.AnyAsync(a => a.Id == DemoData.Appointment4Id)));
			Assert.Equal(others + 1, await InDb(db => db.Appointments.CountAsync()));
		}

		private Task RunSeederAsync() => InDb(async db =>
		{
			await DemoDataSeeder.SeedAsync(db, DateTime.UtcNow, CancellationToken.None);

			return 0;
		});

		private Task<(int Rooms, int Patients, int Doctors, int Appointments)> CountSeededRowsAsync() => InDb(async db => (
			await db.ConsultingRooms.CountAsync(),
			await db.Patients.CountAsync(),
			await db.Doctors.CountAsync(),
			await db.Appointments.CountAsync()));
	}
}