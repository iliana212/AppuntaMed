using AppuntaMed.Domain.Exceptions;
using AppuntaMed.Domain.ValueObjects;

namespace AppuntaMed.Domain.Entities
{
	public class Patient
	{
		public Guid Id { get; private set; }
		public string FirstName { get; private set; } = string.Empty;
		public string LastName { get; private set; } = string.Empty;
		public DateTime DateOfBirth { get; private set; }
		public PhoneNumber PhoneNumber { get; private set; } = null!;
		public Email Email { get; private set; } = null!;
		public string UserId { get; private set; } = string.Empty;

		private Patient() {	}
		public Patient(string firstName, string lastName, DateTime dateBirth, PhoneNumber phoneNumber, Email email, string userId)
		{
			if (string.IsNullOrWhiteSpace(firstName))
				throw new BusinessRuleException($"{nameof(firstName)} is Required");

			if (string.IsNullOrWhiteSpace(lastName))
				throw new BusinessRuleException($"{nameof(lastName)} is Required");

			if (dateBirth == DateTime.MinValue)
				throw new BusinessRuleException($"{nameof(dateBirth)} is Required");
						
			Id = Guid.CreateVersion7();
			FirstName = firstName;
			LastName = lastName;
			DateOfBirth = dateBirth;
			PhoneNumber = phoneNumber;
			Email = email;
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

		public void UpdateDateOfBirth(DateTime dateBirth)
		{
			if (dateBirth == DateTime.MinValue)
				throw new BusinessRuleException($"{nameof(dateBirth)} is Required");

			DateOfBirth = dateBirth;
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
	}
}