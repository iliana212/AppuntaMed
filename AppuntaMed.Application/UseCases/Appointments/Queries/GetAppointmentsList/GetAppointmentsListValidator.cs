using FluentValidation;

namespace AppuntaMed.Application.UseCases.Appointments.Queries.GetAppointmentsList
{
	public class GetAppointmentsListValidator : AbstractValidator<GetAppointmentsListQuery>
	{
		public GetAppointmentsListValidator()
		{
			// Without a range the query silently returned nothing (both dates defaulted to DateTime.MinValue).
			RuleFor(x => x.End)
				.GreaterThan(x => x.Start).WithMessage("End must be after Start");
		}
	}
}
