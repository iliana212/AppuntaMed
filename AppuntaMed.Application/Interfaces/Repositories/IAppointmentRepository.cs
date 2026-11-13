using AppuntaMed.Application.Interfaces.Repositories.Models;
using AppuntaMed.Domain.Entities;

namespace AppuntaMed.Application.Interfaces.Repositories
{
	public interface IAppointmentRepository : IRepository<Appointment>
	{
		Task<bool> DuplicateAppointmentTime(Guid doctorId, DateTime start, DateTime end);
		Task<bool> AnyForDoctor(Guid doctorId);
		Task<bool> AnyForPatient(Guid patientId);
		Task<bool> AnyForConsultingRoom(Guid consultingRoomId);
		new Task<Appointment?> GetById(Guid Id);
		Task<IEnumerable<Appointment>> GetFilterData(AppointmentsFilterDTO filterDTO);
	}
}
