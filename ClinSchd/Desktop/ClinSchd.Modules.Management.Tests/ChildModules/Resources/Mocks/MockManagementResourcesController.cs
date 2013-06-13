using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Management.Resources.Controllers;

namespace ClinSchd.Modules.Management.Resources.Tests.Mocks
{
	internal class MockManagementResourcesController : IManagementResourcesController
    {
        public bool RunCalled;

        public void Run()
        {
            RunCalled = true;
        }
    }
}