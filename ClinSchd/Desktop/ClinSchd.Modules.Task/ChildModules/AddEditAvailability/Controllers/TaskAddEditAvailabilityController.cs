using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Regions;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Task.AddEditAvailability;
using ClinSchd.Modules.Task.AddEditAvailability.Services;

namespace ClinSchd.Modules.Task.AddEditAvailability.Controllers
{
    public class TaskAddEditAvailabilityController : ITaskAddEditAvailabilityController
    {
		private readonly IUnityContainer container;
		private readonly IRegionManager regionManager;
		private IAddEditAvailabilityPresentationModel addEditAvailabilityPresentationModel;
        private readonly IEventAggregator eventAggregator;
		private ITaskAddEditAvailabilityService taskAddEditAvailabilityService;

		public TaskAddEditAvailabilityController (
			IUnityContainer container,
			IRegionManager regionManager,
			IEventAggregator eventAggregator,
			ITaskAddEditAvailabilityService taskAddEditAvailabilityService,
			IAddEditAvailabilityPresentationModel addEditAvailabilityPresentationModel)
        {
			this.container = container;
			this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;
			this.taskAddEditAvailabilityService = taskAddEditAvailabilityService;
			this.addEditAvailabilityPresentationModel = addEditAvailabilityPresentationModel;
		}

		public IAddEditAvailabilityPresentationModel Model { get { return this.addEditAvailabilityPresentationModel; } }

        public void Run()
        {
			this.taskAddEditAvailabilityService.ShowDialog (this.addEditAvailabilityPresentationModel.View,
														this.addEditAvailabilityPresentationModel, () => addEditAvailabilityPresentationModel.OnClose ());
		}
    }

}
