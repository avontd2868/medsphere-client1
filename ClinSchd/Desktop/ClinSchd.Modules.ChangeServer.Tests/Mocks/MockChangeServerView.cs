using System;
using ClinSchd.Modules.ChangeServer.ChangeServer;

namespace ClinSchd.Modules.ChangeServer.Tests.Mocks
{
	public class MockChangeServerView : IChangeServerView
    {
		public ChangeServerPresentationModel Model { get; set; }

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
