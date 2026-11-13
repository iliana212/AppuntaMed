using AppuntaMed.Application.Exceptions;
using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.Interfaces.Security;
using AppuntaMed.Application.Security;
using AppuntaMed.Application.Utilities.Mediator;

namespace AppuntaMed.Application.UseCases.Appointments.Queries.GetAppointmentDetail
{
	public class GetAppointmentDetailHandler(IAppointmentRepository repository, IDoctorRepository doctorRepo, IPatientRepository patientRepo, 
		ICurrentUserService currentUser) : IRequestHandler<GetAppointmentDetailQuery, AppointmentDetailDTO>
	{
		public async Task<AppointmentDetailDTO> Handle(GetAppointmentDetailQuery request)
		{
			var appointment = await repository.GetById(request.Id);

			if (appointment is null)
				throw new NotFoundException("Appointment not found");

			await currentUser.EnsureCanAccess(appointment, doctorRepo, patientRepo);

			return appointment.ToDTO();
		}
	}
}
