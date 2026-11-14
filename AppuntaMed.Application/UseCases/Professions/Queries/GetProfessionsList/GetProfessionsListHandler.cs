using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.Utilities.Mediator;

namespace AppuntaMed.Application.UseCases.Professions.Queries.GetProfessionsList
{
	public class GetProfessionsListHandler(IProfessionRepository repository) : IRequestHandler<GetProfessionsListQuery, List<ProfessionDTO>>
	{
		public async Task<List<ProfessionDTO>> Handle(GetProfessionsListQuery request)
		{
			var professions = await repository.GetAllWithSpecialities();

			return professions.Select(p => p.ToDTO()).ToList();
		}
	}
}
