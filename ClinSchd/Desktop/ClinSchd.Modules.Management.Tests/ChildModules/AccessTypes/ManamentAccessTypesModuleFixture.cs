//===================================================================================
// Microsoft patterns & practices
// Composite Application Guidance for Windows Presentation Foundation and Silverlight
//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===================================================================================
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
//===================================================================================
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.Management.AccessTypes;
using ClinSchd.Modules.Management.AccessTypes.Controllers;
using ClinSchd.Modules.Management.AccessTypes.Services;
using ClinSchd.Modules.Management.AccessTypes.Tests.Mocks;

namespace ClinSchd.Modules.Management.AccessTypes.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
	public class ManagementAccessTypesModuleFixture
    {
        private MockUnityResolver container;
		private MockManagementAccessTypesController controller;

        [TestMethod]
		public void ManagementAccessTypesModuleRegistersManagementAccessTypesViewAndManagementAccessTypesService()
        {
			TestableManagementAccessTypesModule ManagementAccessTypesModule = CreateTestableManagementAccessTypesModule();

			ManagementAccessTypesModule.InvokeRegisterViewsAndServices();

			Assert.AreEqual(typeof(AccessTypesView), container.Types[typeof(IAccessTypesView)]);
			Assert.AreEqual(typeof(ManagementAccessTypesController), container.Types[typeof(IManagementAccessTypesController)]);
			Assert.AreEqual(typeof(AccessTypesPresentationModel), container.Types[typeof(IAccessTypesPresentationModel)]);
			Assert.AreEqual(typeof(ManagementAccessTypesService), container.Types[typeof(IManagementAccessTypesService)]);
#if !SILVERLIGHT
#endif
        }

        [TestMethod]
		public void InitCallsRunOnManagementAccessTypesController()
        {
			var ManagementAccessTypesModule = CreateTestableManagementAccessTypesModule();

			ManagementAccessTypesModule.Initialize();

            Assert.IsTrue(controller.RunCalled);
        }

		private TestableManagementAccessTypesModule CreateTestableManagementAccessTypesModule()
        {
            this.container = new MockUnityResolver();
			this.controller = new MockManagementAccessTypesController();

			container.Bag.Add(typeof(IManagementAccessTypesController), controller);

			return new TestableManagementAccessTypesModule(container);
        }

        internal class TestableManagementAccessTypesModule : ManagementAccessTypesModule
        {
			public TestableManagementAccessTypesModule(IUnityContainer container)
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
