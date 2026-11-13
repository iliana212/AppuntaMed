using AppuntaMed.API.Auth;
using AppuntaMed.Application.Security;
using AppuntaMed.Security;
using System.Security.Claims;
using System.Text.Json;

namespace AppuntaMed.Tests.API.Auth
{
	public class PermissionClaimsTransformationTests
	{
		private readonly PermissionClaimsTransformation _transformation = new();

		private static ClaimsPrincipal Authenticated(params Claim[] claims) =>
			new(new ClaimsIdentity(claims, authenticationType: "test"));

		private static Claim RealmAccess(params string[] roles) =>
			new("realm_access", JsonSerializer.Serialize(new { roles }));

		private static List<string> Permissions(ClaimsPrincipal principal) =>
			principal.FindAll(KeycloakClaims.Permission).Select(c => c.Value).ToList();

		[Fact]
		public async Task TransformAsync_DoctorRole_AddsRoleAndDoctorPermissions()
		{
			var principal = Authenticated(RealmAccess("Doctor"));

			var result = await _transformation.TransformAsync(principal);

			Assert.True(result.IsInRole("Doctor"));
			Assert.Equal(RolePermissions.Map["Doctor"].OrderBy(x => x), Permissions(result).OrderBy(x => x));
		}

		[Fact]
		public async Task TransformAsync_DoctorRole_DoesNotGrantDoctorDelete()
		{
			var result = await _transformation.TransformAsync(Authenticated(RealmAccess("Doctor")));

			Assert.DoesNotContain(AppuntaMed.Application.Security.Permissions.DoctorDelete, Permissions(result));
		}

		[Fact]
		public async Task TransformAsync_MissingRealmAccessClaim_DoesNotThrowAndGrantsNothing()
		{
			var principal = Authenticated(new Claim("sub", "service-account"));

			var result = await _transformation.TransformAsync(principal);

			Assert.Empty(Permissions(result));
		}

		[Fact]
		public async Task TransformAsync_MalformedRealmAccessClaim_GrantsNothing()
		{
			var principal = Authenticated(new Claim("realm_access", "{ not json"));

			var result = await _transformation.TransformAsync(principal);

			Assert.Empty(Permissions(result));
		}

		[Fact]
		public async Task TransformAsync_UnknownRole_AddsRoleButNoPermissions()
		{
			var result = await _transformation.TransformAsync(Authenticated(RealmAccess("offline_access")));

			Assert.True(result.IsInRole("offline_access"));
			Assert.Empty(Permissions(result));
		}

		[Fact]
		public async Task TransformAsync_CalledTwice_DoesNotDuplicateClaims()
		{
			var principal = Authenticated(RealmAccess("Patient"));

			await _transformation.TransformAsync(principal);
			var result = await _transformation.TransformAsync(principal);

			var permissions = Permissions(result);
			Assert.Equal(permissions.Count, permissions.Distinct().Count());
		}

		[Fact]
		public async Task TransformAsync_TwoRolesSharingPermissions_DoesNotDuplicatePermissions()
		{
			var result = await _transformation.TransformAsync(Authenticated(RealmAccess("Doctor", "Patient")));

			var permissions = Permissions(result);
			Assert.Equal(permissions.Count, permissions.Distinct().Count());
		}

		[Fact]
		public async Task TransformAsync_UnauthenticatedPrincipal_IsReturnedUntouched()
		{
			var anonymous = new ClaimsPrincipal(new ClaimsIdentity());

			var result = await _transformation.TransformAsync(anonymous);

			Assert.Empty(result.Claims);
		}
	}
}
