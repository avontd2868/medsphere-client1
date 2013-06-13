using System;
using ClinSchd.Infrastructure.Models;
using Telerik.Windows.Controls;
using System.Windows.Controls;

namespace ClinSchd.Modules.Management.AddAccessType
{
	public interface IAddAccessTypeView
    {
		AddAccessTypePresentationModel Model { get; set; }

		bool? DialogResult { get; set; }
		event EventHandler Closed;
		bool? ShowDialog ();
		object DataContext { get; set; }
		void Close ();
		bool ConfirmUser (string message, string caption);
		void AlertUser (string message, string caption);
		RadColorPicker AccessTypeColorPicker { get; set; }
		TextBox ATName { get; set; }
    }
}
