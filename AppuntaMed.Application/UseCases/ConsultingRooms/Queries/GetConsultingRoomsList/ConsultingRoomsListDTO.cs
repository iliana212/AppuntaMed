using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Application.UseCases.ConsultingRooms.Queries.GetConsultingRoomsList
{
	public record ConsultingRoomsListDTO
	(
		Guid Id, 
		string Name
	);
}
