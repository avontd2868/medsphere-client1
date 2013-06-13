using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.ChangeUser.ChangeUser;
using ClinSchd.Modules.ChangeUser.Controllers;
using ClinSchd.Modules.ChangeUser.Services;
using ClinSchd.Modules.ChangeUser.Tests.Mocks;

namespace ClinSchd.Modules.ChangeUser.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
	public class ChangeUserModuleFixture
    {
        private MockUnityResolver container;
		private MockChangeUserController controller;

        [TestMethod]
		public void ChangeUserModuleRegistersChangeUserViewAndChangeUserService ()
        {
			TestableChangeUserModule ChangeUserModule = CreateTestableChangeUserModule ();

			ChangeUserModule.InvokeRegisterViewsAndServices ();

			Assert.AreEqual (typeof (ChangeUserView), container.Types[typeof (IChangeUserView)]);
			Assert.AreEqual (typeof (ChangeUserController), container.Types[typeof (IChangeUserController)]);
			Assert.AreEqual (typeof (ChangeUserPresentationModel), container.Types[typeof (IChangeUserPresentationModel)]);
			Assert.AreEqual (typeof (ChangeUserService), container.Types[typeof (IChangeUserService)]);
#if !SILVERLIGHT
#endif
        }

        [TestMethod]
		public void InitCallsRunOnChangeUserController ()
        {
			var ChangeUserModule = CreateTestableChangeUserModule ();

			ChangeUserModule.Initialize ();

            Assert.IsTrue(controller.RunCalled);
        }

		private TestableChangeUserModule CreateTestableChangeUserModule ()
        {
            this.container = new MockUnityResolver();
			this.controller = new MockChangeUserController ();

			container.Bag.Add (typeof (IChangeUserController), controller);
			container.Bag.Add (typeof (IChangeUserView), null);

			return new TestableChangeUserModule (container);
        }

        internal class TestableChangeUserModule : ChangeUserModule
        {
			public TestableChangeUserModule (IUnityContainer container)
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
