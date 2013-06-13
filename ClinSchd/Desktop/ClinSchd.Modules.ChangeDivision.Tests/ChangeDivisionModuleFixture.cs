using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.ChangeDivision.ChangeDivision;
using ClinSchd.Modules.ChangeDivision.Controllers;
using ClinSchd.Modules.ChangeDivision.Services;
using ClinSchd.Modules.ChangeDivision.Tests.Mocks;

namespace ClinSchd.Modules.ChangeDivision.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
	public class ChangeDivisionModuleFixture
    {
        private MockUnityResolver container;
		private MockChangeDivisionController controller;

        [TestMethod]
		public void ChangeDivisionModuleRegistersChangeDivisionViewAndChangeDivisionService ()
        {
			TestableChangeDivisionModule ChangeDivisionModule = CreateTestableChangeDivisionModule ();

			ChangeDivisionModule.InvokeRegisterViewsAndServices ();

			Assert.AreEqual (typeof (ChangeDivisionView), container.Types[typeof (IChangeDivisionView)]);
			Assert.AreEqual (typeof (ChangeDivisionController), container.Types[typeof (IChangeDivisionController)]);
			Assert.AreEqual (typeof (ChangeDivisionPresentationModel), container.Types[typeof (IChangeDivisionPresentationModel)]);
			Assert.AreEqual (typeof (ChangeDivisionService), container.Types[typeof (IChangeDivisionService)]);
#if !SILVERLIGHT
#endif
        }

        [TestMethod]
		public void InitCallsRunOnChangeDivisionController ()
        {
			var ChangeDivisionModule = CreateTestableChangeDivisionModule ();

			ChangeDivisionModule.Initialize ();

            Assert.IsTrue(controller.RunCalled);
        }

		private TestableChangeDivisionModule CreateTestableChangeDivisionModule ()
        {
            this.container = new MockUnityResolver();
			this.controller = new MockChangeDivisionController ();

			container.Bag.Add (typeof (IChangeDivisionController), controller);
			container.Bag.Add (typeof (IChangeDivisionView), null);

			return new TestableChangeDivisionModule (container);
        }

        internal class TestableChangeDivisionModule : ChangeDivisionModule
        {
			public TestableChangeDivisionModule (IUnityContainer container)
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
