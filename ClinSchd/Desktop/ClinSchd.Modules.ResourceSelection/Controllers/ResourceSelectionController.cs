using System;

using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Regions;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.ResourceSelection.Services;
using ClinSchd.Modules.ResourceSelection.ResourceSelection;

namespace ClinSchd.Modules.ResourceSelection.Controllers
{
    public class ResourceSelectionController : IResourceSelectionController
    {
        private readonly IRegionManager regionManager;
		private readonly IResourceSelectionPresentationModel patientSelectionPresentationModel;
		private readonly IResourceSelectionService patientSelectionService;
		private readonly IEventAggregator eventAggregator;

		public ResourceSelectionController(IRegionManager regionManager,
			IResourceSelectionPresentationModel patientSelectionPresentationModel,
			IResourceSelectionService patientSelectionService,
			IEventAggregator eventAggregator)
        {
            this.regionManager = regionManager;
			this.patientSelectionPresentationModel = patientSelectionPresentationModel;
			this.patientSelectionService = patientSelectionService;
            this.eventAggregator = eventAggregator;
		}

		public IResourceSelectionPresentationModel Model { get { return this.patientSelectionPresentationModel; } }

        public void Run(string title)
        {
			this.patientSelectionPresentationModel.PaneTitle = title;
			this.patientSelectionService.ShowDialog(
				this.patientSelectionPresentationModel.View,
				this.patientSelectionPresentationModel, () => patientSelectionPresentationModel.OnClose() );
		}
    }
}
