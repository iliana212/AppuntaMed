using AppuntaMed.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace AppuntaMed.Application.UseCases.Doctors.Queries.GetDoctorDetail
{
	public static class MapExtensions
	{
		public static DoctorDetailDTO ToDTO(this Doctor doctor)
		{
			var dto = new DoctorDetailDTO(
				doctor.Id, 
				doctor.FirstName, 
				doctor.LastName, 
				doctor.Profession, 
				doctor.Speciality,
				doctor.Email.Value, 
				doctor.PhoneNumber.Value, 
				$"{doctor.FirstName} {doctor.LastName}"
			);

			return dto;
		}
	}
}
