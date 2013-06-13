using System;

using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Regions;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.ChangeUser.Services;
using ClinSchd.Modules.ChangeUser.ChangeUser;

namespace ClinSchd.Modules.ChangeUser.Controllers
{
    public class ChangeUserController : IChangeUserController
    {
        private readonly IRegionManager regionManager;
		private readonly IChangeUserPresentationModel changeUserPresentationModel;
		private readonly IChangeUserService changeUserService;
		private readonly IEventAggregator eventAggregator;

		public ChangeUserController (IRegionManager regionManager,
			IChangeUserPresentationModel changeUserPresentationModel,
			IChangeUserService changeUserService,
			IEventAggregator eventAggregator)
        {
            this.regionManager = regionManager;
			this.changeUserPresentationModel = changeUserPresentationModel;
			this.changeUserService = changeUserService;
            this.eventAggregator = eventAggregator;
		}

		public IChangeUserPresentationModel Model { get { return this.changeUserPresentationModel; } }

        public void Run()
        {
			this.changeUserService.ShowDialog (
				this.changeUserPresentationModel.View,
				this.changeUserPresentationModel, () => changeUserPresentationModel.OnClose ());
		}
    }
}
