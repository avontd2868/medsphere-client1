using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Regions;
using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Ribbon.Schedule;

namespace ClinSchd.Modules.Ribbon.Controllers
{
    public class RibbonController : IRibbonController
    {
        private readonly IRegionManager regionManager;
		private readonly ISchedulePresentationModel schedulePresentationModel;
        private readonly IEventAggregator eventAggregator;

		public RibbonController (IRegionManager regionManager, ISchedulePresentationModel schedulePresentationModel, IEventAggregator eventAggregator)
        {
            this.regionManager = regionManager;
			this.schedulePresentationModel = schedulePresentationModel;
            this.eventAggregator = eventAggregator;
        }

        public void Run()
        {
			this.regionManager.Regions[RegionNames.MainToolBarRegion].Add (schedulePresentationModel.View);
        }
    }
}
