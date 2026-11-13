using FluentValidation;
using FluentValidation.Validators;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Application.UseCases.ConsultingRooms.Commands.CreateConsultingRoom
{
	public class CreateConsultingRoomValidator : AbstractValidator<CreateConsultingRoomCommand>
	{
		public CreateConsultingRoomValidator()
		{
			RuleFor(p => p.Name)
				.NotEmpty().WithMessage("{PropertyName is required}")
				.MaximumLength(150).WithMessage("{PropertyName must have a maximum of {MaxLength} characters}");
		}
	}
}
