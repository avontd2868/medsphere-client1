using System;

using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Regions;
using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Presentation.Events;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.PatientSelection.Services;
using ClinSchd.Modules.PatientSelection.PatientSelection;

namespace ClinSchd.Modules.PatientSelection.Controllers
{
    public class PatientSelectionController : IPatientSelectionController
    {
        private readonly IRegionManager regionManager;
		private readonly IPatientSelectionPresentationModel patientSelectionPresentationModel;
		private readonly IPatientSelectionService patientSelectionService;
		private readonly IEventAggregator eventAggregator;

		public PatientSelectionController(IRegionManager regionManager,
			IPatientSelectionPresentationModel patientSelectionPresentationModel,
			IPatientSelectionService patientSelectionService,
			IEventAggregator eventAggregator)
        {
            this.regionManager = regionManager;
			this.patientSelectionPresentationModel = patientSelectionPresentationModel;
			this.patientSelectionService = patientSelectionService;
            this.eventAggregator = eventAggregator;
		}
		
		public IPatientSelectionPresentationModel Model { get { return this.patientSelectionPresentationModel; } }

        public void Run()
        {
			this.patientSelectionService.ShowDialog(
				this.patientSelectionPresentationModel.View,
				this.patientSelectionPresentationModel, () => patientSelectionPresentationModel.OnClose() );
		}
    }
}
