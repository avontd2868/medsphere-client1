using System;
using ClinSchd.Modules.PatientAppt.Group;
using Telerik.Windows.Controls;

namespace ClinSchd.Modules.PatientAppt.Tests.Mocks
{
	public class MockGroupView : IGroupView
    {
        public event EventHandler<EventArgs> ShowPatientAppt = delegate { };

        public GroupPresentationModel Model { get; set; }
        
        public void RaiseShowPatientApptEvent()
        {
            ShowPatientAppt(this, EventArgs.Empty);
        }

		public bool? DialogResult { get; set; }

		public event EventHandler Closed;

		public bool? ShowDialog ()
		{
			return null;
		}

		public object DataContext { get; set; }

		public void Close ()
		{
		}

		public bool ConfirmUser (string message, string caption)
		{
			return true;
		}

		public void AlertUser (string message, string caption)
		{
		}

		public RadComboBox ReportComboBox { get; set; }
    }
}
