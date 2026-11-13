using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.Utilities.Mediator;
using AppuntaMed.Application.Utilities.Shared;

namespace AppuntaMed.Application.UseCases.Patients.Queries.GetPatientsList
{
	public class GetPatientsListHandler(IPatientRepository repository) : IRequestHandler<GetPatientsListQuery, PaginationDTO<PatientsListDTO>>
	{
		public async Task<PaginationDTO<PatientsListDTO>> Handle(GetPatientsListQuery request)
		{
			var filter = new PatientFilterDTO
			{
				Page = request.Page,
				PageSize = request.PageSize,
				PatientName = request.PatientName
			};
			var patients = await repository.GetFilter(filter);

			var totalPatients = await repository.CountAll();
			var dto = patients.Select(x => x.ToDTO()).ToList();

			var patientsDTO = new PaginationDTO<PatientsListDTO>
			{
				Items = dto,
				Total = totalPatients
			};
			
			return patientsDTO;
		}
	}
}