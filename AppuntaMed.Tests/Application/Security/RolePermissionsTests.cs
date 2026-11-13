using AppuntaMed.Application.Security;
using System.Reflection;

namespace AppuntaMed.Tests.Application.Security
{
	public class RolePermissionsTests
	{
		private static IEnumerable<string> AllDeclaredPermissions() => 
			typeof(Permissions)
               .GetFields(BindingFlags.Public | BindingFlags.Static)
               .Where(f => f.IsLiteral && f.FieldType == typeof(string))
               .Select(f => (string)f.GetRawConstantValue()!);

		[Theory]
		[InlineData(Permissions.DoctorDelete)]
		[InlineData(Permissions.PatientDelete)]
		[InlineData(Permissions.ConsultingRoomDelete)]
		[InlineData(Permissions.UserManage)]
		public void DestructivePermissions_AreOnlyGrantedToAdmin(string permission)
		{
			var rolesWithPermission = RolePermissions.Map
               .Where(x => x.Value.Contains(permission))
               .Select(x => x.Key)
               .ToList();

			Assert.Single(rolesWithPermission);
			Assert.Equal("Admin", rolesWithPermission[0]);
		}

		[Theory]
		[InlineData(Permissions.DoctorCreate)]
		[InlineData(Permissions.DoctorUpdate)]
		[InlineData(Permissions.DoctorDelete)]
		public void Patient_CannotManageDoctors(string permission)
		{
			Assert.DoesNotContain(permission, RolePermissions.Map["Patient"]);
		}

		[Theory]
		[InlineData(Permissions.AppointmentComplete)]
		[InlineData(Permissions.AppointmentNoShow)]
        public void Patient_CannotCompleteOrMarkNoShow(string permission)
        {
			Assert.DoesNotContain(permission, RolePermissions.Map["Patient"]);
	    }

		[Fact]
        public void Doctor_CanRegisterAndUpdateTheirProfile_ButNotDeleteDoctors()
        {
			Assert.Contains(Permissions.DoctorCreate, RolePermissions.Map["Doctor"]);
			Assert.Contains(Permissions.DoctorUpdate, RolePermissions.Map["Doctor"]);
			Assert.DoesNotContain(Permissions.DoctorDelete, RolePermissions.Map["Doctor"]);
	    }

		[Fact]
        public void Admin_HasEveryDeclaredPermission()
        {
			var missing = AllDeclaredPermissions().Except(RolePermissions.Map["Admin"]).ToList();
	
			Assert.Empty(missing);
	    }

		[Fact]
        public void EveryMappedPermission_IsDeclaredInPermissions()
        {
			var declared = AllDeclaredPermissions().ToHashSet();
			var unknown = RolePermissions.Map.Values.SelectMany(x => x).Where(x => !declared.Contains(x)).ToList();
	
			Assert.Empty(unknown);
	    }

		[Fact]
        public void NoRole_HasDuplicatedPermissions()
        {
			foreach (var (role, permissions) in RolePermissions.Map)
				Assert.True(permissions.Length == permissions.Distinct().Count(), $"{role} has duplicated permissions");
	    }
	}
}
