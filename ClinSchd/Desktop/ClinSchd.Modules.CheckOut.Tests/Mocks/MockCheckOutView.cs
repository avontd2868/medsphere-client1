using System;
using ClinSchd.Modules.CheckOut.CheckOut;

namespace ClinSchd.Modules.CheckOut.Tests.Mocks
{
	public class MockCheckOutView : ICheckOutView
	{
		public CheckOutPresentationModel Model { get; set; }

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
