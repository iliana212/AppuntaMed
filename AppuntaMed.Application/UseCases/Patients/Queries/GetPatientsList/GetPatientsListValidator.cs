using FluentValidation;

namespace AppuntaMed.Application.UseCases.Patients.Queries.GetPatientsList
{
	public class GetPatientsListValidator : AbstractValidator<GetPatientsListQuery>
	{
		public GetPatientsListValidator()
		{
			RuleFor(x => x.Page)
				.GreaterThanOrEqualTo(1).WithMessage("{PropertyName} must be 1 or greater");

			RuleFor(x => x.PageSize)
				.InclusiveBetween(1, 100).WithMessage("{PropertyName} must be between {From} and {To}");
		}
	}
}
