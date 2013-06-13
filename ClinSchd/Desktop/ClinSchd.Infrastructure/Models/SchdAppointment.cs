using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;

namespace ClinSchd.Infrastructure.Models
{
	public class SchdAppointment
	{
		public string APPOINTMENTID { get; set; }
		public string START_TIME { get; set; }
		public string END_TIME { get; set; }
		public string CHECKIN { get; set; }
		public string AUXTIME { get; set; }		
		public string RESOURCENAME { get; set; }
		public string NOSHOW { get; set; }
		public string HRN { get; set; }
		public string ACCESSTYPEID { get; set; }
		public string WALKIN { get; set; }
		public string CHECKOUT { get; set; }
		public string VPROVIDER { get; set; }
		public string CANCELLED { get; set; }
		public string NOTE { get; set; }
		public string Command { get; set; }

		public string PATIENTNAME { get; set; }
		
		private string patientId;
		public string PATIENTID
		{
			get
			{
				return patientId;
			}
			set
			{
				this.patientId = value;
				this.Patient = PatientFactory.Create ();
				this.Patient.IEN = value;
				this.Patient.LoadPatientInfoByIEN ();
			}
		}

		public Patient Patient { get; set; }

		[Dependency]
		public Factory<Patient> PatientFactory { get; set; }
	}
}
