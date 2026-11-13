using AppuntaMed.Application.Utilities.Mediator;

namespace AppuntaMed.Application.UseCases.Doctors.Queries.GetDoctorDetail
{
	public class GetDoctorDetailQuery : IRequest<DoctorDetailDTO>
	{
		public Guid Id { get; set; }
	}
}
