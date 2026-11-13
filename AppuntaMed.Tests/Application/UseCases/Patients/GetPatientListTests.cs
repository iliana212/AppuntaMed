using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.UseCases.Patients.Queries.GetPatientsList;
using AppuntaMed.Domain.Entities;
using AppuntaMed.Domain.ValueObjects;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Tests.Application.UseCases.Patients
{
	public class GetPatientListTests
	{
		private readonly IPatientRepository _repository = Substitute.For<IPatientRepository>();

		[Fact]
		public async Task Handle_ReturnsPaginatedList_WhenPatientsExist()
		{
			// Arrange
			var patients = new List<Patient>
			{
				new Patient("Ana", "López", new DateTime(1990, 1, 1), new PhoneNumber("5551234"), new Email("ana@example.com"), "xxxxx"),
				new Patient("Carlos", "Pérez", new DateTime(1985, 5, 5), new PhoneNumber("5555678"), new Email("carlos@example.com"), "xxxxx")
			};

			_repository.GetFilter(Arg.Any<PatientFilterDTO>()).Returns(Task.FromResult(patients.AsEnumerable()));
			_repository.CountAll().Returns(Task.FromResult(2));

			var handler = new GetPatientsListHandler(_repository);
			var query = new GetPatientsListQuery { Page = 1, PageSize = 2 };

			// Act
			var result = await handler.Handle(query);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(2, result.Total);
			Assert.Equal(2, result.Items.Count);
			await _repository.Received(1).GetFilter(Arg.Any<PatientFilterDTO>());
			await _repository.Received(1).CountAll();
		}

		[Fact]
		public async Task Handle_ReturnsEmptyList_WhenNoPatientsExist()
		{
			// Arrange
			_repository.GetFilter(Arg.Any<PatientFilterDTO>()).Returns(Task.FromResult(Enumerable.Empty<Patient>()));
			_repository.CountAll().Returns(Task.FromResult(0));

			var handler = new GetPatientsListHandler(_repository);
			var query = new GetPatientsListQuery { Page = 1, PageSize = 10 };

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
			_repository.GetFilter(Arg.Any<PatientFilterDTO>()).Returns(Task.FromResult(Enumerable.Empty<Patient>()));
			_repository.CountAll().Returns(Task.FromResult(0));

			var handler = new GetPatientsListHandler(_repository);
			var query = new GetPatientsListQuery { Page = 1, PageSize = 5 };

			// Act
			await handler.Handle(query);

			// Assert
			await _repository.Received(1).GetFilter(Arg.Any<PatientFilterDTO>());
			await _repository.Received(1).CountAll();
		}

		[Fact]
		public async Task Handle_MapsPatientsToDTOsCorrectly()
		{
			// Arrange
			var patient = new Patient("Ana", "López", new DateTime(1990, 1, 1),
				new PhoneNumber("5551234"), new Email("ana@example.com"), "xxxxx");

			_repository.GetFilter(Arg.Any<PatientFilterDTO>()).Returns(Task.FromResult(new List<Patient> { patient }.AsEnumerable()));
			_repository.CountAll().Returns(Task.FromResult(1));

			var handler = new GetPatientsListHandler(_repository);
			var query = new GetPatientsListQuery { Page = 1, PageSize = 2 };

			// Act
			var result = await handler.Handle(query);

			// Assert
			Assert.Single(result.Items);
			var dto = result.Items.First();

			Assert.Equal(patient.FirstName, dto.FirstName);
			Assert.Equal(patient.LastName, dto.LastName);
			Assert.Equal(patient.Email.Value, dto.Email);
			Assert.Equal(patient.PhoneNumber.Value, dto.PhoneNumber);
		}
	}
}
