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

			builder.Property(prop => prop.Profession)
				.HasMaxLength(100)
				.IsRequired();

			builder.Property(prop => prop.Speciality)
				.HasMaxLength(100)
				.IsRequired();

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
