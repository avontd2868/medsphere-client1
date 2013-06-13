using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Regions;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Management.AddResourceGroup;
using ClinSchd.Modules.Management.AddResourceGroup.Services;

namespace ClinSchd.Modules.Management.AddResourceGroup.Controllers
{
    public class ManagementAddResourceGroupController : IManagementAddResourceGroupController
    {
		private readonly IUnityContainer container;
		private readonly IRegionManager regionManager;
		private IAddResourceGroupPresentationModel AddResourceGroupPresentationModel;
        private readonly IEventAggregator eventAggregator;
		private IManagementAddResourceGroupService managementAddResourceGroupService;

		public ManagementAddResourceGroupController(
			IUnityContainer container,
			IRegionManager regionManager,
			IEventAggregator eventAggregator,
			IManagementAddResourceGroupService managementAddResourceGroupService,
			IAddResourceGroupPresentationModel AddResourceGroupPresentationModel)
        {
			this.container = container;
			this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;
			this.managementAddResourceGroupService = managementAddResourceGroupService;
			this.AddResourceGroupPresentationModel = AddResourceGroupPresentationModel;
		}

		public IAddResourceGroupPresentationModel Model { get { return this.AddResourceGroupPresentationModel; } }

        public void Run()
        {
			this.managementAddResourceGroupService.ShowDialog (this.AddResourceGroupPresentationModel.View,
														this.AddResourceGroupPresentationModel, () => AddResourceGroupPresentationModel.OnClose ());
		}
    }

}
