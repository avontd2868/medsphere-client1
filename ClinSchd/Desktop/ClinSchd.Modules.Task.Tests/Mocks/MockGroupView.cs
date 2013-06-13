using System;
using ClinSchd.Modules.Task.Group;

namespace ClinSchd.Modules.Task.Tests.Mocks
{
	public class MockGroupView : IGroupView
    {
        public event EventHandler<EventArgs> ShowTask = delegate { };

        public GroupPresentationModel Model { get; set; }
        
        public void RaiseShowTaskEvent()
        {
            ShowTask(this, EventArgs.Empty);
        }

		public void FocusSchedulerGroup () { }
    }
}
