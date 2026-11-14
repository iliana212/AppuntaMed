using AppuntaMed.Application.Exceptions;
using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.UseCases.Doctors.Queries.GetDoctorDetail;
using AppuntaMed.Domain.Entities;
using AppuntaMed.Domain.ValueObjects;
using NSubstitute;

namespace AppuntaMed.Tests.Application.UseCases.Doctors
{
	public class GetDoctorDetailTests
	{
		private readonly IDoctorRepository _repository = Substitute.For<IDoctorRepository>();

		[Fact]
		public async Task Handle_ValidId_ReturnsDoctorDetailDTO_WithCatalogNames()
		{
			var doctor = TestData.Doctor();
			_repository.GetById(doctor.Id).Returns(Task.FromResult<Doctor?>(doctor));

			var handler = new GetDoctorDetailHandler(_repository);
			var query = new GetDoctorDetailQuery { Id = doctor.Id };

			var result = await handler.Handle(query);

			Assert.NotNull(result);
			Assert.Equal($"{doctor.FirstName} {doctor.LastName}", result.Name);
			Assert.Equal(doctor.Email.Value, result.Email);
			Assert.Equal(doctor.PhoneNumber.Value, result.PhoneNumber);
			Assert.Equal(doctor.ProfessionId, result.ProfessionId);
			Assert.Equal("Physician", result.Profession);
			Assert.Equal(doctor.SpecialityId, result.SpecialityId);
			Assert.Equal("Cardiology", result.Speciality);
		}

		[Fact]
		public async Task Handle_DoctorWithoutSpeciality_ReturnsNullSpeciality()
		{
			var dentist = TestData.Profession("Dentist");
			var doctor = new Doctor("Ana", "Martínez", dentist.Id, null, new Email("ana@example.com"), new PhoneNumber("5551234"), "xxxxx");
			doctor.AssignCatalog(dentist, null);
			_repository.GetById(doctor.Id).Returns(Task.FromResult<Doctor?>(doctor));

			var result = await new GetDoctorDetailHandler(_repository).Handle(new GetDoctorDetailQuery { Id = doctor.Id });

			Assert.Equal("Dentist", result.Profession);
			Assert.Null(result.SpecialityId);
			Assert.Null(result.Speciality);
		}

		[Fact]
		public async Task Handle_CatalogNotLoaded_FailsLoudlyInsteadOfReturningBlankNames()
		{
			// A repository that forgot to Include the catalog must be caught, not hidden.
			var doctor = new Doctor("Ana", "Martínez", Guid.NewGuid(), null, new Email("ana@example.com"), new PhoneNumber("5551234"), "xxxxx");
			_repository.GetById(doctor.Id).Returns(Task.FromResult<Doctor?>(doctor));

			await Assert.ThrowsAsync<InvalidOperationException>(() =>
				new GetDoctorDetailHandler(_repository).Handle(new GetDoctorDetailQuery { Id = doctor.Id }));
 		}

		[Fact]
		public async Task Handle_InvalidId_ThrowsNotFoundException()
		{
			var query = new GetDoctorDetailQuery { Id = Guid.NewGuid() };
			_repository.GetById(query.Id).Returns(Task.FromResult<Doctor?>(null));

			var handler = new GetDoctorDetailHandler(_repository);

			await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(query));
		}
	}
}
