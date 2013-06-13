using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.ChangeDivision.Controllers;

using ClinSchd.Modules.ChangeDivision.ChangeDivision;

namespace ClinSchd.Modules.ChangeDivision.Tests.Mocks
{
	internal class MockChangeDivisionController : IChangeDivisionController
    {
        public bool RunCalled;

		public IChangeDivisionPresentationModel Model { get; set; }

        public void Run()
        {
            RunCalled = true;
        }
    }
}