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
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Management.AccessTypes.Controllers;

namespace ClinSchd.Modules.Management.AccessTypes.Tests.Mocks
{
	internal class MockManagementAccessTypesController : IManagementAccessTypesController
    {
        public bool RunCalled;

        public void Run()
        {
            RunCalled = true;
        }
    }
}