using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.PatientAppt.Group;
using ClinSchd.Modules.PatientAppt.Controllers;
using ClinSchd.Modules.PatientAppt.Services;
using ClinSchd.Modules.PatientAppt.Tests.Mocks;

namespace ClinSchd.Modules.PatientAppt.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class PatientApptModuleFixture
    {
        private MockUnityResolver container;
        private MockPatientApptController controller;

        [TestMethod]
        public void PatientApptModuleRegistersPatientApptViewAndPatientApptService()
        {
            TestablePatientApptModule PatientApptModule = CreateTestablePatientApptModule();

            PatientApptModule.InvokeRegisterViewsAndServices();

			Assert.AreEqual(typeof(GroupView), container.Types[typeof(IGroupView)]);
            Assert.AreEqual(typeof(PatientApptController), container.Types[typeof(IPatientApptController)]);
			Assert.AreEqual(typeof(GroupPresentationModel), container.Types[typeof(IGroupPresentationModel)]);
            Assert.AreEqual(typeof(PatientApptService), container.Types[typeof(IPatientApptService)]);
#if !SILVERLIGHT
#endif
        }

        [TestMethod]
        public void InitCallsRunOnPatientApptController()
        {
            var PatientApptModule = CreateTestablePatientApptModule();

            PatientApptModule.Initialize();

            Assert.IsTrue(controller.RunCalled);
        }

        private TestablePatientApptModule CreateTestablePatientApptModule()
        {
            this.container = new MockUnityResolver();
            this.controller = new MockPatientApptController();

            container.Bag.Add(typeof(IPatientApptController), controller);

            return new TestablePatientApptModule(container);
        }

        internal class TestablePatientApptModule : PatientApptModule
        {
			public TestablePatientApptModule (IUnityContainer container)
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
