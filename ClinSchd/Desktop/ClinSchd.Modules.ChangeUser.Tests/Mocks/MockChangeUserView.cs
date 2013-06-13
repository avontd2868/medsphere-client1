using System;
using ClinSchd.Modules.ChangeUser.ChangeUser;

namespace ClinSchd.Modules.ChangeUser.Tests.Mocks
{
	public class MockChangeUserView : IChangeUserView
    {
		public ChangeUserPresentationModel Model { get; set; }

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
