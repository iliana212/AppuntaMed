using AppuntaMed.Application.Exceptions;
using AppuntaMed.Application.Interfaces.DataStore;
using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.Interfaces.Security;
using AppuntaMed.Application.Security;
using AppuntaMed.Application.Utilities.Mediator;

namespace AppuntaMed.Application.UseCases.Appointments.Commands.CompleteAppointment
{
	public class CompleteAppointmentHandler(IAppointmentRepository repository, IUnitOfWork unitOfWork, IDoctorRepository doctorRepo, 
		ICurrentUserService currentUser) : IRequestHandler<CompleteAppointmentCommand>
	{
		public async Task Handle(CompleteAppointmentCommand request)
		{
			var appointment = await repository.GetById(request.Id);

			if (appointment is null)
				throw new NotFoundException("Appointment not found");

			await currentUser.EnsureDoctorCanAccess(appointment, doctorRepo);

			appointment.CompleteAppointment();

			try
			{
				await repository.Update(appointment);
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
