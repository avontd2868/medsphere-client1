using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.CancelAppt.CancelAppt;
using ClinSchd.Modules.CancelAppt.Controllers;
using ClinSchd.Modules.CancelAppt.Services;

namespace ClinSchd.Modules.CancelAppt
{
	public class CancelApptModule : IModule
    {
        private readonly IUnityContainer container;
		private readonly IEventAggregator eventAggregator;
		private ICancelApptController controller;

		public CancelApptModule (IUnityContainer container, IEventAggregator eventAggregator)
        {
            this.container = container;
			this.eventAggregator = eventAggregator;
		}

        public void Initialize()
        {
            this.RegisterViewsAndServices();
			controller = this.container.Resolve<ICancelApptController> ();
			controller.Run ();
        }		

        protected void RegisterViewsAndServices()
        {
			this.container.RegisterType<ICancelApptController, CancelApptController> ();
			this.container.RegisterType<ICancelApptView, CancelApptView> ();
			this.container.RegisterType<ICancelApptPresentationModel, CancelApptPresentationModel> ();
			this.container.RegisterType<ICancelApptService, CancelApptService> ();
        }
    }
}
