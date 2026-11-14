using AppuntaMed.Domain.Entities;
using AppuntaMed.Domain.Exceptions;

namespace AppuntaMed.Tests.Domain.Entities
{
	public class ProfessionTests
	{
		[Fact]
		public void Constructor_ValidName_CreatesProfessionWithIdAndTrimmedName()
		{
			var profession = new Profession("  Dentist ");

			Assert.NotEqual(Guid.Empty, profession.Id);
			Assert.Equal("Dentist", profession.Name);
			Assert.Empty(profession.Specialities);
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("   ")]
		public void Constructor_BlankName_ThrowsBusinessRuleException(string? name)
		{
			Assert.Throws<BusinessRuleException>(() => new Profession(name!));
		}

		[Fact]
        public void Rename_ValidName_ChangesName()
        {
			var profession = new Profession("Dentist");	
			profession.Rename("Dental Surgeon");
	
			Assert.Equal("Dental Surgeon", profession.Name);
	    }

		[Fact]
        public void Rename_BlankName_ThrowsAndKeepsName()
        {
			var profession = new Profession("Dentist");
	
			Assert.Throws<BusinessRuleException>(() => profession.Rename(" "));
			Assert.Equal("Dentist", profession.Name);
	    }

		[Fact]
        public void AddSpeciality_ValidName_AddsSpecialityLinkedToTheProfession()
        {
			var profession = new Profession("Dentist");	
			var speciality = profession.AddSpeciality("Orthodontics");
	
			Assert.Equal(profession.Id, speciality.ProfessionId);
			Assert.Equal("Orthodontics", speciality.Name);
			Assert.NotEqual(Guid.Empty, speciality.Id);
			Assert.Single(profession.Specialities);
	    }

		[Theory]
		[InlineData("Orthodontics")]
		[InlineData("orthodontics")]
		[InlineData("  ORTHODONTICS ")]
        public void AddSpeciality_DuplicateName_IgnoringCaseAndSpaces_ThrowsBusinessRuleException(string duplicate)
        {
			var profession = new Profession("Dentist");
			profession.AddSpeciality("Orthodontics");

			Assert.Throws<BusinessRuleException>(() => profession.AddSpeciality(duplicate));
			Assert.Single(profession.Specialities);
	    }

		[Theory]
		[InlineData("")]
		[InlineData("   ")]
		public void AddSpeciality_BlankName_ThrowsBusinessRuleException(string name)
		{
			var profession = new Profession("Dentist");

			Assert.Throws<BusinessRuleException>(() => profession.AddSpeciality(name));
		}

		[Fact]
        public void HasSpeciality_ReturnsTrueOnlyForItsOwnSpecialities()
        {
			var dentist = new Profession("Dentist");
			var orthodontics = dentist.AddSpeciality("Orthodontics");
			var physician = new Profession("Physician");
			var cardiology = physician.AddSpeciality("Cardiology");
	
			Assert.True(dentist.HasSpeciality(orthodontics.Id));
			Assert.False(dentist.HasSpeciality(cardiology.Id));
			Assert.False(dentist.HasSpeciality(Guid.NewGuid()));
	    }

		[Fact]
        public void SameSpecialityName_CanExistInDifferentProfessions()
        {
			var dentist = new Profession("Dentist");
			var physician = new Profession("Physician");
	
			dentist.AddSpeciality("Pediatrics");
			var exception = Record.Exception(() => physician.AddSpeciality("Pediatrics"));
	
			Assert.Null(exception);
	    }
	}
}
