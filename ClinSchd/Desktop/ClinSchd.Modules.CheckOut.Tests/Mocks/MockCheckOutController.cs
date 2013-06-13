using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.CheckOut.Controllers;

using ClinSchd.Modules.CheckOut.CheckOut;

namespace ClinSchd.Modules.CheckOut.Tests.Mocks
{
	internal class MockCheckOutController : ICheckOutController
    {
        public bool RunCalled;

		public ICheckOutPresentationModel Model { get; set; }

        public void Run()
        {
            RunCalled = true;
        }
    }
}