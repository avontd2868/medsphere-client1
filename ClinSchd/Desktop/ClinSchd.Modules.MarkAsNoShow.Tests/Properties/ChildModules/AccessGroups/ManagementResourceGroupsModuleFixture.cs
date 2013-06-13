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
using ClinSchd.Modules.Management.ResourceGroups;
using ClinSchd.Modules.Management.ResourceGroups.Controllers;
using ClinSchd.Modules.Management.ResourceGroups.Services;
using ClinSchd.Modules.Management.ResourceGroups.Tests.Mocks;

namespace ClinSchd.Modules.Management.ResourceGroups.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
	public class ManagementResourceGroupsModuleFixture
    {
        private MockUnityResolver container;
		private MockManagementResourceGroupsController controller;

        [TestMethod]
		public void ManagementResourceGroupsModuleRegistersManagementResourceGroupsViewAndManagementResourceGroupsService()
        {
			TestableManagementResourceGroupsModule ManagementResourceGroupsModule = CreateTestableManagementResourceGroupsModule();

			ManagementResourceGroupsModule.InvokeRegisterViewsAndServices();

			Assert.AreEqual(typeof(ResourceGroupsView), container.Types[typeof(IResourceGroupsView)]);
			Assert.AreEqual(typeof(ManagementResourceGroupsController), container.Types[typeof(IManagementResourceGroupsController)]);
			Assert.AreEqual(typeof(ResourceGroupsPresentationModel), container.Types[typeof(IResourceGroupsPresentationModel)]);
			Assert.AreEqual(typeof(ManagementResourceGroupsService), container.Types[typeof(IManagementResourceGroupsService)]);
#if !SILVERLIGHT
#endif
        }

        [TestMethod]
		public void InitCallsRunOnManagementResourceGroupsController()
        {
			var ManagementResourceGroupsModule = CreateTestableManagementResourceGroupsModule();

			ManagementResourceGroupsModule.Initialize();

            Assert.IsTrue(controller.RunCalled);
        }

		private TestableManagementResourceGroupsModule CreateTestableManagementResourceGroupsModule()
        {
            this.container = new MockUnityResolver();
			this.controller = new MockManagementResourceGroupsController();

			container.Bag.Add(typeof(IManagementResourceGroupsController), controller);
			container.Bag.Add(typeof(IResourceGroupsView), null);

			return new TestableManagementResourceGroupsModule(container);
        }

        internal class TestableManagementResourceGroupsModule : ManagementResourceGroupsModule
        {
			public TestableManagementResourceGroupsModule(IUnityContainer container)
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
