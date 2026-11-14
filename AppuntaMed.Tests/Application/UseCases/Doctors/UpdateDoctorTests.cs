using AppuntaMed.Application.Exceptions;
using AppuntaMed.Application.Interfaces.DataStore;
using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.Interfaces.Security;
using AppuntaMed.Application.UseCases.Doctors.Commands.UpdateDoctor;
using AppuntaMed.Domain.Entities;
using AppuntaMed.Domain.Exceptions;
using AppuntaMed.Domain.ValueObjects;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace AppuntaMed.Tests.Application.UseCases.Doctors
{
	public class UpdateDoctorTests
	{
		private readonly IDoctorRepository _repository = Substitute.For<IDoctorRepository>();
		private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
		private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
		private readonly IProfessionRepository _professions = Substitute.For<IProfessionRepository>();

		private readonly Profession _physician = TestData.Profession("Physician", "Cardiology", "Neurology");
		private readonly Profession _dentist = TestData.Profession("Dentist", "Orthodontics");
		private readonly Doctor _doctor;

		public UpdateDoctorTests()
		{
			var cardiology = _physician.Specialities.First(s => s.Name == "Cardiology");
			_doctor = new Doctor("Ana", "Martínez", _physician.Id, cardiology.Id, new Email("ana@example.com"), new PhoneNumber("5551234"), "xxxxx");

			_repository.GetById(_doctor.Id).Returns(Task.FromResult<Doctor?>(_doctor));
			_professions.GetByIdWithSpecialities(_physician.Id).Returns(Task.FromResult<Profession?>(_physician));
			_professions.GetByIdWithSpecialities(_dentist.Id).Returns(Task.FromResult<Profession?>(_dentist));
		}

		private UpdateDoctorHandler CreateHandler() => new(_repository, _unitOfWork, _currentUser, _professions);

		private UpdateDoctorCommand NewCommand(Guid professionId, Guid? specialityId) => new()
        {
			Id = _doctor.Id,
			FirstName = "Ana María",
			LastName = "Martínez",
			ProfessionId = professionId,
			SpecialityId = specialityId,
			Email = "ana@example.com",
			PhoneNumber = "5559999"
		};

		[Fact]
        public async Task Handle_SameProfessionOtherSpeciality_UpdatesAndCommits()
        {
			var neurology = _physician.Specialities.First(s => s.Name == "Neurology");
	
			await CreateHandler().Handle(NewCommand(_physician.Id, neurology.Id));
	
			Assert.Equal("Ana María", _doctor.FirstName);
			Assert.Equal(_physician.Id, _doctor.ProfessionId);
			Assert.Equal(neurology.Id, _doctor.SpecialityId);
			await _unitOfWork.Received(1).Commit();
	    }

		[Fact]
        public async Task Handle_ChangeProfessionAndSpeciality_UpdatesBoth()
        {
			var orthodontics = _dentist.Specialities.Single();
	
			await CreateHandler().Handle(NewCommand(_dentist.Id, orthodontics.Id));
	
			Assert.Equal(_dentist.Id, _doctor.ProfessionId);
			Assert.Equal(orthodontics.Id, _doctor.SpecialityId);
	    }

		[Fact]
		public async Task Handle_NullSpeciality_ClearsSpeciality()
		{
			await CreateHandler().Handle(NewCommand(_dentist.Id, null));
			
			Assert.Equal(_dentist.Id, _doctor.ProfessionId);
			Assert.Null(_doctor.SpecialityId);

			await _unitOfWork.Received(1).Commit();
		}

		[Fact]
		public async Task Handle_SpecialityOfAnotherProfession_ThrowsBusinessRule_AndKeepsData()
		{
			var originalSpeciality = _doctor.SpecialityId;
			var orthodontics = _dentist.Specialities.Single();

			// Dentist's speciality sent together with the Physician profession.
			await Assert.ThrowsAsync<BusinessRuleException>(() => CreateHandler().Handle(NewCommand(_physician.Id, orthodontics.Id)));

			Assert.Equal(_physician.Id, _doctor.ProfessionId);
			Assert.Equal(originalSpeciality, _doctor.SpecialityId);
			Assert.Equal("Ana", _doctor.FirstName);
			await _unitOfWork.DidNotReceive().Commit();
		}

		[Fact]
		public async Task Handle_UnknownProfession_ThrowsBusinessRule()
		{
			await Assert.ThrowsAsync<BusinessRuleException>(() => CreateHandler().Handle(NewCommand(Guid.NewGuid(), null)));

			await _unitOfWork.DidNotReceive().Commit();
		}

		[Fact]
		public async Task Handle_InvalidId_ThrowsNotFoundException()
		{
			var command = NewCommand(_physician.Id, null);
			command.Id = Guid.NewGuid();

			_repository.GetById(command.Id).Returns(Task.FromResult<Doctor?>(null));

			await Assert.ThrowsAsync<NotFoundException>(() => CreateHandler().Handle(command));
		}

		[Fact]
		public async Task Handle_RepositoryThrowsException_RollsBack()
		{
			_repository.Update(_doctor).Throws(new Exception("DB error"));
			
			var ex = await Assert.ThrowsAsync<Exception>(() => CreateHandler().Handle(NewCommand(_physician.Id, null)));
			
			Assert.Equal("DB error", ex.Message);
			await _unitOfWork.Received(1).Rollback();
		}
	}
}
