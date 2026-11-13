using AppuntaMed.Application.Exceptions;
using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.Utilities.Mediator;

namespace AppuntaMed.Application.UseCases.ConsultingRooms.Queries.GetConsultingRoomDetail
{
	public class GetConsultingRoomDetailHandler(IConsultingRoomRepository repository) : IRequestHandler<GetConsultingRoomDetailQuery, ConsultingRoomDetailDTO>
	{
		public async Task<ConsultingRoomDetailDTO> Handle(GetConsultingRoomDetailQuery request)
		{
			var consultingRoom = await repository.GetById(request.Id);

			if (consultingRoom is null)
			{
				throw new NotFoundException("Consulting room not found");
			}

			return consultingRoom.ToDTO();
		}
	}
}
