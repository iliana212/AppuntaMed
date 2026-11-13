using AppuntaMed.Domain.Entities;
using AppuntaMed.Domain.Exceptions;

namespace AppuntaMed.Tests.Domain.Entities
{
	public class ConsultingRoomTest
	{
		[Fact]
		public void Constructor_ValidData_CreatesConsultingRoom()
		{
			var room = new ConsultingRoom("Room 2A");
			Assert.NotNull(room);
			Assert.Equal("Room 2A", room.Name);
		}

		[Fact]
		public void Constructor_NameNull_ThrowsException()
		{
			Assert.Throws<BusinessRuleException>(() => new ConsultingRoom(null!));
		}
	}
}
