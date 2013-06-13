using System;

using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Modules.ChangeServer.ChangeServer
{
	public interface IChangeServerView
    {
		ChangeServerPresentationModel Model { get; set; }

		bool? DialogResult { get; set; }
		event EventHandler Closed;
		bool? ShowDialog();
		object DataContext { get; set; }
		void Close();
    }
}
