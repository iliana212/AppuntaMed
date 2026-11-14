using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.UseCases.Doctors.Queries.GetDoctorsList;
using AppuntaMed.Domain.Entities;
using NSubstitute;

namespace AppuntaMed.Tests.Application.UseCases.Doctors
{
	public class GetDoctorsListTests
	{
		private readonly IDoctorRepository _repository = Substitute.For<IDoctorRepository>();
		private void ReturnDoctors(params Doctor[] doctors)
		{
			_repository.GetFilter(Arg.Any<DoctorFilterDTO>()).Returns(Task.FromResult(doctors.AsEnumerable()));
			_repository.CountFilter(Arg.Any<DoctorFilterDTO>()).Returns(Task.FromResult(doctors.Length));
		}


		[Fact]
		public async Task Handle_ReturnsPaginatedList_WhenDoctorsExist()
		{
			ReturnDoctors(TestData.Doctor("user-1"), TestData.Doctor("user-2"));

			var result = await new GetDoctorsListHandler(_repository).Handle(new GetDoctorsListQuery { Page = 1, PageSize = 2 });
			
			Assert.Equal(2, result.Total);
			Assert.Equal(2, result.Items.Count);
			await _repository.Received(1).GetFilter(Arg.Any<DoctorFilterDTO>());
			await _repository.Received(1).CountFilter(Arg.Any<DoctorFilterDTO>());
		}

		[Fact]
		public async Task Handle_ReturnsEmptyList_WhenNoDoctorsExist()
		{
			ReturnDoctors();
			var result = await new GetDoctorsListHandler(_repository).Handle(new GetDoctorsListQuery { Page = 1, PageSize = 10 });
						
			Assert.Empty(result.Items);
			Assert.Equal(0, result.Total);
		}
				
		[Fact]
		public async Task Handle_MapsDoctorsToDTOsCorrectly_IncludingCatalogNames()
		{
			var doctor = TestData.Doctor();
			ReturnDoctors(doctor);

			var result = await new GetDoctorsListHandler(_repository).Handle(new GetDoctorsListQuery { Page = 1, PageSize = 2 });

			var dto = Assert.Single(result.Items);
			Assert.Equal(doctor.FirstName, dto.FirstName);
			Assert.Equal(doctor.LastName, dto.LastName);
			Assert.Equal(doctor.ProfessionId, dto.ProfessionId);
			Assert.Equal(doctor.Profession.Name, dto.Profession);
			Assert.Equal(doctor.SpecialityId, dto.SpecialityId);
			Assert.Equal(doctor.Speciality!.Name, dto.Speciality);
			Assert.Equal(doctor.Email.Value, dto.Email);
			Assert.Equal(doctor.PhoneNumber.Value, dto.PhoneNumber);
		}

		[Fact]
		public async Task Handle_ForwardsAllFiltersToTheRepository_AndCountsWithTheSameFilter()
		{
			ReturnDoctors();
			var professionId = Guid.NewGuid();
			var specialityId = Guid.NewGuid();
			var query = new GetDoctorsListQuery
            {
				Page = 2, PageSize = 5, DoctorName = "an", ProfessionId = professionId, SpecialityId = specialityId
			};

			await new GetDoctorsListHandler(_repository).Handle(query);

			// The total must respect the filters too, otherwise pagination headers lie.
			await _repository.Received(1).GetFilter(Arg.Is<DoctorFilterDTO>(f => f.Page == 2 && f.PageSize == 5 && f.DoctorName == "an"
				&& f.ProfessionId == professionId && f.SpecialityId == specialityId));
			await _repository.Received(1).CountFilter(Arg.Is<DoctorFilterDTO>(f =>
				f.DoctorName == "an" && f.ProfessionId == professionId && f.SpecialityId == specialityId));
			await _repository.DidNotReceive().CountAll();
		}
	}
}
