using AppuntaMed.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Domain.Entities
{
	public class ConsultingRoom
	{
		public Guid Id { get; private set; }
		public string Name { get; private set; } = null!;

		public ConsultingRoom(string name)
		{
			ApplyBusinessRules(name);

			Name = name;
			Id = Guid.CreateVersion7();
		}

		public void UpdateName(string name)
		{
			ApplyBusinessRules(name);

			Name = name;
		}

		private void ApplyBusinessRules(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new BusinessRuleException($"{nameof(name)} is required");
		}
	}
}
