using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Regions;
using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Management.ResourceGroups;

namespace ClinSchd.Modules.Management.ResourceGroups.Controllers
{
    public class ManagementResourceGroupsController : IManagementResourceGroupsController
    {
		private readonly IUnityContainer container;
		private readonly IRegionManager regionManager;
		private IResourceGroupsPresentationModel ResourceGroupsPresentationModel;
        private readonly IEventAggregator eventAggregator;

		public ManagementResourceGroupsController(
			IUnityContainer container,
			IRegionManager regionManager,
			IEventAggregator eventAggregator)
        {
			this.container = container;
			this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;

			this.regionManager.RegisterViewWithRegion(RegionNames.ManagementGroupRegion, () =>
			{
				this.ResourceGroupsPresentationModel =
					this.container.Resolve<IResourceGroupsPresentationModel>();
				return this.ResourceGroupsPresentationModel.View;
			});
		}

        public void Run()
        {
        }
    }
}
