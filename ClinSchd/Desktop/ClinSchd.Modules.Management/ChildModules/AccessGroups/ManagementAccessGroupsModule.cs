using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Unity;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.Management.AccessGroups;
using ClinSchd.Modules.Management.AccessGroups.Controllers;
using ClinSchd.Modules.Management.AccessGroups.Services;

namespace ClinSchd.Modules.Management.AccessGroups
{
    public class ManagementAccessGroupsModule : IModule
    {
        private readonly IUnityContainer container;

		public ManagementAccessGroupsModule(IUnityContainer container)
        {
            this.container = container;
        }

        public void Initialize()
        {
            this.RegisterViewsAndServices();

			IManagementAccessGroupsController controller = this.container.Resolve<IManagementAccessGroupsController>();
            controller.Run();
        }

        protected void RegisterViewsAndServices()
        {
			this.container.RegisterType<IManagementAccessGroupsController, ManagementAccessGroupsController>(new ContainerControlledLifetimeManager());
			this.container.RegisterType<IAccessGroupsView, AccessGroupsView>();
			this.container.RegisterType<IAccessGroupsPresentationModel, AccessGroupsPresentationModel>();
			this.container.RegisterType<IManagementAccessGroupsService, ManagementAccessGroupsService>(new ContainerControlledLifetimeManager());
        }
    }
}
