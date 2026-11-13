using AppuntaMed.Application.Utilities.Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Application.UseCases.Appointments.Queries.GetAppointmentDetail
{
	public class GetAppointmentDetailQuery : IRequest<AppointmentDetailDTO>
	{
		public required Guid Id { get; set; }
	}
}
