using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClinSchd.Infrastructure.Models
{
	public class CheckInPatient
	{
		public string ApptID { get; set; }
		public string CheckInDateTime { get; set; }
		public string ClinicStopIEN { get; set; }
		public string ProviderIEN { get; set; }
		public bool PrintRouteSlip { get; set; }
		public string HealthSummaryLetterIEN { get; set; }
		public bool PrintWellnessHandout { get; set; }
		public string PCCClinicIEN { get; set; }
		public string PCCFormIEN { get; set; }
		public bool PCCOutGuide { get; set; }
		public bool MakeChartRequest { get; set; }

	}
}
