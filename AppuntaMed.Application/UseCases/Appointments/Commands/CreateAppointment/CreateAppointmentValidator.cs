using FluentValidation;

namespace AppuntaMed.Application.UseCases.Appointments.Commands.CreateAppointment
{
	public class CreateAppointmentValidator : AbstractValidator<CreateAppointmentCommand>
	{
		public CreateAppointmentValidator()
		{
			RuleFor(x => x.Start)
				.LessThan(x => x.End).WithMessage("Start date cannot be greater than End date")
				.GreaterThan(DateTime.UtcNow).WithMessage("Start date cannot be in the past");
		}
	}
}
