using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Regions;
using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Management.Group;
using ClinSchd.Modules.Management.Services;

namespace ClinSchd.Modules.Management.Controllers
{
    public class ManagementController : IManagementController
    {
        private readonly IRegionManager regionManager;
		private readonly IGroupPresentationModel groupPresentationModel;
        private readonly IEventAggregator eventAggregator;
		private readonly IManagementService managementService;

		public ManagementController (IRegionManager regionManager, 
								IGroupPresentationModel groupPresentationModel, 
								IEventAggregator eventAggregator,
								IManagementService managementService)
        {
            this.regionManager = regionManager;
			this.groupPresentationModel = groupPresentationModel;
            this.eventAggregator = eventAggregator;
			this.groupPresentationModel.Controller = this;
			this.managementService = managementService;
		}

		public IGroupPresentationModel Model { get { return this.groupPresentationModel; } }

        public void Run()
        {
			this.managementService.ShowDialog (this.groupPresentationModel.View, this.groupPresentationModel, () => groupPresentationModel.OnClose ());
        }
    }
}
