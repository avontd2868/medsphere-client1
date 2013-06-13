using ClinSchd.Modules.Task.Controllers;

namespace ClinSchd.Modules.Task.Group
{
	public interface IGroupPresentationModel
    {
		IGroupView View { get; }
        ITaskController Controller { get; set; }
    }
}
