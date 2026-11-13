using AppuntaMed.API.Auth;
using AppuntaMed.Application.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace AppuntaMed.API.Utilities
{
	public static class AuthorizationExtensions
	{
		public static IServiceCollection AddAppAuthorization(this IServiceCollection services)
		{
			services.AddAuthorizationBuilder()
				.AddPolicy(Permissions.AppointmentComplete, p => p.Requirements.Add(
						new PermissionRequirement(Permissions.AppointmentComplete)))

				.AddPolicy(Permissions.AppointmentCreate, p => p.Requirements.Add(
						new PermissionRequirement(Permissions.AppointmentCreate)))

				.AddPolicy(Permissions.AppointmentRead, p => p.Requirements.Add(
						new PermissionRequirement(Permissions.AppointmentRead)))

				.AddPolicy(Permissions.AppointmentReadOwn, p => p.Requirements.Add(
						new PermissionRequirement(Permissions.AppointmentReadOwn)))

				.AddPolicy(Permissions.AppointmentNoShow, p => p.Requirements.Add(
						new PermissionRequirement(Permissions.AppointmentNoShow)))

				.AddPolicy(Permissions.AppointmentCancel, p => p.Requirements.Add(
						new PermissionRequirement(Permissions.AppointmentCancel)))

				.AddPolicy(Permissions.PatientCreate, p => p.Requirements.Add(
						new PermissionRequirement(Permissions.PatientCreate)))

				.AddPolicy(Permissions.PatientDelete, p => p.Requirements.Add(
						new PermissionRequirement(Permissions.PatientDelete)))

				.AddPolicy(Permissions.PatientRead, p => p.Requirements.Add(
						new PermissionRequirement(Permissions.PatientRead)))

				.AddPolicy(Permissions.PatientReadOwn, p => p.Requirements.Add(
						new PermissionRequirement(Permissions.PatientReadOwn)))

				.AddPolicy(Permissions.PatientUpdate, p => p.Requirements.Add(
						new PermissionRequirement(Permissions.PatientUpdate)))

				.AddPolicy(Permissions.DoctorCreate, p => p.Requirements.Add(
						new PermissionRequirement(Permissions.DoctorCreate)))

				.AddPolicy(Permissions.DoctorRead, p => p.Requirements.Add(
						new PermissionRequirement(Permissions.DoctorRead)))

				.AddPolicy(Permissions.DoctorUpdate, p => p.Requirements.Add(
						new PermissionRequirement(Permissions.DoctorUpdate)))

				.AddPolicy(Permissions.DoctorDelete, p => p.Requirements.Add(
						new PermissionRequirement(Permissions.DoctorDelete)))

				.AddPolicy(Permissions.ConsultingRoomCreate, p => p.Requirements.Add(
						new PermissionRequirement(Permissions.ConsultingRoomCreate)))

				.AddPolicy(Permissions.ConsultingRoomRead, p => p.Requirements.Add(
						new PermissionRequirement(Permissions.ConsultingRoomRead)))

				.AddPolicy(Permissions.ConsultingRoomUpdate, p => p.Requirements.Add(
						new PermissionRequirement(Permissions.ConsultingRoomUpdate)))

				.AddPolicy(Permissions.ConsultingRoomDelete, p => p.Requirements.Add(
						new PermissionRequirement(Permissions.ConsultingRoomDelete)))

				.AddPolicy(Permissions.UserManage, p => p.Requirements.Add(
						new PermissionRequirement(Permissions.UserManage)));


			services.AddScoped<IAuthorizationHandler, PermissionHandler>();
			services.AddTransient<IClaimsTransformation, PermissionClaimsTransformation>();

			return services;
		}
	}
}