using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClinSchd.Infrastructure.Models
{
	public class AutoRebookData
	{
		public bool IsAccessTypeChecked { get; set; }
		public int AccessTypeID { get; set; }
		public int MinimumDays { get; set; }
		public int MaximumDays { get; set; }
		public SchdAppointment RebookAppointment { get; set; }

		public AutoRebookData ()
		{
			this.RebookAppointment = new SchdAppointment ();
		}
	}
}
