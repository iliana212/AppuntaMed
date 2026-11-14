using AppuntaMed.Domain.Exceptions;

namespace AppuntaMed.Domain.Entities
{
	public class Profession
	{
		private readonly List<Speciality> _specialities = [];

		public Guid Id { get; private set; }
		public string Name { get; private set; } = null!;
		public IReadOnlyCollection<Speciality> Specialities => _specialities;

		private Profession() { }
		public Profession(string name)
		{
			ApplyBusinessRules(name);

			Id = Guid.CreateVersion7();
			Name = name.Trim();
		}

		public void Rename(string name)
		{
			ApplyBusinessRules(name);
			Name = name.Trim();
		}

        public Speciality AddSpeciality(string name)
        {
			ApplyBusinessRules(name);
	
	        if (_specialities.Any(s => s.Name.Equals(name.Trim(), StringComparison.OrdinalIgnoreCase)))
				throw new BusinessRuleException($"Speciality '{name.Trim()}' already exists in {Name}");
	
			var speciality = new Speciality(Id, name);
			_specialities.Add(speciality);
	
	        return speciality;
	    }

        public bool HasSpeciality(Guid specialityId)
        {
			return _specialities.Any(s => s.Id == specialityId);
	    }

        private static void ApplyBusinessRules(string name)
        {
			if (string.IsNullOrWhiteSpace(name))
				throw new BusinessRuleException($"{nameof(name)} is required");
	    }
	}
}
