using System;

namespace ClinSchd.Modules.Management.ResourceGroups
{
	public interface IResourceGroupsView
    {
		ResourceGroupsPresentationModel Model { get; set; }
		void AlertUser (string message, string caption);
		bool ConfirmUser (string message, string caption);
	}
}
