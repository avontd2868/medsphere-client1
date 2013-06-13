using System;
using ClinSchd.Modules.Ribbon.Schedule;

namespace ClinSchd.Modules.Ribbon.Tests.Mocks
{
	public class MockScheduleView : IScheduleView
    {
        public event EventHandler<EventArgs> ShowRibbon = delegate { };

		public SchedulePresentationModel Model { get; set; }
        
        public void RaiseShowRibbonEvent()
        {
            ShowRibbon(this, EventArgs.Empty);
        }

		#region IScheduleView Members


		public void FocusPatientSearch ()
		{
			
		}

		public bool ConfirmUser (string message, string caption)
		{
			return true;
		}

		public void AlertUser (string message, string caption)
		{
		}

		#endregion
	}
}
