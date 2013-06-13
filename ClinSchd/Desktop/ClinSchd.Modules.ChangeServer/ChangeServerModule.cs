using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.ChangeServer.ChangeServer;
using ClinSchd.Modules.ChangeServer.Controllers;
using ClinSchd.Modules.ChangeServer.Services;

namespace ClinSchd.Modules.ChangeServer
{
    public class ChangeServerModule : IModule
    {
        private readonly IUnityContainer container;
		private readonly IEventAggregator eventAggregator;

		public ChangeServerModule (IUnityContainer container, IEventAggregator eventAggregator)
        {
            this.container = container;
			this.eventAggregator = eventAggregator;
		}

        public void Initialize()
        {
            this.RegisterViewsAndServices();
			this.eventAggregator.GetEvent<LaunchChangeServerDialogEvent> ()
				.Subscribe (LaunchChangeServerDialog, ThreadOption.UIThread, true);
        }

		public void LaunchChangeServerDialog (string Title)
		{
			IChangeServerController controller = this.container.Resolve<IChangeServerController> ();
//			controller.Model.ForwardingEvent = forwardingEvent;
			controller.Run();
		}

        protected void RegisterViewsAndServices()
        {
			this.container.RegisterType<IChangeServerController, ChangeServerController> ();
			this.container.RegisterType<IChangeServerView, ChangeServerView> ();
			this.container.RegisterType<IChangeServerPresentationModel, ChangeServerPresentationModel> ();
			this.container.RegisterType<IChangeServerService, ChangeServerService> ();
        }
    }
}
