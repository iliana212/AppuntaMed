using AppuntaMed.Application.UseCases.Patients.Queries.GetPatientsList;
using AppuntaMed.Persistence.Seeding;
using AppuntaMed.Tests.Integration.Infrastructure;
using System.Net;
using System.Net.Http.Json;

namespace AppuntaMed.Tests.Integration
{
	[Collection(ApiCollection.Name)]
	[Trait("Category", "Integration")]
	public sealed class PatientsApiTests(ApiFactory factory) : IntegrationTestBase(factory)
	{
		private static string Url(Guid id) => $"/api/patients/{id}";

		// ------------------------------------------------------------ read one

		[Fact]
		public async Task Get_Patient_ReadsHerOwnProfile()
		{
			var response = await ClientFor(TestUser.Ana).GetAsync(Url(DemoData.AnaPatientId));

			Assert.Equal(HttpStatusCode.OK, response.StatusCode);

			var body = await JsonOf(response);
			Assert.Equal("Ana", body.GetProperty("firstName").GetString());
			Assert.Equal("López", body.GetProperty("lastName").GetString());
		}

		[Fact]
		public async Task Get_Patient_CannotReadAnotherPatient_Returns403Problem()
		{
			var response = await ClientFor(TestUser.Ana).GetAsync(Url(DemoData.BetoPatientId));

			await AssertProblem(response, HttpStatusCode.Forbidden);
		}

		[Fact]
		public async Task Get_Admin_ReadsAnyPatient()
		{
			var response = await ClientFor(TestUser.Admin).GetAsync(Url(DemoData.BetoPatientId));

			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			Assert.Equal("Beto", (await JsonOf(response)).GetProperty("firstName").GetString());
		}

		[Fact]
		public async Task Get_UnknownPatient_Returns404Problem()
		{
			var response = await ClientFor(TestUser.Admin).GetAsync(Url(Guid.NewGuid()));

			await AssertProblem(response, HttpStatusCode.NotFound);
		}

		[Fact]
		public async Task Get_MalformedId_Returns400()
		{
			var response = await ClientFor(TestUser.Admin).GetAsync("/api/patients/not-a-guid");

			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
		}

		[Fact]
		public async Task Get_Doctor_HasNoPermissionToReadAPatientProfile()
		{
			var response = await ClientFor(TestUser.DrCarlos).GetAsync(Url(DemoData.AnaPatientId));

			Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
		}

		// ------------------------------------------------------------ list, filter and paging

		[Fact]
		public async Task List_FilteredByName_ReturnsOnlyMatches_AndTheTotalRespectsTheFilter()
		{
			var response = await ClientFor(TestUser.Admin).GetAsync($"/api/patients?patientName={Uri.EscapeDataString("Ramírez")}");

			Assert.Equal(HttpStatusCode.OK, response.StatusCode);

			var patients = await ReadAs<List<PatientsListDTO>>(response);
			Assert.Equal("Beto", Assert.Single(patients).FirstName);
			Assert.Equal(1, TotalItems(response));
		}

		[Fact]
		public async Task List_FilterWithoutMatches_ReturnsAnEmptyList_AndZeroTotal()
		{
			var response = await ClientFor(TestUser.Admin).GetAsync($"/api/patients?patientName=zzz{Guid.NewGuid():N}");

			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			Assert.Empty(await ReadAs<List<PatientsListDTO>>(response));
			Assert.Equal(0, TotalItems(response));
		}

		[Fact]
		public async Task List_PageSizeOne_ReturnsOneItem_WithTheTotalOfAllPatients()
		{
			var response = await ClientFor(TestUser.Admin).GetAsync("/api/patients?page=1&pageSize=1");

			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			Assert.Single(await ReadAs<List<PatientsListDTO>>(response));
			Assert.True(TotalItems(response) >= 2);
		}

		[Fact]
		public async Task List_TwoConsecutivePages_DoNotRepeatPatients()
		{
			var first = await ReadAs<List<PatientsListDTO>>(await ClientFor(TestUser.Admin).GetAsync("/api/patients?page=1&pageSize=1"));
			var second = await ReadAs<List<PatientsListDTO>>(await ClientFor(TestUser.Admin).GetAsync("/api/patients?page=2&pageSize=1"));

			Assert.NotEqual(Assert.Single(first).Id, Assert.Single(second).Id);
		}

		[Fact]
		public async Task List_PageBeyondTheLastOne_ReturnsAnEmptyList()
		{
			var response = await ClientFor(TestUser.Admin).GetAsync("/api/patients?page=1000&pageSize=10");

			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			Assert.Empty(await ReadAs<List<PatientsListDTO>>(response));
			Assert.True(TotalItems(response) >= 2);
		}

		[Fact]
		public async Task List_MaximumPageSize_IsAccepted()
		{
			var response = await ClientFor(TestUser.Admin).GetAsync("/api/patients?pageSize=100");

			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		}

		[Theory]
		[InlineData("page=0")]
		[InlineData("page=-1")]
		[InlineData("pageSize=0")]
		[InlineData("pageSize=-5")]
		[InlineData("pageSize=101")]
		public async Task List_InvalidPaging_Returns400WithErrors(string query)
		{
			var response = await ClientFor(TestUser.Admin).GetAsync($"/api/patients?{query}");

			var problem = await AssertProblem(response, HttpStatusCode.BadRequest);
			Assert.NotEmpty(ErrorsOf(problem));
		}

		[Fact]
		public async Task List_Patient_HasNoPermissionToListPatients()
		{
			var response = await ClientFor(TestUser.Ana).GetAsync("/api/patients");

			Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
		}

		// ------------------------------------------------------------ create

		[Fact]
		public async Task Create_NewAccount_CreatesItsProfile_AndCanReadItBack()
		{
			var (user, patientId, firstName) = await NewPatientAsync();

			var response = await ClientFor(user).GetAsync(Url(patientId));

			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			Assert.Equal(firstName, (await JsonOf(response)).GetProperty("firstName").GetString());
		}

		[Fact]
		public async Task Create_AccountThatAlreadyHasAProfile_Returns400()
		{
			var response = await ClientFor(TestUser.Ana).PostAsJsonAsync("/api/patients", new
			{
				firstName = "Another",
				lastName = "Profile",
				dateOfBirth = new DateTime(1990, 1, 1),
				email = "another@appuntamed.test",
				phoneNumber = "5551234"
			}, Json);

			await AssertProblem(response, HttpStatusCode.BadRequest);
		}

		[Fact]
		public async Task Create_InvalidData_Returns400_ListingEveryError()
		{
			var response = await ClientFor(TestUser.NewPatient()).PostAsJsonAsync("/api/patients", new
			{
				firstName = "Valid",
				lastName = "Name",
				dateOfBirth = DateTime.UtcNow.Date.AddYears(1),   // in the future
				email = "valid@appuntamed.test",
				phoneNumber = "abc"                               // not a phone number
			}, Json);

			var problem = await AssertProblem(response, HttpStatusCode.BadRequest);
			Assert.True(ErrorsOf(problem).Count >= 2, "Both the birth date and the phone number must be reported");
		}

		[Fact]
		public async Task Create_WithoutAuthentication_Returns401()
		{
			var response = await Anonymous().PostAsJsonAsync("/api/patients", new { firstName = "A" }, Json);

			Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
		}

		// ------------------------------------------------------------ update

		[Fact]
		public async Task Update_Patient_ChangesHerOwnProfile()
		{
			var (user, patientId, _) = await NewPatientAsync();

			var response = await ClientFor(user).PutAsJsonAsync(Url(patientId), new
			{
				firstName = "Renamed",
				lastName = "Patient",
				dateOfBirth = new DateTime(1992, 3, 4),
				email = "renamed@appuntamed.test",
				phoneNumber = "5551234"
			}, Json);

			Assert.Equal(HttpStatusCode.OK, response.StatusCode);

			var body = await JsonOf(await ClientFor(user).GetAsync(Url(patientId)));
			Assert.Equal("Renamed", body.GetProperty("firstName").GetString());
		}

		[Fact]
		public async Task Update_Patient_CannotChangeAnotherPatient_Returns403_AndKeepsTheData()
		{
			var (intruder, _, _) = await NewPatientAsync();

			var response = await ClientFor(intruder).PutAsJsonAsync(Url(DemoData.AnaPatientId), new
			{
				firstName = "Hacked",
				lastName = "Hacked",
				dateOfBirth = new DateTime(1990, 5, 14),
				email = "hacked@appuntamed.test",
				phoneNumber = "5551234"
			}, Json);

			await AssertProblem(response, HttpStatusCode.Forbidden);

			var ana = await JsonOf(await ClientFor(TestUser.Admin).GetAsync(Url(DemoData.AnaPatientId)));
			Assert.Equal("Ana", ana.GetProperty("firstName").GetString());
		}

		// ------------------------------------------------------------ delete

		[Fact]
		public async Task Delete_Admin_RemovesAPatientWithoutAppointments()
		{
			var (_, patientId, _) = await NewPatientAsync();

			var response = await ClientFor(TestUser.Admin).DeleteAsync(Url(patientId));

			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			await AssertProblem(await ClientFor(TestUser.Admin).GetAsync(Url(patientId)), HttpStatusCode.NotFound);
		}

		[Fact]
		public async Task Delete_PatientWithAppointments_Returns400_AndKeepsThePatient()
		{
			var response = await ClientFor(TestUser.Admin).DeleteAsync(Url(DemoData.AnaPatientId));

			var problem = await AssertProblem(response, HttpStatusCode.BadRequest);
			Assert.Contains("appointments", problem.GetProperty("detail").GetString());
			Assert.Equal(HttpStatusCode.OK, (await ClientFor(TestUser.Admin).GetAsync(Url(DemoData.AnaPatientId))).StatusCode);
		}

		[Fact]
		public async Task Delete_Patient_HasNoPermissionToDeletePatients()
		{
			var response = await ClientFor(TestUser.Ana).DeleteAsync(Url(DemoData.BetoPatientId));

			Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
		}

		[Fact]
		public async Task Delete_UnknownPatient_Returns404()
		{
			var response = await ClientFor(TestUser.Admin).DeleteAsync(Url(Guid.NewGuid()));

			await AssertProblem(response, HttpStatusCode.NotFound);
		}
	}
}