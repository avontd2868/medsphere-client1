using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClinSchd.Infrastructure.Models
{
	public class ConflictAppointments
	{
		public string ResourceName { get; set; }
		public string Patient { get; set; }
		public string StartTime { get; set; }
		public string EndTime { get; set; }
	}
}
