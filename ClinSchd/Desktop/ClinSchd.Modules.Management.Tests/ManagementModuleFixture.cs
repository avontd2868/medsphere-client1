using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.Management.Group;
using ClinSchd.Modules.Management.Controllers;
using ClinSchd.Modules.Management.Services;
using ClinSchd.Modules.Management.Tests.Mocks;

namespace ClinSchd.Modules.Management.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class ManagementModuleFixture
    {
        private MockUnityResolver container;
        private MockManagementController controller;

        [TestMethod]
        public void ManagementModuleRegistersManagementViewAndManagementService()
        {
            TestableManagementModule ManagementModule = CreateTestableManagementModule();

            ManagementModule.InvokeRegisterViewsAndServices();

			Assert.AreEqual(typeof(GroupView), container.Types[typeof(IGroupView)]);
            Assert.AreEqual(typeof(ManagementController), container.Types[typeof(IManagementController)]);
			Assert.AreEqual(typeof(GroupPresentationModel), container.Types[typeof(IGroupPresentationModel)]);
            Assert.AreEqual(typeof(ManagementService), container.Types[typeof(IManagementService)]);
#if !SILVERLIGHT
#endif
        }

        [TestMethod]
        public void InitCallsRunOnManagementController()
        {
            var ManagementModule = CreateTestableManagementModule();

            ManagementModule.Initialize();

            Assert.IsTrue(controller.RunCalled);
        }

        private TestableManagementModule CreateTestableManagementModule()
        {
            this.container = new MockUnityResolver();
            this.controller = new MockManagementController();

            container.Bag.Add(typeof(IManagementController), controller);

            return new TestableManagementModule(container);
        }

        internal class TestableManagementModule : ManagementModule
        {
			public TestableManagementModule (IUnityContainer container)
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
