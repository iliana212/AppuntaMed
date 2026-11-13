using AppuntaMed.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Application.Interfaces.Repositories.Models
{
	public class AppointmentsFilterDTO
	{
		public Guid? PatientId { get; set; }
		public Guid? DoctorId { get; set; }
		public Guid? ConsultingRoomId { get; set; }
		public AppointmentStatus? Status { get; set; }
		public DateTime Start { get; set; }
		public DateTime End { get; set; }
	}
}
