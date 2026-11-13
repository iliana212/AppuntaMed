using AppuntaMed.Application.Exceptions;
using AppuntaMed.Application.Interfaces.DataStore;
using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.Interfaces.Security;
using AppuntaMed.Application.Security;
using AppuntaMed.Application.Utilities.Mediator;

namespace AppuntaMed.Application.UseCases.Appointments.Commands.CancelAppointment
{
	public class CancelAppointmentHandler(IAppointmentRepository repository, IUnitOfWork unitOfWork, IDoctorRepository doctorRepo,
		IPatientRepository patientRepo, ICurrentUserService currentUser) : IRequestHandler<CancelAppointmentCommand>
	{
		public async Task Handle(CancelAppointmentCommand request)
		{
			var appointment = await repository.GetById(request.Id);

			if (appointment is null)
				throw new NotFoundException("Appointment not found");

			await currentUser.EnsureCanAccess(appointment, doctorRepo, patientRepo);

			appointment.CancelAppointment();

			try
			{
				await repository.Update(appointment);
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
