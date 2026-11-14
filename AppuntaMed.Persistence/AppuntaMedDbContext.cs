using AppuntaMed.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Persistence
{
	public class AppuntaMedDbContext : DbContext
	{
		public AppuntaMedDbContext(DbContextOptions<AppuntaMedDbContext> options): base(options) { }

		public AppuntaMedDbContext() { }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppuntaMedDbContext).Assembly);
		}

		public DbSet<ConsultingRoom> ConsultingRooms { get; set; }
		public DbSet<Patient> Patients { get; set; } 
		public DbSet<Doctor> Doctors { get; set; }
		public DbSet<Appointment> Appointments { get; set; }
		public DbSet<Profession> Professions { get; set; }
		public DbSet<Speciality> Specialities { get; set; }
	}
}
