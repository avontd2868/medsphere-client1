using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.FindAppt.FindAppt;
using ClinSchd.Modules.FindAppt.Controllers;
using ClinSchd.Modules.FindAppt.Services;
using ClinSchd.Modules.FindAppt.Tests.Mocks;

namespace ClinSchd.Modules.FindAppt.Tests
{
    /// <summary>
	/// Summary description for FindAppt
    /// </summary>
    [TestClass]
	public class FindApptModuleFixture
    {
        private MockUnityResolver container;
		private MockFindApptController controller;

        [TestMethod]
		public void FindApptModuleRegistersFindApptViewAndFindApptService ()
        {
			TestableFindApptModule FindApptModule = CreateTestableFindApptModule ();

			FindApptModule.InvokeRegisterViewsAndServices ();

			Assert.AreEqual (typeof (FindApptView), container.Types[typeof (IFindApptView)]);
			Assert.AreEqual (typeof (FindApptController), container.Types[typeof (IFindApptController)]);
			Assert.AreEqual (typeof (FindApptPresentationModel), container.Types[typeof (IFindApptPresentationModel)]);
			Assert.AreEqual (typeof (FindApptService), container.Types[typeof (IFindApptService)]);
#if !SILVERLIGHT
#endif
        }

        [TestMethod]
		public void InitCallsRunOnChangeDivisionController ()
        {
			var ChangeDivisionModule = CreateTestableFindApptModule ();

			ChangeDivisionModule.Initialize ();

            Assert.IsTrue(controller.RunCalled);
        }

		private TestableFindApptModule CreateTestableFindApptModule ()
        {
            this.container = new MockUnityResolver();
			this.controller = new MockFindApptController ();

			container.Bag.Add (typeof (IFindApptController), controller);
			container.Bag.Add (typeof (IFindApptView), null);

			return new TestableFindApptModule (container);
        }

        internal class TestableFindApptModule : FindApptModule
        {
			public TestableFindApptModule (IUnityContainer container)
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
