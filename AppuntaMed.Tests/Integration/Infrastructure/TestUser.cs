using AppuntaMed.Persistence.Seeding;

namespace AppuntaMed.Tests.Integration.Infrastructure
{
	public sealed record TestUser(string UserId, string UserName, string Email, string Role)
	{
		public static readonly TestUser Admin = new(DemoData.AdminUserId, "admin", "admin@appuntamed.test", "Admin");
		public static readonly TestUser DrCarlos = new(DemoData.DrCarlosUserId, "dr.carlos", "carlos.perez@appuntamed.test", "Doctor");
		public static readonly TestUser DrLaura = new(DemoData.DrLauraUserId, "dr.laura", "laura.gomez@appuntamed.test", "Doctor");
		public static readonly TestUser Ana = new(DemoData.AnaUserId, "ana", "ana.lopez@appuntamed.test", "Patient");
		public static readonly TestUser Beto = new(DemoData.BetoUserId, "beto", "beto.ramirez@appuntamed.test", "Patient");

		/// <summary>A brand-new account WITHOUT a profile yet (it can create one through the API).</summary>
		public static TestUser NewPatient() => New("Patient");

		public static TestUser NewDoctor() => New("Doctor");

		private static TestUser New(string role)
		{
			var id = Guid.NewGuid().ToString();
			return new TestUser(id, $"user-{id[..8]}", $"{id[..8]}@appuntamed.test", role);
		}
    }
}
