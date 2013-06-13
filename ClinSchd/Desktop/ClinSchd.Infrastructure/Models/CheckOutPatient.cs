using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClinSchd.Infrastructure.Models
{
	public class CheckOutPatient
	{
		public string PatientID { get; set; }
		public string AppointmentStartTime { get; set; }
		public string CheckOutDateTime { get; set; }
		public string ApptID { get; set; }
		public string ProviderIEN { get; set; }
	}
}
