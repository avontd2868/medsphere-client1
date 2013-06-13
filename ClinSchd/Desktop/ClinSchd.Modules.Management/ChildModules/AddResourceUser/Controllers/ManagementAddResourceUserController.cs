using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Regions;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Management.AddResourceUser;
using ClinSchd.Modules.Management.AddResourceUser.Services;

namespace ClinSchd.Modules.Management.AddResourceUser.Controllers
{
    public class ManagementAddResourceUserController : IManagementAddResourceUserController
    {
		private readonly IUnityContainer container;
		private readonly IRegionManager regionManager;
		private IAddResourceUserPresentationModel AddResourceUserPresentationModel;
        private readonly IEventAggregator eventAggregator;
		private IManagementAddResourceUserService managementAddResourceUserService;

		public ManagementAddResourceUserController(
			IUnityContainer container,
			IRegionManager regionManager,
			IEventAggregator eventAggregator,
			IManagementAddResourceUserService managementAddResourceUserService,
			IAddResourceUserPresentationModel AddResourceUserPresentationModel)
        {
			this.container = container;
			this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;
			this.managementAddResourceUserService = managementAddResourceUserService;
			this.AddResourceUserPresentationModel = AddResourceUserPresentationModel;
		}

		public IAddResourceUserPresentationModel Model { get { return this.AddResourceUserPresentationModel; } }

        public void Run()
        {
			this.managementAddResourceUserService.ShowDialog (this.AddResourceUserPresentationModel.View,
														this.AddResourceUserPresentationModel, () => AddResourceUserPresentationModel.OnClose ());
		}
    }

}
