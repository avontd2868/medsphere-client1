using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Management.AccessGroups.Controllers;

namespace ClinSchd.Modules.Management.AccessGroups.Tests.Mocks
{
	internal class MockManagementAccessGroupsController : IManagementAccessGroupsController
    {
        public bool RunCalled;

        public void Run()
        {
            RunCalled = true;
        }
    }
}