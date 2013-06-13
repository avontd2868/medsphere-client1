using System;
using ClinSchd.Modules.ChangeDivision.ChangeDivision;

namespace ClinSchd.Modules.ChangeDivision.Tests.Mocks
{
	public class MockChangeDivisionView : IChangeDivisionView
    {
		public ChangeDivisionPresentationModel Model { get; set; }

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
	}
}
