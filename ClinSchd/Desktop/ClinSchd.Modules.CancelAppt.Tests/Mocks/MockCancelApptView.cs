using System;
using ClinSchd.Modules.CancelAppt.CancelAppt;

namespace ClinSchd.Modules.CancelAppt.Tests.Mocks
{
	public class MockCancelApptView : ICancelApptView
	{
		public CancelApptPresentationModel Model { get; set; }

		public bool? DialogResult { get; set; }

		public event EventHandler Closed;

		public bool? ShowDialog()
		{
			return null;
		}

		public object DataContext { get; set; }

		public void Close()
		{
		}

		public bool ConfirmUser (string message, string caption)
		{
			return true;
		}

		public void AlertUser (string message, string caption)
		{
		}
	}
}
