using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.CancelAppt.Controllers;

using ClinSchd.Modules.CancelAppt.CancelAppt;

namespace ClinSchd.Modules.CancelAppt.Tests.Mocks
{
	internal class MockCancelApptController : ICancelApptController
    {
        public bool RunCalled;

		public ICancelApptPresentationModel Model { get; set; }

        public void Run()
        {
            RunCalled = true;
        }
    }
}