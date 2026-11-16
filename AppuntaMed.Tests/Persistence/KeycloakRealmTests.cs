using AppuntaMed.Application.Security;
using AppuntaMed.Persistence.Seeding;
using AppuntaMed.Security;
using System.Data;
using System.Text.Json;

namespace AppuntaMed.Tests.Persistence
{
	public class KeycloakRealmTests
	{
		private static readonly JsonElement Realm = LoadRealm();

		private static JsonElement LoadRealm()
		{
			var directory = new DirectoryInfo(AppContext.BaseDirectory);

			while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "keycloak", "realm-appuntamed.json")))
				directory = directory.Parent;

			Assert.True(directory is not null, "keycloak/realm-appuntamed.json was not found above the test output folder");

			using var document = JsonDocument.Parse(File.ReadAllText(Path.Combine(directory!.FullName, "keycloak", "realm-appuntamed.json")));

			return document.RootElement.Clone();
		}

		private static JsonElement Client() => 
			Realm.GetProperty("clients").EnumerateArray().Single(c => c.GetProperty("clientId").GetString() == "appuntamed-api");

		private static IEnumerable<JsonElement> Users() => Realm.GetProperty("users").EnumerateArray();

		private static JsonElement UserById(string id) =>
			Users().Single(u => u.GetProperty("id").GetString() == id);

		private static IEnumerable<string> RealmRoles(JsonElement user) =>
			user.GetProperty("realmRoles").EnumerateArray().Select(r => r.GetString()!);

		[Fact]
		public void Realm_IsNamedAppuntaMed()
		{
			Assert.Equal("AppuntaMed", Realm.GetProperty("realm").GetString());
		}

		[Fact]
        public void Realm_DefinesEveryRoleUsedByTheApplication()
        {
			var realmRoles = Realm.GetProperty("roles").GetProperty("realm").EnumerateArray()
				.Select(r => r.GetProperty("name").GetString()!)
	            .ToHashSet();
	
			Assert.All(RolePermissions.Map.Keys, role => Assert.Contains(role, realmRoles));
	    }

		[Theory]
		[InlineData(DemoData.AdminUserId, "Admin")]
		[InlineData(DemoData.DrCarlosUserId, "Doctor")]
		[InlineData(DemoData.DrLauraUserId, "Doctor")]
		[InlineData(DemoData.AnaUserId, "Patient")]
		[InlineData(DemoData.BetoUserId, "Patient")]
        public void DemoUsers_ExistWithTheSameIdAndRoleAsInDemoData(string userId, string role)
        {
			var user = UserById(userId);	
			Assert.Contains(role, RealmRoles(user));
	    }

		[Fact]
        public void SeededDoctorsAndPatients_MatchTheirKeycloakUser()
        {
			var plan = DemoData.Build(DateTime.UtcNow);
	
	        foreach (var seed in plan.Doctors)
		    {
				var doctor = seed.Entity;
				var user = UserById(doctor.UserId);
		
				Assert.Equal(doctor.FirstName, user.GetProperty("firstName").GetString());
				Assert.Equal(doctor.LastName, user.GetProperty("lastName").GetString());
				Assert.Equal(doctor.Email.Value, user.GetProperty("email").GetString());
				Assert.Contains("Doctor", RealmRoles(user));
		    }

			foreach (var seed in plan.Patients)
		    {
				var patient = seed.Entity;
				var user = UserById(patient.UserId);
		
				Assert.Equal(patient.FirstName, user.GetProperty("firstName").GetString());
				Assert.Equal(patient.LastName, user.GetProperty("lastName").GetString());
				Assert.Equal(patient.Email.Value, user.GetProperty("email").GetString());
				Assert.Contains("Patient", RealmRoles(user));
		    }
	    }

		[Fact]
        public void DemoUsers_AreEnabled_WithAPermanentPassword()
        {
			Assert.All(Users(), user =>
	        {
				Assert.True(user.GetProperty("enabled").GetBoolean());
		
				var credential = Assert.Single(user.GetProperty("credentials").EnumerateArray());
				Assert.Equal("password", credential.GetProperty("type").GetString());
				Assert.False(credential.GetProperty("temporary").GetBoolean());
		    });
	    }

		[Fact]
        public void Client_RequiresPkceS256()
        {
			var pkce = Client().GetProperty("attributes").GetProperty("pkce.code.challenge.method").GetString();
	
			Assert.Equal("S256", pkce);
	    }

		[Fact]
        public void Client_DoesNotAllowWildcardOriginsOrRedirects()
        {
			var client = Client();
			var origins = client.GetProperty("webOrigins").EnumerateArray().Select(x => x.GetString()).ToList();
			var redirects = client.GetProperty("redirectUris").EnumerateArray().Select(x => x.GetString()).ToList();
	
			Assert.DoesNotContain("*", origins);
			Assert.DoesNotContain("*", redirects);
			Assert.NotEmpty(redirects);
	    }

		[Fact]
        public void Client_EmitsTheRolesClaimAndTheApiAudience_TheApiRelyOn()
        {
			var mappers = Client().GetProperty("protocolMappers").EnumerateArray()
				.Select(m => m.GetProperty("config")).ToList();
	
			Assert.Contains(mappers, c => c.TryGetProperty("claim.name", out var name) && name.GetString() == KeycloakClaims.Roles);
			Assert.Contains(mappers, c => c.TryGetProperty("included.client.audience", out var aud) && aud.GetString() == "appuntamed-api");
	    }
    }
}
