using AppuntaMed.API.DTOs.Appointments;
using AppuntaMed.Application.Security;
using AppuntaMed.Application.UseCases.Appointments.Commands.CancelAppointment;
using AppuntaMed.Application.UseCases.Appointments.Commands.CompleteAppointment;
using AppuntaMed.Application.UseCases.Appointments.Commands.CreateAppointment;
using AppuntaMed.Application.UseCases.Appointments.Commands.NoShowAppointment;
using AppuntaMed.Application.UseCases.Appointments.Queries.GetAppointmentDetail;
using AppuntaMed.Application.UseCases.Appointments.Queries.GetAppointmentsList;
using AppuntaMed.Application.Utilities.Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppuntaMed.API.Controllers
{
	[ApiController]
	[Route("api/appointments")]
	public class AppointmentsController(IMediator mediator) : ControllerBase
	{
		[HttpGet]
		[Authorize(Policy = Permissions.AppointmentRead)]
		public async Task<ActionResult<List<AppointmentsListDTO>>> Get([FromQuery] GetAppointmentsListQuery query)
		{
			var result = await mediator.Send(query);
			return result;
		}

		[HttpGet("{id}")]
		[Authorize(Policy = Permissions.AppointmentReadOwn)]
		public async Task<ActionResult<AppointmentDetailDTO>> Get(Guid id)
		{
			var query = new GetAppointmentDetailQuery { Id = id };
			var result = await mediator.Send(query);

			return result;
		}

		[HttpPost]
		[Authorize(Policy = Permissions.AppointmentCreate)]
		public async Task<IActionResult> Post(CreateAppointmentDTO appointmentDTO)
		{
			var command = new CreateAppointmentCommand
			{
				DoctorId = appointmentDTO.DoctorId,
				PatientId = appointmentDTO.PatientId,
				ConsultingRoomId = appointmentDTO.ConsultingRoomId,
				Start = appointmentDTO.Start,
				End = appointmentDTO.End
			};

			await mediator.Send(command);
			return Ok();
		}

		[HttpPost("{id}/complete")]
		[Authorize(Policy = Permissions.AppointmentComplete)]
		public async Task<IActionResult> Complete (Guid id)
		{
			var command = new CompleteAppointmentCommand { Id = id };
			await mediator.Send(command);
			return Ok();
		}

		[HttpPost("{id}/cancel")]
		[Authorize(Policy = Permissions.AppointmentCancel)]
		public async Task<IActionResult> Cancel(Guid id)
		{
			var command = new CancelAppointmentCommand { Id = id };
			await mediator.Send(command);
			return Ok();
		}

		[HttpPost("{id}/noshow")]
		[Authorize(Policy = Permissions.AppointmentNoShow)]
		public async Task<IActionResult> NoShow(Guid id)
		{
			var command = new NoShowAppointmentCommand { Id = id };
			await mediator.Send(command);
			return Ok();
		}
	}
}
