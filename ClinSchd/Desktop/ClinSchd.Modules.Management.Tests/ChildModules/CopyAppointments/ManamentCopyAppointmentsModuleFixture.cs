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
using ClinSchd.Modules.Management.CopyAppointments;
using ClinSchd.Modules.Management.CopyAppointments.Controllers;
using ClinSchd.Modules.Management.CopyAppointments.Services;
using ClinSchd.Modules.Management.CopyAppointments.Tests.Mocks;

namespace ClinSchd.Modules.Management.CopyAppointments.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
	public class ManagementCopyAppointmentsModuleFixture
    {
        private MockUnityResolver container;
		private MockManagementCopyAppointmentsController controller;

        [TestMethod]
		public void ManagementCopyAppointmentsModuleRegistersManagementCopyAppointmentsViewAndManagementCopyAppointmentsService ()
        {
			TestableManagementCopyAppointmentsModule ManagementCopyAppointmentsModule = CreateTestableManagementCopyAppointmentsModule ();

			ManagementCopyAppointmentsModule.InvokeRegisterViewsAndServices ();

			Assert.AreEqual (typeof (CopyAppointmentsView), container.Types[typeof (ICopyAppointmentsView)]);
			Assert.AreEqual (typeof (ManagementCopyAppointmentsController), container.Types[typeof (IManagementCopyAppointmentsController)]);
			Assert.AreEqual (typeof (CopyAppointmentsPresentationModel), container.Types[typeof (ICopyAppointmentsPresentationModel)]);
			Assert.AreEqual (typeof (ManagementCopyAppointmentsService), container.Types[typeof (IManagementCopyAppointmentsService)]);
#if !SILVERLIGHT
#endif
        }

        [TestMethod]
		public void InitCallsRunOnManagementCopyAppointmentsController ()
        {
			var ManagementCopyAppointmentsModule = CreateTestableManagementCopyAppointmentsModule ();

			ManagementCopyAppointmentsModule.Initialize ();

            Assert.IsTrue(controller.RunCalled);
        }

		private TestableManagementCopyAppointmentsModule CreateTestableManagementCopyAppointmentsModule ()
        {
            this.container = new MockUnityResolver();
			this.controller = new MockManagementCopyAppointmentsController ();

			container.Bag.Add (typeof (IManagementCopyAppointmentsController), controller);

			return new TestableManagementCopyAppointmentsModule (container);
        }

        internal class TestableManagementCopyAppointmentsModule : ManagementCopyAppointmentsModule
        {
			public TestableManagementCopyAppointmentsModule (IUnityContainer container)
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
