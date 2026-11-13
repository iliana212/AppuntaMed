using AppuntaMed.Application.Exceptions;
using AppuntaMed.Application.Interfaces.DataStore;
using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.Utilities.Mediator;
using AppuntaMed.Domain.Exceptions;

namespace AppuntaMed.Application.UseCases.Doctors.Commands.DeleteDoctor
{
	public class DeleteDoctorHandler(IDoctorRepository repository, IUnitOfWork unitOfWork, IAppointmentRepository appointments) : IRequestHandler<DeleteDoctorCommand>
	{
		public async Task Handle(DeleteDoctorCommand request)
		{
			var doctor = await repository.GetById(request.Id);

			if (doctor is null)
				throw new NotFoundException("Doctor not found");

			if (await appointments.AnyForDoctor(doctor.Id))
				throw new BusinessRuleException("Cannot delete a doctor that has appointments");

			try
			{
				await repository.Delete(doctor);
				await unitOfWork.Commit();
			}
			catch (Exception)
			{
				await unitOfWork.Rollback();
				throw;
			}
		}
	}
}
