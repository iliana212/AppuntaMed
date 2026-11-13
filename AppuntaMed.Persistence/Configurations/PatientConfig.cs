using AppuntaMed.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Persistence.Configurations
{
	public class PatientConfig : IEntityTypeConfiguration<Patient>
	{
		public void Configure(EntityTypeBuilder<Patient> builder)
		{
			builder.Property(prop => prop.FirstName)
				.HasMaxLength(50)
				.IsRequired();

			builder.Property(prop => prop.LastName)
				.HasMaxLength(100)
				.IsRequired();

			builder.Property(prop => prop.DateOfBirth)
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
