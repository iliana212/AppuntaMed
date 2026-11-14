using System.ComponentModel.DataAnnotations;

namespace AppuntaMed.API.DTOs.Doctors
{
	public class UpdateDoctorDTO
	{
		[Required]
		[StringLength(50)]
		public required string FirstName { get; set; }
		[Required]
		[StringLength(100)]
		public required string LastName { get; set; }
		public required Guid ProfessionId { get; set; }
		public Guid? SpecialityId { get; set; }
		[Required]
		[StringLength(254)]
		[EmailAddress]
		public required string Email { get; set; }
		public required string PhoneNumber { get; set; }
	}
}
