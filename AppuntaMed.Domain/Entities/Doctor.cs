using AppuntaMed.Domain.Exceptions;
using AppuntaMed.Domain.ValueObjects;

namespace AppuntaMed.Domain.Entities
{
	public class Doctor
	{
		public Guid Id { get; private set; }
		public string FirstName { get; private set; } = string.Empty;
		public string LastName { get; private set; } = string.Empty;
		public Guid ProfessionId { get; private set; }
		public Guid? SpecialityId { get; private set; }
		public Email Email { get; private set; } = null!;
		public PhoneNumber PhoneNumber { get; private set; } = null!;
		public string UserId { get; private set; } = string.Empty;
		public Profession Profession { get; private set; } = null!;
		public Speciality? Speciality { get; private set; }

		private Doctor() { }
		public Doctor(string firstName, string lastName, Guid professionId, Guid? specialityId, Email email, PhoneNumber phoneNumber, string userId)
		{
			if (string.IsNullOrWhiteSpace(firstName))
				throw new BusinessRuleException($"{nameof(firstName)} is Required");

			if (string.IsNullOrWhiteSpace(lastName))
				throw new BusinessRuleException($"{nameof(lastName)} is Required");

			if (email is null)
				throw new BusinessRuleException($"{nameof(email)} is Required");

			ValidateSpecialization(professionId, specialityId);

			Id = Guid.CreateVersion7();
			FirstName = firstName;
			LastName = lastName;
			Email = email;
			PhoneNumber = phoneNumber;
			ProfessionId = professionId;
			SpecialityId = specialityId;
			UserId = userId;
		}

		public void UpdateFirstName(string firstName)
		{
			if (string.IsNullOrWhiteSpace(firstName))
				throw new BusinessRuleException($"{nameof(firstName)} is Required");

			FirstName = firstName;
		}

		public void UpdateLastName(string lastName)
		{
			if (string.IsNullOrWhiteSpace(lastName))
				throw new BusinessRuleException($"{nameof(lastName)} is Required");

			LastName = lastName;
		}

		public void UpdateSpecialization(Guid professionId, Guid? specialityId)
		{
			ValidateSpecialization(professionId, specialityId);

			ProfessionId = professionId;
			SpecialityId = specialityId;
		}

		public void UpdateEmail(Email email)
		{
			if (email is null)
				throw new BusinessRuleException($"{nameof(email)} is Required");

			Email = email;
		}

		public void UpdatePhoneNumber(PhoneNumber phoneNumber)
		{
			if (phoneNumber is null)
				throw new BusinessRuleException($"{nameof(phoneNumber)} is Required");

			PhoneNumber = phoneNumber;
		}

		public void AssignCatalog(Profession profession, Speciality? speciality)
		{
			if (profession.Id != ProfessionId)
				throw new BusinessRuleException("The profession does not match the doctor's profession");

			if (speciality is not null && speciality.Id != SpecialityId)
				throw new BusinessRuleException("The speciality does not match the doctor's speciality");

			Profession = profession;
			Speciality = speciality;
		}

		private static void ValidateSpecialization(Guid professionId, Guid? specialityId)
		{
			if (professionId == Guid.Empty)
				throw new BusinessRuleException($"{nameof(professionId)} is Required");

			if (specialityId == Guid.Empty)
				throw new BusinessRuleException($"{nameof(specialityId)} must be a valid id or null");
		}
	}
}
