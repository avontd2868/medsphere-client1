using System;
using ClinSchd.Modules.Management.ResourceGroups;

namespace ClinSchd.Modules.Management.ResourceGroups.Tests.Mocks
{
	public class MockResourceGroupsView : IResourceGroupsView
    {
		public event EventHandler<EventArgs> ShowManagementResourceGroups = delegate { };

		public ResourceGroupsPresentationModel Model { get; set; }

		public void RaiseShowManagementResourceGroupsEvent()
        {
			ShowManagementResourceGroups(this, EventArgs.Empty);
        }
        
        public void Refresh()
        {
        }

		public bool PromptUser (string message, string caption)
		{
			return true;
		}
    }
}
