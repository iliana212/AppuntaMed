using AppuntaMed.Application.Interfaces.DataStore;
using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.Interfaces.Security;
using AppuntaMed.Application.UseCases.Patients.Commands.CreatePatient;
using AppuntaMed.Domain.Entities;
using AppuntaMed.Domain.ValueObjects;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace AppuntaMed.Tests.Application.UseCases.Patients
{
	public class CreatePatientTests
	{
		private readonly IPatientRepository _repository = Substitute.For<IPatientRepository>();
		private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
		private readonly ICurrentUserService _userService = Substitute.For<ICurrentUserService>();

		// -------------------------------
		// Caso válido
		// -------------------------------
		[Fact]
		public async Task Handle_ValidCommand_CreatesPatientAndCommits()
		{
			// Arrange
			var command = new CreatePatientCommand { FirstName = "Iliana", LastName = "García", DateOfBirth = new DateTime(1995, 5, 10), 
				PhoneNumber = "555-1234", Email = "iliana@example.com" };
			
			var patient = new Patient(
				command.FirstName, command.LastName, command.DateOfBirth,
				new PhoneNumber(command.PhoneNumber), new Email(command.Email), "xxxxx"
			);

			_repository.Add(Arg.Any<Patient>()).Returns(Task.FromResult(patient));

			var handler = new CreatePatientHandler(_repository, _unitOfWork, _userService);

			// Act
			var result = await handler.Handle(command);

			// Assert
			Assert.Equal(patient.Id, result);
			await _repository.Received(1).Add(Arg.Any<Patient>());
			await _unitOfWork.Received(1).Commit();
			await _unitOfWork.DidNotReceive().Rollback();
		}

		// -------------------------------
		// Error en repositorio
		// -------------------------------
		[Fact]
		public async Task Handle_RepositoryThrowsException_RollsBackAndRethrows()
		{
			// Arrange
			var command = new CreatePatientCommand { FirstName="Carlos",LastName= "López", DateOfBirth= new DateTime(1990, 1, 1), 
				PhoneNumber="555-9876", Email= "carlos@example.com"	};

			_repository.Add(Arg.Any<Patient>()).Throws(new Exception("DB error"));

			var handler = new CreatePatientHandler(_repository, _unitOfWork, _userService);

			// Act & Assert
			var ex = await Assert.ThrowsAsync<Exception>(() => handler.Handle(command));
			Assert.Equal("DB error", ex.Message);

			await _unitOfWork.Received(1).Rollback();
			await _unitOfWork.DidNotReceive().Commit();
		}
	}
}
