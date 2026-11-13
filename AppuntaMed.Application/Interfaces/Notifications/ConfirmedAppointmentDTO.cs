namespace AppuntaMed.Application.Interfaces.Notifications
{
	public record ConfirmedAppointmentDTO
	(
		Guid Id,
		string Patient,
		string Patient_Email,
		string Doctor,
		string ConsultingRoom,
		DateTime AppoitmentDate
	);
}
