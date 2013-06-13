using ClinSchd.Modules.Management.Controllers;
using ClinSchd.Infrastructure.Models;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Events;

namespace ClinSchd.Modules.Management.Group
{
	public interface IGroupPresentationModel
    {
		IGroupView View { get; }
		IManagementController Controller { get; set; }

		void OnClose ();

    }
}
