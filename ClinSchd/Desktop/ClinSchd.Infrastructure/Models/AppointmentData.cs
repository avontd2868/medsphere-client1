using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClinSchd.Infrastructure.Models
{
	public class AppointmentData
	{
		public string ResourceName { get; set; }
		public string Notes { get; set; }
		public string StartTime { get; set; }
		public string EndTime { get; set; }
		public string Duration { get; set; }
		public string AppointmentCommand { get; set; }
		public string AppointmentID { get; set; }
		public string AccessTypeID { get; set; }
		public int Slots { get; set; }
		private int Column { get; set; }
		public string AccessTypeName { get; set; }

	}
}
