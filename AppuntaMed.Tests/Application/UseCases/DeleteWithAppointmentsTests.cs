using AppuntaMed.Application.Interfaces.DataStore;
using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.UseCases.ConsultingRooms.Commands.DeleteConsultingRoom;
using AppuntaMed.Application.UseCases.Doctors.Commands.DeleteDoctor;
using AppuntaMed.Application.UseCases.Patients.Commands.DeletePatient;
using AppuntaMed.Domain.Entities;
using AppuntaMed.Domain.Exceptions;
using NSubstitute;

namespace AppuntaMed.Tests.Application.UseCases
{
	public class DeleteWithAppointmentsTests
	{
		private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
		private readonly IAppointmentRepository _appointments = Substitute.For<IAppointmentRepository>();

		[Fact]
		public async Task DeleteDoctor_WithAppointments_ThrowsBusinessRule_AndDeletesNothing()
		{
			var doctor = TestData.Doctor();
			var repository = Substitute.For<IDoctorRepository>();
			repository.GetById(doctor.Id).Returns(Task.FromResult<Doctor?>(doctor));
			_appointments.AnyForDoctor(doctor.Id).Returns(Task.FromResult(true));
			var handler = new DeleteDoctorHandler(repository, _unitOfWork, _appointments);

			await Assert.ThrowsAsync<BusinessRuleException>(() => handler.Handle(new DeleteDoctorCommand { Id = doctor.Id }));

			await repository.DidNotReceive().Delete(Arg.Any<Doctor>());
			await _unitOfWork.DidNotReceive().Commit();
		}

		[Fact]
		public async Task DeleteDoctor_WithoutAppointments_Deletes()
		{
			var doctor = TestData.Doctor();
			var repository = Substitute.For<IDoctorRepository>();
			repository.GetById(doctor.Id).Returns(Task.FromResult<Doctor?>(doctor));
			_appointments.AnyForDoctor(doctor.Id).Returns(Task.FromResult(false));
			var handler = new DeleteDoctorHandler(repository, _unitOfWork, _appointments);

			await handler.Handle(new DeleteDoctorCommand { Id = doctor.Id });

			await repository.Received(1).Delete(doctor);
			await _unitOfWork.Received(1).Commit();
		}

		[Fact]
        public async Task DeletePatient_WithAppointments_ThrowsBusinessRule_AndDeletesNothing()
        {
			var patient = TestData.Patient();
			var repository = Substitute.For<IPatientRepository>();
			repository.GetById(patient.Id).Returns(Task.FromResult<Patient?>(patient));
			_appointments.AnyForPatient(patient.Id).Returns(Task.FromResult(true));
			var handler = new DeletePatientHandler(repository, _unitOfWork, _appointments);
	
			await Assert.ThrowsAsync<BusinessRuleException>(() => handler.Handle(new DeletePatientCommand { Id = patient.Id }));
	
			await repository.DidNotReceive().Delete(Arg.Any<Patient>());
			await _unitOfWork.DidNotReceive().Commit();
	    }

		[Fact]
        public async Task DeleteConsultingRoom_WithAppointments_ThrowsBusinessRule_AndDeletesNothing()
        {
			var room = new ConsultingRoom("Room A");
			var repository = Substitute.For<IConsultingRoomRepository>();
			repository.GetById(room.Id).Returns(Task.FromResult<ConsultingRoom?>(room));
			_appointments.AnyForConsultingRoom(room.Id).Returns(Task.FromResult(true));
			var handler = new DeleteConsultingRoomHandler(repository, _unitOfWork, _appointments);
	
			await Assert.ThrowsAsync<BusinessRuleException>(() => handler.Handle(new DeleteConsultingRoomCommand { Id = room.Id }));
	
			await repository.DidNotReceive().Delete(Arg.Any<ConsultingRoom>());
			await _unitOfWork.DidNotReceive().Commit();
	    }
	}
}
