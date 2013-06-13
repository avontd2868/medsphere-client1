using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClinSchd.Infrastructure.Models
{
	public class FindAppointmentData
	{
		public string StartDate { get; set; }
		public string EndDate { get; set; }
		public string SchedulerType { get; set; }
		public string AMPM { get; set; }
		public string DAYOFWEEK { get; set; }
		public bool Monday { get; set; }
		public bool Tuesday { get; set; }
		public bool Wednesday { get; set; }
		public bool Thursday { get; set; }
		public bool Friday { get; set; }
		public bool Saturday { get; set; }
		public bool Sunday { get; set; }
		public IList<SchdResource> Resources { get; set; }
		public IList<FindGroupedAccessTypes> AccessTypes { get; set; }

		public FindAppointmentData ()
		{
			this.Resources = new List<SchdResource> ();
			this.AccessTypes = new List<FindGroupedAccessTypes> ();
		}
	}
}
