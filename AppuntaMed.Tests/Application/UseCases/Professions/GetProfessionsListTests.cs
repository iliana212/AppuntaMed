using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.UseCases.Professions.Queries.GetProfessionsList;
using AppuntaMed.Domain.Entities;
using NSubstitute;

namespace AppuntaMed.Tests.Application.UseCases.Professions
{
	public class GetProfessionsListTests
	{
		private readonly IProfessionRepository _repository = Substitute.For<IProfessionRepository>();

		[Fact]
		public async Task Handle_ReturnsEachProfessionWithItsSpecialities()
		{
			var dentist = TestData.Profession("Dentist", "Orthodontics", "Endodontics");
			var physician = TestData.Profession("Physician", "Cardiology");
			_repository.GetAllWithSpecialities().Returns(Task.FromResult<IEnumerable<Profession>>(new[] { dentist, physician }));

			var result = await new GetProfessionsListHandler(_repository).Handle(new GetProfessionsListQuery());

			Assert.Equal(2, result.Count);
			Assert.Equal("Dentist", result[0].Name);
			Assert.Equal(dentist.Id, result[0].Id);
			Assert.Equal(new[] { "Orthodontics", "Endodontics" }, result[0].Specialities.Select(s => s.Name));
			Assert.Equal("Cardiology", Assert.Single(result[1].Specialities).Name);
		}

		[Fact]
		public async Task Handle_ProfessionWithoutSpecialities_ReturnsEmptySpecialityList()
		{
			_repository.GetAllWithSpecialities()
				.Returns(Task.FromResult<IEnumerable<Profession>>(new[] { TestData.Profession("Nutritionist") }));

			var result = await new GetProfessionsListHandler(_repository).Handle(new GetProfessionsListQuery());

			Assert.Empty(Assert.Single(result).Specialities);
		}

		[Fact]
        public async Task Handle_EmptyCatalog_ReturnsEmptyList()
        {
			_repository.GetAllWithSpecialities().Returns(Task.FromResult(Enumerable.Empty<Profession>()));
	
			var result = await new GetProfessionsListHandler(_repository).Handle(new GetProfessionsListQuery());
	
			Assert.Empty(result);
	    }
	}
}
