using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Unity;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.Navigation.Group;
using ClinSchd.Modules.Navigation.Controllers;
using ClinSchd.Modules.Navigation.Services;

namespace ClinSchd.Modules.Navigation
{
    public class NavigationModule : IModule
    {
        private readonly IUnityContainer container;

        public NavigationModule(IUnityContainer container)
        {
            this.container = container;
        }

        public void Initialize()
        {
            this.RegisterViewsAndServices();

            INavigationController controller = this.container.Resolve<INavigationController>();
            controller.Run();
        }

        protected void RegisterViewsAndServices()
        {
            this.container.RegisterType<INavigationController, NavigationController>(new ContainerControlledLifetimeManager());
			this.container.RegisterType<IGroupView, GroupView>();
			this.container.RegisterType<IGroupPresentationModel, GroupPresentationModel>();
            this.container.RegisterType<INavigationService, NavigationService>(new ContainerControlledLifetimeManager());
        }
    }
}
