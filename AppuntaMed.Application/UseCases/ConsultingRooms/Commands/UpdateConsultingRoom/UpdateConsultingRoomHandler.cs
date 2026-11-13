using AppuntaMed.Application.Exceptions;
using AppuntaMed.Application.Interfaces.DataStore;
using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.Utilities.Mediator;

namespace AppuntaMed.Application.UseCases.ConsultingRooms.Commands.UpdateConsultingRoom
{
	public class UpdateConsultingRoomHandler(IConsultingRoomRepository repository, IUnitOfWork unitOfWork) : IRequestHandler<UpdateConsultingRoomCommand>
	{
		public async Task Handle(UpdateConsultingRoomCommand request)
		{
			var consultingRoom = await repository.GetById(request.Id);

			if (consultingRoom is null)
				throw new NotFoundException("Consulting room not found");

			consultingRoom.UpdateName(request.Name);

			try
			{
				await repository.Update(consultingRoom);
				await unitOfWork.Commit();
			}
			catch(Exception)
			{
				await unitOfWork.Rollback();
				throw;
			}
		}
	}
}
