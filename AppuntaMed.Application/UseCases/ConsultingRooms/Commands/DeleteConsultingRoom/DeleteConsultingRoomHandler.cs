using AppuntaMed.Application.Exceptions;
using AppuntaMed.Application.Interfaces.DataStore;
using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.Utilities.Mediator;
using AppuntaMed.Domain.Exceptions;

namespace AppuntaMed.Application.UseCases.ConsultingRooms.Commands.DeleteConsultingRoom
{
	public class DeleteConsultingRoomHandler(IConsultingRoomRepository repository, IUnitOfWork unitOfWork, IAppointmentRepository appointments) : IRequestHandler<DeleteConsultingRoomCommand>
	{
		public async Task Handle(DeleteConsultingRoomCommand request)
		{
			var consultingRoom = await repository.GetById(request.Id);

			if (consultingRoom is null)
				throw new NotFoundException("Consulting room not found");

			if (await appointments.AnyForConsultingRoom(consultingRoom.Id))
				throw new BusinessRuleException("Cannot delete a consulting room that has appointments");

			try
			{
				await repository.Delete(consultingRoom);
				await unitOfWork.Commit();
			}
			catch (Exception) 
			{
				await unitOfWork.Rollback();
				throw;
			}
		}
	}
}
