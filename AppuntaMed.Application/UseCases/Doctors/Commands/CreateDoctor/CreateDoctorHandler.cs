using AppuntaMed.Application.Interfaces.DataStore;
using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.Interfaces.Security;
using AppuntaMed.Application.Utilities.Mediator;
using AppuntaMed.Domain.Entities;
using AppuntaMed.Domain.Exceptions;
using AppuntaMed.Domain.ValueObjects;

namespace AppuntaMed.Application.UseCases.Doctors.Commands.CreateDoctor
{
	public class CreateDoctorHandler(IDoctorRepository repository, IUnitOfWork unitOfWork, ICurrentUserService currentUser) : IRequestHandler<CreateDoctorCommand, Guid>
	{
		public async Task<Guid> Handle(CreateDoctorCommand request)
		{
			var email = new Email(request.Email);
			var phone = new PhoneNumber(request.PhoneNumber);
			var userId = currentUser.UserId!;

			var exists = await repository.ExistsByUserId(userId);

			if (exists)
				throw new BusinessRuleException("Doctor profile already exists");

			var doctor = new Doctor(request.FirstName, request.LastName, request.Profession, request.Speciality, email, phone, userId);
						
			try
			{
				var result = await repository.Add(doctor);
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
