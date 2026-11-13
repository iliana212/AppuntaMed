using AppuntaMed.Application.Utilities.Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Application.UseCases.ConsultingRooms.Commands.UpdateConsultingRoom
{
	public class UpdateConsultingRoomCommand : IRequest
	{
		public Guid Id { get; set; }
		public required string Name { get; set; }
	}
}
