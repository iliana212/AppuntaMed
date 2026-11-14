namespace AppuntaMed.Application.Security
{
	public static class RolePermissions
	{
		public static readonly Dictionary<string, string[]> Map = new()
		{
			["Admin"] =
			[
				Permissions.AppointmentCreate,
				Permissions.AppointmentRead,
				Permissions.AppointmentComplete,
				Permissions.AppointmentCancel,
				Permissions.AppointmentNoShow,
				Permissions.AppointmentReadOwn,
				Permissions.PatientRead,
				Permissions.PatientUpdate,
				Permissions.PatientDelete,
				Permissions.PatientCreate,
				Permissions.PatientReadOwn,
				Permissions.DoctorRead,
				Permissions.ProfessionRead,
				Permissions.DoctorCreate,
				Permissions.DoctorDelete,
				Permissions.DoctorUpdate,
				Permissions.ConsultingRoomCreate,
				Permissions.ConsultingRoomRead,
				Permissions.ConsultingRoomUpdate,
				Permissions.ConsultingRoomDelete,
				Permissions.UserManage
			],

			["Doctor"] =
			[
				Permissions.AppointmentRead,
				Permissions.AppointmentNoShow,
				Permissions.AppointmentReadOwn,
				Permissions.AppointmentComplete,
				Permissions.AppointmentCancel,
				Permissions.PatientRead,
				Permissions.DoctorRead,
				Permissions.ProfessionRead,
				Permissions.DoctorUpdate,
				Permissions.DoctorCreate,
				Permissions.ConsultingRoomRead
			],

			["Patient"] =
			[
				Permissions.AppointmentCreate,
				Permissions.AppointmentReadOwn,
				Permissions.AppointmentCancel,
				Permissions.DoctorRead,
				Permissions.ProfessionRead,
				Permissions.ConsultingRoomRead,
				Permissions.PatientReadOwn,
				Permissions.PatientUpdate,
				Permissions.PatientCreate
			]
		};
	}
}
