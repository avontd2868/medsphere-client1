using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.ResourceSelection.Controllers;

using ClinSchd.Modules.ResourceSelection.ResourceSelection;

namespace ClinSchd.Modules.ResourceSelection.Tests.Mocks
{
	internal class MockResourceSelectionController : IResourceSelectionController
    {
        public bool RunCalled;

		public IResourceSelectionPresentationModel Model { get; set; }

        public void Run(string test)
        {
            RunCalled = true;
        }
    }
}