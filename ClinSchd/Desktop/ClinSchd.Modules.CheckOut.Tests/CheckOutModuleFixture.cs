using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.CheckOut.CheckOut;
using ClinSchd.Modules.CheckOut.Controllers;
using ClinSchd.Modules.CheckOut.Services;
using ClinSchd.Modules.CheckOut.Tests.Mocks;

namespace ClinSchd.Modules.CheckOut.Tests
{
    /// <summary>
	/// Summary description for CheckOut
    /// </summary>
    [TestClass]
	public class CheckOutModuleFixture
    {
        private MockUnityResolver container;
		private MockCheckOutController controller;

        [TestMethod]
		public void CheckOutModuleRegistersCheckOutViewAndCheckOutService ()
        {
			TestableCheckOutModule CheckOutModule = CreateTestableCheckOutModule ();

			CheckOutModule.InvokeRegisterViewsAndServices ();

			Assert.AreEqual (typeof (CheckOutView), container.Types[typeof (ICheckOutView)]);
			Assert.AreEqual (typeof (CheckOutController), container.Types[typeof (ICheckOutController)]);
			Assert.AreEqual (typeof (CheckOutPresentationModel), container.Types[typeof (ICheckOutPresentationModel)]);
			Assert.AreEqual (typeof (CheckOutService), container.Types[typeof (ICheckOutService)]);
#if !SILVERLIGHT
#endif
        }

        [TestMethod]
		public void InitCallsRunOnChangeDivisionController ()
        {
			var ChangeDivisionModule = CreateTestableCheckOutModule ();

			ChangeDivisionModule.Initialize ();

            Assert.IsTrue(controller.RunCalled);
        }

		private TestableCheckOutModule CreateTestableCheckOutModule ()
        {
            this.container = new MockUnityResolver();
			this.controller = new MockCheckOutController ();

			container.Bag.Add (typeof (ICheckOutController), controller);
			container.Bag.Add (typeof (ICheckOutView), null);

			return new TestableCheckOutModule (container);
        }

        internal class TestableCheckOutModule : CheckOutModule
        {
			public TestableCheckOutModule (IUnityContainer container)
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
