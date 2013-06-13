using System;
using ClinSchd.Modules.Management.AccessGroups;

namespace ClinSchd.Modules.Management.AccessGroups.Tests.Mocks
{
	public class MockAccessGroupsView : IAccessGroupsView
    {
		public event EventHandler<EventArgs> ShowManagementAccessGroups = delegate { };

		public AccessGroupsPresentationModel Model { get; set; }

		public void RaiseShowManagementAccessGroupsEvent()
        {
			ShowManagementAccessGroups(this, EventArgs.Empty);
        }
        
        public void Refresh()
        {
        }

		public bool PromptUser (string message, string caption)
		{
			return true;
		}

		public void AlertUser (string messagge, string Title)
		{
		}

		public bool ConfirmUser (string messagge, string Title)
		{
			return true;
		}
    }
}
