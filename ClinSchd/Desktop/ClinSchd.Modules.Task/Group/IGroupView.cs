using System;
using Telerik.Windows.Controls;

namespace ClinSchd.Modules.Task.Group
{
	public interface IGroupView
    {
        event EventHandler<EventArgs> ShowTask;

        GroupPresentationModel Model { get; set; }
		void FocusSchedulerGroup ();
    }
}
