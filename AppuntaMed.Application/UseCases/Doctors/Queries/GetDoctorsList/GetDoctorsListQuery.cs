using AppuntaMed.Application.Utilities.Mediator;
using AppuntaMed.Application.Utilities.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Application.UseCases.Doctors.Queries.GetDoctorsList
{
	public class GetDoctorsListQuery : DoctorFilterDTO, IRequest<PaginationDTO<DoctorsListDTO>>
	{
	}
}
