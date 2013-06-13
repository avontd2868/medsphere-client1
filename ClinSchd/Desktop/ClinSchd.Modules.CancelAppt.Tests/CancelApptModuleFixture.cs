using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.CancelAppt.CancelAppt;
using ClinSchd.Modules.CancelAppt.Controllers;
using ClinSchd.Modules.CancelAppt.Services;
using ClinSchd.Modules.CancelAppt.Tests.Mocks;

namespace ClinSchd.Modules.CancelAppt.Tests
{
    /// <summary>
	/// Summary description for CancelAppt
    /// </summary>
    [TestClass]
	public class CancelApptModuleFixture
    {
        private MockUnityResolver container;
		private MockCancelApptController controller;

        [TestMethod]
		public void CancelApptModuleRegistersCancelApptViewAndCancelApptService ()
        {
			TestableCancelApptModule CancelApptModule = CreateTestableCancelApptModule ();

			CancelApptModule.InvokeRegisterViewsAndServices ();

			Assert.AreEqual (typeof (CancelApptView), container.Types[typeof (ICancelApptView)]);
			Assert.AreEqual (typeof (CancelApptController), container.Types[typeof (ICancelApptController)]);
			Assert.AreEqual (typeof (CancelApptPresentationModel), container.Types[typeof (ICancelApptPresentationModel)]);
			Assert.AreEqual (typeof (CancelApptService), container.Types[typeof (ICancelApptService)]);
#if !SILVERLIGHT
#endif
        }

        [TestMethod]
		public void InitCallsRunOnChangeDivisionController ()
        {
			var ChangeDivisionModule = CreateTestableCancelApptModule ();

			ChangeDivisionModule.Initialize ();

            Assert.IsTrue(controller.RunCalled);
        }

		private TestableCancelApptModule CreateTestableCancelApptModule ()
        {
            this.container = new MockUnityResolver();
			this.controller = new MockCancelApptController ();

			container.Bag.Add (typeof (ICancelApptController), controller);
			container.Bag.Add (typeof (ICancelApptView), null);

			return new TestableCancelApptModule (container);
        }

        internal class TestableCancelApptModule : CancelApptModule
        {
			public TestableCancelApptModule (IUnityContainer container)
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
