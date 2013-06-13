using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.ChangeUser.Controllers;

using ClinSchd.Modules.ChangeUser.ChangeUser;

namespace ClinSchd.Modules.ChangeUser.Tests.Mocks
{
	internal class MockChangeUserController : IChangeUserController
    {
        public bool RunCalled;

		public IChangeUserPresentationModel Model { get; set; }

        public void Run()
        {
            RunCalled = true;
        }
    }
}