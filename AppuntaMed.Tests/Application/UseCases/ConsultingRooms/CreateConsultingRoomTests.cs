using AppuntaMed.Application.Interfaces.DataStore;
using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.UseCases.ConsultingRooms.Commands.CreateConsultingRoom;
using AppuntaMed.Domain.Entities;
using FluentValidation.Results;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace AppuntaMed.Tests.Application.UseCases.ConsultingRooms
{
	public class CreateConsultingRoomTests
	{
		private readonly IConsultingRoomRepository _repository = Substitute.For<IConsultingRoomRepository>();
		private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

		// -------------------------------
		// Caso válido
		// -------------------------------
		[Fact]
		public async Task Handle_ValidCommand_CreatesConsultingRoomAndCommits()
		{
			// Arrange
			var command = new CreateConsultingRoomCommand { Name = "Room A" };
			var consultingRoom = new ConsultingRoom(command.Name);
			var validationResult = new ValidationResult(); // válido por defecto

			_repository.Add(Arg.Any<ConsultingRoom>()).Returns(Task.FromResult(consultingRoom));

			var handler = new CreateConsultingRoomHandler(_repository, _unitOfWork);

			// Act
			var result = await handler.Handle(command);

			// Assert
			Assert.Equal(consultingRoom.Id, result);
			await _repository.Received(1).Add(Arg.Any<ConsultingRoom>());
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
			var command = new CreateConsultingRoomCommand { Name = "Room B" };
			var validationResult = new ValidationResult();

			_repository.Add(Arg.Any<ConsultingRoom>()).Throws(new Exception("DB error"));

			var handler = new CreateConsultingRoomHandler(_repository, _unitOfWork);

			// Act & Assert
			var ex = await Assert.ThrowsAsync<Exception>(() => handler.Handle(command));
			Assert.Equal("DB error", ex.Message);

			await _unitOfWork.Received(1).Rollback();
			await _unitOfWork.DidNotReceive().Commit();
		}

		[Fact]
		public async Task Handle_ValidCommand_CallsRepositoryAddThenCommit_NoRollback()
		{
			// Arrange
			var command = new CreateConsultingRoomCommand { Name = "Room A" };
			var consultingRoom = new ConsultingRoom(command.Name);
			var validationResult = new ValidationResult(); // válido

			_repository.Add(Arg.Any<ConsultingRoom>()).Returns(Task.FromResult(consultingRoom));

			var handler = new CreateConsultingRoomHandler(_repository, _unitOfWork);

			// Act
			await handler.Handle(command);

			// Assert: verificar orden de llamadas
			Received.InOrder(() =>
			{
				_repository.Add(Arg.Any<ConsultingRoom>());
				_unitOfWork.Commit();
			});

			// Confirmar que Rollback nunca fue llamado
			await _unitOfWork.DidNotReceive().Rollback();
		}

		[Fact]
		public async Task Handle_RepositoryThrowsException_CallsAddThenRollback_NeverCommit()
		{
			// Arrange
			var command = new CreateConsultingRoomCommand { Name = "Room B" };
			var validationResult = new ValidationResult(); // válido

			_repository.Add(Arg.Any<ConsultingRoom>()).Throws(new Exception("DB error"));

			var handler = new CreateConsultingRoomHandler(_repository, _unitOfWork);

			// Act & Assert
			var ex = await Assert.ThrowsAsync<Exception>(() => handler.Handle(command));
			Assert.Equal("DB error", ex.Message);

			// Verificar orden de llamadas
			Received.InOrder(() =>
			{
				_repository.Add(Arg.Any<ConsultingRoom>());
				_unitOfWork.Rollback();
			});

			// Confirmar que Commit nunca fue llamado
			await _unitOfWork.DidNotReceive().Commit();
		}
	}
}
