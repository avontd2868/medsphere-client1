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
using ClinSchd.Modules.Management.Resources;
using ClinSchd.Modules.Management.Resources.Controllers;
using ClinSchd.Modules.Management.Resources.Services;
using ClinSchd.Modules.Management.Resources.Tests.Mocks;

namespace ClinSchd.Modules.Management.Resources.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
	public class ManagementResourcesModuleFixture
    {
        private MockUnityResolver container;
		private MockManagementResourcesController controller;

        [TestMethod]
		public void ManagementResourcesModuleRegistersManagementResourcesViewAndManagementResourcesService()
        {
			TestableManagementResourcesModule ManagementResourcesModule = CreateTestableManagementResourcesModule();

			ManagementResourcesModule.InvokeRegisterViewsAndServices();

#if !SILVERLIGHT
			Assert.AreEqual(typeof(ResourcesView), container.Types[typeof(IResourcesView)]);
			Assert.AreEqual(typeof(ManagementResourcesController), container.Types[typeof(IManagementResourcesController)]);
			Assert.AreEqual(typeof(ResourcesPresentationModel), container.Types[typeof(IResourcesPresentationModel)]);
			Assert.AreEqual(typeof(ManagementResourcesService), container.Types[typeof(IManagementResourcesService)]);
#endif
        }

		private TestableManagementResourcesModule CreateTestableManagementResourcesModule()
        {
            this.container = new MockUnityResolver();
			this.controller = new MockManagementResourcesController();

			container.Bag.Add(typeof(IManagementResourcesController), controller);
			container.Bag.Add(typeof(IResourcesView), null);

			return new TestableManagementResourcesModule(container);
        }

        internal class TestableManagementResourcesModule : ManagementResourcesModule
        {
			public TestableManagementResourcesModule(IUnityContainer container)
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
