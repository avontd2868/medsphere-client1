using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.MarkAsNoShow.Controllers;

using ClinSchd.Modules.MarkAsNoShow.MarkAsNoShow;

namespace ClinSchd.Modules.MarkAsNoShow.Tests.Mocks
{
	internal class MockMarkAsNoShowController : IMarkAsNoShowController
    {
        public bool RunCalled;

		public IMarkAsNoShowPresentationModel Model { get; set; }

        public void Run()
        {
            RunCalled = true;
        }
    }
}