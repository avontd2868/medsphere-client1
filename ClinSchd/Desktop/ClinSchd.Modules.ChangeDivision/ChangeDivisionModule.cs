using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.ChangeDivision.ChangeDivision;
using ClinSchd.Modules.ChangeDivision.Controllers;
using ClinSchd.Modules.ChangeDivision.Services;

namespace ClinSchd.Modules.ChangeDivision
{
    public class ChangeDivisionModule : IModule
    {
        private readonly IUnityContainer container;
		private readonly IEventAggregator eventAggregator;
		private IChangeDivisionController controller;

		public ChangeDivisionModule (IUnityContainer container, IEventAggregator eventAggregator)
        {
            this.container = container;
			this.eventAggregator = eventAggregator;
		}

        public void Initialize()
        {
			this.RegisterViewsAndServices ();
			controller = this.container.Resolve<IChangeDivisionController> ();
			controller.Run ();
        }

        protected void RegisterViewsAndServices()
        {
			this.container.RegisterType<IChangeDivisionController, ChangeDivisionController> ();
			this.container.RegisterType<IChangeDivisionView, ChangeDivisionView> ();
			this.container.RegisterType<IChangeDivisionPresentationModel, ChangeDivisionPresentationModel> ();
			this.container.RegisterType<IChangeDivisionService, ChangeDivisionService> ();
        }
    }
}
