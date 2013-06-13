using System;
using ClinSchd.Modules.Task.Scheduler;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Scheduler;
using ClinSchd.Modules.Task.Group;

namespace ClinSchd.Modules.Task.Scheduler.Tests.Mocks
{
	public class MockSchedulerView : ISchedulerView
    {
		public ITaskPresentationModel Model { get; set; }
		public RadScheduler Scheduler { get; set; }

		public void Collapse()
		{
		}

		public void Expand()
		{
		}

		public void TimeScaleLoaded ()
		{
		}
		public bool ConfirmUser (string message, string caption)
		{
			return true;
		}

		public void AlertUser (string message, string caption)
		{
		}

		public void Close ()
		{
		}

		public void ScrollSchedulerToOffset ()
		{
			throw new NotImplementedException ();
		}
	}
}
