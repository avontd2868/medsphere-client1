using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.PatientAppt.Controllers;
using ClinSchd.Modules.PatientAppt.Group;

namespace ClinSchd.Modules.PatientAppt.Tests.Mocks
{
	internal class MockPatientApptController : IPatientApptController
    {
        public bool RunCalled;

		public IGroupPresentationModel Model { get; set; }

        public void Run()
        {
            RunCalled = true;
        }
    }
}