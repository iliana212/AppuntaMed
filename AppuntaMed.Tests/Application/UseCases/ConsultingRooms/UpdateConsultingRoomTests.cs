using AppuntaMed.Application.Exceptions;
using AppuntaMed.Application.Interfaces.DataStore;
using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.UseCases.ConsultingRooms.Commands.UpdateConsultingRoom;
using AppuntaMed.Domain.Entities;
using NSubstitute;
using NSubstitute.ExceptionExtensions;


namespace AppuntaMed.Tests.Application.UseCases.ConsultingRooms
{
	public class UpdateConsultingRoomTests
	{
		private readonly IConsultingRoomRepository _repository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly UpdateConsultingRoomHandler _handler;

		public UpdateConsultingRoomTests()
		{
			_repository = Substitute.For<IConsultingRoomRepository>();			
			_unitOfWork = Substitute.For<IUnitOfWork>();
			_handler = new UpdateConsultingRoomHandler(_repository,_unitOfWork);
		}

		// -------------------------------
		// Caso válido: actualización exitosa
		// -------------------------------
		[Fact]
		public async Task Handle_ValidCommand_UpdatesConsultingRoomAndCommits()
		{
			// Arrange
			var consultingRoom = new ConsultingRoom("Old Name");
			_repository.GetById(consultingRoom.Id)!.Returns(Task.FromResult(consultingRoom));

			var command = new UpdateConsultingRoomCommand { Id = consultingRoom.Id, Name = "New Name" };

			// Act
			await _handler.Handle(command);

			// Assert
			Assert.Equal("New Name", consultingRoom.Name);
			await _repository.Received(1).Update(consultingRoom);
			await _unitOfWork.Received(1).Commit();
			await _unitOfWork.DidNotReceive().Rollback();
		}

		// -------------------------------
		// Caso inválido: sala no encontrada
		// -------------------------------
		[Fact]
		public async Task Handle_InvalidId_ThrowsNotFoundException()
		{
			// Arrange
			var invalidId = Guid.NewGuid();
			_repository.GetById(invalidId).Returns(Task.FromResult<ConsultingRoom?>(null));

			var command = new UpdateConsultingRoomCommand { Id = invalidId, Name = "New Name" };

			// Act & Assert
			await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command));

			await _repository.DidNotReceive().Update(Arg.Any<ConsultingRoom>());
			await _unitOfWork.DidNotReceive().Commit();
			await _unitOfWork.DidNotReceive().Rollback();
		}

		// -------------------------------
		// Caso error en repositorio
		// -------------------------------
		[Fact]
		public async Task Handle_RepositoryThrowsException_RollsBackAndRethrows()
		{
			// Arrange
			var consultingRoom = new ConsultingRoom("Old Name");
			_repository.GetById(consultingRoom.Id)!.Returns(Task.FromResult(consultingRoom));
			_repository.Update(consultingRoom).Throws(new Exception("DB error"));

			var command = new UpdateConsultingRoomCommand { Id = consultingRoom.Id, Name = "New Name" };

			// Act & Assert
			var ex = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command));
			Assert.Equal("DB error", ex.Message);

			await _unitOfWork.Received(1).Rollback();
			await _unitOfWork.DidNotReceive().Commit();
		}

		// -------------------------------
		// Caso válido: orden correcto
		// -------------------------------
		[Fact]
		public async Task Handle_ValidCommand_CallsGetByIdThenUpdateThenCommit()
		{
			// Arrange
			var consultingRoom = new ConsultingRoom("Old Name");
			_repository.GetById(consultingRoom.Id)!.Returns(Task.FromResult(consultingRoom));

			var command = new UpdateConsultingRoomCommand{ Id = consultingRoom.Id, Name = "New Name" };

			// Act
			await _handler.Handle(command);

			// Assert: verificar orden de llamadas
			Received.InOrder(() =>
			{
				_repository.GetById(consultingRoom.Id);
				_repository.Update(consultingRoom);
				_unitOfWork.Commit();
			});

			await _unitOfWork.DidNotReceive().Rollback();
		}

		// -------------------------------
		// Caso error en repositorio: orden correcto
		// -------------------------------
		[Fact]
		public async Task Handle_RepositoryThrowsException_CallsGetByIdThenUpdateThenRollback_NoCommit()
		{
			// Arrange
			var consultingRoom = new ConsultingRoom("Old Name");
			_repository.GetById(consultingRoom.Id)!.Returns(Task.FromResult(consultingRoom));
			_repository.Update(consultingRoom).Throws(new Exception("DB error"));

			var command = new UpdateConsultingRoomCommand{ Id = consultingRoom.Id, Name = "New Name" };

			// Act & Assert
			var ex = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command));
			Assert.Equal("DB error", ex.Message);

			// Verificar orden de llamadas
			Received.InOrder(() =>
			{
				_repository.GetById(consultingRoom.Id);
				_repository.Update(consultingRoom);
				_unitOfWork.Rollback();
			});

			await _unitOfWork.DidNotReceive().Commit();
		}
	}
}
