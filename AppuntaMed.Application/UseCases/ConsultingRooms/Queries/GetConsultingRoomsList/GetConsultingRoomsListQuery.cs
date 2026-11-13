using AppuntaMed.Application.Utilities.Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Application.UseCases.ConsultingRooms.Queries.GetConsultingRoomsList
{
	public class GetConsultingRoomsListQuery: IRequest<List<ConsultingRoomsListDTO>>
	{
	}
}
