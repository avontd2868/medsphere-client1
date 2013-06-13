using System;

using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Modules.ResourceSelection.ResourceSelection
{
	public interface IResourceSelectionView
    {
		ResourceSelectionPresentationModel Model { get; set; }

		bool? DialogResult { get; set; }
		event EventHandler Closed;
		bool? ShowDialog();
		object DataContext { get; set; }
		void Close();
    }
}
