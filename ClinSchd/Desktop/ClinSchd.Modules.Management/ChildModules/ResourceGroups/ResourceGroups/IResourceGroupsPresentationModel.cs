using ClinSchd.Modules.Management.ResourceGroups.Controllers;

namespace ClinSchd.Modules.Management.ResourceGroups
{
	public interface IResourceGroupsPresentationModel
    {
		IResourceGroupsView View { get; }
    }
}
