using AppuntaMed.Domain.Entities;

namespace AppuntaMed.Application.UseCases.Professions.Queries.GetProfessionsList
{
	public static class MapExtensions
	{
		public static ProfessionDTO ToDTO(this Profession profession)
		{
			var specialities = profession.Specialities
			   .Select(s => new SpecialityDTO(s.Id, s.Name))
			   .ToList();

			return new ProfessionDTO(profession.Id, profession.Name, specialities);
		}
	}
}
