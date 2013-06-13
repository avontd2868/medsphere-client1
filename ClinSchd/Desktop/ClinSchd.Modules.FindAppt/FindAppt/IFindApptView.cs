using System;

using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Modules.FindAppt.FindAppt
{
	public interface IFindApptView
	{
		FindApptPresentationModel Model { get; set; }

		bool? DialogResult { get; set; }
		event EventHandler Closed;
		bool? ShowDialog();
		object DataContext { get; set; }
		void Close();
		bool ConfirmUser (string message, string caption);
		void AlertUser (string message, string caption);
	}
}
