using AppuntaMed.Application.Interfaces.DataStore;
using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.Interfaces.Security;
using AppuntaMed.Application.UseCases.Doctors.Commands.CreateDoctor;
using AppuntaMed.Domain.Entities;
using AppuntaMed.Domain.Exceptions;
using AppuntaMed.Domain.ValueObjects;
using AppuntaMed.Persistence.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Numerics;
using System.Reflection.Metadata;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace AppuntaMed.Tests.Application.UseCases.Doctors
{
	public class CreateDoctorTests
	{
		private readonly IDoctorRepository _repository = Substitute.For<IDoctorRepository>();
		private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
		private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
		private readonly IProfessionRepository _professions = Substitute.For<IProfessionRepository>();
		private readonly Profession _dentist = TestData.Profession("Dentist", "Orthodontics", "Endodontics");

		public CreateDoctorTests()
		{
			_currentUser.UserId.Returns("user-new");
			_professions.GetByIdWithSpecialities(_dentist.Id).Returns(Task.FromResult<Profession?>(_dentist));
		}

		private CreateDoctorHandler CreateHandler() => new(_repository, _unitOfWork, _currentUser, _professions);

		private CreateDoctorCommand NewCommand(Guid? professionId = null, Guid? specialityId = null) => new()
        {
			FirstName = "Ana",
			LastName = "Martínez",
			ProfessionId = professionId ?? _dentist.Id,
			SpecialityId = specialityId,
			Email = "ana@example.com",
			PhoneNumber = "5551234"
		};

		[Fact]
		public async Task Handle_ValidCommand_CreatesDoctorWithProfessionAndSpeciality()
		{
			var orthodontics = _dentist.Specialities.First(s => s.Name == "Orthodontics");
			var command = NewCommand(specialityId: orthodontics.Id);
			var created = TestData.Doctor();
			_repository.Add(Arg.Any<Doctor>()).Returns(Task.FromResult(created));
			
			var result = await CreateHandler().Handle(command);

			Assert.Equal(created.Id, result);
			await _repository.Received(1).Add(Arg.Is<Doctor>(d => d.ProfessionId == _dentist.Id && d.SpecialityId == orthodontics.Id && d.UserId == "user-new"));
			await _unitOfWork.Received(1).Commit();
		}

		[Fact]
		public async Task Handle_WithoutSpeciality_CreatesDoctorWithNullSpeciality()
		{
			var created = TestData.Doctor();
			_repository.Add(Arg.Any<Doctor>()).Returns(Task.FromResult(created));

			await CreateHandler().Handle(NewCommand());

			await _repository.Received(1).Add(Arg.Is<Doctor>(d => d.ProfessionId == _dentist.Id && d.SpecialityId == null));
			await _unitOfWork.Received(1).Commit();
		}

		[Fact]
		public async Task Handle_UnknownProfession_ThrowsBusinessRule_AndSavesNothing()
		{
			var command = NewCommand(professionId: Guid.NewGuid());
			
			await Assert.ThrowsAsync<BusinessRuleException>(() => CreateHandler().Handle(command));
			
			await _repository.DidNotReceive().Add(Arg.Any<Doctor>());
			await _unitOfWork.DidNotReceive().Commit();
		}

		[Fact]
		public async Task Handle_SpecialityOfAnotherProfession_ThrowsBusinessRule_AndSavesNothing()
		{
			var cardiology = TestData.Profession("Physician", "Cardiology").Specialities.Single();
			var command = NewCommand(specialityId: cardiology.Id);

			await Assert.ThrowsAsync<BusinessRuleException>(() => CreateHandler().Handle(command));

			await _repository.DidNotReceive().Add(Arg.Any<Doctor>());
			await _unitOfWork.DidNotReceive().Commit();
		}

		[Fact]
		public async Task Handle_DoctorProfileAlreadyExists_ThrowsBusinessRule()
		{
			_repository.ExistsByUserId("user-new").Returns(Task.FromResult(true));

			await Assert.ThrowsAsync<BusinessRuleException>(() => CreateHandler().Handle(NewCommand()));

			await _repository.DidNotReceive().Add(Arg.Any<Doctor>());
		}

		[Fact]
        public async Task Handle_RepositoryThrowsException_RollsBack()
        {
			_repository.Add(Arg.Any<Doctor>()).Throws(new Exception("DB error"));

			var ex = await Assert.ThrowsAsync<Exception>(() => CreateHandler().Handle(NewCommand()));

			Assert.Equal("DB error", ex.Message);
			await _unitOfWork.Received(1).Rollback();
		}
	}
}
