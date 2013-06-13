using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.Management.AccessGroups;
using ClinSchd.Modules.Management.AccessGroups.Controllers;
using ClinSchd.Modules.Management.AccessGroups.Services;
using ClinSchd.Modules.Management.AccessGroups.Tests.Mocks;

namespace ClinSchd.Modules.Management.AccessGroups.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
	public class ManagementAccessGroupsModuleFixture
    {
        private MockUnityResolver container;
		private MockManagementAccessGroupsController controller;

        [TestMethod]
		public void ManagementAccessGroupsModuleRegistersManagementAccessGroupsViewAndManagementAccessGroupsService()
        {
			TestableManagementAccessGroupsModule ManagementAccessGroupsModule = CreateTestableManagementAccessGroupsModule();

			ManagementAccessGroupsModule.InvokeRegisterViewsAndServices();

			Assert.AreEqual(typeof(AccessGroupsView), container.Types[typeof(IAccessGroupsView)]);
			Assert.AreEqual(typeof(ManagementAccessGroupsController), container.Types[typeof(IManagementAccessGroupsController)]);
			Assert.AreEqual(typeof(AccessGroupsPresentationModel), container.Types[typeof(IAccessGroupsPresentationModel)]);
			Assert.AreEqual(typeof(ManagementAccessGroupsService), container.Types[typeof(IManagementAccessGroupsService)]);
#if !SILVERLIGHT
#endif
        }

        [TestMethod]
		public void InitCallsRunOnManagementAccessGroupsController()
        {
			var ManagementAccessGroupsModule = CreateTestableManagementAccessGroupsModule();

			ManagementAccessGroupsModule.Initialize();

            Assert.IsTrue(controller.RunCalled);
        }

		private TestableManagementAccessGroupsModule CreateTestableManagementAccessGroupsModule()
        {
            this.container = new MockUnityResolver();
			this.controller = new MockManagementAccessGroupsController();

			container.Bag.Add(typeof(IManagementAccessGroupsController), controller);
			container.Bag.Add(typeof(IAccessGroupsView), null);

			return new TestableManagementAccessGroupsModule(container);
        }

        internal class TestableManagementAccessGroupsModule : ManagementAccessGroupsModule
        {
			public TestableManagementAccessGroupsModule(IUnityContainer container)
                : base(container)
            {
            }

            public void InvokeRegisterViewsAndServices()
            {
                base.RegisterViewsAndServices();
            }
        }
    }
}
