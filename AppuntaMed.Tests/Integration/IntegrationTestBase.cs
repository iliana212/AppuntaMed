using AppuntaMed.Domain.Enums;
using AppuntaMed.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AppuntaMed.Tests.Integration.Infrastructure;

namespace AppuntaMed.Tests.Integration
{
	public abstract class IntegrationTestBase(ApiFactory factory)
	{
		protected static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);
		protected ApiFactory Factory { get; } = factory;
		protected HttpClient ClientFor(TestUser user) => Factory.CreateClientFor(user);
		protected HttpClient Anonymous() => Factory.CreateClient();

		// ---------------------------------------------------------------- reading responses

		protected static async Task<JsonElement> JsonOf(HttpResponseMessage response)
		{
			using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
			return document.RootElement.Clone();
		}

		protected static async Task<T> ReadAs<T>(HttpResponseMessage response)
		{
			var value = await response.Content.ReadFromJsonAsync<T>(Json);
			return value!;
		}

		/// <summary>Checks the status AND the RFC 7807 shape (application/problemjson with the same status in the body).</summary>
		protected static async Task<JsonElement> AssertProblem(HttpResponseMessage response, HttpStatusCode expected)
		{
			Assert.Equal(expected, response.StatusCode);
			Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

			var body = await JsonOf(response);
			Assert.Equal((int)expected, body.GetProperty("status").GetInt32());

			return body;
		}

		/// <summary>The "errors" list written by the API's exception handler for validation failures.</summary>
		protected static List<string> ErrorsOf(JsonElement problem) =>
			problem.GetProperty("errors").EnumerateArray().Select(e => e.GetString()!).ToList();

		protected static int TotalItems(HttpResponseMessage response) =>
			int.Parse(response.Headers.GetValues("total-items").Single());

		// ---------------------------------------------------------------- database access (read the REAL state)

		protected async Task<T> InDb<T>(Func<AppuntaMedDbContext, Task<T>> action)
		{
			await using var scope = Factory.Services.CreateAsyncScope();
			var db = scope.ServiceProvider.GetRequiredService<AppuntaMedDbContext>();

			return await action(db);
		}

		protected Task<Guid?> PatientIdOf(string userId) =>
			InDb(db => db.Patients.Where(p => p.UserId == userId).Select(p => (Guid?)p.Id).SingleOrDefaultAsync());

		protected Task<Guid?> DoctorIdOf(string userId) =>
			InDb(db => db.Doctors.Where(d => d.UserId == userId).Select(d => (Guid?)d.Id).SingleOrDefaultAsync());

		protected Task<Guid?> AppointmentIdAt(Guid doctorId, DateTime start) =>
			InDb(db => db.Appointments
				.Where(a => a.DoctorId == doctorId && a.TimeInterval.Start == start)
				.Select(a => (Guid?)a.Id)
				.FirstOrDefaultAsync());

		protected Task<AppointmentStatus> StatusOf(Guid appointmentId) =>
			InDb(db => db.Appointments.Where(a => a.Id == appointmentId).Select(a => a.Status).SingleAsync());

		// ---------------------------------------------------------------- arrange: create data THROUGH the API

		protected async Task<(TestUser User, Guid PatientId, string FirstName)> NewPatientAsync()
		{
			var user = TestUser.NewPatient();
			var token = Guid.NewGuid().ToString("N")[..8];
			var firstName = $"Test{token}";

			var response = await ClientFor(user).PostAsJsonAsync("/api/patients", new
			{
				firstName,
				lastName = "Patient",
				dateOfBirth = new DateTime(1992, 3, 4),
				email = $"test.{token}@appuntamed.test",
				phoneNumber = "5551234"
			}, Json);

			Assert.Equal(HttpStatusCode.OK, response.StatusCode);

			var id = await PatientIdOf(user.UserId);
			Assert.True(id.HasValue, "The patient profile was not created");

			return (user, id.Value, firstName);
		}

		protected async Task<(TestUser User, Guid DoctorId, string FirstName)> NewDoctorAsync(Guid professionId, Guid? specialityId = null)
		{
			var user = TestUser.NewDoctor();
			var token = Guid.NewGuid().ToString("N")[..8];
			var firstName = $"Doc{token}";

			var response = await ClientFor(user).PostAsJsonAsync("/api/doctors", new
			{
				firstName,
				lastName = "Doctor",
				professionId,
				specialityId,
				email = $"doc.{token}@appuntamed.test",
				phoneNumber = "5559876"
			}, Json);

			Assert.Equal(HttpStatusCode.OK, response.StatusCode);

			var id = await DoctorIdOf(user.UserId);
			Assert.True(id.HasValue, "The doctor profile was not created");

			return (user, id.Value, firstName);
		}

		protected Task<HttpResponseMessage> BookAsync(TestUser actor, Guid patientId, Guid doctorId, Guid roomId, DateTime start, DateTime end) =>
			ClientFor(actor).PostAsJsonAsync("/api/appointments",
				new { patientId, doctorId, consultingRoomId = roomId, start, end }, Json);

		/// <summary>Books an appointment and returns its id (the API answers 200 without a body, so it is read from the database).</summary>
		protected async Task<Guid> BookOkAsync(TestUser actor, Guid patientId, Guid doctorId, Guid roomId, (DateTime Start, DateTime End) slot)
		{
			var response = await BookAsync(actor, patientId, doctorId, roomId, slot.Start, slot.End);

			Assert.Equal(HttpStatusCode.OK, response.StatusCode);

			var id = await AppointmentIdAt(doctorId, slot.Start);
			Assert.True(id.HasValue, "The appointment was not stored");

			return id.Value;
		}
	}
}
