using System;
using ClinSchd.Modules.ResourceSelection.ResourceSelection;

namespace ClinSchd.Modules.ResourceSelection.Tests.Mocks
{
	public class MockResourceSelectionView : IResourceSelectionView
    {
		public ResourceSelectionPresentationModel Model { get; set; }

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
