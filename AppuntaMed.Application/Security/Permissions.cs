namespace AppuntaMed.Application.Security
{
	public static class Permissions
	{
		public const string AppointmentCreate = "appointment.create";
		public const string AppointmentRead = "appointment.read";
		public const string AppointmentReadOwn = "appointment.read.own";
		public const string AppointmentComplete = "appointment.complete";
		public const string AppointmentNoShow = "appointment.noshow";
		public const string AppointmentCancel = "appointment.cancel";

		public const string PatientCreate = "patient.create";
		public const string PatientDelete = "patient.delete";
		public const string PatientRead = "patient.read";
		public const string PatientReadOwn = "patient.read.own";
		public const string PatientUpdate = "patient.update";

		public const string DoctorCreate = "doctor.create";
		public const string DoctorRead = "doctor.read";
		public const string DoctorUpdate = "doctor.update";
		public const string DoctorDelete = "doctor.delete";

		public const string ConsultingRoomCreate = "consultingroom.create";
		public const string ConsultingRoomRead = "consultingroom.read";
		public const string ConsultingRoomUpdate = "consultingroom.update";
		public const string ConsultingRoomDelete = "consultingroom.delete";

		public const string UserManage = "user.manage";
	}
}
