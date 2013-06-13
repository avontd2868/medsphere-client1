using System;
using Telerik.Windows.Controls;

namespace ClinSchd.Modules.PatientAppt.Group
{
	public interface IGroupView
    {
		event EventHandler<EventArgs> ShowPatientAppt;

        GroupPresentationModel Model { get; set; }

		bool? DialogResult { get; set; }
		event EventHandler Closed;
		bool? ShowDialog ();
		object DataContext { get; set; }
		void Close ();
		bool ConfirmUser (string message, string caption);
		void AlertUser (string message, string caption);
		RadComboBox ReportComboBox { get; }
	}
}
