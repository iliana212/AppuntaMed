using AppuntaMed.Domain.Entities;
using AppuntaMed.Persistence.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppuntaMed.Persistence.Configurations
{
	public class ProfessionConfig : IEntityTypeConfiguration<Profession>
	{
		public void Configure(EntityTypeBuilder<Profession> builder)
		{
			builder.Property(prop => prop.Name)
				.HasMaxLength(100)
				.IsRequired();

			builder.HasIndex(prop => prop.Name).IsUnique();

			builder.HasMany(prop => prop.Specialities)
				.WithOne()
				.HasForeignKey(speciality => speciality.ProfessionId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Navigation(prop => prop.Specialities).UsePropertyAccessMode(PropertyAccessMode.Field);

			builder.HasData(CatalogData.Professions.Select(p => new { p.Id, p.Name }));
		}
	}
}
