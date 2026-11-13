using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.UseCases.Patients.Queries.GetPatientsList;
using AppuntaMed.Application.Utilities.Mediator;
using AppuntaMed.Application.Utilities.Shared;
using System;
using System.Collections.Generic;
using System.Text;

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
				DoctorName = request.DoctorName
			};
			var doctors = await repository.GetFilter(filter);

			var totalPatients = await repository.CountAll();
			var dto = doctors.Select(x => x.ToDTO()).ToList();

			var doctorsDTO = new PaginationDTO<DoctorsListDTO>
			{
				Items = dto,
				Total = totalPatients
			};

			return doctorsDTO;
		}
	}
}
