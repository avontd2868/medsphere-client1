using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.PatientSelection.PatientSelection;
using ClinSchd.Modules.PatientSelection.Controllers;
using ClinSchd.Modules.PatientSelection.Services;
using ClinSchd.Modules.PatientSelection.Tests.Mocks;

namespace ClinSchd.Modules.PatientSelection.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class PatientSelectionModuleFixture
    {
        private MockUnityResolver container;
        private MockPatientSelectionController controller;

        [TestMethod]
        public void PatientSelectionModuleRegistersPatientSelectionViewAndPatientSelectionService()
        {
            TestablePatientSelectionModule PatientSelectionModule = CreateTestablePatientSelectionModule();

            PatientSelectionModule.InvokeRegisterViewsAndServices();

			Assert.AreEqual(typeof(PatientSelectionView), container.Types[typeof(IPatientSelectionView)]);
            Assert.AreEqual(typeof(PatientSelectionController), container.Types[typeof(IPatientSelectionController)]);
			Assert.AreEqual(typeof(PatientSelectionPresentationModel), container.Types[typeof(IPatientSelectionPresentationModel)]);
            Assert.AreEqual(typeof(PatientSelectionService), container.Types[typeof(IPatientSelectionService)]);
#if !SILVERLIGHT
#endif
        }

        [TestMethod]
        public void InitCallsRunOnPatientSelectionController()
        {
            var PatientSelectionModule = CreateTestablePatientSelectionModule();

            PatientSelectionModule.Initialize();

            Assert.IsTrue(controller.RunCalled);
        }

        private TestablePatientSelectionModule CreateTestablePatientSelectionModule()
        {
            this.container = new MockUnityResolver();
            this.controller = new MockPatientSelectionController();

            container.Bag.Add(typeof(IPatientSelectionController), controller);
            container.Bag.Add(typeof(IPatientSelectionView), null);

            return new TestablePatientSelectionModule(container);
        }

        internal class TestablePatientSelectionModule : PatientSelectionModule
        {
            public TestablePatientSelectionModule(IUnityContainer container)
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
