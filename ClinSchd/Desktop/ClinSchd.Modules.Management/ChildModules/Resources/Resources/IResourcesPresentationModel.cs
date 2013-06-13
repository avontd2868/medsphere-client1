using ClinSchd.Modules.Management.Resources.Controllers;

namespace ClinSchd.Modules.Management.Resources
{
	public interface IResourcesPresentationModel
    {
		IResourcesView View { get; }
    }
}
