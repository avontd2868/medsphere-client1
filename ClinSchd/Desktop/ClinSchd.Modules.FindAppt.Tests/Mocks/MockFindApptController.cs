using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.FindAppt.Controllers;

using ClinSchd.Modules.FindAppt.FindAppt;

namespace ClinSchd.Modules.FindAppt.Tests.Mocks
{
	internal class MockFindApptController : IFindApptController
    {
        public bool RunCalled;

		public IFindApptPresentationModel Model { get; set; }

        public void Run()
        {
            RunCalled = true;
        }
    }
}