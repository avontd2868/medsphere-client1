using System;
using ClinSchd.Modules.Management.Group;

namespace ClinSchd.Modules.Management.Tests.Mocks
{
	public class MockGroupView : IGroupView
    {
        public event EventHandler<EventArgs> ShowManagement = delegate { };

        public GroupPresentationModel Model { get; set; }
        
        public void RaiseShowManagementEvent()
        {
            ShowManagement(this, EventArgs.Empty);
        }

		public bool? DialogResult { get; set; }

		public event EventHandler Closed;

		public bool? ShowDialog ()
		{
			return null;
		}

		public object DataContext { get; set; }

		public void Close ()
		{
		}
    }
}
