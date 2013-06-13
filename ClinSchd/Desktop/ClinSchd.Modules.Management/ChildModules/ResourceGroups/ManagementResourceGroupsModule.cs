using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Unity;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.Management.ResourceGroups;
using ClinSchd.Modules.Management.ResourceGroups.Controllers;
using ClinSchd.Modules.Management.ResourceGroups.Services;

namespace ClinSchd.Modules.Management.ResourceGroups
{
    public class ManagementResourceGroupsModule : IModule
    {
        private readonly IUnityContainer container;

		public ManagementResourceGroupsModule(IUnityContainer container)
        {
            this.container = container;
        }

        public void Initialize()
        {
            this.RegisterViewsAndServices();

			IManagementResourceGroupsController controller = this.container.Resolve<IManagementResourceGroupsController>();
            controller.Run();
        }

        protected void RegisterViewsAndServices()
        {
			this.container.RegisterType<IManagementResourceGroupsController, ManagementResourceGroupsController>(new ContainerControlledLifetimeManager());
			this.container.RegisterType<IResourceGroupsView, ResourceGroupsView>();
			this.container.RegisterType<IResourceGroupsPresentationModel, ResourceGroupsPresentationModel>();
			this.container.RegisterType<IManagementResourceGroupsService, ManagementResourceGroupsService>(new ContainerControlledLifetimeManager());
        }
    }
}
