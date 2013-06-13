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
using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Unity;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.Navigation.ResourceTree;
using ClinSchd.Modules.Navigation.ResourceTree.Controllers;
using ClinSchd.Modules.Navigation.ResourceTree.Services;

namespace ClinSchd.Modules.Navigation.ResourceTree
{
    public class NavigationResourceTreeModule : IModule
    {
        private readonly IUnityContainer container;

		public NavigationResourceTreeModule(IUnityContainer container)
        {
            this.container = container;
        }

        public void Initialize()
        {
            this.RegisterViewsAndServices();

			INavigationResourceTreeController controller = this.container.Resolve<INavigationResourceTreeController>();
            controller.Run();
        }

        protected void RegisterViewsAndServices()
        {
			this.container.RegisterType<INavigationResourceTreeController, NavigationResourceTreeController>(new ContainerControlledLifetimeManager());
            this.container.RegisterType<IResourceTreeView, ResourceTreeView>();
            this.container.RegisterType<IResourceTreePresentationModel, ResourceTreePresentationModel>();
			this.container.RegisterType<INavigationResourceTreeService, NavigationResourceTreeService>(new ContainerControlledLifetimeManager());
        }
    }
}
