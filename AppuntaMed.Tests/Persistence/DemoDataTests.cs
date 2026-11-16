using AppuntaMed.Domain.Enums;
using AppuntaMed.Persistence.Seeding;

namespace AppuntaMed.Tests.Persistence
{
	public class DemoDataTests
	{
		public static TheoryData<DateTime> MomentsOfTheDay()
		{
			var today = DateTime.UtcNow.Date;

			return new TheoryData<DateTime>
			{
				DateTime.UtcNow,
				today,                                   // 00:00
				today.AddDays(1).AddTicks(-1),           // 23:59:59.9999999
			};
		}

       private static DemoDataPlan Plan() => DemoData.Build(DateTime.UtcNow);

		[Fact]
        public void Build_ProducesTheExpectedAmountOfData()
        {
			var plan = Plan();
	
			Assert.Equal(3, plan.ConsultingRooms.Count);
			Assert.Equal(5, plan.Patients.Count);
			Assert.Equal(5, plan.Doctors.Count);
			Assert.Equal(12, plan.Appointments.Count);
	    }

		[Fact]
        public void FixedIds_AreUniqueAndNotEmpty()
        {
			var plan = Plan();
	
			var ids = plan.ConsultingRooms.Select(x => x.Id)
				.Concat(plan.Patients.Select(x => x.Id))
	            .Concat(plan.Doctors.Select(x => x.Id))
	            .Concat(plan.Appointments.Select(x => x.Id))
	            .ToList();
	
			Assert.DoesNotContain(Guid.Empty, ids);
			Assert.Equal(ids.Count, ids.Distinct().Count());
	    }

		[Fact]
        public void FixedIds_AreStableBetweenBuilds()
        {
			var first = Plan().Appointments.Select(x => x.Id).ToList();
			var second = Plan().Appointments.Select(x => x.Id).ToList();
	
			Assert.Equal(first, second);
	    }

		[Fact]
        public void KeycloakUserIds_AreDistinctValidGuids()
        {
			var userIds = new[]
	        {
				DemoData.AdminUserId, DemoData.DrCarlosUserId, DemoData.DrLauraUserId,
                DemoData.AnaUserId, DemoData.BetoUserId
            };
	
			Assert.All(userIds, id => Assert.True(Guid.TryParse(id, out _), $"'{id}' is not a valid Keycloak user id"));
			Assert.Equal(userIds.Length, userIds.Distinct().Count());
	    }

		[Fact]
        public void Doctors_AndPatients_AreLinkedToTheirKeycloakUser()
        {
			var plan = Plan();
	
			Assert.Equal(DemoData.DrCarlosUserId, plan.Doctors.Single(d => d.Id == DemoData.DrCarlosId).Entity.UserId);
			Assert.Equal(DemoData.DrLauraUserId, plan.Doctors.Single(d => d.Id == DemoData.DrLauraId).Entity.UserId);
			Assert.Equal(DemoData.AnaUserId, plan.Patients.Single(p => p.Id == DemoData.AnaPatientId).Entity.UserId);
			Assert.Equal(DemoData.BetoUserId, plan.Patients.Single(p => p.Id == DemoData.BetoPatientId).Entity.UserId);
	    }

		[Fact]
        public void Doctors_UseAnExistingProfession_AndASpecialityOfThatProfession()
        {
			foreach (var seed in Plan().Doctors)
		    {
				var doctor = seed.Entity;
		
				Assert.Contains(CatalogData.Professions, p => p.Id == doctor.ProfessionId);
		
		        if (doctor.SpecialityId is { } specialityId)
					Assert.Contains(CatalogData.Specialities, s => s.Id == specialityId && s.ProfessionId == doctor.ProfessionId);
		    }
	    }

		[Fact]
        public void Appointments_ReferenceSeededDoctorsPatientsAndRooms()
        {
			var plan = Plan();
			var doctorIds = plan.Doctors.Select(x => x.Id).ToHashSet();
			var patientIds = plan.Patients.Select(x => x.Id).ToHashSet();
			var roomIds = plan.ConsultingRooms.Select(x => x.Id).ToHashSet();
	
	        foreach (var seed in plan.Appointments)
		    {
				Assert.Contains(seed.Entity.DoctorId, doctorIds);
				Assert.Contains(seed.Entity.PatientId, patientIds);
				Assert.Contains(seed.Entity.ConsultingRoomId, roomIds);
		    }
	    }

		[Theory]
		[MemberData(nameof(MomentsOfTheDay))]
        public void Appointments_AreAlwaysInTheFuture_WhateverTheTimeOfDay(DateTime now)
        {
			var plan = DemoData.Build(now);
	
			Assert.All(plan.Appointments, a => Assert.True(a.Entity.TimeInterval.Start > now));
	    }

		[Fact]
        public void ScheduledAppointments_NeverOverlap_ForTheSameDoctorOrRoom()
        {
			var scheduled = Plan().Appointments
				.Select(x => x.Entity)
	            .Where(a => a.Status == AppointmentStatus.Scheduled)
	            .ToList();
	
			var groups = scheduled.GroupBy(a => a.DoctorId).Select(g => g.ToList())
	            .Concat(scheduled.GroupBy(a => a.ConsultingRoomId).Select(g => g.ToList()));
	
			foreach (var group in groups)
		    {
				for (var i = 0; i < group.Count; i++)
			    {
					for (var j = i + 1; j < group.Count; j++)
				    {
						var overlap = group[i].TimeInterval.Start < group[j].TimeInterval.End
				                        && group[i].TimeInterval.End > group[j].TimeInterval.Start;
				
						Assert.False(overlap, "Demo appointments must not overlap for a doctor or a room");
				    }
			    }
		    }
	    }

		[Fact]
        public void Appointments_CoverScheduledAndCancelledStatuses()
        {
			var statuses = Plan().Appointments.Select(a => a.Entity.Status).ToList();
	
			Assert.Equal(11, statuses.Count(s => s == AppointmentStatus.Scheduled));
			Assert.Single(statuses, s => s == AppointmentStatus.Cancelled);
	    }

		[Fact]
        public void Emails_UseTheReservedTestDomain_SoReminderEmailsNeverReachRealPeople()
        {
			var plan = Plan();
	
			var emails = plan.Doctors.Select(d => d.Entity.Email.Value)
				.Concat(plan.Patients.Select(p => p.Entity.Email.Value));
	
			Assert.All(emails, email => Assert.EndsWith("@appuntamed.test", email));
	    }
	}
}
