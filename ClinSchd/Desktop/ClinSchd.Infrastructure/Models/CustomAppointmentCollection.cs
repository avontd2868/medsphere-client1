using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Windows.Controls.Scheduler;

namespace ClinSchd.Infrastructure.Models
{
	public class CustomAppointmentCollection : BaseAppointmentCollection<IAppointment>
	{
		public CustomAppointmentCollection ()
		{
		}
		public CustomAppointmentCollection (IEnumerable<IAppointment> appointments)
			: base (appointments.ToList<IAppointment> ())
		{
		}
		public override IAppointment CreateNewAppointment ()
		{
			return new CustomAppointment ();
		}
	}
}
