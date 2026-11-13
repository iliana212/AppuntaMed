using AppuntaMed.Application.Exceptions;
using AppuntaMed.Application.Interfaces.DataStore;
using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.Interfaces.Security;
using AppuntaMed.Application.Security;
using AppuntaMed.Application.Utilities.Mediator;

namespace AppuntaMed.Application.UseCases.Appointments.Commands.NoShowAppointment
{
	public class NoShowAppointmentHandler(IAppointmentRepository repository, IUnitOfWork unitOfWork, IDoctorRepository doctorRepo,
		ICurrentUserService currentUser) : IRequestHandler<NoShowAppointmentCommand>
	{
		public async Task Handle(NoShowAppointmentCommand request)
		{
			var appointment = await repository.GetById(request.Id);

			if (appointment is null)
				throw new NotFoundException("Appointment not found");

			await currentUser.EnsureDoctorCanAccess(appointment, doctorRepo);

			appointment.NoShowAppointment();

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
