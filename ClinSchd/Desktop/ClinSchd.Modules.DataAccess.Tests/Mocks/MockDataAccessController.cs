using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.DataAccess.Controllers;

namespace ClinSchd.Modules.DataAccess.Tests.Mocks
{
    internal class MockDataAccessController : IDataAccessController
    {
        public bool CurrentDataAccessDataAccessChangedCalled;
        public bool RunCalled;

        public bool ShowDataAccessCalled { get; private set; }

        public void ShowDataAccess()
        {
            ShowDataAccessCalled = true;
        }

        public void Run()
        {
            RunCalled = true;
        }
    }
}