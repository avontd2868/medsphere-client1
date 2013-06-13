using System;

namespace ClinSchd.Modules.Task.Group
{
	public interface ITaskView
    {
        ITaskPresentationModel Model { get; set; }
    }
}
