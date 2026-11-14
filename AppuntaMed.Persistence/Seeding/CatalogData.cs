namespace AppuntaMed.Persistence.Seeding
{
	public sealed record ProfessionSeed(Guid Id, string Name);
	public sealed record SpecialitySeed(Guid Id, Guid ProfessionId, string Name);

	public static class CatalogData
	{
		public static readonly Guid PhysicianId = new("6b792a54-4790-54b6-9a3c-afda9ef91c02");
		public static readonly Guid DentistId = new("67c0884c-ef4f-5457-8017-9eaffff23484");
		public static readonly Guid PsychologistId = new("835fbbce-e62b-5cf7-8d8f-fa26f607dcbc");
		public static readonly Guid NutritionistId = new("38113eab-b310-54d2-98b0-fd0450bafdda");
		public static readonly Guid PhysiotherapistId = new("4bc5ba9b-f30a-5d65-9e16-a8489db164f2");

		public static readonly Guid CardiologyId = new("f5e73d96-1596-5201-8839-c19b357982c7");
		public static readonly Guid DermatologyId = new("41e09464-6560-5c1d-b10a-c211bec0000b");
		public static readonly Guid PediatricsId = new("09354c8d-3ceb-5b8c-9472-7c2c4459f4ed");
		public static readonly Guid NeurologyId = new("d57c3270-0034-5a73-b4bf-181842b8282b");
		public static readonly Guid GynecologyId = new("a1612ff6-43da-5f0b-bebd-a050bb743a67");
		public static readonly Guid OphthalmologyId = new("277830e8-1588-5127-87a1-3a033f9039ec");
		public static readonly Guid OrthopedicsId = new("d1742153-3c77-50f8-926a-91549b814fde");
		public static readonly Guid OrthodonticsId = new("94aca0ac-0beb-567d-8643-7e70dfedf1e1");
		public static readonly Guid EndodonticsId = new("72cb5346-81f7-524f-a8ca-b6300ed0fa75");
		public static readonly Guid PeriodonticsId = new("17375fd5-cfec-5718-b98a-8b1c373ee7f5");
		public static readonly Guid PediatricDentistryId = new("447f5876-4071-508a-a527-0cacbe8c8f7e");
		public static readonly Guid OralSurgeryId = new("52f28894-8139-5bef-a1c4-c743ee322b1d");
		public static readonly Guid ClinicalPsychologyId = new("6b812dc3-cc3e-5cf5-9b27-b6c02f9e291b");
		public static readonly Guid ChildPsychologyId = new("f67434c6-c1a8-53e0-b69a-882ea762f2db");
		public static readonly Guid NeuropsychologyId = new("186f64ad-3715-5339-b385-90fc46e1876f");
		public static readonly Guid ClinicalNutritionId = new("c3628f69-0e58-5876-a247-a28d3ebddd2d");
		public static readonly Guid SportsNutritionId = new("4f739c23-17df-5e66-bcf6-ace6e9e08599");
		public static readonly Guid SportsRehabilitationId = new("222c255e-75ad-58e4-8c18-964dd79b709e");
		public static readonly Guid NeurologicalRehabilitationId = new("52ab4b2d-5586-529b-9031-1a6b38380b40");

		public static IReadOnlyList<ProfessionSeed> Professions { get; } =
		[
			new(PhysicianId, "Physician"),
			new(DentistId, "Dentist"),
			new(PsychologistId, "Psychologist"),
			new(NutritionistId, "Nutritionist"),
			new(PhysiotherapistId, "Physiotherapist"),
        ];

		public static IReadOnlyList<SpecialitySeed> Specialities { get; } =
		[
			new(CardiologyId, PhysicianId, "Cardiology"),
			new(DermatologyId, PhysicianId, "Dermatology"),
			new(PediatricsId, PhysicianId, "Pediatrics"),
			new(NeurologyId, PhysicianId, "Neurology"),
			new(GynecologyId, PhysicianId, "Gynecology"),
			new(OphthalmologyId, PhysicianId, "Ophthalmology"),
			new(OrthopedicsId, PhysicianId, "Orthopedics"),
			new(OrthodonticsId, DentistId, "Orthodontics"),
			new(EndodonticsId, DentistId, "Endodontics"),
			new(PeriodonticsId, DentistId, "Periodontics"),
			new(PediatricDentistryId, DentistId, "Pediatric Dentistry"),
			new(OralSurgeryId, DentistId, "Oral Surgery"),
			new(ClinicalPsychologyId, PsychologistId, "Clinical Psychology"),
			new(ChildPsychologyId, PsychologistId, "Child Psychology"),
			new(NeuropsychologyId, PsychologistId, "Neuropsychology"),
			new(ClinicalNutritionId, NutritionistId, "Clinical Nutrition"),
			new(SportsNutritionId, NutritionistId, "Sports Nutrition"),
			new(SportsRehabilitationId, PhysiotherapistId, "Sports Rehabilitation"),
			new(NeurologicalRehabilitationId, PhysiotherapistId, "Neurological Rehabilitation"),
        ];
	}
}