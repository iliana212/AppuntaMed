using AppuntaMed.Application.Utilities.Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Application.UseCases.ConsultingRooms.Commands.DeleteConsultingRoom
{
	public class DeleteConsultingRoomCommand: IRequest
	{
		public Guid Id { get; set; }
	}
}
