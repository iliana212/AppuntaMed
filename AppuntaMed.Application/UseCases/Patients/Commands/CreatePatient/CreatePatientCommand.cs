using AppuntaMed.Application.Utilities.Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Application.UseCases.Patients.Commands.CreatePatient
{
	public class CreatePatientCommand : IRequest<Guid>
	{
		public required string FirstName { get; set; }
		public required string LastName { get; set; }
		public DateTime DateOfBirth { get; set; }
		public required string Email { get; set; }
		public required string PhoneNumber { get; set; }
	}
}
