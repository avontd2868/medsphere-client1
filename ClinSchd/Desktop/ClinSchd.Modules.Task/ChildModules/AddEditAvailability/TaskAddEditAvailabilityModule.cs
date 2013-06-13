using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Events;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.Task.AddEditAvailability;
using ClinSchd.Modules.Task.AddEditAvailability.Controllers;
using ClinSchd.Modules.Task.AddEditAvailability.Services;

namespace ClinSchd.Modules.Task.AddEditAvailability
{
    public class TaskAddEditAvailabilityModule : IModule
    {
        private readonly IUnityContainer container;
		private readonly IEventAggregator eventAggregator;
		private ITaskAddEditAvailabilityController controller;

		public TaskAddEditAvailabilityModule (IUnityContainer container, IEventAggregator eventAggregator)
        {
            this.container = container;
			this.eventAggregator = eventAggregator;
		}

		public void Initialize()
		{
			this.RegisterViewsAndServices();

			this.eventAggregator.GetEvent<AddEditAvailabilityEvent> ().Subscribe (LaunchAddEditAvailabilityDialog, ThreadOption.UIThread, true);
		}

		public void LaunchAddEditAvailabilityDialog (SchdAvailability schdAvailability)
		{
			controller = this.container.Resolve<ITaskAddEditAvailabilityController> ();
			controller.Model.SchdAvailability = schdAvailability;
			controller.Model.OnPropertyChanged ("SchdAvailability");
			if (controller.Model.ValidationMessage.IsValid) {
				controller.Run ();
				controller.Model.ValidationMessage.IsValid = true;
				controller.Model.ValidationMessage.Title = string.Empty;
				controller.Model.ValidationMessage.Message = string.Empty;

			} else {
				controller.Model.View.AlertUser (controller.Model.ValidationMessage.Message, controller.Model.ValidationMessage.Title);
			}
		}

        protected void RegisterViewsAndServices()
        {
			this.container.RegisterType<ITaskAddEditAvailabilityController, TaskAddEditAvailabilityController> ();
			this.container.RegisterType<IAddEditAvailabilityView, AddEditAvailabilityView> ();
			this.container.RegisterType<IAddEditAvailabilityPresentationModel, AddEditAvailabilityPresentationModel> ();
			this.container.RegisterType<ITaskAddEditAvailabilityService, TaskAddEditAvailabilityService> ();
        }
    }
}
