using AppuntaMed.Domain.Enums;
using AppuntaMed.Domain.Exceptions;
using AppuntaMed.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Domain.Entities
{
	public class Appointment
	{
		public Guid Id { get; private set; }
		public Guid PatientId { get; private set; }
		public Guid DoctorId { get; private set; }
		public Guid ConsultingRoomId { get; private set; }
		public AppointmentStatus Status { get; private set; }
		public TimeInterval TimeInterval { get; private set; } = null!;
		public Patient? Patient { get; private set; }
		public Doctor? Doctor { get; private set; }
		public ConsultingRoom? ConsultingRoom { get; private set; }

		private Appointment() { }
		public Appointment(Guid patient, Guid doctor, Guid consultingRoom, TimeInterval interval)
		{			
			if (interval.Start < DateTime.UtcNow)
				throw new BusinessRuleException("Start date cannot be behind Today");

			Id = Guid.CreateVersion7();
			PatientId = patient;
			DoctorId = doctor;
			ConsultingRoomId = consultingRoom;
			TimeInterval = interval;
			Status = AppointmentStatus.Scheduled;
		}

		public void CancelAppointment()
		{
			if (Status != AppointmentStatus.Scheduled)
				throw new BusinessRuleException("You only can cancel scheduled appointments");

			Status = AppointmentStatus.Cancelled;
		}

		public void CompleteAppointment()
		{
			if (Status != AppointmentStatus.Scheduled)
				throw new BusinessRuleException("You only can complete scheduled appointments");

			Status = AppointmentStatus.Completed;
		}

		public void NoShowAppointment()
		{
			if (Status != AppointmentStatus.Scheduled)
				throw new BusinessRuleException("You only can update scheduled appointments");

			Status = AppointmentStatus.NoShow;
		}

		public void AssignReferences(Patient patient, Doctor doctor, ConsultingRoom consultingRoom)
		{
			Patient = patient;
			Doctor = doctor;
			ConsultingRoom = consultingRoom;
		}
	}
}
