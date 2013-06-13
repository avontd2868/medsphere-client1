using System;
using Telerik.Windows.Controls;

namespace ClinSchd.Modules.Ribbon.Schedule
{
    public interface IScheduleView
    {
		SchedulePresentationModel Model { get; set; }
		void FocusPatientSearch ();
		bool ConfirmUser (string message, string caption);
		void AlertUser (string message, string caption);
    }
}
