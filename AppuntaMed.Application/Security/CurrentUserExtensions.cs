using AppuntaMed.Application.Interfaces.Security;

namespace AppuntaMed.Application.Security
{
	public static class CurrentUserExtensions
	{
		public static bool IsDoctor(this ICurrentUserService user)
		{
			return user.Roles.Contains("Doctor");
		}

		public static bool IsPatient(this ICurrentUserService user)
		{
			return user.Roles.Contains("Patient");
		}

		public static bool IsAdmin(this ICurrentUserService user)
		{
			return user.Roles.Contains("Admin");
		}
	}
}
