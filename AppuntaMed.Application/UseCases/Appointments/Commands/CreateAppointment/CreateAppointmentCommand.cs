using AppuntaMed.Application.Utilities.Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Application.UseCases.Appointments.Commands.CreateAppointment
{
	public class CreateAppointmentCommand : IRequest<Guid>
	{
		public Guid PatientId { get; set; }
		public Guid DoctorId { get; set; }
		public Guid ConsultingRoomId { get; set; }
		public DateTime Start {  get; set; }
		public DateTime End { get; set; }
	}
}
