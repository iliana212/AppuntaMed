using AppuntaMed.Application.Exceptions;
using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.UseCases.ConsultingRooms.Queries.GetConsultingRoomDetail;
using AppuntaMed.Domain.Entities;
using NSubstitute;

namespace AppuntaMed.Tests.Application.UseCases.ConsultingRooms
{
	public class GetConsultingRoomDetailTests
	{
		private readonly IConsultingRoomRepository _repository = Substitute.For<IConsultingRoomRepository>();

		[Fact]
		public async Task Handle_ValidId_ReturnsConsultingRoomDetailDTO()
		{
			// Arrange
			var consultingRoom = new ConsultingRoom("Room A");
			_repository.GetById(consultingRoom.Id)!.Returns(Task.FromResult(consultingRoom));

			var handler = new GetConsultingRoomDetailHandler(_repository);
			var query = new GetConsultingRoomDetailQuery { Id = consultingRoom.Id };

			// Act
			var result = await handler.Handle(query);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(consultingRoom.Id, result.Id);
			Assert.Equal(consultingRoom.Name, result.Name);

			await _repository.Received(1).GetById(consultingRoom.Id);
		}

		[Fact]
		public async Task Handle_InvalidId_ThrowsNotFoundException()
		{
			// Arrange
			var invalidId = Guid.NewGuid();
			_repository.GetById(invalidId).Returns(Task.FromResult<ConsultingRoom?>(null));

			var handler = new GetConsultingRoomDetailHandler(_repository);
			var query = new GetConsultingRoomDetailQuery { Id = invalidId };

			// Act & Assert
			await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(query));

			await _repository.Received(1).GetById(invalidId);
		}
	}
}
