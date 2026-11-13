using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.Utilities.Mediator;

namespace AppuntaMed.Application.UseCases.ConsultingRooms.Queries.GetConsultingRoomsList
{
	public class GetConsultingRoomsListHandler(IConsultingRoomRepository repository) : IRequestHandler<GetConsultingRoomsListQuery, List<ConsultingRoomsListDTO>>
	{
		public async Task<List<ConsultingRoomsListDTO>> Handle(GetConsultingRoomsListQuery request)
		{
			var consultingRooms = await repository.GetAll();
			var dto = consultingRooms.Select(rooms => rooms.ToDTO()).ToList();

			return dto;
		}
	}
}
