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
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Regions;
using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Navigation.ResourceTree;

namespace ClinSchd.Modules.Navigation.ResourceTree.Controllers
{
    public class NavigationResourceTreeController : INavigationResourceTreeController
    {
        private readonly IRegionManager regionManager;
        private readonly IResourceTreePresentationModel ResourceTreePresentationModel;
        private readonly IEventAggregator eventAggregator;

		public NavigationResourceTreeController(IRegionManager regionManager, IResourceTreePresentationModel ResourceTreePresentationModel, IEventAggregator eventAggregator)
        {
            this.regionManager = regionManager;
            this.ResourceTreePresentationModel = ResourceTreePresentationModel;
            this.eventAggregator = eventAggregator;
            this.ResourceTreePresentationModel.Controller = this;
        }

        public void Run()
        {
			this.regionManager.Regions[RegionNames.NavigatorGroupRegion].Add(ResourceTreePresentationModel.View);
        }
    }
}
