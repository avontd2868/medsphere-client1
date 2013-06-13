using System;

using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Regions;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.ChangeServer.Services;
using ClinSchd.Modules.ChangeServer.ChangeServer;

namespace ClinSchd.Modules.ChangeServer.Controllers
{
    public class ChangeServerController : IChangeServerController
    {
        private readonly IRegionManager regionManager;
		private readonly IChangeServerPresentationModel changeServerPresentationModel;
		private readonly IChangeServerService changeServerService;
		private readonly IEventAggregator eventAggregator;

		public ChangeServerController (IRegionManager regionManager,
			IChangeServerPresentationModel changeServerPresentationModel,
			IChangeServerService changeServerService,
			IEventAggregator eventAggregator)
        {
            this.regionManager = regionManager;
			this.changeServerPresentationModel = changeServerPresentationModel;
			this.changeServerService = changeServerService;
            this.eventAggregator = eventAggregator;
		}

		public IChangeServerPresentationModel Model { get { return this.changeServerPresentationModel; } }

        public void Run()
        {
			this.changeServerService.ShowDialog (
				this.changeServerPresentationModel.View,
				this.changeServerPresentationModel, () => changeServerPresentationModel.OnClose ());
		}
    }
}
