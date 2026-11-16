using AppuntaMed.Domain.Entities;
using AppuntaMed.Domain.ValueObjects;

namespace AppuntaMed.Persistence.Seeding
{
	///An entity together with the FIXED id it must be stored with.
	public sealed record Seed<T>(Guid Id, T Entity);

	public sealed record DemoDataPlan(
		IReadOnlyList<Seed<ConsultingRoom>> ConsultingRooms,
		IReadOnlyList<Seed<Patient>> Patients,
		IReadOnlyList<Seed<Doctor>> Doctors,
		IReadOnlyList<Seed<Appointment>> Appointments);

	// Demo data for local development and Docker. It is OPT-IN (Seeding:DemoData=true) and idempotent.
	public static class DemoData
	{
		// Keycloak user ids (the "sub" claim of their tokens). They MUST match keycloak/realm-appuntamed.json:
		// Doctor.UserId / Patient.UserId is what links a Keycloak account with its profile in this database.
		public const string AdminUserId = "e076d907-a519-567a-a438-ca75f576bbd3";
		
		public const string DrCarlosUserId = "c3316a2d-6dd7-51f7-abac-f9432fd112c2";
		public const string DrLauraUserId = "16b2e4aa-5f23-534d-92f7-78a61a651c25";
		public const string DrMiguelUserId = "8c5f9d40-7e31-4d0b-9c3a-4f1d8fe1a101";
		public const string DrPatriciaUserId = "6a8f17e1-22d7-4ec7-9fd0-e5c6ef271102";
		public const string DrJorgeUserId = "5b21ac7d-d22b-48d1-a592-9c8b64f31103";
		
		public const string AnaUserId = "fca9d998-e332-514f-a64a-708b45d2c81b";
		public const string BetoUserId = "2f251930-1bc7-5725-9e5d-d547bf4aa7b7";
		public const string CarmenUserId = "f41ac7b2-9b87-4144-a705-724f5c4c2101";
		public const string DiegoUserId = "c6e57d8f-12de-46c7-a0f1-77f1ebf32102";
		public const string ElenaUserId = "d8f8c8d4-38f9-4048-9516-1aab42e32103";

		public static readonly Guid Room101Id = new("0fd3f2a0-c10e-5142-829c-38f366519bd9");
		public static readonly Guid Room102Id = new("d973d2d8-f786-5dc9-b46f-95450a0da419");
		public static readonly Guid DentalOfficeAId = new("a7a2c133-6fc3-54e5-a0cf-3f622ee26874");

		public static readonly Guid AnaPatientId = new("57a08074-5780-5a82-adaa-12cc4235ac03");
		public static readonly Guid BetoPatientId = new("6ab6493a-9e2d-5717-bc5d-3f3efe218611");
		public static readonly Guid CarmenPatientId = new("b225b7b2-4ed9-4355-9cfc-5f5c60db2101");
		public static readonly Guid DiegoPatientId = new("b225b7b2-4ed9-4355-9cfc-5f5c60db2102");
		public static readonly Guid ElenaPatientId = new("b225b7b2-4ed9-4355-9cfc-5f5c60db2103");

		public static readonly Guid DrCarlosId = new("bfd58f44-0db5-5e51-b77d-e9f73d38bb14");
		public static readonly Guid DrLauraId = new("a50bba96-38b2-5bfe-b6ce-64d46058a97c");
		public static readonly Guid DrMiguelId = new("7ec82444-f8d5-4cb8-a7f4-e659949f3101");
		public static readonly Guid DrPatriciaId = new("7ec82444-f8d5-4cb8-a7f4-e659949f3102");
		public static readonly Guid DrJorgeId = new("7ec82444-f8d5-4cb8-a7f4-e659949f3103");		

		public static readonly Guid Appointment1Id = new("2bc7c952-b514-5306-a56b-9b4daa89fc4c");
		public static readonly Guid Appointment2Id = new("6015cc75-9061-5903-8ed8-11dd9b57cdd2");
		public static readonly Guid Appointment3Id = new("74168774-e457-5cac-b7ff-ff72ba40f3c7");
		public static readonly Guid Appointment4Id = new("11b71509-177a-54b6-95f7-e4083b570f17");
		public static readonly Guid Appointment5Id = new("e4ff8b7f-7ef5-5183-a356-6e48d453533b");
		public static readonly Guid Appointment6Id = new("18ece875-d35f-47af-b689-786151cb6101");
		public static readonly Guid Appointment7Id = new("18ece875-d35f-47af-b689-786151cb6102");
		public static readonly Guid Appointment8Id = new("18ece875-d35f-47af-b689-786151cb6103");
		public static readonly Guid Appointment9Id = new("18ece875-d35f-47af-b689-786151cb6104");
		public static readonly Guid Appointment10Id = new("18ece875-d35f-47af-b689-786151cb6105");
		public static readonly Guid Appointment11Id = new("18ece875-d35f-47af-b689-786151cb6106");
		public static readonly Guid Appointment12Id = new("18ece875-d35f-47af-b689-786151cb6107");

		/// <param name="utcNow">Appointments are placed relative to this moment (always in the future).</param>
		public static DemoDataPlan Build(DateTime utcNow)
		{
			var today = utcNow.Date;

			List<Seed<ConsultingRoom>> rooms =
			[
				new(Room101Id, new ConsultingRoom("Consulting Room 101")),
				new(Room102Id, new ConsultingRoom("Consulting Room 102")),
				new(DentalOfficeAId, new ConsultingRoom("Dental Office A")),
			];

			List<Seed<Patient>> patients =
			[
				new(AnaPatientId, new Patient("Ana", "López", new DateTime(1990, 5, 14),
					new PhoneNumber("52 477 555 0101"), new Email("ana.lopez@appuntamed.test"), AnaUserId)),
				new(BetoPatientId, new Patient("Beto", "Ramírez", new DateTime(1985, 11, 2),
					new PhoneNumber("52 477 555 0102"), new Email("beto.ramirez@appuntamed.test"), BetoUserId)),
				new(CarmenPatientId, new Patient("Carmen", "Martínez", new DateTime(1992, 8, 21),
					new PhoneNumber("52 477 555 0103"), new Email("carmen.martinez@appuntamed.test"), CarmenUserId)),
				new(DiegoPatientId, new Patient("Diego", "Hernández", new DateTime(1988, 3, 10),
					new PhoneNumber("52 477 555 0104"), new Email("diego.hernandez@appuntamed.test"), DiegoUserId)),
				new(ElenaPatientId, new Patient("Elena", "Torres", new DateTime(1995, 12, 5),
					new PhoneNumber("52 477 555 0105"), new Email("elena.torres@appuntamed.test"), ElenaUserId))
			];

			List<Seed<Doctor>> doctors =
			[
			    new(DrCarlosId, new Doctor("Carlos", "Pérez", CatalogData.PhysicianId, CatalogData.CardiologyId,
				    new Email("carlos.perez@appuntamed.test"), new PhoneNumber("52 477 555 0201"), DrCarlosUserId)),
			    new(DrLauraId, new Doctor("Laura", "Gómez", CatalogData.DentistId, CatalogData.OrthodonticsId,
				    new Email("laura.gomez@appuntamed.test"), new PhoneNumber("52 477 555 0202"), DrLauraUserId)),
				new(DrMiguelId, new Doctor("Miguel", "Sánchez", CatalogData.PhysicianId, CatalogData.CardiologyId,
					new Email("miguel.sanchez@appuntamed.test"), new PhoneNumber("52 477 555 0203"), DrMiguelUserId)),
				new(DrPatriciaId, new Doctor("Patricia", "Ruiz", CatalogData.DentistId, CatalogData.OrthodonticsId,
					new Email("patricia.ruiz@appuntamed.test"), new PhoneNumber("52 477 555 0204"), DrPatriciaUserId)),
				new(DrJorgeId, new Doctor("Jorge", "Navarro", CatalogData.PhysicianId, CatalogData.CardiologyId,
					new Email("jorge.navarro@appuntamed.test"), new PhoneNumber("52 477 555 0205"), DrJorgeUserId))
			];

			// The domain does not allow appointments in the past, so everything is scheduled from 2 days on.
			var cancelled = NewAppointment(today, 5, 15, 30, AnaPatientId, DrCarlosId, Room102Id);
			cancelled.CancelAppointment();

			List<Seed<Appointment>> appointments =
			[
                new(Appointment1Id, NewAppointment(today, 2, 15, 30, AnaPatientId, DrCarlosId, Room101Id)),
                new(Appointment2Id, NewAppointment(today, 2, 16, 30, BetoPatientId, DrCarlosId, Room101Id)),
                new(Appointment3Id, NewAppointment(today, 3, 17, 60, AnaPatientId, DrLauraId, DentalOfficeAId)),
                new(Appointment4Id, NewAppointment(today, 4, 15, 60, BetoPatientId, DrLauraId, DentalOfficeAId)),
                new(Appointment5Id, cancelled),
			    new(Appointment6Id, NewAppointment(today, 6, 15, 30, CarmenPatientId, DrCarlosId, Room101Id)),
				new(Appointment7Id, NewAppointment(today, 7, 16, 30, DiegoPatientId, DrCarlosId, Room102Id)),
				new(Appointment8Id, NewAppointment(today, 8, 15, 60, ElenaPatientId, DrLauraId, DentalOfficeAId)),
				new(Appointment9Id, NewAppointment(today, 9, 17, 30, AnaPatientId, DrMiguelId, Room101Id)),
				new(Appointment10Id, NewAppointment(today, 10, 16, 30, CarmenPatientId, DrPatriciaId, DentalOfficeAId)),
				new(Appointment11Id, NewAppointment(today, 11, 15, 60, DiegoPatientId, DrJorgeId, Room102Id)),
				new(Appointment12Id, NewAppointment(today, 12, 17, 30, ElenaPatientId, DrMiguelId, Room101Id))
			];

			return new DemoDataPlan(rooms, patients, doctors, appointments);
		}

        private static Appointment NewAppointment(DateTime today, int daysAhead, int hourUtc, int minutes, Guid patientId, Guid doctorId, Guid roomId)
        {
			var start = today.AddDays(daysAhead).AddHours(hourUtc);
			return new Appointment(patientId, doctorId, roomId, new TimeInterval(start, start.AddMinutes(minutes)));
	    }
    }
}
