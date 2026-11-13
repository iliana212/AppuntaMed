using Microsoft.AspNetCore.Authorization;

namespace AppuntaMed.API.Auth
{
	public class PermissionRequirement : IAuthorizationRequirement
	{
		public string Permission { get; }

		public PermissionRequirement(string permission)
		{
			Permission = permission;
		}
	}
}
