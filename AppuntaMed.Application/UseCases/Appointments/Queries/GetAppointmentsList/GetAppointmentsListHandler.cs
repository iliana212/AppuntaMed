using AppuntaMed.Application.Exceptions;
using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.Interfaces.Security;
using AppuntaMed.Application.Security;
using AppuntaMed.Application.Utilities.Mediator;

namespace AppuntaMed.Application.UseCases.Appointments.Queries.GetAppointmentsList
{
	public class GetAppointmentsListHandler(IAppointmentRepository repository, IDoctorRepository doctorRepo, IPatientRepository patientRepo, 
		ICurrentUserService currentUser) : IRequestHandler<GetAppointmentsListQuery, List<AppointmentsListDTO>>
	{		
		public async Task<List<AppointmentsListDTO>> Handle(GetAppointmentsListQuery request)
		{
			if (currentUser.IsDoctor())
			{
				var doctorLogged = await doctorRepo.GetByUserId(currentUser.UserId!);

				if (doctorLogged is null || request.DoctorId != doctorLogged.Id)
					throw new ForbiddenException("Forbidden access");
			}

			if (currentUser.IsPatient())
			{
				var patientLogged = await patientRepo.GetByUserId(currentUser.UserId!);

				if (patientLogged is null || request.PatientId != patientLogged.Id)
					throw new ForbiddenException("Forbidden access");
			}

			var appointments = await repository.GetFilterData(request);
			var dto = appointments.Select(x => x.ToDTO()).ToList();
			return dto;
		}
	}
}
