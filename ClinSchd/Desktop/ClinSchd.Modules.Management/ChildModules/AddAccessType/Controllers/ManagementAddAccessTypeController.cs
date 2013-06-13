using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Regions;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Management.AddAccessType;
using ClinSchd.Modules.Management.AddAccessType.Services;

namespace ClinSchd.Modules.Management.AddAccessType.Controllers
{
    public class ManagementAddAccessTypeController : IManagementAddAccessTypeController
    {
		private readonly IUnityContainer container;
		private readonly IRegionManager regionManager;
		private IAddAccessTypePresentationModel AddAccessTypePresentationModel;
        private readonly IEventAggregator eventAggregator;
		private IManagementAddAccessTypeService managementAddAccessTypeService;

		public ManagementAddAccessTypeController(
			IUnityContainer container,
			IRegionManager regionManager,
			IEventAggregator eventAggregator,
			IManagementAddAccessTypeService managementAddAccessTypeService,
			IAddAccessTypePresentationModel AddAccessTypePresentationModel)
        {
			this.container = container;
			this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;
			this.managementAddAccessTypeService = managementAddAccessTypeService;
			this.AddAccessTypePresentationModel = AddAccessTypePresentationModel;
		}

		public IAddAccessTypePresentationModel Model { get { return this.AddAccessTypePresentationModel; } }

        public void Run()
        {
			this.managementAddAccessTypeService.ShowDialog (this.AddAccessTypePresentationModel.View,
														this.AddAccessTypePresentationModel, () => AddAccessTypePresentationModel.OnClose ());
		}
    }

}
