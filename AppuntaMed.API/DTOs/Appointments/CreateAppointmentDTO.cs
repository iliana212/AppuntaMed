namespace AppuntaMed.API.DTOs.Appointments
{
	public class CreateAppointmentDTO
	{
		public Guid PatientId { get; set; }
		public Guid DoctorId { get; set; }
		public Guid ConsultingRoomId { get; set; }
		public DateTime Start { get; set; }
		public DateTime End { get; set; }
	}
}
