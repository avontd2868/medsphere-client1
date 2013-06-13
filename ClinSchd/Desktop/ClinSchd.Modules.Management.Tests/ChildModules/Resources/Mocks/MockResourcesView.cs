using System;
using ClinSchd.Modules.Management.Resources;

namespace ClinSchd.Modules.Management.Resources.Tests.Mocks
{
	public class MockResourcesView : IResourcesView
    {
		public ResourcesPresentationModel Model { get; set; }

		public void AlertUser (string message, string caption)
		{
		}

		public bool ConfirmUser (string message, string caption)
		{
			return true;
		}

    }
}
