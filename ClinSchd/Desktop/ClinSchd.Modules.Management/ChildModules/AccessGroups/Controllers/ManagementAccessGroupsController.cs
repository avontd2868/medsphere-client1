using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Regions;
using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Management.AccessGroups;

namespace ClinSchd.Modules.Management.AccessGroups.Controllers
{
    public class ManagementAccessGroupsController : IManagementAccessGroupsController
    {
		private readonly IUnityContainer container;
		private readonly IRegionManager regionManager;
		private IAccessGroupsPresentationModel AccessGroupsPresentationModel;
        private readonly IEventAggregator eventAggregator;

		public ManagementAccessGroupsController(
			IUnityContainer container,
			IRegionManager regionManager,
			IEventAggregator eventAggregator)
        {
			this.container = container;
			this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;

			this.regionManager.RegisterViewWithRegion(RegionNames.ManagementGroupRegion, () =>
			{
				this.AccessGroupsPresentationModel =
					this.container.Resolve<IAccessGroupsPresentationModel>();
				return this.AccessGroupsPresentationModel.View;
			});
		}

        public void Run()
        {
        }
    }
}
