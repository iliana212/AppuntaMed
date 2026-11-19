using AppuntaMed.Application.UseCases.Appointments.Queries.GetAppointmentsList;
using AppuntaMed.Application.UseCases.Doctors.Queries.GetDoctorsList;
using AppuntaMed.Application.UseCases.Patients.Queries.GetPatientsList;
using FluentValidation.TestHelper;

namespace AppuntaMed.Tests.Application.UseCases
{
	public class ListQueryValidatorsTests
	{
		public static TheoryData<int, int, bool> PagingCases => new()
		{
			{ 1, 1, true },        // smallest valid values
			{ 1, 10, true },       // defaults
			{ 1, 100, true },      // upper limit
			{ 1000, 10, true },    // a far page is valid (it just returns nothing)
			{ 0, 10, false },      // page below 1
			{ -1, 10, false },
			{ 1, 0, false },       // page size below 1
			{ 1, -5, false },
			{ 1, 101, false },     // page size above the limit
		};

		[Theory]
		[MemberData(nameof(PagingCases))]
		public void Doctors_Paging_IsValidatedWithinLimits(int page, int pageSize, bool valid)
		{
			var result = new GetDoctorsListValidator().TestValidate(new GetDoctorsListQuery { Page = page, PageSize = pageSize });

			Assert.Equal(valid, result.IsValid);
		}

		[Theory]
		[MemberData(nameof(PagingCases))]
		public void Patients_Paging_IsValidatedWithinLimits(int page, int pageSize, bool valid)
		{
			var result = new GetPatientsListValidator().TestValidate(new GetPatientsListQuery { Page = page, PageSize = pageSize });

			Assert.Equal(valid, result.IsValid);
		}

		[Fact]
		public void Doctors_InvalidPage_ReportsThePageProperty()
		{
			new GetDoctorsListValidator().TestValidate(new GetDoctorsListQuery { Page = 0 }).ShouldHaveValidationErrorFor(x => x.Page);
		}

		[Fact]
		public void Doctors_InvalidPageSize_ReportsThePageSizeProperty()
		{
			new GetDoctorsListValidator().TestValidate(new GetDoctorsListQuery { PageSize = 101 }).ShouldHaveValidationErrorFor(x => x.PageSize);
		}

		[Fact]
		public void Appointments_EndAfterStart_IsValid()
		{
			var start = new DateTime(2030, 1, 1, 10, 0, 0);

			new GetAppointmentsListValidator().TestValidate(new GetAppointmentsListQuery { Start = start, End = start.AddMinutes(1) })
				.ShouldNotHaveAnyValidationErrors();
		}

		[Fact]
		public void Appointments_EndEqualToStart_IsInvalid()
		{
			var start = new DateTime(2030, 1, 1, 10, 0, 0);

			new GetAppointmentsListValidator().TestValidate(new GetAppointmentsListQuery { Start = start, End = start })
				.ShouldHaveValidationErrorFor(x => x.End);
		}

		[Fact]
		public void Appointments_EndBeforeStart_IsInvalid()
		{
			var start = new DateTime(2030, 1, 1, 10, 0, 0);

			new GetAppointmentsListValidator().TestValidate(new GetAppointmentsListQuery { Start = start, End = start.AddDays(-1) })
				.ShouldHaveValidationErrorFor(x => x.End);
		}

		[Fact]
		public void Appointments_WithoutRange_IsInvalid_InsteadOfSilentlyReturningNothing()
		{
			new GetAppointmentsListValidator().TestValidate(new GetAppointmentsListQuery())
				.ShouldHaveValidationErrorFor(x => x.End);
		}
	}
}
