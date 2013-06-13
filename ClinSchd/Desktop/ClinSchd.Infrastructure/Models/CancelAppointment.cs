using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClinSchd.Infrastructure.Models
{
	public class CancelAppointment
	{
		public string ApptID { get; set; }
		public bool CancelledByClinic { get; set; }
		public string ReasonIEN { get; set; }
		public string Notes { get; set; }
	}
}
