using AppuntaMed.Application.UseCases.Doctors.Commands.CreateDoctor;
using AppuntaMed.Application.UseCases.Doctors.Commands.UpdateDoctor;
using FluentValidation.TestHelper;

namespace AppuntaMed.Tests.Application.UseCases.Doctors
{
	public class DoctorValidatorsTests
	{
		private readonly CreateDoctorValidator _createValidator = new();
		private readonly UpdateDoctorValidator _updateValidator = new();

		private static CreateDoctorCommand NewCreate(Guid professionId, Guid? specialityId) => new()
        {
			FirstName = "Ana", LastName = "Martínez", ProfessionId = professionId, SpecialityId = specialityId,
			Email = "ana@example.com", PhoneNumber = "5551234"
		};

		private static UpdateDoctorCommand NewUpdate(Guid professionId, Guid? specialityId) => new()
        {
			Id = Guid.NewGuid(), FirstName = "Ana", LastName = "Martínez", ProfessionId = professionId, SpecialityId = specialityId,
			Email = "ana@example.com", PhoneNumber = "5551234"
		};

		[Fact]
        public void Create_ProfessionWithoutSpeciality_IsValid()
        {
			_createValidator.TestValidate(NewCreate(Guid.NewGuid(), null)).ShouldNotHaveAnyValidationErrors();
	    }

		[Fact]
        public void Create_ProfessionAndSpeciality_IsValid()
        {
			_createValidator.TestValidate(NewCreate(Guid.NewGuid(), Guid.NewGuid())).ShouldNotHaveAnyValidationErrors();
	    }

		[Fact]
        public void Create_EmptyProfessionId_IsInvalid()
        {
			_createValidator.TestValidate(NewCreate(Guid.Empty, null)).ShouldHaveValidationErrorFor(x => x.ProfessionId);
	    }

		[Fact]
        public void Create_EmptySpecialityId_IsInvalid_ButNullIsAllowed()
        {
			_createValidator.TestValidate(NewCreate(Guid.NewGuid(), Guid.Empty)).ShouldHaveValidationErrorFor(x => x.SpecialityId);
			_createValidator.TestValidate(NewCreate(Guid.NewGuid(), null)).ShouldNotHaveValidationErrorFor(x => x.SpecialityId);
	    }

		[Fact]
        public void Update_ProfessionWithoutSpeciality_IsValid()
        {
			_updateValidator.TestValidate(NewUpdate(Guid.NewGuid(), null)).ShouldNotHaveAnyValidationErrors();
	    }

		[Fact]
        public void Update_EmptyProfessionId_IsInvalid()
        {
			_updateValidator.TestValidate(NewUpdate(Guid.Empty, null)).ShouldHaveValidationErrorFor(x => x.ProfessionId);
	    }

		[Fact]
        public void Update_EmptySpecialityId_IsInvalid()
        {
			_updateValidator.TestValidate(NewUpdate(Guid.NewGuid(), Guid.Empty)).ShouldHaveValidationErrorFor(x => x.SpecialityId);
	    }
	}
}
