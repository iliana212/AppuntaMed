using AppuntaMed.Domain.Entities;
using AppuntaMed.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AppuntaMed.Tests.Persistence
{
	public class EfModelTests
	{
		private static AppuntaMedDbContext CreateContext()
		{
			var options = new DbContextOptionsBuilder<AppuntaMedDbContext>()
               .UseSqlServer("Server=localhost;Database=ModelValidationOnly;Trusted_Connection=True;TrustServerCertificate=True")
               .Options;

			return new AppuntaMedDbContext(options);
		}

		[Fact]
		public void Model_BuildsAndValidates()
		{
			using var context = CreateContext();

			foreach (var type in new[] { typeof(Doctor), typeof(Patient), typeof(Appointment), typeof(ConsultingRoom), typeof(Profession), typeof(Speciality) })
				Assert.NotNull(context.Model.FindEntityType(type));
		}

		[Fact]
		public void Doctor_Profession_IsARequiredForeignKey_WithRestrict()
		{
			using var context = CreateContext();
			var doctor = context.Model.FindEntityType(typeof(Doctor))!;
			var fk = doctor.GetForeignKeys().Single(f => f.PrincipalEntityType.ClrType == typeof(Profession));

			Assert.Equal([nameof(Doctor.ProfessionId)], fk.Properties.Select(p => p.Name));
			Assert.True(fk.IsRequired);
			Assert.Equal(DeleteBehavior.Restrict, fk.DeleteBehavior);
		}

		[Fact]
        public void Doctor_Speciality_IsACompositeOptionalForeignKey_SoItMustBelongToTheSameProfession()
        {
	        using var context = CreateContext();
			var doctor = context.Model.FindEntityType(typeof(Doctor))!;
	
			var fk = doctor.GetForeignKeys().Single(f => f.PrincipalEntityType.ClrType == typeof(Speciality));
	
			Assert.Equal([nameof(Doctor.ProfessionId), nameof(Doctor.SpecialityId)], fk.Properties.Select(p => p.Name));
			Assert.Equal([nameof(Speciality.ProfessionId), nameof(Speciality.Id)], fk.PrincipalKey.Properties.Select(p => p.Name));
			Assert.False(fk.IsRequired);
			Assert.Equal(DeleteBehavior.Restrict, fk.DeleteBehavior);
	    }

		[Fact]
        public void Doctor_SpecialityId_IsNullable_ProfessionId_IsNot()
        {
	        using var context = CreateContext();
			var doctor = context.Model.FindEntityType(typeof(Doctor))!;
	
			Assert.False(doctor.FindProperty(nameof(Doctor.ProfessionId))!.IsNullable);
			Assert.True(doctor.FindProperty(nameof(Doctor.SpecialityId))!.IsNullable);
	    }

		[Fact]
        public void Catalog_NamesAreUnique()
        {
	        using var context = CreateContext();
	
			var professionIndexes = context.Model.FindEntityType(typeof(Profession))!.GetIndexes();
			var specialityIndexes = context.Model.FindEntityType(typeof(Speciality))!.GetIndexes();
	
			Assert.Contains(professionIndexes, i => i.IsUnique && i.Properties.Select(p => p.Name).SequenceEqual([nameof(Profession.Name)]));
			Assert.Contains(specialityIndexes, i => i.IsUnique && i.Properties.Select(p => p.Name).SequenceEqual([nameof(Speciality.ProfessionId), nameof(Speciality.Name)]));
	    }

		[Fact]
        public void Profession_DeleteIsRestricted_WhenItHasSpecialities()
        {
			using var context = CreateContext();
			var speciality = context.Model.FindEntityType(typeof(Speciality))!;
	
			var fk = speciality.GetForeignKeys().Single(f => f.PrincipalEntityType.ClrType == typeof(Profession));
	
			Assert.Equal(DeleteBehavior.Restrict, fk.DeleteBehavior);
	    }

		[Fact]
        public void Appointments_NeverCascadeDelete_FromDoctorPatientOrRoom()
        {
	        using var context = CreateContext();
			var appointment = context.Model.FindEntityType(typeof(Appointment))!;
	
			var foreignKeys = appointment.GetForeignKeys().ToList();
	
			Assert.Equal(3, foreignKeys.Count);
			Assert.All(foreignKeys, fk => Assert.Equal(DeleteBehavior.Restrict, fk.DeleteBehavior));
	    }
	}
}
