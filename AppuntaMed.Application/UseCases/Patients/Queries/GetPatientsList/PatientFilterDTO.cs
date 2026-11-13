using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Application.UseCases.Patients.Queries.GetPatientsList
{
	public class PatientFilterDTO
	{
		public int Page { get; set; } = 1;
		public int PageSize { get; set; } = 10;
		public string? PatientName { get; set; } 
	}	
}
