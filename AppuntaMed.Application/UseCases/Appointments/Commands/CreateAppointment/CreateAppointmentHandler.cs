using AppuntaMed.Application.Exceptions;
using AppuntaMed.Application.Interfaces.DataStore;
using AppuntaMed.Application.Interfaces.Notifications;
using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.Interfaces.Security;
using AppuntaMed.Application.Security;
using AppuntaMed.Application.Utilities.Mediator;
using AppuntaMed.Domain.Entities;
using AppuntaMed.Domain.ValueObjects;

namespace AppuntaMed.Application.UseCases.Appointments.Commands.CreateAppointment
{
	public class CreateAppointmentHandler(IAppointmentRepository repository, IUnitOfWork unitOfWork, INotificationService notification,
		IPatientRepository patientRepo, ICurrentUserService currentUser) : IRequestHandler<CreateAppointmentCommand, Guid>
	{
		public async Task<Guid> Handle(CreateAppointmentCommand request)
		{
			await currentUser.EnsureActsAsPatient(request.PatientId, patientRepo);

			var appointmentExists = await repository.DuplicateAppointmentTime(request.DoctorId, request.Start, request.End);

			if (appointmentExists)
				throw new ValidationExceptions("Already exists an appointment scheduled on that time");

			var interval = new TimeInterval(request.Start, request.End);
			var appointment = new Appointment(request.PatientId, request.DoctorId, request.ConsultingRoomId, interval);

			Guid? id = null;

			try
			{
				var result = await repository.Add(appointment);
				await unitOfWork.Commit();
				id = result.Id;
			}
			catch (Exception)
			{
				await unitOfWork.Rollback();
				throw;
			}

			var newAppointment = await repository.GetById(id.Value);
			var notificacionDTO = newAppointment!.ToDTO();
			await notification.SendConfirmationAppointment(notificacionDTO);

			return id.Value;
		}
	}
}
