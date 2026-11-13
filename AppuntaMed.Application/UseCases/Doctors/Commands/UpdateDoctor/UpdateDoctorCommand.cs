using AppuntaMed.Application.Utilities.Mediator;

namespace AppuntaMed.Application.UseCases.Doctors.Commands.UpdateDoctor
{
	public class UpdateDoctorCommand : IRequest
	{
		public Guid Id { get; set; }
		public required string FirstName { get; set; }
		public required string LastName { get; set; }
		public required string Profession { get; set; }
		public required string Speciality { get; set; }
		public required string Email { get; set; }
		public required string PhoneNumber { get; set; }		
	}
}
