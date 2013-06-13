using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.Navigation.Group;
using ClinSchd.Modules.Navigation.Controllers;
using ClinSchd.Modules.Navigation.Services;
using ClinSchd.Modules.Navigation.Tests.Mocks;

namespace ClinSchd.Modules.Navigation.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class NavigationModuleFixture
    {
        private MockUnityResolver container;
        private MockNavigationController controller;

        private TestableNavigationModule CreateTestableNavigationModule()
        {
            this.container = new MockUnityResolver();
            this.controller = new MockNavigationController();

            container.Bag.Add(typeof(INavigationController), controller);

            return new TestableNavigationModule(container);
        }

        internal class TestableNavigationModule : NavigationModule
        {
            public TestableNavigationModule(IUnityContainer container)
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
