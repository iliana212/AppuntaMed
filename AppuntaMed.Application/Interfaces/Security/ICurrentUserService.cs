namespace AppuntaMed.Application.Interfaces.Security
{
	public interface ICurrentUserService
	{
		string? UserId { get; }
		string? Email { get; }
		string? UserName { get; }
		IEnumerable<string> Roles { get; }
		IEnumerable<string> Permissions { get; }
		bool IsAuthenticated { get; }
	}
}
