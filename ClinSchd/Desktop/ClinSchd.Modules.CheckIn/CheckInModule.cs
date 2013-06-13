using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.CheckIn.CheckIn;
using ClinSchd.Modules.CheckIn.Controllers;
using ClinSchd.Modules.CheckIn.Services;

namespace ClinSchd.Modules.CheckIn
{
	public class CheckInModule : IModule
    {
        private readonly IUnityContainer container;
		private readonly IEventAggregator eventAggregator;
		private ICheckInController controller;

		public CheckInModule (IUnityContainer container, IEventAggregator eventAggregator)
        {
            this.container = container;
			this.eventAggregator = eventAggregator;
		}

        public void Initialize ()
        {
            this.RegisterViewsAndServices();
			controller = this.container.Resolve<ICheckInController> ();
			controller.Run ();
        }

        protected void RegisterViewsAndServices()
        {
			this.container.RegisterType<ICheckInController, CheckInController> ();
			this.container.RegisterType<ICheckInView, CheckInView> ();
			this.container.RegisterType<ICheckInPresentationModel, CheckInPresentationModel> ();
			this.container.RegisterType<ICheckInService, CheckInService> ();
        }
    }
}
