using AppuntaMed.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppuntaMed.Persistence.Configurations
{
	public class AppointmentConfig : IEntityTypeConfiguration<Appointment>
	{
		public void Configure(EntityTypeBuilder<Appointment> builder)
		{
			builder.HasOne(a => a.Patient).WithMany().HasForeignKey(a => a.PatientId).OnDelete(DeleteBehavior.Restrict);
			builder.HasOne(a => a.Doctor).WithMany().HasForeignKey(a => a.DoctorId).OnDelete(DeleteBehavior.Restrict);
			builder.HasOne(a => a.ConsultingRoom).WithMany().HasForeignKey(a => a.ConsultingRoomId).OnDelete(DeleteBehavior.Restrict);

			builder.ComplexProperty(prop => prop.TimeInterval, action =>
			{
				action.Property(e => e.Start).HasColumnName("Start");
				action.Property(e => e.End).HasColumnName("End");
			});
		}
	}
}
