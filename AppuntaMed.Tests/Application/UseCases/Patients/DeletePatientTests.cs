using AppuntaMed.Application.Exceptions;
using AppuntaMed.Application.Interfaces.DataStore;
using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.UseCases.Patients.Commands.DeletePatient;
using AppuntaMed.Domain.Entities;
using AppuntaMed.Domain.ValueObjects;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace AppuntaMed.Tests.Application.UseCases.Patients
{
	public class DeletePatientTests
	{
		private readonly IPatientRepository _repository = Substitute.For<IPatientRepository>();
		private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
		private readonly IAppointmentRepository _appointments = Substitute.For<IAppointmentRepository>();

		[Fact]
		public async Task Handle_ValidId_DeletesPatientAndCommits()
		{
			// Arrange
			var patient = new Patient("Carlos", "Pérez", new DateTime(1985, 5, 5),
				new PhoneNumber("5555678"), new Email("carlos@example.com"), "xxxxx");

			_repository.GetById(patient.Id)!.Returns(Task.FromResult(patient));

			var command = new DeletePatientCommand { Id = patient.Id };
			var handler = new DeletePatientHandler(_repository, _unitOfWork, _appointments);

			// Act
			await handler.Handle(command);

			// Assert
			await _repository.Received(1).Delete(patient);
			await _unitOfWork.Received(1).Commit();
			await _unitOfWork.DidNotReceive().Rollback();
		}

		[Fact]
		public async Task Handle_InvalidId_ThrowsNotFoundException()
		{
			// Arrange
			var command = new DeletePatientCommand { Id = Guid.NewGuid() };
			_repository.GetById(command.Id).Returns(Task.FromResult<Patient?>(null));

			var handler = new DeletePatientHandler(_repository, _unitOfWork, _appointments);

			// Act & Assert
			await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command));
			await _unitOfWork.DidNotReceive().Commit();
		}

		[Fact]
		public async Task Handle_RepositoryThrowsException_RollsBack()
		{
			// Arrange
			var patient = new Patient("Carlos", "Pérez", new DateTime(1985, 5, 5),
				new PhoneNumber("5555678"), new Email("carlos@example.com"), "xxxxx");

			_repository.GetById(patient.Id)!.Returns(Task.FromResult(patient));
			_repository.Delete(patient).Throws(new Exception("DB error"));

			var command = new DeletePatientCommand { Id = patient.Id };
			var handler = new DeletePatientHandler(_repository, _unitOfWork, _appointments);

			// Act & Assert
			var ex = await Assert.ThrowsAsync<Exception>(() => handler.Handle(command));
			Assert.Equal("DB error", ex.Message);

			await _unitOfWork.Received(1).Rollback();
			await _unitOfWork.DidNotReceive().Commit();
		}
	}
}
