using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.ResourceSelection.ResourceSelection;
using ClinSchd.Modules.ResourceSelection.Controllers;
using ClinSchd.Modules.ResourceSelection.Services;
using ClinSchd.Modules.ResourceSelection.Tests.Mocks;

namespace ClinSchd.Modules.ResourceSelection.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class ResourceSelectionModuleFixture
    {
        private MockUnityResolver container;
		private MockResourceSelectionController controller;

        [TestMethod]
        public void PatientSelectionModuleRegistersPatientSelectionViewAndPatientSelectionService()
        {
			TestablePatientSelectionModule ResourceSelectionModule = CreateTestablePatientSelectionModule ();

			ResourceSelectionModule.InvokeRegisterViewsAndServices ();

			Assert.AreEqual (typeof (ResourceSelectionView), container.Types[typeof (IResourceSelectionView)]);
            Assert.AreEqual(typeof(ResourceSelectionController), container.Types[typeof(IResourceSelectionController)]);
			Assert.AreEqual (typeof (ResourceSelectionPresentationModel), container.Types[typeof (IResourceSelectionPresentationModel)]);
            Assert.AreEqual(typeof(ResourceSelectionService), container.Types[typeof(IResourceSelectionService)]);
#if !SILVERLIGHT
#endif
        }

        [TestMethod]
        public void InitCallsRunOnPatientSelectionController()
        {
			var ResourceSelectionModule = CreateTestablePatientSelectionModule ();

			ResourceSelectionModule.Initialize ();

            Assert.IsTrue(controller.RunCalled);
        }

        private TestablePatientSelectionModule CreateTestablePatientSelectionModule()
        {
            this.container = new MockUnityResolver();
			this.controller = new MockResourceSelectionController ();

            container.Bag.Add(typeof(IResourceSelectionController), controller);
			container.Bag.Add (typeof (IResourceSelectionView), null);

            return new TestablePatientSelectionModule(container);
        }

        internal class TestablePatientSelectionModule : ResourceSelectionModule
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
