using AppuntaMed.Application.Utilities.Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Application.UseCases.ConsultingRooms.Commands.CreateConsultingRoom
{
	public class CreateConsultingRoomCommand : IRequest<Guid>
	{
		public required string Name { get; set; }
	}
}
