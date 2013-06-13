using ClinSchd.Modules.Task.MultiScheduler.Controllers;
using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Modules.Task.MultiScheduler
{
	public interface IMultiSchedulerPresentationModel
    {
		IMultiSchedulerView View { get; }
        ITaskMultiSchedulerController Controller { get; set; }
		string PaneTitle { get; set; }
		SchdResourceGroup SelectedResourceGroup { get; set; }
    }
}
