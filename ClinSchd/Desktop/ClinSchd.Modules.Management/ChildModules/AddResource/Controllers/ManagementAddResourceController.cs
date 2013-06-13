using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Regions;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Management.AddResource;
using ClinSchd.Modules.Management.AddResource.Services;

namespace ClinSchd.Modules.Management.AddResource.Controllers
{
    public class ManagementAddResourceController : IManagementAddResourceController
    {
		private readonly IUnityContainer container;
		private readonly IRegionManager regionManager;
		private IAddResourcePresentationModel addResourcePresentationModel;
        private readonly IEventAggregator eventAggregator;
		private IManagementAddResourceService managementAddResourceService;

		public ManagementAddResourceController(
			IUnityContainer container,
			IRegionManager regionManager,
			IEventAggregator eventAggregator,
			IManagementAddResourceService managementAddResourceService,
			IAddResourcePresentationModel addResourcePresentationModel)
        {
			this.container = container;
			this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;
			this.managementAddResourceService = managementAddResourceService;
			this.addResourcePresentationModel = addResourcePresentationModel;
		}

		public IAddResourcePresentationModel Model { get { return this.addResourcePresentationModel; } }

        public void Run()
        {
			this.managementAddResourceService.ShowDialog (this.addResourcePresentationModel.View,
														this.addResourcePresentationModel, () => addResourcePresentationModel.OnClose ());
		}
    }

}
