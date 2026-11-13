using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.Interfaces.Security;
using AppuntaMed.Application.Utilities.Mediator;

namespace AppuntaMed.Application.UseCases.Account.Queries.HasProfile
{
	public class HasProfileHandler(ICurrentUserService currentUser, IDoctorRepository doctor, IPatientRepository patient) : IRequestHandler<HasProfileQuery, bool>
	{
		public async Task<bool> Handle(HasProfileQuery request)
		{
			var userId = currentUser.UserId;

			var isDoctor = await doctor.ExistsByUserId(userId!);

			var isPatient = await patient.ExistsByUserId(userId!);

			return isDoctor || isPatient;
		}
	}
}
