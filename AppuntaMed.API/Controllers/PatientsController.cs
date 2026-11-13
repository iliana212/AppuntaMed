using AppuntaMed.API.DTOs.Patients;
using AppuntaMed.API.Utilities;
using AppuntaMed.Application.Security;
using AppuntaMed.Application.UseCases.Patients.Commands.CreatePatient;
using AppuntaMed.Application.UseCases.Patients.Commands.DeletePatient;
using AppuntaMed.Application.UseCases.Patients.Commands.UpdatePatient;
using AppuntaMed.Application.UseCases.Patients.Queries.GetPatientDetail;
using AppuntaMed.Application.UseCases.Patients.Queries.GetPatientsList;
using AppuntaMed.Application.Utilities.Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppuntaMed.API.Controllers
{
	[ApiController]
	[Route("api/patients")]
	public class PatientsController(IMediator mediator) : ControllerBase
	{
		[HttpGet]
		[Authorize(Policy = Permissions.PatientRead)]
		public async Task<ActionResult<List<PatientsListDTO>>> Get([FromQuery] GetPatientsListQuery query)
		{			
			var patients = await mediator.Send(query);
			HttpContext.PaginationHeader(patients.Total);
			return patients.Items;
		}

		[HttpGet("{id}")]
		[Authorize(Policy = Permissions.PatientReadOwn)]
		public async Task<ActionResult<PatientDetailDTO>> Get(Guid id)
		{
			var query = new GetPatientDetailQuery() { Id = id };
			var patient = await mediator.Send(query);

			return patient;
		}

		[HttpPost]
		[Authorize(Policy = Permissions.PatientCreate)]
		public async Task<IActionResult> Post(CreatePatientDTO patientDTO)
		{
			var command = new CreatePatientCommand
			{
				FirstName = patientDTO.FirstName,
				LastName = patientDTO.LastName,
				DateOfBirth = patientDTO.DateOfBirth,
				Email = patientDTO.Email,
				PhoneNumber = patientDTO.PhoneNumber
			};

			await mediator.Send(command);

			return Ok();
		}

		[HttpPut("{id}")]
		[Authorize(Policy = Permissions.PatientUpdate)]
		public async Task<IActionResult> Put(Guid id, UpdatePatientDTO patientDTO)
		{
			var command = new UpdatePatientCommand
			{
				Id = id,
				FirstName = patientDTO.FirstName,
				LastName = patientDTO.LastName,
				DateOfBirth = patientDTO.DateOfBirth,
				Email = patientDTO.Email,
				PhoneNumber = patientDTO.PhoneNumber
			};

			await mediator.Send(command);
			return Ok();
		}

		[HttpDelete("{id}")]
		[Authorize(Policy = Permissions.PatientDelete)]
		public async Task<IActionResult> Delete (Guid id)
		{
			var command = new DeletePatientCommand { Id = id };
			await mediator.Send(command);
			return Ok();
		}
	}
}
