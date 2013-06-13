using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.PatientSelection.PatientSelection;
using ClinSchd.Modules.PatientSelection.Controllers;
using ClinSchd.Modules.PatientSelection.Services;

namespace ClinSchd.Modules.PatientSelection
{
    public class PatientSelectionModule : IModule
    {
        private readonly IUnityContainer container;
		private readonly IEventAggregator eventAggregator;
		private IPatientSelectionController controller;

		public PatientSelectionModule(IUnityContainer container, IEventAggregator eventAggregator)
        {
            this.container = container;
			this.eventAggregator = eventAggregator;
		}

        public void Initialize()
        {
            this.RegisterViewsAndServices();
			this.eventAggregator.GetEvent<LaunchPatientSelectionDialogEvent> ()
				.Subscribe(LaunchPatientSelectionDialog, ThreadOption.UIThread, true);
			this.eventAggregator.GetEvent<PassAppointmentsEvent> ().Subscribe (PassAppointments, ThreadOption.UIThread, true);
        }

		public void PassAppointments (SchdAppointment newAppointment)
		{
			controller = this.container.Resolve<IPatientSelectionController> ();
			controller.Model.SelectedAppointment = newAppointment;
		}

		public void LaunchPatientSelectionDialog(string title)
		{
			if (controller != null) {
				controller.Model.Command = title;
			} else {
				controller = this.container.Resolve<IPatientSelectionController> ();
			}
			controller.Run ();
		}

        protected void RegisterViewsAndServices()
        {
            this.container.RegisterType<IPatientSelectionController, PatientSelectionController>();
			this.container.RegisterType<IPatientSelectionView, PatientSelectionView>();
			this.container.RegisterType<IPatientSelectionPresentationModel, PatientSelectionPresentationModel>();
            this.container.RegisterType<IPatientSelectionService, PatientSelectionService>();
        }
    }
}
