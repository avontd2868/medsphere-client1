using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.PatientSelection.Controllers;

using ClinSchd.Modules.PatientSelection.PatientSelection;

namespace ClinSchd.Modules.PatientSelection.Tests.Mocks
{
    internal class MockPatientSelectionController : IPatientSelectionController
    {
        public bool RunCalled;

		public IPatientSelectionPresentationModel Model { get; set; }

        public void Run()
        {
            RunCalled = true;
        }
    }
}