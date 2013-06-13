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
using ClinSchd.Modules.Management.WorkStations;
using ClinSchd.Modules.Management.WorkStations.Controllers;
using ClinSchd.Modules.Management.WorkStations.Services;
using ClinSchd.Modules.Management.WorkStations.Tests.Mocks;

namespace ClinSchd.Modules.Management.WorkStations.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
	public class ManagementWorkStationsModuleFixture
    {
        private MockUnityResolver container;
		private MockManagementWorkStationsController controller;

        [TestMethod]
		public void ManagementWorkStationsModuleRegistersManagementWorkStationsViewAndManagementWorkStationsService ()
        {
			TestableManagementWorkStationsModule ManagementWorkStationsModule = CreateTestableManagementWorkStationsModule ();

			ManagementWorkStationsModule.InvokeRegisterViewsAndServices ();

			Assert.AreEqual (typeof (WorkStationsView), container.Types[typeof (IWorkStationsView)]);
			Assert.AreEqual (typeof (ManagementWorkStationsController), container.Types[typeof (IManagementWorkStationsController)]);
			Assert.AreEqual (typeof (WorkStationsPresentationModel), container.Types[typeof (IWorkStationsPresentationModel)]);
			Assert.AreEqual (typeof (ManagementWorkStationsService), container.Types[typeof (IManagementWorkStationsService)]);
#if !SILVERLIGHT
#endif
        }

        [TestMethod]
		public void InitCallsRunOnManagementWorkStationsController ()
        {
			var ManagementWorkStationsModule = CreateTestableManagementWorkStationsModule ();

			ManagementWorkStationsModule.Initialize ();

            Assert.IsTrue(controller.RunCalled);
        }

		private TestableManagementWorkStationsModule CreateTestableManagementWorkStationsModule ()
        {
            this.container = new MockUnityResolver();
			this.controller = new MockManagementWorkStationsController ();

			container.Bag.Add (typeof (IManagementWorkStationsController), controller);

			return new TestableManagementWorkStationsModule (container);
        }

        internal class TestableManagementWorkStationsModule : ManagementWorkStationsModule
        {
			public TestableManagementWorkStationsModule (IUnityContainer container)
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
