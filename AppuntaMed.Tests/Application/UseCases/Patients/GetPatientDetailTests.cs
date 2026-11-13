using AppuntaMed.Application.Exceptions;
using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.Interfaces.Security;
using AppuntaMed.Application.UseCases.Patients.Queries.GetPatientDetail;
using AppuntaMed.Domain.Entities;
using AppuntaMed.Domain.ValueObjects;
using NSubstitute;

namespace AppuntaMed.Tests.Application.UseCases.Patients
{
	public class GetPatientDetailTests
	{
		private readonly IPatientRepository _repository = Substitute.For<IPatientRepository>();
		private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();

		[Fact]
		public async Task Handle_ValidId_ReturnsPatientDetailDTO_WithFullName()
		{
			// Arrange
			var patient = new Patient("Ana", "López", new DateTime(1990, 1, 1),
				new PhoneNumber("5551234"), new Email("ana@example.com"), "xxxxx");

			_repository.GetById(patient.Id)!.Returns(Task.FromResult(patient));

			var handler = new GetPatientDetailHandler(_repository, _currentUser);
			var query = new GetPatientDetailQuery { Id = patient.Id };

			// Act
			var result = await handler.Handle(query);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(patient.FirstName, result.FirstName);
			Assert.Equal(patient.LastName, result.LastName);
			Assert.Equal(patient.Email.Value, result.Email);
			Assert.Equal(patient.PhoneNumber.Value, result.PhoneNumber);
			Assert.Equal($"{patient.FirstName} {patient.LastName}", result.Name);

			await _repository.Received(1).GetById(patient.Id);
		}

		[Fact]
		public async Task Handle_InvalidId_ThrowsNotFoundException()
		{
			// Arrange
			var query = new GetPatientDetailQuery { Id = Guid.NewGuid() };
			_repository.GetById(query.Id).Returns(Task.FromResult<Patient?>(null));

			var handler = new GetPatientDetailHandler(_repository, _currentUser);

			// Act & Assert
			await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(query));
			await _repository.Received(1).GetById(query.Id);
		}
	}
}
