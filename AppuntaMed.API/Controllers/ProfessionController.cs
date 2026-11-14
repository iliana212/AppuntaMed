using AppuntaMed.Application.Security;
using AppuntaMed.Application.UseCases.Professions.Queries.GetProfessionsList;
using AppuntaMed.Application.Utilities.Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppuntaMed.API.Controllers
{
	[ApiController]
	[Route("api/professions")]
	public class ProfessionsController(IMediator mediator) : ControllerBase
	{
		[HttpGet]
		[Authorize(Policy = Permissions.ProfessionRead)]
		public async Task<ActionResult<List<ProfessionDTO>>> Get()
		{
			var query = new GetProfessionsListQuery();
			var result = await mediator.Send(query);

			return result;
		}
	}
}
