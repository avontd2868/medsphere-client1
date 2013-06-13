using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Task.Controllers;

namespace ClinSchd.Modules.Task.Tests.Mocks
{
    internal class MockTaskController : ITaskController
    {
        public bool RunCalled;

        public void Run()
        {
            RunCalled = true;
        }
    }
}