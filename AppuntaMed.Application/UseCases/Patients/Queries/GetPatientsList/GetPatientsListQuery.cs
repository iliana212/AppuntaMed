using AppuntaMed.Application.Utilities.Mediator;
using AppuntaMed.Application.Utilities.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Application.UseCases.Patients.Queries.GetPatientsList
{
	public class GetPatientsListQuery : PatientFilterDTO, IRequest<PaginationDTO<PatientsListDTO>>
	{
	}
}
