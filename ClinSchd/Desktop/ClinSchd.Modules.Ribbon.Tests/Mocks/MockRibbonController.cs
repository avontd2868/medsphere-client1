using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Ribbon.Controllers;

namespace ClinSchd.Modules.Ribbon.Tests.Mocks
{
    internal class MockRibbonController : IRibbonController
    {
        public bool RunCalled;

        public void Run()
        {
            RunCalled = true;
        }
    }
}