using AppuntaMed.Application.Exceptions;
using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.Utilities.Mediator;

namespace AppuntaMed.Application.UseCases.Doctors.Queries.GetDoctorDetail
{
	public class GetDoctorDetailHandler(IDoctorRepository repository) : IRequestHandler<GetDoctorDetailQuery, DoctorDetailDTO>
	{
		public async Task<DoctorDetailDTO> Handle(GetDoctorDetailQuery request)
		{
			var doctor = await repository.GetById(request.Id);

			if (doctor is null)
				throw new NotFoundException("Doctor not found");

			return doctor.ToDTO();
		}
	}
}
