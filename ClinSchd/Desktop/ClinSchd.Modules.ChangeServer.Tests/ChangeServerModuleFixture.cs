using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.ChangeServer.ChangeServer;
using ClinSchd.Modules.ChangeServer.Controllers;
using ClinSchd.Modules.ChangeServer.Services;
using ClinSchd.Modules.ChangeServer.Tests.Mocks;

namespace ClinSchd.Modules.ChangeServer.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
	public class ChangeServerModuleFixture
    {
        private MockUnityResolver container;
		private MockChangeServerController controller;

        [TestMethod]
		public void ChangeServerModuleRegistersChangeServerViewAndChangeServerService ()
        {
			TestableChangeServerModule ChangeServerModule = CreateTestableChangeServerModule ();

			ChangeServerModule.InvokeRegisterViewsAndServices ();

			Assert.AreEqual (typeof (ChangeServerView), container.Types[typeof (IChangeServerView)]);
			Assert.AreEqual (typeof (ChangeServerController), container.Types[typeof (IChangeServerController)]);
			Assert.AreEqual (typeof (ChangeServerPresentationModel), container.Types[typeof (IChangeServerPresentationModel)]);
			Assert.AreEqual (typeof (ChangeServerService), container.Types[typeof (IChangeServerService)]);
#if !SILVERLIGHT
#endif
        }

        [TestMethod]
		public void InitCallsRunOnChangeServerController ()
        {
			var ChangeServerModule = CreateTestableChangeServerModule ();

			ChangeServerModule.Initialize ();

            Assert.IsTrue(controller.RunCalled);
        }

		private TestableChangeServerModule CreateTestableChangeServerModule ()
        {
            this.container = new MockUnityResolver();
			this.controller = new MockChangeServerController ();

			container.Bag.Add (typeof (IChangeServerController), controller);
			container.Bag.Add (typeof (IChangeServerView), null);

			return new TestableChangeServerModule (container);
        }

        internal class TestableChangeServerModule : ChangeServerModule
        {
			public TestableChangeServerModule (IUnityContainer container)
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
