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
using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClinSchd.Modules.Management.CopyAppointments.Services;

namespace ClinSchd.Modules.Management.CopyAppointments.Tests.Services
{
    [TestClass]
	public class ManagementCopyAppointmentsServiceFixture
    {
        [TestMethod]
        public void HavingACurrentCultureDifferentThanEnglishShouldNotThrows()
        {
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("es-AR");

			ManagementCopyAppointmentsService ManagementAccessTypesService = new ManagementCopyAppointmentsService ();

            Thread.CurrentThread.CurrentCulture = currentCulture;
        }
    }
}
