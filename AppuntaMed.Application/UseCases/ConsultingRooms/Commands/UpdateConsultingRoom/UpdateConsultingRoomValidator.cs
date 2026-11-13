using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Application.UseCases.ConsultingRooms.Commands.UpdateConsultingRoom
{
	public class UpdateConsultingRoomValidator : AbstractValidator<UpdateConsultingRoomCommand>
	{
		public UpdateConsultingRoomValidator()
		{
			RuleFor(p => p.Name)
				.NotEmpty().WithMessage("{PropertyName} is required")
				.MaximumLength(150).WithMessage("{PropertyName must have a maximum of {MaxLength} characters}");
		}
	}
}
