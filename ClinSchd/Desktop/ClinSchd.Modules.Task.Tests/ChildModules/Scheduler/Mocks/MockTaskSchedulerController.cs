using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Task.Scheduler.Controllers;

namespace ClinSchd.Modules.Task.Scheduler.Tests.Mocks
{
	internal class MockTaskSchedulerController : ITaskSchedulerController
    {
        public bool RunCalled;

		public void Run()
		{
			RunCalled = true;
		}



		#region ITaskSchedulerController Members


		public void RemoveViewFromList (ISchedulerView View)
		{
			throw new System.NotImplementedException ();
		}

		#endregion
	}
}