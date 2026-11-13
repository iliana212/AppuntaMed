using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.UseCases.Doctors.Queries.GetDoctorsList;
using AppuntaMed.Domain.Entities;
using AppuntaMed.Domain.ValueObjects;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Tests.Application.UseCases.Doctors
{
	public class GetDoctorsListTests
	{
		private readonly IDoctorRepository _repository = Substitute.For<IDoctorRepository>();

		[Fact]
		public async Task Handle_ReturnsPaginatedList_WhenDoctorsExist()
		{
			// Arrange
			var filter = new DoctorFilterDTO { Page = 1, PageSize = 2 };
			var doctors = new List<Doctor>
		{
			new Doctor("Ana", "Martínez", "Cardiology", "Heart Surgery", new Email("ana@example.com"), new PhoneNumber("5551234"), "xxxxx"),
			new Doctor("Carlos", "Pérez", "Neurology", "Epilepsy", new Email("carlos@example.com"), new PhoneNumber("5555678"), "xxxxx")
		};

			_repository.GetFilter(Arg.Any<DoctorFilterDTO>())
				.Returns(Task.FromResult(doctors.AsEnumerable()));
			_repository.CountAll().Returns(Task.FromResult(2));

			var handler = new GetDoctorsListHandler(_repository);
			var query = new GetDoctorsListQuery { Page = 1, PageSize = 2 };

			// Act
			var result = await handler.Handle(query);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(2, result.Total);
			Assert.Equal(2, result.Items.Count);
			await _repository.Received(1).GetFilter(Arg.Any<DoctorFilterDTO>());
			await _repository.Received(1).CountAll();
		}

		[Fact]
		public async Task Handle_ReturnsEmptyList_WhenNoDoctorsExist()
		{
			// Arrange
			_repository.GetFilter(Arg.Any<DoctorFilterDTO>())
				.Returns(Task.FromResult(Enumerable.Empty<Doctor>()));
			_repository.CountAll().Returns(Task.FromResult(0));

			var handler = new GetDoctorsListHandler(_repository);
			var query = new GetDoctorsListQuery { Page = 1, PageSize = 10 };

			// Act
			var result = await handler.Handle(query);

			// Assert
			Assert.NotNull(result);
			Assert.Empty(result.Items);
			Assert.Equal(0, result.Total);
		}

		[Fact]
		public async Task Handle_CallsRepositoryMethodsOnce()
		{
			// Arrange
			_repository.GetFilter(Arg.Any<DoctorFilterDTO>())
				.Returns(Task.FromResult(Enumerable.Empty<Doctor>()));
			_repository.CountAll().Returns(Task.FromResult(0));

			var handler = new GetDoctorsListHandler(_repository);
			var query = new GetDoctorsListQuery { Page = 1, PageSize = 5 };

			// Act
			await handler.Handle(query);

			// Assert
			await _repository.Received(1).GetFilter(Arg.Any<DoctorFilterDTO>());
			await _repository.Received(1).CountAll();
		}

		[Fact]
		public async Task Handle_MapsDoctorsToDTOsCorrectly()
		{
			// Arrange
			var doctor = new Doctor("Ana", "Martínez", "Cardiology", "Heart Surgery", new Email("ana@example.com"), new PhoneNumber("5551234"), "xxxxx");

			_repository.GetFilter(Arg.Any<DoctorFilterDTO>())
				.Returns(Task.FromResult(new List<Doctor> { doctor }.AsEnumerable()));
			_repository.CountAll().Returns(Task.FromResult(1));

			var handler = new GetDoctorsListHandler(_repository);
			var query = new GetDoctorsListQuery { Page = 1, PageSize = 2 };

			// Act
			var result = await handler.Handle(query);

			// Assert
			Assert.Single(result.Items);
			var dto = result.Items.First();

			Assert.Equal(doctor.FirstName, dto.FirstName);
			Assert.Equal(doctor.LastName, dto.LastName);
			Assert.Equal(doctor.Profession, dto.Profession);
			Assert.Equal(doctor.Speciality, dto.Speciality);
			Assert.Equal(doctor.Email.Value, dto.Email);
			Assert.Equal(doctor.PhoneNumber.Value, dto.PhoneNumber);
		}
	}
}
