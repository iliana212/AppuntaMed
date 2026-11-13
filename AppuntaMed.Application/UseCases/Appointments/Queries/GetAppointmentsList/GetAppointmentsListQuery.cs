using AppuntaMed.Application.Interfaces.Repositories.Models;
using AppuntaMed.Application.Utilities.Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Application.UseCases.Appointments.Queries.GetAppointmentsList
{
	public class GetAppointmentsListQuery : AppointmentsFilterDTO, IRequest<List<AppointmentsListDTO>>
	{
	}
}
