using AppuntaMed.Application.Exceptions;
using AppuntaMed.Application.Interfaces.DataStore;
using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.Interfaces.Security;
using AppuntaMed.Application.Security;
using AppuntaMed.Application.Utilities.Mediator;
using AppuntaMed.Domain.ValueObjects;

namespace AppuntaMed.Application.UseCases.Doctors.Commands.UpdateDoctor
{
	public class UpdateDoctorHandler(IDoctorRepository repository, IUnitOfWork unitOfWork, ICurrentUserService currentUser,
		IProfessionRepository professions) : IRequestHandler<UpdateDoctorCommand>
	{
		public async Task Handle(UpdateDoctorCommand request)
		{
			var doctor = await repository.GetById(request.Id);

			if (doctor is null)
				throw new NotFoundException("Doctor not found");

			currentUser.EnsureOwnsDoctorProfile(doctor);

			await professions.EnsureValidSpecialization(request.ProfessionId, request.SpecialityId);

			doctor.UpdateFirstName(request.FirstName);
			doctor.UpdateLastName(request.LastName);
			doctor.UpdateSpecialization(request.ProfessionId, request.SpecialityId);
			doctor.UpdateEmail(new Email(request.Email));
			doctor.UpdatePhoneNumber(new PhoneNumber(request.PhoneNumber));			

			try
			{
				await repository.Update(doctor);
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
