using AppuntaMed.Application.UseCases.Appointments.Queries.GetAppointmentsList;
using AppuntaMed.Domain.Enums;
using AppuntaMed.Persistence.Seeding;
using AppuntaMed.Tests.Integration.Infrastructure;
using System.Net;

namespace AppuntaMed.Tests.Integration
{
	/// <summary>
	/// Every test books its own appointment in its own unique slot (see TestSlots) and never touches the demo ones,
	/// except to READ them. Anything that needs an actor with no permission gets a plain 403 (policy); anything
	/// decided by the use case (ownership, business rules) gets a problemjson body.
	/// </summary>
	[Collection(ApiCollection.Name)]
	[Trait("Category", "Integration")]
	public sealed class AppointmentsApiTests(ApiFactory factory) : IntegrationTestBase(factory)
	{
		private static string Url(Guid id) => $"/api/appointments/{id}";

		// ------------------------------------------------------------ create: the normal case

		[Fact]
		public async Task Create_PatientBooksForHerself_StoresItScheduled_AndSendsTheConfirmationEmail()
		{
			var slot = TestSlots.Next();

			var id = await BookOkAsync(TestUser.Ana, DemoData.AnaPatientId, DemoData.DrCarlosId, DemoData.Room102Id, slot);

			Assert.Equal(AppointmentStatus.Scheduled, await StatusOf(id));

			var confirmation = Assert.Single(Factory.Notifications.Confirmations, c => c.AppoitmentDate == slot.Start);
			Assert.Equal("ana.lopez@appuntamed.test", confirmation.Patient_Email);
			Assert.Contains("Carlos", confirmation.Doctor);
		}

		[Fact]
		public async Task Create_Admin_CanBookForAnyPatient()
		{
			var slot = TestSlots.Next();

			var id = await BookOkAsync(TestUser.Admin, DemoData.BetoPatientId, DemoData.DrCarlosId, DemoData.Room102Id, slot);

			Assert.Equal(AppointmentStatus.Scheduled, await StatusOf(id));
		}

		// ------------------------------------------------------------ create: who can book

		[Fact]
		public async Task Create_PatientCannotBookForAnotherPatient_Returns403_AndStoresNothing()
		{
			var slot = TestSlots.Next();

			var response = await BookAsync(TestUser.Ana, DemoData.BetoPatientId, DemoData.DrCarlosId, DemoData.Room102Id, slot.Start, slot.End);

			await AssertProblem(response, HttpStatusCode.Forbidden);
			Assert.Null(await AppointmentIdAt(DemoData.DrCarlosId, slot.Start));
			Assert.DoesNotContain(Factory.Notifications.Confirmations, c => c.AppoitmentDate == slot.Start);
		}

		[Fact]
		public async Task Create_Doctor_HasNoPermissionToBook()
		{
			var slot = TestSlots.Next();

			var response = await BookAsync(TestUser.DrCarlos, DemoData.AnaPatientId, DemoData.DrCarlosId, DemoData.Room102Id, slot.Start, slot.End);

			Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
		}

		[Fact]
		public async Task Create_WithoutAuthentication_Returns401()
		{
			var slot = TestSlots.Next();

			var response = await BookAsync(new TestUser("", "", "", ""), DemoData.AnaPatientId, DemoData.DrCarlosId, DemoData.Room102Id, slot.Start, slot.End);

			Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
		}

		// ------------------------------------------------------------ create: time rules and boundaries

		[Fact]
		public async Task Create_OverlappingTheSameDoctor_Returns400()
		{
			var slot = TestSlots.Next(minutes: 60);
			await BookOkAsync(TestUser.Ana, DemoData.AnaPatientId, DemoData.DrCarlosId, DemoData.Room102Id, slot);

			var response = await BookAsync(TestUser.Admin, DemoData.BetoPatientId, DemoData.DrCarlosId, DemoData.Room102Id,
				slot.Start.AddMinutes(30), slot.End.AddMinutes(30));

			var problem = await AssertProblem(response, HttpStatusCode.BadRequest);
			Assert.Contains(ErrorsOf(problem), e => e.Contains("Already exists an appointment"));
		}

		[Fact]
		public async Task Create_StartingExactlyWhenTheOtherEnds_IsAllowed()
		{
			var slot = TestSlots.Next(minutes: 60);
			await BookOkAsync(TestUser.Ana, DemoData.AnaPatientId, DemoData.DrCarlosId, DemoData.Room102Id, slot);

			var response = await BookAsync(TestUser.Admin, DemoData.BetoPatientId, DemoData.DrCarlosId, DemoData.Room102Id,
				slot.End, slot.End.AddMinutes(30));

			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		}

		[Fact]
		public async Task Create_SameTimeWithAnotherDoctor_IsAllowed()
		{
			var slot = TestSlots.Next();
			await BookOkAsync(TestUser.Ana, DemoData.AnaPatientId, DemoData.DrCarlosId, DemoData.Room102Id, slot);

			var response = await BookAsync(TestUser.Admin, DemoData.BetoPatientId, DemoData.DrLauraId, DemoData.DentalOfficeAId, slot.Start, slot.End);

			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		}

		[Fact]
		public async Task Create_ASlotFreedByACancellation_CanBeBookedAgain()
		{
			var slot = TestSlots.Next();
			var id = await BookOkAsync(TestUser.Ana, DemoData.AnaPatientId, DemoData.DrCarlosId, DemoData.Room102Id, slot);
			Assert.Equal(HttpStatusCode.OK, (await ClientFor(TestUser.Ana).PostAsync($"{Url(id)}/cancel", null)).StatusCode);

			var response = await BookAsync(TestUser.Admin, DemoData.BetoPatientId, DemoData.DrCarlosId, DemoData.Room102Id, slot.Start, slot.End);

			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		}

		[Fact]
		public async Task Create_StartInThePast_Returns400()
		{
			var start = DateTime.UtcNow.AddHours(-2);

			var response = await BookAsync(TestUser.Ana, DemoData.AnaPatientId, DemoData.DrCarlosId, DemoData.Room102Id, start, start.AddHours(1));

			var problem = await AssertProblem(response, HttpStatusCode.BadRequest);
			Assert.Contains(ErrorsOf(problem), e => e.Contains("past"));
		}

		[Fact]
		public async Task Create_EndBeforeStart_Returns400()
		{
			var slot = TestSlots.Next();

			var response = await BookAsync(TestUser.Ana, DemoData.AnaPatientId, DemoData.DrCarlosId, DemoData.Room102Id, slot.End, slot.Start);

			await AssertProblem(response, HttpStatusCode.BadRequest);
			Assert.Null(await AppointmentIdAt(DemoData.DrCarlosId, slot.End));
		}

		[Fact]
		public async Task Create_ZeroLengthAppointment_Returns400()
		{
			var slot = TestSlots.Next();

			var response = await BookAsync(TestUser.Ana, DemoData.AnaPatientId, DemoData.DrCarlosId, DemoData.Room102Id, slot.Start, slot.Start);

			await AssertProblem(response, HttpStatusCode.BadRequest);
		}

		// ------------------------------------------------------------ read one

		[Fact]
		public async Task Get_Patient_ReadsHerOwnAppointment()
		{
			var response = await ClientFor(TestUser.Ana).GetAsync(Url(DemoData.Appointment1Id));

			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		}

		[Fact]
		public async Task Get_Doctor_ReadsTheirOwnAppointment()
		{
			var response = await ClientFor(TestUser.DrCarlos).GetAsync(Url(DemoData.Appointment1Id));

			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		}

		[Fact]
		public async Task Get_Admin_ReadsAnyAppointment()
		{
			var response = await ClientFor(TestUser.Admin).GetAsync(Url(DemoData.Appointment1Id));

			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		}

		[Theory]
		[InlineData("beto")]
		[InlineData("dr.laura")]
		public async Task Get_SomeoneElsesAppointment_Returns403Problem(string who)
		{
			var user = who == "beto" ? TestUser.Beto : TestUser.DrLaura;

			var response = await ClientFor(user).GetAsync(Url(DemoData.Appointment1Id));

			await AssertProblem(response, HttpStatusCode.Forbidden);
		}

		[Fact]
		public async Task Get_UnknownAppointment_Returns404()
		{
			var response = await ClientFor(TestUser.Admin).GetAsync(Url(Guid.NewGuid()));

			await AssertProblem(response, HttpStatusCode.NotFound);
		}

		// ------------------------------------------------------------ cancel

		[Fact]
		public async Task Cancel_Patient_CancelsHerOwnAppointment()
		{
			var id = await BookOkAsync(TestUser.Ana, DemoData.AnaPatientId, DemoData.DrCarlosId, DemoData.Room102Id, TestSlots.Next());

			var response = await ClientFor(TestUser.Ana).PostAsync($"{Url(id)}/cancel", null);

			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			Assert.Equal(AppointmentStatus.Cancelled, await StatusOf(id));
		}

		[Fact]
		public async Task Cancel_Patient_CannotCancelAnotherPatientsAppointment_Returns403_AndKeepsItScheduled()
		{
			var id = await BookOkAsync(TestUser.Ana, DemoData.AnaPatientId, DemoData.DrCarlosId, DemoData.Room102Id, TestSlots.Next());

			var response = await ClientFor(TestUser.Beto).PostAsync($"{Url(id)}/cancel", null);

			await AssertProblem(response, HttpStatusCode.Forbidden);
			Assert.Equal(AppointmentStatus.Scheduled, await StatusOf(id));
		}

		[Fact]
		public async Task Cancel_Doctor_CannotCancelAnotherDoctorsAppointment()
		{
			var id = await BookOkAsync(TestUser.Ana, DemoData.AnaPatientId, DemoData.DrCarlosId, DemoData.Room102Id, TestSlots.Next());

			var response = await ClientFor(TestUser.DrLaura).PostAsync($"{Url(id)}/cancel", null);

			await AssertProblem(response, HttpStatusCode.Forbidden);
			Assert.Equal(AppointmentStatus.Scheduled, await StatusOf(id));
		}

		[Fact]
		public async Task Cancel_Admin_CancelsAnyAppointment()
		{
			var id = await BookOkAsync(TestUser.Ana, DemoData.AnaPatientId, DemoData.DrCarlosId, DemoData.Room102Id, TestSlots.Next());

			var response = await ClientFor(TestUser.Admin).PostAsync($"{Url(id)}/cancel", null);

			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			Assert.Equal(AppointmentStatus.Cancelled, await StatusOf(id));
		}

		[Fact]
		public async Task Cancel_AnAlreadyCancelledAppointment_Returns400()
		{
			var id = await BookOkAsync(TestUser.Ana, DemoData.AnaPatientId, DemoData.DrCarlosId, DemoData.Room102Id, TestSlots.Next());
			await ClientFor(TestUser.Ana).PostAsync($"{Url(id)}/cancel", null);

			var response = await ClientFor(TestUser.Ana).PostAsync($"{Url(id)}/cancel", null);

			await AssertProblem(response, HttpStatusCode.BadRequest);
		}

		[Fact]
		public async Task Cancel_UnknownAppointment_Returns404()
		{
			var response = await ClientFor(TestUser.Admin).PostAsync($"{Url(Guid.NewGuid())}/cancel", null);

			await AssertProblem(response, HttpStatusCode.NotFound);
		}

		[Fact]
		public async Task Cancel_WithoutAuthentication_Returns401()
		{
			var response = await Anonymous().PostAsync($"{Url(DemoData.Appointment1Id)}/cancel", null);

			Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
		}

		// ------------------------------------------------------------ complete / no-show

		[Fact]
		public async Task Complete_Doctor_CompletesTheirOwnAppointment()
		{
			var id = await BookOkAsync(TestUser.Ana, DemoData.AnaPatientId, DemoData.DrCarlosId, DemoData.Room102Id, TestSlots.Next());

			var response = await ClientFor(TestUser.DrCarlos).PostAsync($"{Url(id)}/complete", null);

			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			Assert.Equal(AppointmentStatus.Completed, await StatusOf(id));
		}

		[Fact]
		public async Task Complete_Doctor_CannotCompleteAnotherDoctorsAppointment()
		{
			var id = await BookOkAsync(TestUser.Ana, DemoData.AnaPatientId, DemoData.DrCarlosId, DemoData.Room102Id, TestSlots.Next());

			var response = await ClientFor(TestUser.DrLaura).PostAsync($"{Url(id)}/complete", null);

			await AssertProblem(response, HttpStatusCode.Forbidden);
			Assert.Equal(AppointmentStatus.Scheduled, await StatusOf(id));
		}

		[Fact]
		public async Task Complete_Admin_CompletesWithoutHavingADoctorProfile()
		{
			var id = await BookOkAsync(TestUser.Ana, DemoData.AnaPatientId, DemoData.DrCarlosId, DemoData.Room102Id, TestSlots.Next());

			var response = await ClientFor(TestUser.Admin).PostAsync($"{Url(id)}/complete", null);

			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			Assert.Equal(AppointmentStatus.Completed, await StatusOf(id));
		}

		[Fact]
		public async Task Complete_Patient_HasNoPermission()
		{
			var id = await BookOkAsync(TestUser.Ana, DemoData.AnaPatientId, DemoData.DrCarlosId, DemoData.Room102Id, TestSlots.Next());

			var response = await ClientFor(TestUser.Ana).PostAsync($"{Url(id)}/complete", null);

			Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
			Assert.Equal(AppointmentStatus.Scheduled, await StatusOf(id));
		}

		[Fact]
		public async Task Complete_ACancelledAppointment_Returns400()
		{
			var id = await BookOkAsync(TestUser.Ana, DemoData.AnaPatientId, DemoData.DrCarlosId, DemoData.Room102Id, TestSlots.Next());
			await ClientFor(TestUser.Ana).PostAsync($"{Url(id)}/cancel", null);

			var response = await ClientFor(TestUser.DrCarlos).PostAsync($"{Url(id)}/complete", null);

			await AssertProblem(response, HttpStatusCode.BadRequest);
			Assert.Equal(AppointmentStatus.Cancelled, await StatusOf(id));
		}

		[Fact]
		public async Task NoShow_Doctor_MarksTheirOwnAppointment()
		{
			var id = await BookOkAsync(TestUser.Ana, DemoData.AnaPatientId, DemoData.DrCarlosId, DemoData.Room102Id, TestSlots.Next());

			var response = await ClientFor(TestUser.DrCarlos).PostAsync($"{Url(id)}/noshow", null);

			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			Assert.Equal(AppointmentStatus.NoShow, await StatusOf(id));
		}

		[Fact]
		public async Task NoShow_AnotherDoctor_Returns403()
		{
			var id = await BookOkAsync(TestUser.Ana, DemoData.AnaPatientId, DemoData.DrCarlosId, DemoData.Room102Id, TestSlots.Next());

			var response = await ClientFor(TestUser.DrLaura).PostAsync($"{Url(id)}/noshow", null);

			await AssertProblem(response, HttpStatusCode.Forbidden);
			Assert.Equal(AppointmentStatus.Scheduled, await StatusOf(id));
		}

		// ------------------------------------------------------------ list

		[Fact]
		public async Task List_Doctor_SeesTheirOwnAppointmentsInTheRange()
		{
			var slot = TestSlots.Next();
			var id = await BookOkAsync(TestUser.Ana, DemoData.AnaPatientId, DemoData.DrCarlosId, DemoData.Room102Id, slot);

			var response = await ClientFor(TestUser.DrCarlos).GetAsync(
				$"/api/appointments?doctorId={DemoData.DrCarlosId}&start={TestSlots.Query(slot.Start)}&end={TestSlots.Query(slot.End)}");

			Assert.Equal(HttpStatusCode.OK, response.StatusCode);

			var appointment = Assert.Single(await ReadAs<List<AppointmentsListDTO>>(response));
			Assert.Equal(id, appointment.Id);
		}

		[Fact]
		public async Task List_RangeEndingBeforeTheAppointmentEnds_DoesNotIncludeIt()
		{
			var slot = TestSlots.Next(minutes: 60);
			await BookOkAsync(TestUser.Ana, DemoData.AnaPatientId, DemoData.DrCarlosId, DemoData.Room102Id, slot);

			var response = await ClientFor(TestUser.DrCarlos).GetAsync(
				$"/api/appointments?doctorId={DemoData.DrCarlosId}&start={TestSlots.Query(slot.Start)}&end={TestSlots.Query(slot.End.AddMinutes(-1))}");

			Assert.Empty(await ReadAs<List<AppointmentsListDTO>>(response));
		}

		[Fact]
		public async Task List_Doctor_CannotListAnotherDoctorsAppointments_Returns403()
		{
			var slot = TestSlots.Next();

			var response = await ClientFor(TestUser.DrCarlos).GetAsync(
				$"/api/appointments?doctorId={DemoData.DrLauraId}&start={TestSlots.Query(slot.Start)}&end={TestSlots.Query(slot.End)}");

			await AssertProblem(response, HttpStatusCode.Forbidden);
		}

		[Fact]
		public async Task List_DoctorWithoutAskingForTheirOwnId_Returns403()
		{
			var slot = TestSlots.Next();

			var response = await ClientFor(TestUser.DrCarlos).GetAsync(
				$"/api/appointments?start={TestSlots.Query(slot.Start)}&end={TestSlots.Query(slot.End)}");

			await AssertProblem(response, HttpStatusCode.Forbidden);
		}

		[Fact]
		public async Task List_Patient_HasNoPermissionToListAppointments()
		{
			var response = await ClientFor(TestUser.Ana).GetAsync("/api/appointments?start=2030-01-01T00:00:00&end=2030-01-02T00:00:00");

			Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
		}

		[Theory]
		[InlineData("")]
		[InlineData("?start=2030-01-02T00:00:00&end=2030-01-02T00:00:00")]
		[InlineData("?start=2030-01-03T00:00:00&end=2030-01-02T00:00:00")]
		public async Task List_WithoutAValidRange_Returns400(string query)
		{
			var response = await ClientFor(TestUser.Admin).GetAsync($"/api/appointments{query}");

			var problem = await AssertProblem(response, HttpStatusCode.BadRequest);
			Assert.Contains(ErrorsOf(problem), e => e.Contains("End must be after Start"));
		}

		[Fact]
		public async Task List_Admin_FiltersByStatus()
		{
			var response = await ClientFor(TestUser.Admin).GetAsync(
				$"/api/appointments?status=Cancelled&consultingRoomId={DemoData.Room102Id}&start=2000-01-01T00:00:00&end=2100-01-01T00:00:00");

			Assert.Equal(HttpStatusCode.OK, response.StatusCode);

			var appointments = await ReadAs<List<AppointmentsListDTO>>(response);
			Assert.Contains(appointments, a => a.Id == DemoData.Appointment5Id);
			Assert.All(appointments, a => Assert.Equal("Cancelled", a.Status));
		}
	}
}