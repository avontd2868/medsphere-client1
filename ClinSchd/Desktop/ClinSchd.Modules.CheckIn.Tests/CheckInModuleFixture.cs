using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.CheckIn.CheckIn;
using ClinSchd.Modules.CheckIn.Controllers;
using ClinSchd.Modules.CheckIn.Services;
using ClinSchd.Modules.CheckIn.Tests.Mocks;

namespace ClinSchd.Modules.CheckIn.Tests
{
    /// <summary>
	/// Summary description for CheckIn
    /// </summary>
    [TestClass]
	public class CheckInModuleFixture
    {
        private MockUnityResolver container;
		private MockCheckInController controller;

        [TestMethod]
		public void CheckInModuleRegistersCheckInViewAndCheckInService ()
        {
			TestableCheckInModule CheckInModule = CreateTestableCheckInModule ();

			CheckInModule.InvokeRegisterViewsAndServices ();

			Assert.AreEqual (typeof (CheckInView), container.Types[typeof (ICheckInView)]);
			Assert.AreEqual (typeof (CheckInController), container.Types[typeof (ICheckInController)]);
			Assert.AreEqual (typeof (CheckInPresentationModel), container.Types[typeof (ICheckInPresentationModel)]);
			Assert.AreEqual (typeof (CheckInService), container.Types[typeof (ICheckInService)]);
#if !SILVERLIGHT
#endif
        }

        [TestMethod]
		public void InitCallsRunOnChangeDivisionController ()
        {
			var ChangeDivisionModule = CreateTestableCheckInModule ();

			ChangeDivisionModule.Initialize ();

            Assert.IsTrue(controller.RunCalled);
        }

		private TestableCheckInModule CreateTestableCheckInModule ()
        {
            this.container = new MockUnityResolver();
			this.controller = new MockCheckInController ();

			container.Bag.Add (typeof (ICheckInController), controller);
			container.Bag.Add (typeof (ICheckInView), null);

			return new TestableCheckInModule (container);
        }

        internal class TestableCheckInModule : CheckInModule
        {
			public TestableCheckInModule (IUnityContainer container)
                : base(container, null)
            {
            }

            public void InvokeRegisterViewsAndServices()
            {
                base.RegisterViewsAndServices();
            }
        }
    }
}
