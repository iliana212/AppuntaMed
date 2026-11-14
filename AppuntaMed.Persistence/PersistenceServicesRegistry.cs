using AppuntaMed.Application.Interfaces.DataStore;
using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Persistence.Repositories;
using AppuntaMed.Persistence.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Persistence
{
	public static class PersistenceServicesRegistry
	{
		public static IServiceCollection AddPersistenceServices(this IServiceCollection services)
		{
			services.AddDbContext<AppuntaMedDbContext>(options => options.UseSqlServer("name=AppuntaMedSqlServer"));

			services.AddScoped<IConsultingRoomRepository, ConsultingRoomRepository>();

			services.AddScoped<IPatientRepository, PatientRepository>();

			services.AddScoped<IDoctorRepository, DoctorRepository>();

			services.AddScoped<IAppointmentRepository, AppointmentRepository>();

			services.AddScoped<IProfessionRepository, ProfessionRepository>();

			services.AddScoped<IUnitOfWork, UnitOfWorkEFCore>();
			
			return services;
		}
	}
}
