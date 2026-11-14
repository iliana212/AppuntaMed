using AppuntaMed.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Collections.Specialized.BitVector32;

namespace AppuntaMed.Persistence.Configurations
{
	public class DoctorConfig : IEntityTypeConfiguration<Doctor>
	{
		public void Configure(EntityTypeBuilder<Doctor> builder)
		{
			builder.Property(prop => prop.FirstName)
				.HasMaxLength(50)
				.IsRequired();

			builder.Property(prop => prop.LastName)
				.HasMaxLength(100)
				.IsRequired();

			builder.HasOne(prop => prop.Profession)
                .WithMany()
                .HasForeignKey(prop => prop.ProfessionId)
                .OnDelete(DeleteBehavior.Restrict)
 				.IsRequired();

			builder.HasOne(prop => prop.Speciality)
                .WithMany()
                .HasForeignKey(prop => new { prop.ProfessionId, prop.SpecialityId })
                .HasPrincipalKey(speciality => new { speciality.ProfessionId, speciality.Id })
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

			builder.ComplexProperty(prop => prop.Email, action =>
			{
				action.Property(e => e.Value).HasColumnName("Email").HasMaxLength(254);
			});

			builder.ComplexProperty(prop => prop.PhoneNumber, action =>
			{
				action.Property(e => e.Value).HasColumnName("PhoneNumber");
			});

			builder.Property(prop => prop.UserId)
				.HasMaxLength(100)
				.IsRequired();

			builder.HasIndex(p => p.UserId).IsUnique();
		}
	}
}
