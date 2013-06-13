using System;
using ClinSchd.Modules.PatientSelection.PatientSelection;

namespace ClinSchd.Modules.PatientSelection.Tests.Mocks
{
	public class MockPatientSelectionView : IPatientSelectionView
    {
		public PatientSelectionPresentationModel Model { get; set; }

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
