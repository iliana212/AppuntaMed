using AppuntaMed.Domain.Exceptions;
using AppuntaMed.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Domain.Entities
{
	public class Doctor
	{
		public Guid Id { get; private set; }
		public string FirstName { get; private set; } = string.Empty;
		public string LastName { get; private set; } = string.Empty;
		public string Profession { get; private set; } = string.Empty;
		public string Speciality { get; private set; } = string.Empty;
		public Email Email { get; private set; } = null!;
		public PhoneNumber PhoneNumber { get; private set; } = null!;
		public string UserId { get; private set; } = string.Empty;

		private Doctor() { }
		public Doctor(string firstName, string lastName, string profession, string specialty, Email email, PhoneNumber phoneNumber, string userId)
		{
			if (string.IsNullOrWhiteSpace(firstName))
				throw new BusinessRuleException($"{nameof(firstName)} is Required");

			if (string.IsNullOrWhiteSpace(lastName))
				throw new BusinessRuleException($"{nameof(lastName)} is Required");

			if (email is null)
				throw new BusinessRuleException($"{nameof(email)} is Required");

			Id = Guid.CreateVersion7();
			FirstName = firstName;
			LastName = lastName;
			Email = email;
			PhoneNumber = phoneNumber;
			Profession = profession;
			Speciality = specialty;
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

		public void UpdateProfession(string profession)
		{
			if (string.IsNullOrWhiteSpace(profession))
				throw new BusinessRuleException($"{nameof(profession)} is Required");

			Profession = profession;
		}

		public void UpdateSpeciality(string speciality)
		{
			if (string.IsNullOrWhiteSpace(speciality))
				throw new BusinessRuleException($"{nameof(speciality)} is Required");

			Speciality = speciality;
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
