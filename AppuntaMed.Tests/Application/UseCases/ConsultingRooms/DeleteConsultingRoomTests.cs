using AppuntaMed.Application.Exceptions;
using AppuntaMed.Application.Interfaces.DataStore;
using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.UseCases.ConsultingRooms.Commands.DeleteConsultingRoom;
using AppuntaMed.Domain.Entities;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace AppuntaMed.Tests.Application.UseCases.ConsultingRooms
{
	public class DeleteConsultingRoomTests
	{
		private readonly IConsultingRoomRepository _repository = Substitute.For<IConsultingRoomRepository>();
		private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
		private readonly IAppointmentRepository _appointments = Substitute.For<IAppointmentRepository>();

		// -------------------------------
		// Caso válido: borrado exitoso
		// -------------------------------
		[Fact]
		public async Task Handle_ValidCommand_DeletesConsultingRoomAndCommits()
		{
			// Arrange
			var consultingRoom = new ConsultingRoom("Room A");
			_repository.GetById(consultingRoom.Id)!.Returns(Task.FromResult(consultingRoom));

			var handler = new DeleteConsultingRoomHandler(_repository, _unitOfWork, _appointments);
			var command = new DeleteConsultingRoomCommand { Id = consultingRoom.Id };

			// Act
			await handler.Handle(command);

			// Assert: orden correcto
			Received.InOrder(() =>
			{
				_repository.GetById(consultingRoom.Id);
				_repository.Delete(consultingRoom);
				_unitOfWork.Commit();
			});

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

			var handler = new DeleteConsultingRoomHandler(_repository, _unitOfWork, _appointments);
			var command = new DeleteConsultingRoomCommand { Id = invalidId };

			// Act & Assert
			await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command));

			await _repository.DidNotReceive().Delete(Arg.Any<ConsultingRoom>());
			await _unitOfWork.DidNotReceive().Commit();
			await _unitOfWork.DidNotReceive().Rollback();
		}

		// -------------------------------
		// Caso error en repositorio
		// -------------------------------
		[Fact]
		public async Task Handle_RepositoryThrowsException_CallsRollbackAndRethrows()
		{
			// Arrange
			var consultingRoom = new ConsultingRoom("Room B");
			_repository.GetById(consultingRoom.Id)!.Returns(Task.FromResult(consultingRoom));
			_repository.Delete(consultingRoom).Throws(new Exception("DB error"));

			var handler = new DeleteConsultingRoomHandler(_repository, _unitOfWork, _appointments);
			var command = new DeleteConsultingRoomCommand { Id = consultingRoom.Id };

			// Act & Assert
			var ex = await Assert.ThrowsAsync<Exception>(() => handler.Handle(command));
			Assert.Equal("DB error", ex.Message);

			// Verificar orden de llamadas
			Received.InOrder(() =>
			{
				_repository.GetById(consultingRoom.Id);
				_repository.Delete(consultingRoom);
				_unitOfWork.Rollback();
			});

			await _unitOfWork.DidNotReceive().Commit();
		}
	}
}
