using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.UseCases.ConsultingRooms.Queries.GetConsultingRoomsList;
using AppuntaMed.Domain.Entities;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Tests.Application.UseCases.ConsultingRooms
{
	public class GetConsultingRoomsListTests
	{
		private readonly IConsultingRoomRepository _repository = Substitute.For<IConsultingRoomRepository>();

		// -------------------------------
		// Caso válido: lista con elementos
		// -------------------------------
		[Fact]
		public async Task Handle_ReturnsListOfConsultingRoomDetailDTO()
		{
			// Arrange
			var consultingRooms = new List<ConsultingRoom>
			{
				new ConsultingRoom("Room A"),
				new ConsultingRoom("Room B"),
				new ConsultingRoom("Room C")
			};

			_repository.GetAll().Returns(Task.FromResult<IEnumerable<ConsultingRoom>>(consultingRooms));

			var handler = new GetConsultingRoomsListHandler(_repository);
			var query = new GetConsultingRoomsListQuery();

			// Act
			var result = await handler.Handle(query);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(3, result.Count());
			Assert.Contains(result, r => r.Name == "Room A");
			Assert.Contains(result, r => r.Name == "Room B");
			Assert.Contains(result, r => r.Name == "Room C");

			await _repository.Received(1).GetAll();
		}

		// -------------------------------
		// Caso vacío: sin elementos
		// -------------------------------
		[Fact]
		public async Task Handle_EmptyList_ReturnsEmptyCollection()
		{
			// Arrange
			_repository.GetAll().Returns(Task.FromResult<IEnumerable<ConsultingRoom>>(new List<ConsultingRoom>()));

			var handler = new GetConsultingRoomsListHandler(_repository);
			var query = new GetConsultingRoomsListQuery();

			// Act
			var result = await handler.Handle(query);

			// Assert
			Assert.NotNull(result);
			Assert.Empty(result);

			await _repository.Received(1).GetAll();
		}
	}
}
