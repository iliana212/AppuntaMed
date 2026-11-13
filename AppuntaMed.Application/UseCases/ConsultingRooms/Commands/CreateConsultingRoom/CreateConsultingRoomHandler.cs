using AppuntaMed.Application.Exceptions;
using AppuntaMed.Application.Interfaces.DataStore;
using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.Utilities.Mediator;
using AppuntaMed.Domain.Entities;

namespace AppuntaMed.Application.UseCases.ConsultingRooms.Commands.CreateConsultingRoom
{
	public class CreateConsultingRoomHandler(IConsultingRoomRepository repository, IUnitOfWork unitOfWork) : IRequestHandler<CreateConsultingRoomCommand, Guid>
	{
		public async Task<Guid> Handle(CreateConsultingRoomCommand command)
		{
			var consultingRoom = new ConsultingRoom(command.Name);
			try
			{
				var response = await repository.Add(consultingRoom);
				await unitOfWork.Commit();
				return response.Id;
			}
			catch (Exception)
			{
				await unitOfWork.Rollback();
				throw;
			}
			
		}
	}
}
