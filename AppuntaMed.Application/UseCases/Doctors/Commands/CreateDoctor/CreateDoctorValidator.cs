using FluentValidation;

namespace AppuntaMed.Application.UseCases.Doctors.Commands.CreateDoctor
{
	public class CreateDoctorValidator : AbstractValidator<CreateDoctorCommand>
	{
		public CreateDoctorValidator()
		{
			RuleFor(p => p.FirstName)
				.NotEmpty().WithMessage("{PropertyName} is required")
				.MaximumLength(50).WithMessage("{PropertyName} must be {MaxLength} chars or fewer");

			RuleFor(p => p.LastName)
				.NotEmpty().WithMessage("{PropertyName} is required")
				.MaximumLength(100).WithMessage("{PropertyName} must be {MaxLength} chars or fewer");

			RuleFor(p => p.Profession)
				.NotEmpty().WithMessage("{PropertyName} is required")
				.MaximumLength(100).WithMessage("{PropertyName} must be {MaxLength} chars or fewer");

			RuleFor(p => p.Speciality)
				.NotEmpty().WithMessage("{PropertyName} is required")
				.MaximumLength(100).WithMessage("{PropertyName} must be {MaxLength} chars or fewer");

			RuleFor(p => p.Email)
				.NotEmpty().WithMessage("{PropertyName} is required")
				.MaximumLength(254).WithMessage("{PropertyName} must be {MaxLength} chars or fewer")
				.EmailAddress().WithMessage("{PropertyName} is not valid");

			RuleFor(p => p.PhoneNumber)
				.Matches(@"^\+?[0-9\s\-\(\)]{7,20}$").WithMessage("{PropertyName} is not valid");

		}
	}
}
