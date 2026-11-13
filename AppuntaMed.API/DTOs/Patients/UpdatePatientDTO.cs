using System.ComponentModel.DataAnnotations;

namespace AppuntaMed.API.DTOs.Patients
{
	public class UpdatePatientDTO
	{
		[Required]
		[StringLength(50)]
		public required string FirstName { get; set; }
		[Required]
		[StringLength(100)]
		public required string LastName { get; set; }
		public required DateTime DateOfBirth { get; set; }
		[Required]
		[StringLength(254)]
		[EmailAddress]
		public required string Email { get; set; }
		public required string PhoneNumber { get; set; }
	}
}
