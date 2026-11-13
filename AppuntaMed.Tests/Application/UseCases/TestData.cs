using AppuntaMed.Domain.Entities;
using AppuntaMed.Domain.ValueObjects;

namespace AppuntaMed.Tests.Application.UseCases
{
	public class TestData
	{
		public static Patient Patient(string userId = "user-ana") =>
			new("Ana", "López", new DateTime(1990, 1, 1), new PhoneNumber("5551234"), new Email("ana@example.com"), userId);

		public static Doctor Doctor(string userId = "user-carlos") =>
			new("Carlos", "Pérez", "Cardiology", "Heart Surgery", new Email("carlos@example.com"), new PhoneNumber("5555678"), userId);

		public static Appointment Appointment(Patient patient, Doctor doctor) =>
			new(patient.Id, doctor.Id, Guid.NewGuid(), new TimeInterval(DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2)));
	}
}
