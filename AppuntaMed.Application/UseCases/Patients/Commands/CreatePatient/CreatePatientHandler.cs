using AppuntaMed.Application.Interfaces.DataStore;
using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.Interfaces.Security;
using AppuntaMed.Application.Utilities.Mediator;
using AppuntaMed.Domain.Entities;
using AppuntaMed.Domain.Exceptions;
using AppuntaMed.Domain.ValueObjects;

namespace AppuntaMed.Application.UseCases.Patients.Commands.CreatePatient
{
	public class CreatePatientHandler(IPatientRepository repository, IUnitOfWork unitOfWork, ICurrentUserService currentUser) : IRequestHandler<CreatePatientCommand, Guid>
	{
		public async Task<Guid> Handle(CreatePatientCommand request)
		{
			var email = new Email(request.Email);
			var phone = new PhoneNumber(request.PhoneNumber);
			var userId = currentUser.UserId!;

			var exists = await repository.ExistsByUserId(userId);

			if (exists)
				throw new BusinessRuleException("Patient profile already exists");

			var patient = new Patient(request.FirstName, request.LastName, request.DateOfBirth, phone, email, userId);
						
			try
			{
				var result = await repository.Add(patient);
				await unitOfWork.Commit();
				return result.Id;
			}
			catch (Exception)
			{
				await unitOfWork.Rollback();
				throw;
			}
		}
	}
}
