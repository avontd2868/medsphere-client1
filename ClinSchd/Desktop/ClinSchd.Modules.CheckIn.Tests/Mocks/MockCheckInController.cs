using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.CheckIn.Controllers;

using ClinSchd.Modules.CheckIn.CheckIn;

namespace ClinSchd.Modules.CheckIn.Tests.Mocks
{
	internal class MockCheckInController : ICheckInController
    {
        public bool RunCalled;

		public ICheckInPresentationModel Model { get; set; }

        public void Run()
        {
            RunCalled = true;
        }
    }
}