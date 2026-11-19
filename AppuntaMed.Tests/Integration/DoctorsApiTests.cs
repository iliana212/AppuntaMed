using AppuntaMed.Application.UseCases.Doctors.Queries.GetDoctorsList;
using AppuntaMed.Domain.Entities;
using AppuntaMed.Persistence.Seeding;
using AppuntaMed.Tests.Integration.Infrastructure;
using System.Net;
using System.Net.Http.Json;

namespace AppuntaMed.Tests.Integration
{
	[Collection(ApiCollection.Name)]
	[Trait("Category", "Integration")]
	public sealed class DoctorsApiTests(ApiFactory factory) : IntegrationTestBase(factory)
	{
		private static string Url(Guid id) => $"/api/doctors/{id}";

		private static List<Guid> DemoDoctorIds(Func<Doctor, bool> predicate) =>
			DemoData.Build(DateTime.UtcNow).Doctors.Where(seed => predicate(seed.Entity)).Select(seed => seed.Id).Order().ToList();

		// ------------------------------------------------------------ read one

		[Fact]
		public async Task Get_ReturnsTheDoctor_WithProfessionAndSpecialityNames()
		{
			var response = await ClientFor(TestUser.Ana).GetAsync(Url(DemoData.DrCarlosId));

			Assert.Equal(HttpStatusCode.OK, response.StatusCode);

			var body = await JsonOf(response);
			Assert.Equal("Physician", body.GetProperty("profession").GetString());
			Assert.Equal("Cardiology", body.GetProperty("speciality").GetString());
			Assert.Equal(CatalogData.CardiologyId, body.GetProperty("specialityId").GetGuid());
		}

		[Fact]
		public async Task Get_DoctorWithoutSpeciality_ReturnsNullSpeciality()
		{
			var (_, doctorId, _) = await NewDoctorAsync(CatalogData.NutritionistId, specialityId: null);

			var body = await JsonOf(await ClientFor(TestUser.Ana).GetAsync(Url(doctorId)));

			Assert.Equal("Nutritionist", body.GetProperty("profession").GetString());
			Assert.Equal(System.Text.Json.JsonValueKind.Null, body.GetProperty("speciality").ValueKind);
		}

		[Fact]
		public async Task Get_UnknownDoctor_Returns404Problem()
		{
			var response = await ClientFor(TestUser.Ana).GetAsync(Url(Guid.NewGuid()));

			await AssertProblem(response, HttpStatusCode.NotFound);
		}

		[Fact]
		public async Task Get_MalformedId_Returns400()
		{
			var response = await ClientFor(TestUser.Ana).GetAsync("/api/doctors/42");

			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
		}

		// ------------------------------------------------------------ list: filters and paging

		[Fact]
		public async Task List_FilteredByProfession_ReturnsOnlyThatProfession_AndTheTotalMatches()
		{
			var response = await ClientFor(TestUser.Ana).GetAsync($"/api/doctors?professionId={CatalogData.DentistId}");
			var doctors = await ReadAs<List<DoctorsListDTO>>(response);

			Assert.Equal(DemoDoctorIds(d => d.ProfessionId == CatalogData.DentistId), doctors.Select(d => d.Id).Order().ToList());
			Assert.All(doctors, d => Assert.Equal("Dentist", d.Profession));
			Assert.Equal(doctors.Count, TotalItems(response));
		}

		[Fact]
		public async Task List_FilteredBySpeciality_ReturnsOnlyThatSpeciality()
		{
			var response = await ClientFor(TestUser.Ana).GetAsync($"/api/doctors?specialityId={CatalogData.OrthodonticsId}");

			var doctors = await ReadAs<List<DoctorsListDTO>>(response);

			Assert.Equal(DemoDoctorIds(d => d.SpecialityId == CatalogData.OrthodonticsId), doctors.Select(x => x.Id).Order().ToList());
			Assert.Contains(doctors, x => x.Id == DemoData.DrLauraId);
			Assert.All(doctors, x => Assert.Equal("Orthodontics", x.Speciality));
		}

		[Fact]
		public async Task List_FilteredByProfessionAndAnotherProfessionsSpeciality_ReturnsNothing()
		{
			var response = await ClientFor(TestUser.Ana).GetAsync(
				$"/api/doctors?professionId={CatalogData.DentistId}&specialityId={CatalogData.CardiologyId}");

			Assert.Empty(await ReadAs<List<DoctorsListDTO>>(response));
			Assert.Equal(0, TotalItems(response));
		}

		[Fact]
		public async Task List_FilteredByName_TheTotalRespectsTheFilter()
		{
			var (_, _, firstName) = await NewDoctorAsync(CatalogData.PsychologistId, CatalogData.ClinicalPsychologyId);

			var response = await ClientFor(TestUser.Ana).GetAsync($"/api/doctors?doctorName={firstName}");

			var doctor = Assert.Single(await ReadAs<List<DoctorsListDTO>>(response));
			Assert.Equal(firstName, doctor.FirstName);
			Assert.Equal(1, TotalItems(response));
		}

		[Fact]
		public async Task List_PageSizeOne_ReturnsOneDoctor_WithTheTotalOfAll()
		{
			var response = await ClientFor(TestUser.Ana).GetAsync("/api/doctors?page=1&pageSize=1");

			Assert.Single(await ReadAs<List<DoctorsListDTO>>(response));
			Assert.True(TotalItems(response) >= 2);
		}

		[Fact]
		public async Task List_PageBeyondTheLastOne_ReturnsAnEmptyList()
		{
			var response = await ClientFor(TestUser.Ana).GetAsync("/api/doctors?page=1000");

			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			Assert.Empty(await ReadAs<List<DoctorsListDTO>>(response));
		}

		[Theory]
		[InlineData("page=0")]
		[InlineData("pageSize=0")]
		[InlineData("pageSize=101")]
		public async Task List_InvalidPaging_Returns400WithErrors(string query)
		{
			var response = await ClientFor(TestUser.Ana).GetAsync($"/api/doctors?{query}");

			var problem = await AssertProblem(response, HttpStatusCode.BadRequest);
			Assert.NotEmpty(ErrorsOf(problem));
		}

		// ------------------------------------------------------------ create

		[Fact]
		public async Task Create_WithProfessionAndSpeciality_StoresBoth()
		{
			var (_, doctorId, _) = await NewDoctorAsync(CatalogData.PsychologistId, CatalogData.NeuropsychologyId);

			var body = await JsonOf(await ClientFor(TestUser.Ana).GetAsync(Url(doctorId)));

			Assert.Equal("Psychologist", body.GetProperty("profession").GetString());
			Assert.Equal("Neuropsychology", body.GetProperty("speciality").GetString());
		}

		[Fact]
		public async Task Create_SpecialityOfAnotherProfession_Returns400_AndCreatesNothing()
		{
			var user = TestUser.NewDoctor();

			var response = await ClientFor(user).PostAsJsonAsync("/api/doctors", new
			{
				firstName = "Wrong",
				lastName = "Match",
				professionId = CatalogData.PsychologistId,
				specialityId = CatalogData.OrthodonticsId,     // belongs to Dentist
				email = "wrong.match@appuntamed.test",
				phoneNumber = "5559876"
			}, Json);

			var problem = await AssertProblem(response, HttpStatusCode.BadRequest);
			Assert.Contains("does not belong", problem.GetProperty("detail").GetString());
			Assert.Null(await DoctorIdOf(user.UserId));
		}

		[Fact]
		public async Task Create_UnknownProfession_Returns400()
		{
			var response = await ClientFor(TestUser.NewDoctor()).PostAsJsonAsync("/api/doctors", new
			{
				firstName = "No",
				lastName = "Profession",
				professionId = Guid.NewGuid(),
				email = "no.profession@appuntamed.test",
				phoneNumber = "5559876"
			}, Json);

			var problem = await AssertProblem(response, HttpStatusCode.BadRequest);
			Assert.Contains("does not exist", problem.GetProperty("detail").GetString());
		}

		[Fact]
		public async Task Create_EmptyProfessionId_Returns400_WithTheValidationErrors()
		{
			var response = await ClientFor(TestUser.NewDoctor()).PostAsJsonAsync("/api/doctors", new
			{
				firstName = "No",
				lastName = "Profession",
				professionId = Guid.Empty,
				email = "empty.profession@appuntamed.test",
				phoneNumber = "5559876"
			}, Json);

			var problem = await AssertProblem(response, HttpStatusCode.BadRequest);
			Assert.Contains(ErrorsOf(problem), e => e.Contains("required"));
		}

		[Fact]
		public async Task Create_AccountThatAlreadyHasADoctorProfile_Returns400()
		{
			var (user, _, _) = await NewDoctorAsync(CatalogData.PsychologistId);

			var response = await ClientFor(user).PostAsJsonAsync("/api/doctors", new
			{
				firstName = "Second",
				lastName = "Profile",
				professionId = CatalogData.PsychologistId,
				email = "second.profile@appuntamed.test",
				phoneNumber = "5559876"
			}, Json);

			await AssertProblem(response, HttpStatusCode.BadRequest);
		}

		[Fact]
		public async Task Create_Patient_HasNoPermissionToCreateDoctors()
		{
			var response = await ClientFor(TestUser.Ana).PostAsJsonAsync("/api/doctors", new
			{
				firstName = "X",
				lastName = "Y",
				professionId = CatalogData.PsychologistId,
				email = "x@appuntamed.test",
				phoneNumber = "5559876"
			}, Json);

			Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
		}

		// ------------------------------------------------------------ update

		[Fact]
		public async Task Update_Doctor_ChangesProfessionAndSpecialityTogether()
		{
			var (user, doctorId, _) = await NewDoctorAsync(CatalogData.PsychologistId, CatalogData.ClinicalPsychologyId);

			var response = await ClientFor(user).PutAsJsonAsync(Url(doctorId), new
			{
				firstName = "Moved",
				lastName = "Doctor",
				professionId = CatalogData.NutritionistId,
				specialityId = CatalogData.SportsNutritionId,
				email = "moved@appuntamed.test",
				phoneNumber = "5559876"
			}, Json);

			Assert.Equal(HttpStatusCode.OK, response.StatusCode);

			var body = await JsonOf(await ClientFor(TestUser.Ana).GetAsync(Url(doctorId)));
			Assert.Equal("Nutritionist", body.GetProperty("profession").GetString());
			Assert.Equal("Sports Nutrition", body.GetProperty("speciality").GetString());
		}

		[Fact]
		public async Task Update_NewProfessionKeepingTheOldSpeciality_Returns400_AndKeepsTheData()
		{
			var (user, doctorId, _) = await NewDoctorAsync(CatalogData.PsychologistId, CatalogData.ClinicalPsychologyId);

			var response = await ClientFor(user).PutAsJsonAsync(Url(doctorId), new
			{
				firstName = "Doc",
				lastName = "Doctor",
				professionId = CatalogData.NutritionistId,
				specialityId = CatalogData.ClinicalPsychologyId,     // still the old profession's speciality
				email = "keep@appuntamed.test",
				phoneNumber = "5559876"
			}, Json);

			await AssertProblem(response, HttpStatusCode.BadRequest);

			var body = await JsonOf(await ClientFor(TestUser.Ana).GetAsync(Url(doctorId)));
			Assert.Equal("Psychologist", body.GetProperty("profession").GetString());
		}

		[Fact]
		public async Task Update_Doctor_CannotChangeAnotherDoctor_Returns403()
		{
			var (intruder, _, _) = await NewDoctorAsync(CatalogData.PsychologistId);

			var response = await ClientFor(intruder).PutAsJsonAsync(Url(DemoData.DrCarlosId), new
			{
				firstName = "Hacked",
				lastName = "Hacked",
				professionId = CatalogData.PsychologistId,
				email = "hacked@appuntamed.test",
				phoneNumber = "5559876"
			}, Json);

			await AssertProblem(response, HttpStatusCode.Forbidden);

			var body = await JsonOf(await ClientFor(TestUser.Ana).GetAsync(Url(DemoData.DrCarlosId)));
			Assert.Equal("Physician", body.GetProperty("profession").GetString());
		}

		// ------------------------------------------------------------ delete

		[Fact]
		public async Task Delete_Admin_RemovesADoctorWithoutAppointments()
		{
			var (_, doctorId, _) = await NewDoctorAsync(CatalogData.PsychologistId);

			var response = await ClientFor(TestUser.Admin).DeleteAsync(Url(doctorId));

			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			await AssertProblem(await ClientFor(TestUser.Ana).GetAsync(Url(doctorId)), HttpStatusCode.NotFound);
		}

		[Fact]
		public async Task Delete_DoctorWithAppointments_Returns400_AndKeepsTheDoctor()
		{
			var response = await ClientFor(TestUser.Admin).DeleteAsync(Url(DemoData.DrCarlosId));

			await AssertProblem(response, HttpStatusCode.BadRequest);
			Assert.Equal(HttpStatusCode.OK, (await ClientFor(TestUser.Ana).GetAsync(Url(DemoData.DrCarlosId))).StatusCode);
		}

		[Theory]
		[InlineData("Doctor")]
		[InlineData("Patient")]
		public async Task Delete_OnlyAdminsCanDeleteDoctors(string role)
		{
			var user = TestUser.DrLaura with { Role = role };

			var response = await ClientFor(user).DeleteAsync(Url(DemoData.DrCarlosId));

			Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
		}
	}
}