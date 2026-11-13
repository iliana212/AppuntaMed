using System.ComponentModel.DataAnnotations;

namespace AppuntaMed.API.DTOs.ConsultingRooms
{
	public class UpdateConsultingRoomDTO
	{
		[Required]
		[StringLength(150)]
		public required string Name { get; set; }
	}
}
