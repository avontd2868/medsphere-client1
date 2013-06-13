using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Unity;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.Ribbon.Schedule;
using ClinSchd.Modules.Ribbon.Controllers;
using ClinSchd.Modules.Ribbon.Services;

namespace ClinSchd.Modules.Ribbon
{
    public class RibbonModule : IModule
    {
        private readonly IUnityContainer container;

        public RibbonModule(IUnityContainer container)
        {
            this.container = container;
        }

        public void Initialize()
        {
            this.RegisterViewsAndServices();

            IRibbonController controller = this.container.Resolve<IRibbonController>();
            controller.Run();
        }

        protected void RegisterViewsAndServices()
        {
            this.container.RegisterType<IRibbonController, RibbonController>(new ContainerControlledLifetimeManager());
			this.container.RegisterType<IScheduleView, ScheduleView> ();
			this.container.RegisterType<ISchedulePresentationModel, SchedulePresentationModel> ();
            this.container.RegisterType<IRibbonService, RibbonService>(new ContainerControlledLifetimeManager());
        }
    }
}
