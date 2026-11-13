using AppuntaMed.Application.Utilities.Mediator;

namespace AppuntaMed.Application.UseCases.ConsultingRooms.Queries.GetConsultingRoomDetail
{
	public class GetConsultingRoomDetailQuery : IRequest<ConsultingRoomDetailDTO>
	{
		public Guid Id { get; set; }
	}
}
