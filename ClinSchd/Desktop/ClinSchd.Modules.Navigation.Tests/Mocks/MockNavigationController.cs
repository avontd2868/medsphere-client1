using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Navigation.Controllers;

namespace ClinSchd.Modules.Navigation.Tests.Mocks
{
    internal class MockNavigationController : INavigationController
    {
		public bool CurrentNavigationGroupChangedCalled;
        public bool RunCalled;

        public bool ShowNavigationCalled { get; private set; }

        public void ShowNavigation()
        {
            ShowNavigationCalled = true;
        }

        public void Run()
        {
            RunCalled = true;
        }
    }
}