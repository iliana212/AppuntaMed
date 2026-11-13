using AppuntaMed.Application.Exceptions;
using AppuntaMed.Application.Interfaces.DataStore;
using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.Interfaces.Security;
using AppuntaMed.Application.UseCases.Patients.Commands.UpdatePatient;
using AppuntaMed.Domain.Entities;
using AppuntaMed.Domain.ValueObjects;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace AppuntaMed.Tests.Application.UseCases.Patients
{
	public class UpdatePatientTests
	{
		private readonly IPatientRepository _repository = Substitute.For<IPatientRepository>();
		private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
		private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();

		[Fact]
		public async Task Handle_ValidId_UpdatesPatientAndCommits()
		{
			// Arrange
			var patient = new Patient("Ana", "López", new DateTime(1990, 1, 1),
				new PhoneNumber("5551234"), new Email("ana@example.com"), "xxxxx");

			_repository.GetById(patient.Id)!.Returns(Task.FromResult(patient));

			var command = new UpdatePatientCommand 
			{ 
				Id = patient.Id, 
				FirstName = "Ana María", 
				LastName = "López", 
				DateOfBirth = patient.DateOfBirth, 
				PhoneNumber = "5559999", 
				Email= "ana@example.com" 
			};

			var handler = new UpdatePatientHandler(_repository, _unitOfWork, _currentUser);

			// Act
			await handler.Handle(command);

			// Assert
			await _repository.Received(1).GetById(patient.Id);
			await _unitOfWork.Received(1).Commit();
			await _unitOfWork.DidNotReceive().Rollback();
		}

		[Fact]
		public async Task Handle_InvalidId_ThrowsNotFoundException()
		{
			// Arrange
			var command = new UpdatePatientCommand
			{
				Id = Guid.NewGuid(),
				FirstName = "Ana",
				LastName = "López",
				DateOfBirth = new DateTime(1990, 1, 1),
				PhoneNumber = "5551234",
				Email = "ana@example.com"
			};

			_repository.GetById(command.Id).Returns(Task.FromResult<Patient?>(null));

			var handler = new UpdatePatientHandler(_repository, _unitOfWork, _currentUser);

			// Act & Assert
			await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command));
			await _unitOfWork.DidNotReceive().Commit();
		}

		[Fact]
		public async Task Handle_RepositoryThrowsException_RollsBack()
		{
			// Arrange
			var patient = new Patient("Ana", "López", new DateTime(1990, 1, 1),
				new PhoneNumber("5551234"), new Email("ana@example.com"), "xxxxx");

			_repository.GetById(patient.Id)!.Returns(Task.FromResult(patient));
			_repository.Update(patient).Throws(new Exception("DB error"));

			var command = new UpdatePatientCommand
			{
				Id = patient.Id,
				FirstName = "Ana María",
				LastName = "López",
				DateOfBirth = patient.DateOfBirth,
				PhoneNumber = "5559999",
				Email = "ana@example.com"
			};

			var handler = new UpdatePatientHandler(_repository, _unitOfWork, _currentUser);

			// Act & Assert
			var ex = await Assert.ThrowsAsync<Exception>(() => handler.Handle(command));
			Assert.Equal("DB error", ex.Message);

			await _unitOfWork.Received(1).Rollback();
			await _unitOfWork.DidNotReceive().Commit();
		}
	}
}
