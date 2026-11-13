using AppuntaMed.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Persistence.Configurations
{
	public class ConsultingRoomConfig : IEntityTypeConfiguration<ConsultingRoom>
	{
		public void Configure(EntityTypeBuilder<ConsultingRoom> builder)
		{
			builder.Property(prop => prop.Name)
				.HasMaxLength(150)
				.IsRequired();
		}
	}
}
