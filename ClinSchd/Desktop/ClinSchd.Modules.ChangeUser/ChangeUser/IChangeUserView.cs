using System;

using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Modules.ChangeUser.ChangeUser
{
	public interface IChangeUserView
    {
		ChangeUserPresentationModel Model { get; set; }

		bool? DialogResult { get; set; }
		event EventHandler Closed;
		bool? ShowDialog();
		object DataContext { get; set; }
		void Close();
    }
}
