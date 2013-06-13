using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Unity;
using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.Management.Group;
using ClinSchd.Modules.Management.Controllers;
using ClinSchd.Modules.Management.Services;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;

namespace ClinSchd.Modules.Management
{
    public class ManagementModule : IModule
    {
        private readonly IUnityContainer container;
		private readonly IEventAggregator eventAggregator;

		public ManagementModule (IUnityContainer container, IEventAggregator eventAggregator)
        {
            this.container = container;
			this.eventAggregator = eventAggregator;

        }

        public void Initialize()
        {
            this.RegisterViewsAndServices();

			this.eventAggregator.GetEvent<LaunchSchedulingManagementDialogEvent> ().Subscribe (LaunchSchedulingManagementDialog, ThreadOption.UIThread, true);

        }

		public void LaunchSchedulingManagementDialog (string title)
		{
			IManagementController controller = this.container.Resolve<IManagementController> ();
			controller.Run ();
		}

        protected void RegisterViewsAndServices()
        {
			this.container.RegisterType<IManagementController, ManagementController> (new ContainerControlledLifetimeManager ());
			this.container.RegisterType<IGroupView, GroupView>();
			this.container.RegisterType<IGroupPresentationModel, GroupPresentationModel>();
			this.container.RegisterType<IManagementService, ManagementService> (new ContainerControlledLifetimeManager ());
        }
    }
}
