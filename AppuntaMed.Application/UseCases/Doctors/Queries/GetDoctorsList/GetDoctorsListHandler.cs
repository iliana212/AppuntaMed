using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.Utilities.Mediator;
using AppuntaMed.Application.Utilities.Shared;

namespace AppuntaMed.Application.UseCases.Doctors.Queries.GetDoctorsList
{
	public class GetDoctorsListHandler(IDoctorRepository repository) : IRequestHandler<GetDoctorsListQuery, PaginationDTO<DoctorsListDTO>>
	{
		public async Task<PaginationDTO<DoctorsListDTO>> Handle(GetDoctorsListQuery request)
		{
			var filter = new DoctorFilterDTO
			{
				Page = request.Page,
				PageSize = request.PageSize,
				DoctorName = request.DoctorName,
				ProfessionId = request.ProfessionId,
				SpecialityId = request.SpecialityId
			};
			var doctors = await repository.GetFilter(filter);

			var totalDoctors = await repository.CountFilter(filter);
			var dto = doctors.Select(x => x.ToDTO()).ToList();

			var doctorsDTO = new PaginationDTO<DoctorsListDTO>
			{
				Items = dto,
				Total = totalDoctors
			};

			return doctorsDTO;
		}
	}
}
