using AppuntaMed.Application.Exceptions;
using AppuntaMed.Application.Interfaces.DataStore;
using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.Interfaces.Security;
using AppuntaMed.Application.Security;
using AppuntaMed.Application.Utilities.Mediator;
using AppuntaMed.Domain.ValueObjects;

namespace AppuntaMed.Application.UseCases.Patients.Commands.UpdatePatient
{
	public class UpdatePatientHandler(IPatientRepository repository, IUnitOfWork unitOfWork, ICurrentUserService currentUser) : IRequestHandler<UpdatePatientCommand>
	{
		public async Task Handle(UpdatePatientCommand request)
		{
			var patient = await repository.GetById(request.Id);

			if (patient is null)
				throw new NotFoundException("Patient not found");

			currentUser.EnsureOwnsPatientProfile(patient);

			patient.UpdateFirstName(request.FirstName);
			patient.UpdateLastName(request.LastName);
			patient.UpdateDateOfBirth(request.DateOfBirth);
			patient.UpdatePhoneNumber(new PhoneNumber(request.PhoneNumber));
			patient.UpdateEmail(new Email(request.Email));

			try
			{
				await repository.Update(patient);
				await unitOfWork.Commit();
			}
			catch(Exception)
			{
				await unitOfWork.Rollback();
				throw;
			}
		}
	}
}
