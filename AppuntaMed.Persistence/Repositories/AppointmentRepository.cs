using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.Interfaces.Repositories.Models;
using AppuntaMed.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Persistence.Repositories
{
	public class AppointmentRepository : Repository<Appointment>, IAppointmentRepository
	{
		private readonly AppuntaMedDbContext _context;

		public AppointmentRepository(AppuntaMedDbContext context) : base(context)
		{
			_context = context;
		}

		public async Task<bool> AnyForConsultingRoom(Guid consultingRoomId)
		{
			return await _context.Appointments.AnyAsync(x => x.ConsultingRoomId == consultingRoomId);
		}

		public async Task<bool> AnyForDoctor(Guid doctorId)
		{
			return await _context.Appointments.AnyAsync(x => x.DoctorId == doctorId);
		}

		public async Task<bool> AnyForPatient(Guid patientId)
		{
			return await _context.Appointments.AnyAsync(x => x.PatientId == patientId);
		}

		public async Task<bool> DuplicateAppointmentTime(Guid doctorId, DateTime start, DateTime end)
		{
			return await _context.Appointments.Where(x => x.DoctorId == doctorId
			&& x.Status == Domain.Enums.AppointmentStatus.Scheduled
			&& start < x.TimeInterval.End
			&& end > x.TimeInterval.Start).AnyAsync();
		}

		new public async Task<Appointment?> GetById(Guid id)
		{
			return await _context.Appointments
				.Include(x => x.Doctor)
				.Include(x => x.Patient)
				.Include(x => x.ConsultingRoom)
				.FirstOrDefaultAsync(x => x.Id == id);
		}

		public async Task<IEnumerable<Appointment>> GetFilterData(AppointmentsFilterDTO filterDTO)
		{
			var queryable = _context.Appointments
				.Include(x => x.Doctor)
				.Include(x => x.Patient)
				.Include(x => x.ConsultingRoom)
				.AsQueryable();

			if (filterDTO.ConsultingRoomId is not null)
				queryable = queryable.Where(x => x.ConsultingRoomId == filterDTO.ConsultingRoomId);

			if (filterDTO.DoctorId is not null)
				queryable = queryable.Where(x => x.DoctorId == filterDTO.DoctorId);

			if (filterDTO.PatientId is not null)
				queryable = queryable.Where(x => x.PatientId == filterDTO.PatientId);

			if (filterDTO.Status is not null)
				queryable = queryable.Where(x => x.Status == filterDTO.Status);

			return await queryable.Where(x => x.TimeInterval.Start >= filterDTO.Start
				&& x.TimeInterval.End <= filterDTO.End)
				.OrderBy(x => x.TimeInterval.Start)
				.ToListAsync();				
		}
	}
}
