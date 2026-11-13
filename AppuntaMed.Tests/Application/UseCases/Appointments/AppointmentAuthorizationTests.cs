using AppuntaMed.Application.Exceptions;
using AppuntaMed.Application.Interfaces.DataStore;
using AppuntaMed.Application.Interfaces.Notifications;
using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.Interfaces.Security;
using AppuntaMed.Application.UseCases.Appointments.Commands.CancelAppointment;
using AppuntaMed.Application.UseCases.Appointments.Commands.CompleteAppointment;
using AppuntaMed.Application.UseCases.Appointments.Commands.CreateAppointment;
using AppuntaMed.Application.UseCases.Appointments.Commands.NoShowAppointment;
using AppuntaMed.Application.UseCases.Appointments.Queries.GetAppointmentDetail;
using AppuntaMed.Application.UseCases.Appointments.Queries.GetAppointmentsList;
using AppuntaMed.Domain.Entities;
using AppuntaMed.Domain.Enums;
using AppuntaMed.Domain.ValueObjects;
using NSubstitute;

namespace AppuntaMed.Tests.Application.UseCases.Appointments
{
	public class AppointmentAuthorizationTests
	{
		private readonly IAppointmentRepository _repository = Substitute.For<IAppointmentRepository>();
		private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
		private readonly IDoctorRepository _doctorRepo = Substitute.For<IDoctorRepository>();
		private readonly IPatientRepository _patientRepo = Substitute.For<IPatientRepository>();
		private readonly INotificationService _notification = Substitute.For<INotificationService>();
		private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();

		private readonly Patient _ana = TestData.Patient("user-ana");
		private readonly Doctor _carlos = TestData.Doctor("user-carlos");
		private readonly Appointment _appointment;

		public AppointmentAuthorizationTests()
		{
			_appointment = TestData.Appointment(_ana, _carlos);
			_appointment.AssignReferences(_ana, _carlos, new ConsultingRoom("Room A"));
			_repository.GetById(_appointment.Id).Returns(Task.FromResult<Appointment?>(_appointment));
		}

		private void LoginAs(string userId, string role)
		{
			_currentUser.UserId.Returns(userId);
			_currentUser.Roles.Returns(new[] { role });
		}

        private void RegisterDoctorProfile(Doctor doctor) =>
			_doctorRepo.GetByUserId(doctor.UserId).Returns(Task.FromResult<Doctor?>(doctor));

        private void RegisterPatientProfile(Patient patient) =>
			_patientRepo.GetByUserId(patient.UserId).Returns(Task.FromResult<Patient?>(patient));

	[Fact]
        public async Task Cancel_PatientCancelsOwnAppointment_Succeeds()
        {
			LoginAs("user-ana", "Patient");
			RegisterPatientProfile(_ana);
			var handler = new CancelAppointmentHandler(_repository, _unitOfWork, _doctorRepo, _patientRepo, _currentUser);
	
			await handler.Handle(new CancelAppointmentCommand { Id = _appointment.Id });
	
			Assert.Equal(AppointmentStatus.Cancelled, _appointment.Status);
			await _unitOfWork.Received(1).Commit();
	    }

		[Fact]
        public async Task Cancel_PatientCancelsSomeoneElsesAppointment_ThrowsForbidden_AndChangesNothing()
        {
			var beto = TestData.Patient("user-beto");
			LoginAs("user-beto", "Patient");
			RegisterPatientProfile(beto);
			var handler = new CancelAppointmentHandler(_repository, _unitOfWork, _doctorRepo, _patientRepo, _currentUser);
	
			await Assert.ThrowsAsync<ForbiddenException>(() => handler.Handle(new CancelAppointmentCommand { Id = _appointment.Id }));
	
			Assert.Equal(AppointmentStatus.Scheduled, _appointment.Status);
			await _unitOfWork.DidNotReceive().Commit();
	    }

		[Fact]
        public async Task Cancel_DoctorCancelsAppointmentOfAnotherDoctor_ThrowsForbidden()
        {
			var laura = TestData.Doctor("user-laura");
			LoginAs("user-laura", "Doctor");
			RegisterDoctorProfile(laura);
			var handler = new CancelAppointmentHandler(_repository, _unitOfWork, _doctorRepo, _patientRepo, _currentUser);
	
			await Assert.ThrowsAsync<ForbiddenException>(() => handler.Handle(new CancelAppointmentCommand { Id = _appointment.Id }));
	
			await _unitOfWork.DidNotReceive().Commit();
	    }

		[Fact]
        public async Task Cancel_Admin_CanCancelAnyAppointment()
        {
			LoginAs("admin-user", "Admin");
			var handler = new CancelAppointmentHandler(_repository, _unitOfWork, _doctorRepo, _patientRepo, _currentUser);
	
			await handler.Handle(new CancelAppointmentCommand { Id = _appointment.Id });
	
			Assert.Equal(AppointmentStatus.Cancelled, _appointment.Status);
	    }

		[Fact]
        public async Task NoShow_OwnDoctor_Succeeds()
        {
			LoginAs("user-carlos", "Doctor");
			RegisterDoctorProfile(_carlos);
			var handler = new NoShowAppointmentHandler(_repository, _unitOfWork, _doctorRepo, _currentUser);
	
			await handler.Handle(new NoShowAppointmentCommand { Id = _appointment.Id });
	
			Assert.Equal(AppointmentStatus.NoShow, _appointment.Status);
			await _unitOfWork.Received(1).Commit();
	    }

		[Fact]
        public async Task NoShow_DoctorOfAnotherAppointment_ThrowsForbidden()
        {
			var laura = TestData.Doctor("user-laura");
			LoginAs("user-laura", "Doctor");
			RegisterDoctorProfile(laura);
			var handler = new NoShowAppointmentHandler(_repository, _unitOfWork, _doctorRepo, _currentUser);
	
			await Assert.ThrowsAsync<ForbiddenException>(() => handler.Handle(new NoShowAppointmentCommand { Id = _appointment.Id }));
	
			Assert.Equal(AppointmentStatus.Scheduled, _appointment.Status);
	    }

		[Fact]
        public async Task Complete_DoctorWithoutProfile_ThrowsForbidden_NotNullReference()
        {
			LoginAs("user-no-profile", "Doctor");
			_doctorRepo.GetByUserId("user-no-profile").Returns(Task.FromResult<Doctor?>(null));
			var handler = new CompleteAppointmentHandler(_repository, _unitOfWork, _doctorRepo, _currentUser);
	
			await Assert.ThrowsAsync<ForbiddenException>(() => handler.Handle(new CompleteAppointmentCommand { Id = _appointment.Id }));
	    }

		[Fact]
        public async Task Complete_Admin_CanCompleteWithoutDoctorProfile()
        {
			LoginAs("admin-user", "Admin");
			var handler = new CompleteAppointmentHandler(_repository, _unitOfWork, _doctorRepo, _currentUser);
	
			await handler.Handle(new CompleteAppointmentCommand { Id = _appointment.Id });
	
			Assert.Equal(AppointmentStatus.Completed, _appointment.Status);
	    }

		[Fact]
        public async Task Detail_PatientReadsOwnAppointment_ReturnsIt()
        {
			LoginAs("user-ana", "Patient");
			RegisterPatientProfile(_ana);
			var handler = new GetAppointmentDetailHandler(_repository, _doctorRepo, _patientRepo, _currentUser);
	
			var result = await handler.Handle(new GetAppointmentDetailQuery { Id = _appointment.Id });
	
			Assert.Equal(_appointment.Id, result.Id);
	    }

		[Fact]
        public async Task Detail_PatientReadsSomeoneElsesAppointment_ThrowsForbidden()
        {
			var beto = TestData.Patient("user-beto");
			LoginAs("user-beto", "Patient");
			RegisterPatientProfile(beto);
			var handler = new GetAppointmentDetailHandler(_repository, _doctorRepo, _patientRepo, _currentUser);
	
			await Assert.ThrowsAsync<ForbiddenException>(() => handler.Handle(new GetAppointmentDetailQuery { Id = _appointment.Id }));
	    }

		[Fact]
        public async Task List_DoctorWithoutProfile_ThrowsForbidden_NotNullReference()
        {
			LoginAs("user-no-profile", "Doctor");
			_doctorRepo.GetByUserId("user-no-profile").Returns(Task.FromResult<Doctor?>(null));
			var handler = new GetAppointmentsListHandler(_repository, _doctorRepo, _patientRepo, _currentUser);
			var query = new GetAppointmentsListQuery { DoctorId = _carlos.Id, Start = DateTime.UtcNow, End = DateTime.UtcNow.AddDays(1) };
	
			await Assert.ThrowsAsync<ForbiddenException>(() => handler.Handle(query));
	    }

		private static CreateAppointmentCommand NewCommand(Guid patientId, Guid doctorId) => new()
		{
			PatientId = patientId,
			DoctorId = doctorId,
			ConsultingRoomId = Guid.NewGuid(),
			Start = DateTime.UtcNow.AddHours(1),
			End = DateTime.UtcNow.AddHours(2)
		};

		[Fact]
        public async Task Create_PatientBooksForAnotherPatient_ThrowsForbidden_AndSavesNothing()
        {
			LoginAs("user-ana", "Patient");
			RegisterPatientProfile(_ana);
			var handler = new CreateAppointmentHandler(_repository, _unitOfWork, _notification, _patientRepo, _currentUser);
			var victim = TestData.Patient("user-victim");
	
			await Assert.ThrowsAsync<ForbiddenException>(() => handler.Handle(NewCommand(victim.Id, _carlos.Id)));
	
			await _repository.DidNotReceive().Add(Arg.Any<Appointment>());
			await _unitOfWork.DidNotReceive().Commit();
			await _notification.DidNotReceive().SendConfirmationAppointment(Arg.Any<ConfirmedAppointmentDTO>());
	    }

		[Fact]
        public async Task Create_PatientBooksForSelf_CreatesAppointment()
        {
			LoginAs("user-ana", "Patient");
			RegisterPatientProfile(_ana);
			var command = NewCommand(_ana.Id, _carlos.Id);
			var created = new Appointment(_ana.Id, _carlos.Id, command.ConsultingRoomId, new TimeInterval(command.Start, command.End));
			created.AssignReferences(_ana, _carlos, new ConsultingRoom("Room A"));
			_repository.Add(Arg.Any<Appointment>()).Returns(Task.FromResult(created));
			_repository.GetById(created.Id).Returns(Task.FromResult<Appointment?>(created));
			var handler = new CreateAppointmentHandler(_repository, _unitOfWork, _notification, _patientRepo, _currentUser);
	
			var id = await handler.Handle(command);
	
			Assert.Equal(created.Id, id);
			await _unitOfWork.Received(1).Commit();
	    }
	}
}
