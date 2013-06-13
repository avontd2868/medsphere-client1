using System;
using ClinSchd.Modules.Navigation.Group;
using System.Windows.Threading;

namespace ClinSchd.Modules.Navigation.Tests.Mocks
{
	public class MockGroupView : IGroupView
    {
        public event EventHandler<EventArgs> ShowNavigation = delegate { };

		public GroupPresentationModel Model { get; set; }
        
        public void RaiseShowNavigationEvent()
        {
            ShowNavigation(this, EventArgs.Empty);
        }

		public void CollapseComboBoxes () { }
    }
}
