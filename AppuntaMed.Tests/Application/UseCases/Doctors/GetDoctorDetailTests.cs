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
		public async Task Handle_ValidId_ReturnsDoctorDetailDTO()
		{
			var doctor = new Doctor("Ana", "Martínez", "Cardiology", "Heart Surgery", new Email("ana@example.com"), new PhoneNumber("5551234"), "xxxxx");
			_repository.GetById(doctor.Id)!.Returns(Task.FromResult(doctor));

			var handler = new GetDoctorDetailHandler(_repository);
			var query = new GetDoctorDetailQuery { Id = doctor.Id };

			var result = await handler.Handle(query);

			Assert.NotNull(result);
			Assert.Equal($"{doctor.FirstName} {doctor.LastName}", result.Name);
			Assert.Equal(doctor.Email.Value, result.Email);
			Assert.Equal(doctor.PhoneNumber.Value, result.PhoneNumber);
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
