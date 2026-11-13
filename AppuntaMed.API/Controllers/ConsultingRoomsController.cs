using AppuntaMed.API.DTOs.ConsultingRooms;
using AppuntaMed.Application.Security;
using AppuntaMed.Application.UseCases.ConsultingRooms.Commands.CreateConsultingRoom;
using AppuntaMed.Application.UseCases.ConsultingRooms.Commands.DeleteConsultingRoom;
using AppuntaMed.Application.UseCases.ConsultingRooms.Commands.UpdateConsultingRoom;
using AppuntaMed.Application.UseCases.ConsultingRooms.Queries.GetConsultingRoomDetail;
using AppuntaMed.Application.UseCases.ConsultingRooms.Queries.GetConsultingRoomsList;
using AppuntaMed.Application.Utilities.Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppuntaMed.API.Controllers
{
	[ApiController]
	[Route("api/consultingrooms")]
	[Authorize(Policy = Permissions.UserManage)]
	public class ConsultingRoomsController(IMediator mediator) : ControllerBase
	{
		[HttpPost]
		public async Task<IActionResult> Post(CreateConsultingRoomDTO consultingRoomDTO)
		{
			var command = new CreateConsultingRoomCommand { Name = consultingRoomDTO.Name };
			await mediator.Send(command);

			return Ok();
		}

		[HttpGet]
		public async Task<ActionResult<List<ConsultingRoomsListDTO>>> Get()
		{
			var query = new GetConsultingRoomsListQuery();
			var result = await mediator.Send(query);

			return result;
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<ConsultingRoomDetailDTO>> Get(Guid id)
		{
			var query = new GetConsultingRoomDetailQuery { Id = id };
			var result = await mediator.Send(query);

			return result;
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Put (Guid id, UpdateConsultingRoomDTO consultingRoomDTO)
		{
			var command = new UpdateConsultingRoomCommand { Id = id, Name = consultingRoomDTO.Name };
			await mediator.Send(command);

			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(Guid id)
		{
			var command = new DeleteConsultingRoomCommand { Id = id };
			await mediator.Send(command);

			return NoContent();
		}
	}
}
