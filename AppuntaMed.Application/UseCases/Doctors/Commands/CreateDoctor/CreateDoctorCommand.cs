using AppuntaMed.Application.Utilities.Mediator;

namespace AppuntaMed.Application.UseCases.Doctors.Commands.CreateDoctor
{
	public class CreateDoctorCommand : IRequest<Guid>
	{
		public required string FirstName { get; set; }
		public required string LastName { get; set; }
		public required Guid ProfessionId { get; set; }
		public Guid? SpecialityId { get; set; }
		public required string Email { get; set; }
		public required string PhoneNumber { get; set; }
	}
}
