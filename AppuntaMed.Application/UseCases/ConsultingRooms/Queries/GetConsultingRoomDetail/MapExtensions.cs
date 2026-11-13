using AppuntaMed.Domain.Entities;

namespace AppuntaMed.Application.UseCases.ConsultingRooms.Queries.GetConsultingRoomDetail
{
	public static class MapExtensions
	{
		public static ConsultingRoomDetailDTO ToDTO(this ConsultingRoom consultingRoom)
		{
			var dto = new ConsultingRoomDetailDTO(consultingRoom.Id, consultingRoom.Name);
			return dto;
		}
	}
}
