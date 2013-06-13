using System;

namespace ClinSchd.Modules.Management.AccessGroups
{
	public interface IAccessGroupsView
    {
		AccessGroupsPresentationModel Model { get; set; }
		void AlertUser (string message, string caption);
		bool ConfirmUser (string message, string caption);
	}
}
