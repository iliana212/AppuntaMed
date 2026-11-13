using AppuntaMed.Application.Exceptions;
using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.Interfaces.Security;
using AppuntaMed.Application.Security;
using AppuntaMed.Application.Utilities.Mediator;

namespace AppuntaMed.Application.UseCases.Patients.Queries.GetPatientDetail
{
	public class GetPatientDetailHandler(IPatientRepository repository, ICurrentUserService currentUser) : IRequestHandler<GetPatientDetailQuery, PatientDetailDTO>
	{
		public async Task<PatientDetailDTO> Handle(GetPatientDetailQuery request)
		{
			var patient = await repository.GetById(request.Id);

			if (patient is null)
				throw new NotFoundException("Patient not found");

			currentUser.EnsureOwnsPatientProfile(patient);

			return patient.ToDTO();
		}
	}
}
