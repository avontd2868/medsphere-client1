using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.Task.Scheduler;
using ClinSchd.Modules.Task.Scheduler.Controllers;
using ClinSchd.Modules.Task.Scheduler.Services;
using ClinSchd.Modules.Task.Scheduler.Tests.Mocks;

namespace ClinSchd.Modules.Task.Scheduler.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
	public class TaskSchedulerModuleFixture
    {
        private MockUnityResolver container;
		private MockTaskSchedulerController controller;

        [TestMethod]
		public void TaskSchedulerModuleRegistersTaskSchedulerViewAndTaskSchedulerService()
        {
			TestableTaskSchedulerModule taskSchedulerModule = CreateTestableTaskSchedulerModule();

			taskSchedulerModule.InvokeRegisterViewsAndServices();

			Assert.AreEqual(typeof(SchedulerView), container.Types[typeof(ISchedulerView)]);
			Assert.AreEqual(typeof(TaskSchedulerController), container.Types[typeof(ITaskSchedulerController)]);
			Assert.AreEqual(typeof(SchedulerPresentationModel), container.Types[typeof(ISchedulerPresentationModel)]);
			Assert.AreEqual(typeof(TaskSchedulerService), container.Types[typeof(ITaskSchedulerService)]);
#if !SILVERLIGHT
#endif
        }

        [TestMethod]
		public void InitCallsRunOnTaskSchedulerController()
        {
			var taskSchedulerModule = CreateTestableTaskSchedulerModule();

			taskSchedulerModule.Initialize();

            Assert.IsTrue(controller.RunCalled);
        }

		private TestableTaskSchedulerModule CreateTestableTaskSchedulerModule()
        {
            this.container = new MockUnityResolver();
			this.controller = new MockTaskSchedulerController();

			container.Bag.Add(typeof(ITaskSchedulerController), controller);

			return new TestableTaskSchedulerModule(container);
        }

        internal class TestableTaskSchedulerModule : TaskSchedulerModule
        {
			public TestableTaskSchedulerModule(IUnityContainer container)
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
