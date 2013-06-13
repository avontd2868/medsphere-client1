using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.MarkAsNoShow.MarkAsNoShow;
using ClinSchd.Modules.MarkAsNoShow.Controllers;
using ClinSchd.Modules.MarkAsNoShow.Services;

namespace ClinSchd.Modules.MarkAsNoShow
{
	public class MarkAsNoShowModule : IModule
	{
		private readonly IUnityContainer container;
		private readonly IEventAggregator eventAggregator;
		private IMarkAsNoShowController controller;

		public MarkAsNoShowModule (IUnityContainer container, IEventAggregator eventAggregator)
		{
			this.container = container;
			this.eventAggregator = eventAggregator;
		}

		public void Initialize()
		{
			this.RegisterViewsAndServices ();
			controller = this.container.Resolve<IMarkAsNoShowController> ();
			controller.Run ();
		}

		protected void RegisterViewsAndServices()
		{
			this.container.RegisterType<IMarkAsNoShowController, MarkAsNoShowController> ();
			this.container.RegisterType<IMarkAsNoShowView, MarkAsNoShowView> ();
			this.container.RegisterType<IMarkAsNoShowPresentationModel, MarkAsNoShowPresentationModel> ();
			this.container.RegisterType<IMarkAsNoShowService, MarkAsNoShowService> ();
		}
	}
}
