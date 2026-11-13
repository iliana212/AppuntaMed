using AppuntaMed.API.DTOs.Doctors;
using AppuntaMed.API.Utilities;
using AppuntaMed.Application.Security;
using AppuntaMed.Application.UseCases.Doctors.Commands.CreateDoctor;
using AppuntaMed.Application.UseCases.Doctors.Commands.DeleteDoctor;
using AppuntaMed.Application.UseCases.Doctors.Commands.UpdateDoctor;
using AppuntaMed.Application.UseCases.Doctors.Queries.GetDoctorDetail;
using AppuntaMed.Application.UseCases.Doctors.Queries.GetDoctorsList;
using AppuntaMed.Application.Utilities.Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppuntaMed.API.Controllers
{
	[ApiController]
	[Route("api/doctors")]
	public class DoctorsController(IMediator mediator) : ControllerBase
	{
		[HttpGet]
		[Authorize(Policy = Permissions.DoctorRead)]
		public async Task<ActionResult<List<DoctorsListDTO>>> Get([FromQuery] GetDoctorsListQuery query)
		{
			var doctors = await mediator.Send(query);
			HttpContext.PaginationHeader(doctors.Total);
			return doctors.Items;
		}

		[HttpGet("{id}")]
		[Authorize(Policy = Permissions.DoctorRead)]
		public async Task<ActionResult<DoctorDetailDTO>> Get(Guid id)
		{
			var query = new GetDoctorDetailQuery() { Id = id };
			var doctor = await mediator.Send(query);

			return doctor;
		}

		[HttpPost]
		[Authorize(Policy = Permissions.DoctorCreate)]
		public async Task<IActionResult> Post(CreateDoctorDTO doctorDTO)
		{
			var command = new CreateDoctorCommand
			{
				FirstName = doctorDTO.FirstName,
				LastName = doctorDTO.LastName,
				Profession = doctorDTO.Profession,
				Speciality = doctorDTO.Speciality,
				Email = doctorDTO.Email,
				PhoneNumber = doctorDTO.PhoneNumber				
			};

			await mediator.Send(command);

			return Ok();
		}

		[HttpPut("{id}")]
		[Authorize(Policy = Permissions.DoctorUpdate)]
		public async Task<IActionResult> Put(Guid id, UpdateDoctorDTO doctorDTO)
		{
			var command = new UpdateDoctorCommand
			{
				Id = id,
				FirstName = doctorDTO.FirstName,
				LastName = doctorDTO.LastName,
				Profession = doctorDTO.Profession,
				Speciality = doctorDTO.Speciality,
				Email = doctorDTO.Email,
				PhoneNumber = doctorDTO.PhoneNumber
			};

			await mediator.Send(command);
			return Ok();
		}

		[HttpDelete("{id}")]
		[Authorize(Policy = Permissions.DoctorDelete)]
		public async Task<IActionResult> Delete(Guid id)
		{
			var command = new DeleteDoctorCommand { Id = id };
			await mediator.Send(command);
			return Ok();
		}
	}
}
