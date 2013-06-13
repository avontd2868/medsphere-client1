using System;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Scheduler;
using System.Windows;
using System.Windows.Media.Animation;
using System.Collections.Generic;
using ClinSchd.Modules.Task.Group;

namespace ClinSchd.Modules.Task.EmptyScheduler
{
	public interface IEmptySchedulerView : ITaskView
    {
		void Collapse();
		void Expand();		
		bool ConfirmUser (string message, string caption);
		void AlertUser (string message, string caption);

	}
}
