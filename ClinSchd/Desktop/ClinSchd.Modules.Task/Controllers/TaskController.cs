using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Regions;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Task.Group;

namespace ClinSchd.Modules.Task.Controllers
{
    public class TaskController : ITaskController
    {
        private readonly IRegionManager regionManager;
		private readonly IGroupPresentationModel groupPresentationModel;
        private readonly IEventAggregator eventAggregator;

		public TaskController(IRegionManager regionManager, IGroupPresentationModel groupPresentationModel, IEventAggregator eventAggregator)
        {
            this.regionManager = regionManager;
			this.groupPresentationModel = groupPresentationModel;
            this.eventAggregator = eventAggregator;
			this.groupPresentationModel.Controller = this;
		}

        public void Run()
        {
			this.regionManager.Regions[RegionNames.TaskRegion].Add(groupPresentationModel.View);
		}
    }
}
