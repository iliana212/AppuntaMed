using AppuntaMed.Application.UseCases.ConsultingRooms.Commands.CreateConsultingRoom;
using AppuntaMed.Application.UseCases.ConsultingRooms.Commands.UpdateConsultingRoom;
using AppuntaMed.Application.UseCases.Patients.Commands.CreatePatient;
using AppuntaMed.Application.Utilities.Mediator;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AppuntaMed.Application
{
	public static class ApplicationServicesRegistry
	{
		public static IServiceCollection AddApplicationServices(this IServiceCollection services)
		{
			services.AddTransient<IMediator, Mediator>();

			services.Scan(scan =>
				scan.FromAssembliesOf(typeof(IMediator))
				.AddClasses(c => c.AssignableTo(typeof(IRequestHandler<>))).AsImplementedInterfaces().WithScopedLifetime()
				.AddClasses(c => c.AssignableTo(typeof(IRequestHandler<,>))).AsImplementedInterfaces().WithScopedLifetime()
				.AddClasses(c => c.AssignableTo(typeof(IValidator<>))).AsImplementedInterfaces().WithTransientLifetime()
			);


			/*
			services.AddScoped<IRequestHandler<CreateConsultingRoomCommand, Guid>, CreateConsultingRoomHandler>();

			services.AddScoped<IRequestHandler<GetConsultingRoomDetailQuery, ConsultingRoomDetailDTO>, GetConsultingRoomDetailHandler>();

			services.AddScoped<IRequestHandler<GetConsultingRoomsListQuery, List<ConsultingRoomsListDTO>>, GetConsultingRoomsListHandler>();

			services.AddScoped<IRequestHandler<UpdateConsultingRoomCommand>, UpdateConsultingRoomHandler>();

			services.AddScoped<IRequestHandler<DeleteConsultingRoomCommand>, DeleteConsultingRoomHandler>();

			services.AddTransient<IValidator<CreateConsultingRoomCommand>, CreateConsultingRoomValidator>();
			services.AddTransient<IValidator<UpdateConsultingRoomCommand>,  UpdateConsultingRoomValidator>();
			services.AddTransient<IValidator<CreatePatientCommand>, CreatePatientValidator>();
			*/

			return services;
		}
	}
}
