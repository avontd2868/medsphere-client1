using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClinSchd.Infrastructure.Models
{
	public class FindAppointmentResult
	{
		public string StartTime { get; set; }
		public string EndTime { get; set; }
		public string ResourceName { get; set; }
		public string Slots { get; set; }
		public string AccessType { get; set; }
	}
}
