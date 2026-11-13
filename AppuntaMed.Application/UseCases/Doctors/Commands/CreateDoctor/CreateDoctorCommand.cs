using AppuntaMed.Application.Utilities.Mediator;

namespace AppuntaMed.Application.UseCases.Doctors.Commands.CreateDoctor
{
	public class CreateDoctorCommand : IRequest<Guid>
	{
		public required string FirstName { get; set; }
		public required string LastName { get; set; }
		public required string Profession { get; set; }
		public required string Speciality { get; set; }
		public required string Email { get; set; }
		public required string PhoneNumber { get; set; }
	}
}
