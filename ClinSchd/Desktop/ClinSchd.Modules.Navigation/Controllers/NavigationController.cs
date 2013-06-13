using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Regions;
using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Navigation.Group;

namespace ClinSchd.Modules.Navigation.Controllers
{
    public class NavigationController : INavigationController
    {
        private readonly IRegionManager regionManager;
		private readonly IGroupPresentationModel groupPresentationModel;
        private readonly IEventAggregator eventAggregator;

		public NavigationController(IRegionManager regionManager, IGroupPresentationModel groupPresentationModel, IEventAggregator eventAggregator)
        {
            this.regionManager = regionManager;
			this.groupPresentationModel = groupPresentationModel;
            this.eventAggregator = eventAggregator;
			this.groupPresentationModel.Controller = this;
        }

        public void Run()
        {
			this.regionManager.Regions[RegionNames.NavigatorRegion].Add(groupPresentationModel.View);
        }
    }
}
