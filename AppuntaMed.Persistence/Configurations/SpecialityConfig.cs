using AppuntaMed.Domain.Entities;
using AppuntaMed.Persistence.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppuntaMed.Persistence.Configurations
{
	public class SpecialityConfig : IEntityTypeConfiguration<Speciality>
	{
		public void Configure(EntityTypeBuilder<Speciality> builder)
		{
			builder.Property(prop => prop.Name)
				.HasMaxLength(100)
				.IsRequired();

			builder.HasAlternateKey(prop => new { prop.ProfessionId, prop.Id });

			builder.HasIndex(prop => new { prop.ProfessionId, prop.Name }).IsUnique();

			builder.HasData(CatalogData.Specialities.Select(s => new { s.Id, s.ProfessionId, s.Name }));
		}
	}
}
