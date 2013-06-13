using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Regions;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Management.Resources;

namespace ClinSchd.Modules.Management.Resources.Controllers
{
    public class ManagementResourcesController : IManagementResourcesController
    {
		private readonly IUnityContainer container;
		private readonly IRegionManager regionManager;
		private IResourcesPresentationModel ResourcesPresentationModel;
        private readonly IEventAggregator eventAggregator;

		public ManagementResourcesController(
			IUnityContainer container,
			IRegionManager regionManager,
			IEventAggregator eventAggregator)
        {
			this.container = container;
			this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;

			this.regionManager.RegisterViewWithRegion(RegionNames.ManagementGroupRegion, () =>
			{
				this.ResourcesPresentationModel =
					this.container.Resolve<IResourcesPresentationModel>();
				return this.ResourcesPresentationModel.View;
			});
		}
		
        public void Run()
        {
		}
    }
}
