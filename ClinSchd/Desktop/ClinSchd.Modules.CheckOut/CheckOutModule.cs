using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.CheckOut.CheckOut;
using ClinSchd.Modules.CheckOut.Controllers;
using ClinSchd.Modules.CheckOut.Services;

namespace ClinSchd.Modules.CheckOut
{
	public class CheckOutModule : IModule
    {
        private readonly IUnityContainer container;
		private readonly IEventAggregator eventAggregator;
		private ICheckOutController controller;

		public CheckOutModule (IUnityContainer container, IEventAggregator eventAggregator)
        {
            this.container = container;
			this.eventAggregator = eventAggregator;
		}

		public void Initialize ()
		{
			this.RegisterViewsAndServices ();
			controller = this.container.Resolve<ICheckOutController> ();
			controller.Run ();
		}

        protected void RegisterViewsAndServices()
        {
			this.container.RegisterType<ICheckOutController, CheckOutController> ();
			this.container.RegisterType<ICheckOutView, CheckOutView> ();
			this.container.RegisterType<ICheckOutPresentationModel, CheckOutPresentationModel> ();
			this.container.RegisterType<ICheckOutService, CheckOutService> ();
        }
    }
}
