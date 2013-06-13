using System;

using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Modules.UserLogin.UserLogin
{
	public interface IUserLoginView
    {
		UserLoginPresentationModel Model { get; set; }

		bool? DialogResult { get; set; }
		event EventHandler Closed;
		bool? ShowDialog ();
		object DataContext { get; set; }
		void Close ();
		bool ConfirmUser (string message, string caption);
		void AlertUser (string message, string caption);
    }
}
