using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.ChangeUser.ChangeUser;
using ClinSchd.Modules.ChangeUser.Controllers;
using ClinSchd.Modules.ChangeUser.Services;

namespace ClinSchd.Modules.ChangeUser
{
    public class ChangeUserModule : IModule
    {
        private readonly IUnityContainer container;
		private readonly IEventAggregator eventAggregator;

		public ChangeUserModule (IUnityContainer container, IEventAggregator eventAggregator)
        {
            this.container = container;
			this.eventAggregator = eventAggregator;
		}

        public void Initialize()
        {
            this.RegisterViewsAndServices();
			this.eventAggregator.GetEvent<LaunchChangeUserDialogEvent> ()
				.Subscribe (LaunchChangeUserDialog, ThreadOption.UIThread, true);
        }

		public void LaunchChangeUserDialog (string Title)
		{
			IChangeUserController controller = this.container.Resolve<IChangeUserController> ();
//			controller.Model.ForwardingEvent = forwardingEvent;
			controller.Run();
		}

        protected void RegisterViewsAndServices()
        {
			this.container.RegisterType<IChangeUserController, ChangeUserController> ();
			this.container.RegisterType<IChangeUserView, ChangeUserView> ();
			this.container.RegisterType<IChangeUserPresentationModel, ChangeUserPresentationModel> ();
			this.container.RegisterType<IChangeUserService, ChangeUserService> ();
        }
    }
}
