using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.Ribbon.Schedule;
using ClinSchd.Modules.Ribbon.Controllers;
using ClinSchd.Modules.Ribbon.Services;
using ClinSchd.Modules.Ribbon.Tests.Mocks;

namespace ClinSchd.Modules.Ribbon.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class RibbonModuleFixture
    {
        private MockUnityResolver container;
        private MockRibbonController controller;

        [TestMethod]
        public void RibbonModuleRegistersRibbonViewAndRibbonService()
        {
            TestableRibbonModule RibbonModule = CreateTestableRibbonModule();

            RibbonModule.InvokeRegisterViewsAndServices();

			Assert.AreEqual (typeof (ScheduleView), container.Types[typeof (IScheduleView)]);
            Assert.AreEqual(typeof(RibbonController), container.Types[typeof(IRibbonController)]);
			Assert.AreEqual (typeof (SchedulePresentationModel), container.Types[typeof (ISchedulePresentationModel)]);
            Assert.AreEqual(typeof(RibbonService), container.Types[typeof(IRibbonService)]);
#if !SILVERLIGHT
#endif
        }

        [TestMethod]
        public void InitCallsRunOnRibbonController()
        {
            var RibbonModule = CreateTestableRibbonModule();

            RibbonModule.Initialize();

            Assert.IsTrue(controller.RunCalled);
        }

        private TestableRibbonModule CreateTestableRibbonModule()
        {
            this.container = new MockUnityResolver();
            this.controller = new MockRibbonController();

            container.Bag.Add(typeof(IRibbonController), controller);

            return new TestableRibbonModule(container);
        }

        internal class TestableRibbonModule : RibbonModule
        {
            public TestableRibbonModule(IUnityContainer container)
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
