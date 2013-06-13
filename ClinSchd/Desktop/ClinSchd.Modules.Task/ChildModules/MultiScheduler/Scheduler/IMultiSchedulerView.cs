using System;
using Telerik.Windows.Controls;
using Microsoft.Practices.Composite.Regions;

namespace ClinSchd.Modules.Task.MultiScheduler
{
	public interface IMultiSchedulerView
    {
        event EventHandler<EventArgs> ShowTask;

        MultiSchedulerPresentationModel Model { get; set; }
		RadSplitContainer MultiSchedulerGroupControl { get; }
    }
}