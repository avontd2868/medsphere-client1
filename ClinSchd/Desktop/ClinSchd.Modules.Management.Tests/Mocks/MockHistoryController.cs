using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Management.Controllers;
using ClinSchd.Modules.Management.Group;

namespace ClinSchd.Modules.Management.Tests.Mocks
{
	internal class MockManagementController : IManagementController
    {
        public bool RunCalled;

		public IGroupPresentationModel Model { get; set; }

        public void Run()
        {
            RunCalled = true;
        }
    }
}