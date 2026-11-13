using AppuntaMed.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Application.UseCases.Doctors.Queries.GetDoctorsList
{
	public static class MapExtensions
	{
		public static DoctorsListDTO ToDTO(this Doctor doctor)
		{
			var dto = new DoctorsListDTO(doctor.Id, doctor.FirstName, doctor.LastName,
				doctor.Profession,doctor.Speciality, doctor.Email.Value, doctor.PhoneNumber.Value);

			return dto;
		}
	}
}
