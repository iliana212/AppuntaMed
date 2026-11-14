using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Application.UseCases.Doctors.Queries.GetDoctorsList
{
	public class DoctorFilterDTO
	{
		public int Page { get; set; } = 1;
		public int PageSize { get; set; } = 10;
		public string? DoctorName { get; set; }
		public Guid? ProfessionId { get; set; }
		public Guid? SpecialityId { get; set; }
	}
}
