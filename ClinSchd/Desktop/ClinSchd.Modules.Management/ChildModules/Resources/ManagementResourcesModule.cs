using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.Management.Resources;
using ClinSchd.Modules.Management.Resources.Controllers;
using ClinSchd.Modules.Management.Resources.Services;

namespace ClinSchd.Modules.Management.Resources
{
    public class ManagementResourcesModule : IModule
    {
        private readonly IUnityContainer container;

		public ManagementResourcesModule (IUnityContainer container, IEventAggregator eventAggregator)
        {
            this.container = container;
		}

		public void Initialize()
		{
			this.RegisterViewsAndServices();

			IManagementResourcesController controller = this.container.Resolve<IManagementResourcesController>();
			controller.Run();
		}

        protected void RegisterViewsAndServices()
        {
			this.container.RegisterType<IManagementResourcesController, ManagementResourcesController>();
			this.container.RegisterType<IResourcesView, ResourcesView>();
			this.container.RegisterType<IResourcesPresentationModel, ResourcesPresentationModel>();
			this.container.RegisterType<IManagementResourcesService, ManagementResourcesService>();
        }
    }
}
