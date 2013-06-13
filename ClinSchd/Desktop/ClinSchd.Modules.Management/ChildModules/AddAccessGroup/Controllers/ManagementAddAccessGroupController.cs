using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Regions;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Management.AddAccessGroup;
using ClinSchd.Modules.Management.AddAccessGroup.Services;

namespace ClinSchd.Modules.Management.AddAccessGroup.Controllers
{
    public class ManagementAddAccessGroupController : IManagementAddAccessGroupController
    {
		private readonly IUnityContainer container;
		private readonly IRegionManager regionManager;
		private IAddAccessGroupPresentationModel AddAccessGroupPresentationModel;
        private readonly IEventAggregator eventAggregator;
		private IManagementAddAccessGroupService managementAddAccessGroupService;

		public ManagementAddAccessGroupController(
			IUnityContainer container,
			IRegionManager regionManager,
			IEventAggregator eventAggregator,
			IManagementAddAccessGroupService managementAddAccessGroupService,
			IAddAccessGroupPresentationModel AddAccessGroupPresentationModel)
        {
			this.container = container;
			this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;
			this.managementAddAccessGroupService = managementAddAccessGroupService;
			this.AddAccessGroupPresentationModel = AddAccessGroupPresentationModel;
		}

		public IAddAccessGroupPresentationModel Model { get { return this.AddAccessGroupPresentationModel; } }

        public void Run()
        {
			this.managementAddAccessGroupService.ShowDialog (this.AddAccessGroupPresentationModel.View,
														this.AddAccessGroupPresentationModel, () => AddAccessGroupPresentationModel.OnClose ());
		}
    }

}
