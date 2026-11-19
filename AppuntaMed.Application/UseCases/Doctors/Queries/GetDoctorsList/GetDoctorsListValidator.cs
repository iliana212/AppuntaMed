using FluentValidation;

namespace AppuntaMed.Application.UseCases.Doctors.Queries.GetDoctorsList
{
	public class GetDoctorsListValidator : AbstractValidator<GetDoctorsListQuery>
	{
		public GetDoctorsListValidator()
		{
			RuleFor(x => x.Page)
				.GreaterThanOrEqualTo(1).WithMessage("{PropertyName} must be 1 or greater");

			// An upper bound stops a single request from reading the whole table.
			RuleFor(x => x.PageSize)
				.InclusiveBetween(1, 100).WithMessage("{PropertyName} must be between {From} and {To}");
		}
	}
}
