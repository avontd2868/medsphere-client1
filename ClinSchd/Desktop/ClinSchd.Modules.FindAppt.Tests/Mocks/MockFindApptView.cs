using System;
using ClinSchd.Modules.FindAppt.FindAppt;

namespace ClinSchd.Modules.FindAppt.Tests.Mocks
{
	public class MockFindApptView : IFindApptView
	{
		public FindApptPresentationModel Model { get; set; }

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
