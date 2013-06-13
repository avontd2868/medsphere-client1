using System;

namespace ClinSchd.Modules.Management.Resources
{
	public interface IResourcesView
    {
		ResourcesPresentationModel Model { get; set; }
		void AlertUser (string message, string caption);
		bool ConfirmUser (string message, string caption);
    }
}
