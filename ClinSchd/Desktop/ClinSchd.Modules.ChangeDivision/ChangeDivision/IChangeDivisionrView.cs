using System;

using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Modules.ChangeDivision.ChangeDivision
{
	public interface IChangeDivisionView
    {
		ChangeDivisionPresentationModel Model { get; set; }

		bool? DialogResult { get; set; }
		event EventHandler Closed;
		bool? ShowDialog ();
		object DataContext { get; set; }
		void Close ();
		bool ConfirmUser (string message, string caption);
		void AlertUser (string message, string caption);
    }
}
