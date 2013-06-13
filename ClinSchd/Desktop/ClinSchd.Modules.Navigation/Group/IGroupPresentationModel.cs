using ClinSchd.Modules.Navigation.Controllers;

namespace ClinSchd.Modules.Navigation.Group
{
	public interface IGroupPresentationModel
    {
		IGroupView View { get; }
		INavigationController Controller { get; set; }
    }
}
