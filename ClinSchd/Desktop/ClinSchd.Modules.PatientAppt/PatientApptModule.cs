using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Unity;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.PatientAppt.Group;
using ClinSchd.Modules.PatientAppt.Controllers;
using ClinSchd.Modules.PatientAppt.Services;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;

namespace ClinSchd.Modules.PatientAppt
{
    public class PatientApptModule : IModule
    {
        private readonly IUnityContainer container;
		private readonly IEventAggregator eventAggregator;
		private IPatientApptController controller;

		public PatientApptModule (IUnityContainer container, IEventAggregator eventAggregator)
        {
            this.container = container;
			this.eventAggregator = eventAggregator;
        }

		public void Initialize ()
		{
			this.RegisterViewsAndServices ();
			controller = this.container.Resolve<IPatientApptController> ();
			controller.Run ();
		}

        protected void RegisterViewsAndServices()
        {
			this.container.RegisterType<IPatientApptController, PatientApptController> (new ContainerControlledLifetimeManager ());
			this.container.RegisterType<IGroupView, GroupView>();
			this.container.RegisterType<IGroupPresentationModel, GroupPresentationModel>();
			this.container.RegisterType<IPatientApptService, PatientApptService> (new ContainerControlledLifetimeManager ());
        }
    }
}
