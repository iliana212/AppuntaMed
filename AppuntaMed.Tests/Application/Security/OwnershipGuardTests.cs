using AppuntaMed.Application.Exceptions;
using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.Interfaces.Security;
using AppuntaMed.Application.Security;
using AppuntaMed.Domain.Entities;
using AppuntaMed.Tests.Application.UseCases;
using NSubstitute;

namespace AppuntaMed.Tests.Application.Security
{
	public class OwnershipGuardTests
	{
		private readonly ICurrentUserService _user = Substitute.For<ICurrentUserService>();
		private readonly IDoctorRepository _doctors = Substitute.For<IDoctorRepository>();
		private readonly IPatientRepository _patients = Substitute.For<IPatientRepository>();

		private void Login(string userId, params string[] roles)
		{
			_user.UserId.Returns(userId);
			_user.Roles.Returns(roles);
		}

		[Fact]
		public void EnsureOwnsPatientProfile_PatientIsOwner_DoesNotThrow()
		{
			Login("user-ana", "Patient");
			var exception = Record.Exception(() => _user.EnsureOwnsPatientProfile(TestData.Patient("user-ana")));
			Assert.Null(exception);
		}

		[Fact]
        public void EnsureOwnsPatientProfile_PatientIsNotOwner_ThrowsForbidden()
        {
			Login("user-other", "Patient");	
			Assert.Throws<ForbiddenException>(() => _user.EnsureOwnsPatientProfile(TestData.Patient("user-ana")));
	    }

		[Fact]
		public void EnsureOwnsPatientProfile_PatientWithoutUserId_ThrowsForbidden()
		{
			_user.UserId.Returns((string?)null);
			_user.Roles.Returns(new[] { "Patient" });
	
			Assert.Throws<ForbiddenException>(() => _user.EnsureOwnsPatientProfile(TestData.Patient("user-ana")));
	    }

		[Theory]
		[InlineData("Admin")]
		[InlineData("Doctor")]
        public void EnsureOwnsPatientProfile_NonPatientRoles_AreNotRestricted(string role)
        {
			Login("someone-else", role);	
			var exception = Record.Exception(() => _user.EnsureOwnsPatientProfile(TestData.Patient("user-ana")));
			Assert.Null(exception);
	    }

		[Fact]
        public void EnsureOwnsDoctorProfile_DoctorIsOwner_DoesNotThrow()
        {
			Login("user-carlos", "Doctor");	
			var exception = Record.Exception(() => _user.EnsureOwnsDoctorProfile(TestData.Doctor("user-carlos")));
			Assert.Null(exception);
	    }

		[Fact]
        public void EnsureOwnsDoctorProfile_DoctorIsNotOwner_ThrowsForbidden()
        {
			Login("user-other", "Doctor");	
			Assert.Throws<ForbiddenException>(() => _user.EnsureOwnsDoctorProfile(TestData.Doctor("user-carlos")));
	    }

		[Fact]
        public void EnsureOwnsDoctorProfile_Admin_IsNotRestricted()
        {
			Login("admin-user", "Admin");	
			var exception = Record.Exception(() => _user.EnsureOwnsDoctorProfile(TestData.Doctor("user-carlos")));
			Assert.Null(exception);
	    }

		[Fact]
        public async Task EnsureDoctorCanAccess_OwnAppointment_DoesNotThrow()
        {
			var doctor = TestData.Doctor();
			var appointment = TestData.Appointment(TestData.Patient(), doctor);
			Login("user-carlos", "Doctor");
			_doctors.GetByUserId("user-carlos").Returns(Task.FromResult<Doctor?>(doctor));
	
			await _user.EnsureDoctorCanAccess(appointment, _doctors);
	    }

		[Fact]
        public async Task EnsureDoctorCanAccess_AppointmentOfAnotherDoctor_ThrowsForbidden()
        {
			var appointment = TestData.Appointment(TestData.Patient(), TestData.Doctor("user-carlos"));
			var loggedDoctor = TestData.Doctor("user-laura");
			Login("user-laura", "Doctor");
			_doctors.GetByUserId("user-laura").Returns(Task.FromResult<Doctor?>(loggedDoctor));
	
			await Assert.ThrowsAsync<ForbiddenException>(() => _user.EnsureDoctorCanAccess(appointment, _doctors));
	    }

		[Fact]
        public async Task EnsureDoctorCanAccess_DoctorWithoutProfile_ThrowsForbiddenInsteadOfNullReference()
        {
			var appointment = TestData.Appointment(TestData.Patient(), TestData.Doctor());
			Login("user-no-profile", "Doctor");
			_doctors.GetByUserId("user-no-profile").Returns(Task.FromResult<Doctor?>(null));
	
			await Assert.ThrowsAsync<ForbiddenException>(() => _user.EnsureDoctorCanAccess(appointment, _doctors));
	    }

		[Fact]
        public async Task EnsureDoctorCanAccess_Admin_DoesNotNeedDoctorProfile()
        {
			var appointment = TestData.Appointment(TestData.Patient(), TestData.Doctor());
			Login("admin-user", "Admin");
	
			await _user.EnsureDoctorCanAccess(appointment, _doctors);
	
			await _doctors.DidNotReceive().GetByUserId(Arg.Any<string>());
	    }

		[Fact]
        public async Task EnsurePatientCanAccess_OwnAppointment_DoesNotThrow()
        {
			var patient = TestData.Patient();
			var appointment = TestData.Appointment(patient, TestData.Doctor());
			Login("user-ana", "Patient");
			_patients.GetByUserId("user-ana").Returns(Task.FromResult<Patient?>(patient));
	
			await _user.EnsurePatientCanAccess(appointment, _patients);
	    }

		[Fact]
        public async Task EnsurePatientCanAccess_AppointmentOfAnotherPatient_ThrowsForbidden()
        {
			var appointment = TestData.Appointment(TestData.Patient("user-ana"), TestData.Doctor());
			var loggedPatient = TestData.Patient("user-beto");
			Login("user-beto", "Patient");
			_patients.GetByUserId("user-beto").Returns(Task.FromResult<Patient?>(loggedPatient));
	
			await Assert.ThrowsAsync<ForbiddenException>(() => _user.EnsurePatientCanAccess(appointment, _patients));
	    }

		[Fact]
        public async Task EnsureActsAsPatient_PatientBooksForSelf_DoesNotThrow()
        {
			var patient = TestData.Patient();
			Login("user-ana", "Patient");
			_patients.GetByUserId("user-ana").Returns(Task.FromResult<Patient?>(patient));
	
			await _user.EnsureActsAsPatient(patient.Id, _patients);
	    }

		[Fact]
        public async Task EnsureActsAsPatient_PatientBooksForSomeoneElse_ThrowsForbidden()
        {
			var patient = TestData.Patient();
			Login("user-ana", "Patient");
			_patients.GetByUserId("user-ana").Returns(Task.FromResult<Patient?>(patient));
	
			await Assert.ThrowsAsync<ForbiddenException>(() => _user.EnsureActsAsPatient(Guid.NewGuid(), _patients));
	    }

		[Fact]
        public async Task EnsureActsAsPatient_Admin_CanBookForAnyPatient()
        {
			Login("admin-user", "Admin");
	
			await _user.EnsureActsAsPatient(Guid.NewGuid(), _patients);
			await _patients.DidNotReceive().GetByUserId(Arg.Any<string>());
	    }
    }
}