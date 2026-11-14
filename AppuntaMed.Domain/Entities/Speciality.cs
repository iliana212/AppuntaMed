using AppuntaMed.Domain.Exceptions;

namespace AppuntaMed.Domain.Entities
{
	public class Speciality
	{
		public Guid Id { get; private set; }
		public Guid ProfessionId { get; private set; }
		public string Name { get; private set; } = null!;

		private Speciality() { }
		internal Speciality(Guid professionId, string name)
		{
			if (professionId == Guid.Empty)
				throw new BusinessRuleException($"{nameof(professionId)} is required");

			if (string.IsNullOrWhiteSpace(name))
				throw new BusinessRuleException($"{nameof(name)} is required");

			Id = Guid.CreateVersion7();
			ProfessionId = professionId;
			Name = name.Trim();
		}
	}
}
