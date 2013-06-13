using System;

using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Modules.PatientSelection.PatientSelection
{
	public interface IPatientSelectionView
    {
		PatientSelectionPresentationModel Model { get; set; }

		bool? DialogResult { get; set; }
		event EventHandler Closed;
		bool? ShowDialog();
		object DataContext { get; set; }
		void Close();
    }
}
