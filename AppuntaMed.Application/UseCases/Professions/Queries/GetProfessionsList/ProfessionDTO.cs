namespace AppuntaMed.Application.UseCases.Professions.Queries.GetProfessionsList
{
	public record SpecialityDTO
	(
		Guid Id,
		string Name
	);

	public record ProfessionDTO
	(
		Guid Id,
		string Name,
		List<SpecialityDTO> Specialities
	);
}
