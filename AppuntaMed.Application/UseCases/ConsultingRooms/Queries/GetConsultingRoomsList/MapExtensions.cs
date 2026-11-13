using AppuntaMed.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Application.UseCases.ConsultingRooms.Queries.GetConsultingRoomsList
{
	public static class MapExtensions
	{
		public static ConsultingRoomsListDTO ToDTO(this ConsultingRoom consultingRoom)
		{
			var dto = new ConsultingRoomsListDTO(consultingRoom.Id, consultingRoom.Name);
			return dto;
		}
	}
}
