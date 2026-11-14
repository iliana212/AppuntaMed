using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Domain.Exceptions;

namespace AppuntaMed.Application.UseCases.Doctors
{
	public static class DoctorSpecializationRules
	{
		public static async Task EnsureValidSpecialization(this IProfessionRepository professions, Guid professionId, Guid? specialityId)
		{
			var profession = await professions.GetByIdWithSpecialities(professionId);

			if (profession is null)
				throw new BusinessRuleException("The selected profession does not exist");

			if (specialityId is null)
				return;

			if (!profession.HasSpeciality(specialityId.Value))
				throw new BusinessRuleException("The selected speciality does not belong to the selected profession");
		}
}
}
