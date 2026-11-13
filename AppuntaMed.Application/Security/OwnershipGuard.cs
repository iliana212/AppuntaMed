using AppuntaMed.Application.Exceptions;
using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Application.Interfaces.Security;
using AppuntaMed.Domain.Entities;

namespace AppuntaMed.Application.Security
{
	public static class OwnershipGuard
	{
		public static void EnsureOwnsPatientProfile(this ICurrentUserService user, Patient patient)
		{
			if (user.IsPatient() && !IsSameUser(user, patient.UserId)) 
				throw Forbidden();
		}

        public static void EnsureOwnsDoctorProfile(this ICurrentUserService user, Doctor doctor)
        {
			if (user.IsDoctor() && !IsSameUser(user, doctor.UserId))
				throw Forbidden();
	    }

        public static async Task EnsureCanAccess(this ICurrentUserService user, Appointment appointment, IDoctorRepository doctors, IPatientRepository patients)
        {
			await user.EnsureDoctorCanAccess(appointment, doctors);
			await user.EnsurePatientCanAccess(appointment, patients);
	    }

        public static async Task EnsureDoctorCanAccess(this ICurrentUserService user, Appointment appointment, IDoctorRepository doctors)
        {
			if (!user.IsDoctor()) return;
	
			Doctor? doctor = null;
	
	        if (user.UserId is not null)
				doctor = await doctors.GetByUserId(user.UserId);
	
	        if (doctor is null || doctor.Id != appointment.DoctorId)
				throw Forbidden();
	    }

        public static async Task EnsurePatientCanAccess(this ICurrentUserService user, Appointment appointment, IPatientRepository patients)
        {
			if (!user.IsPatient()) return;
	
			Patient? patient = null;
	
	        if (user.UserId is not null)
				patient = await patients.GetByUserId(user.UserId);
	
	        if (patient is null || patient.Id != appointment.PatientId)
				throw Forbidden();
	    }

        public static async Task EnsureActsAsPatient(this ICurrentUserService user, Guid patientId, IPatientRepository patients)
        {
			if (!user.IsPatient()) return;
	
			Patient? patient = null;
	
			if (user.UserId is not null)
				patient = await patients.GetByUserId(user.UserId);
	
			if (patient is null || patient.Id != patientId)
				throw Forbidden();
	    }

        private static bool IsSameUser(ICurrentUserService user, string ownerUserId)
        {
			return user.UserId is not null && user.UserId == ownerUserId;
	    }

        private static ForbiddenException Forbidden() => new("Forbidden access");
    }
}