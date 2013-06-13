using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.ChangeServer.Controllers;

using ClinSchd.Modules.ChangeServer.ChangeServer;

namespace ClinSchd.Modules.ChangeServer.Tests.Mocks
{
	internal class MockChangeServerController : IChangeServerController
    {
        public bool RunCalled;

		public IChangeServerPresentationModel Model { get; set; }

        public void Run()
        {
            RunCalled = true;
        }
    }
}