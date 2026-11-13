using AppuntaMed.Application.Exceptions;
using AppuntaMed.Application.Interfaces.DataStore;
using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.Utilities.Mediator;
using AppuntaMed.Domain.Exceptions;

namespace AppuntaMed.Application.UseCases.Patients.Commands.DeletePatient
{
	public class DeletePatientHandler(IPatientRepository repository, IUnitOfWork unitOfWork, IAppointmentRepository appointments) : IRequestHandler<DeletePatientCommand>
	{
		public async Task Handle(DeletePatientCommand request)
		{
			var patient = await repository.GetById(request.Id);

			if (patient is null)			
				throw new NotFoundException("Patient not found");

			if (await appointments.AnyForPatient(patient.Id))
				throw new BusinessRuleException("Cannot delete a patient that has appointments");

			try
			{
				await repository.Delete(patient);
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
