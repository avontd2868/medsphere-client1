using ClinSchd.Modules.Management.AccessGroups.Controllers;

namespace ClinSchd.Modules.Management.AccessGroups
{
	public interface IAccessGroupsPresentationModel
    {
		IAccessGroupsView View { get; }
    }
}
