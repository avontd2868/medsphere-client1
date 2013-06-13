using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.Task.Group;
using ClinSchd.Modules.Task.Controllers;
using ClinSchd.Modules.Task.Services;
using ClinSchd.Modules.Task.Tests.Mocks;

namespace ClinSchd.Modules.Task.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class TaskModuleFixture
    {
        private MockUnityResolver container;
        private MockTaskController controller;

        [TestMethod]
        public void TaskModuleRegistersTaskViewAndTaskService()
        {
            TestableTaskModule taskModule = CreateTestableTaskModule();

            taskModule.InvokeRegisterViewsAndServices();

			Assert.AreEqual(typeof(GroupView), container.Types[typeof(IGroupView)]);
            Assert.AreEqual(typeof(TaskController), container.Types[typeof(ITaskController)]);
			Assert.AreEqual(typeof(GroupPresentationModel), container.Types[typeof(IGroupPresentationModel)]);
            Assert.AreEqual(typeof(TaskService), container.Types[typeof(ITaskService)]);
#if !SILVERLIGHT
#endif
        }

        [TestMethod]
        public void InitCallsRunOnTaskController()
        {
            var taskModule = CreateTestableTaskModule();

            taskModule.Initialize();

            Assert.IsTrue(controller.RunCalled);
        }

        private TestableTaskModule CreateTestableTaskModule()
        {
            this.container = new MockUnityResolver();
            this.controller = new MockTaskController();

            container.Bag.Add(typeof(ITaskController), controller);

            return new TestableTaskModule(container);
        }

        internal class TestableTaskModule : TaskModule
        {
            public TestableTaskModule(IUnityContainer container)
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
