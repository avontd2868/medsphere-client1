using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Regions;
using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.PatientAppt.Group;
using ClinSchd.Modules.PatientAppt.Services;
using Microsoft.Practices.Unity;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.PatientAppt.Controllers;
using Microsoft.Practices.Composite.Presentation.Commands;
using System.Collections.ObjectModel;


namespace ClinSchd.Modules.PatientAppt.Controllers
{
    public class PatientApptController : IPatientApptController
    {
		private readonly IUnityContainer container;
        private readonly IEventAggregator eventAggregator;
		private readonly IDataAccessService dataAccessService;

		public PatientApptController (IUnityContainer container, IDataAccessService dataAccessService,
								IEventAggregator eventAggregator)
        {
			this.container = container;
            this.eventAggregator = eventAggregator;
			this.dataAccessService = dataAccessService;

		}

		public void Run()
        {
			this.eventAggregator.GetEvent<ViewAppointmentEvent> ().Subscribe (LaunchViewAppointmentDialog, ThreadOption.UIThread, true);
			this.eventAggregator.GetEvent<AddNewAppointmentEvent> ().Subscribe (LaunchNewAppointmentDialog, ThreadOption.UIThread, true);
			this.eventAggregator.GetEvent<AddWalkInAppointmentEvent> ().Subscribe (LaunchWalkinAppointmentDialog, ThreadOption.UIThread, true);
			this.eventAggregator.GetEvent<EditAppointmentEvent> ().Subscribe (LaunchEditAppointmentDialog, ThreadOption.UIThread, true);
        }

		private void LaunchViewAppointmentDialog (SchdAppointment appointment)
		{
			IPatientApptService PatientApptService = container.Resolve<IPatientApptService> ();
			IGroupPresentationModel Model = container.Resolve<IGroupPresentationModel> ();
			Model.GetPatientInformation (appointment.PATIENTID);
			Model.GetViewAppointment (appointment);
			Model.Controller = this;
			Model.Command = "View Appointment";
			Model.PaneTitle = "View Appointment";
			Model.OnPropertyChanged ("PaneTitle");
			Model.IsNoteEnabled = false;
			Model.IsDurationEnabled = false;
			PatientApptService.ShowDialog (Model.View, Model, () => Model.OnClose ());
		}
		private void LaunchNewAppointmentDialog (SchdAppointment appointment)
		{
			IPatientApptService PatientApptService = container.Resolve<IPatientApptService> ();
			IGroupPresentationModel Model = container.Resolve<IGroupPresentationModel> ();
			Model.Controller = this;
			string errorMessage = string.Empty;
			bool bOverbookAppointment = false;
			bool isConflictAppointment = false;

			errorMessage = this.dataAccessService.IsHoliday (Convert.ToDateTime(appointment.START_TIME));

			if (errorMessage == string.Empty || (Model.View.ConfirmUser (Convert.ToDateTime (appointment.START_TIME).ToShortDateString () + " is a holiday (" + errorMessage + ").  Are you sure you want to make this appointment?", "New Appointment")) ) {
				//check conflict appointment
				isConflictAppointment = this.dataAccessService.CheckConflictAppointment (appointment.RESOURCENAME, appointment.PATIENTID, appointment.START_TIME, appointment.END_TIME);

				if (isConflictAppointment) {
					Model.View.AlertUser ("Patient " + appointment.PATIENTNAME + " already had appt at " + appointment.START_TIME + " in clinic " + appointment.RESOURCENAME, "New Appointment");
					return;
				}

				//check overbook authority
				bOverbookAppointment = this.dataAccessService.OverBookAuthority (appointment);

				//check slots available
				errorMessage = this.dataAccessService.CheckSlotsAvailable (appointment.START_TIME, appointment.END_TIME, appointment.RESOURCENAME);

				if (errorMessage != string.Empty) {
					if (bOverbookAppointment) {
						if (Model.View.ConfirmUser (errorMessage, "New Appointment")) {

							errorMessage = string.Empty;
							if (Convert.ToDateTime(appointment.START_TIME).Date > DateTime.Now.Date) {
								errorMessage = this.dataAccessService.NoShow (appointment.PATIENTID, appointment.RESOURCENAME, "AddNew");
							}

							if (errorMessage != string.Empty) {
								Model.View.AlertUser ("Unable to make a new appointment.\nPatient: " +
									appointment.PATIENTNAME + errorMessage, "Patient No-Shows");
								return;
							}
							Model.GetNewAppointment (appointment);
							Model.Command = "New Appointment";
							Model.PaneTitle = "New Appointment";
							Model.OnPropertyChanged ("PaneTitle");
							Model.IsNoteEnabled = true;
							Model.IsDurationEnabled = false;
							PatientApptService.ShowDialog (Model.View, Model, () => Model.OnClose ());
						}
					} else {
						Model.View.AlertUser ("You do not have permission to overbook this appointment.", "New Appointment");
						return;
					}
				} else {
					Model.GetNewAppointment (appointment);
					Model.Command = "New Appointment";
					Model.PaneTitle = "New Appointment";
					Model.OnPropertyChanged ("PaneTitle");
					Model.IsNoteEnabled = true;
					Model.IsDurationEnabled = false;
					PatientApptService.ShowDialog (Model.View, Model, () => Model.OnClose ());
				}
			}
		}

		private void LaunchEditAppointmentDialog (SchdAppointment appointment)
		{
			IPatientApptService PatientApptService = container.Resolve<IPatientApptService> ();
			IGroupPresentationModel Model = container.Resolve<IGroupPresentationModel> ();
			Model.Controller = this;
			Model.GetEditAppointment (appointment);
			Model.Command = "Edit Appointment";
			Model.PaneTitle = "Edit Appointment";
			Model.OnPropertyChanged ("PaneTitle");
			Model.IsNoteEnabled = true;
			Model.IsDurationEnabled = true;
			PatientApptService.ShowDialog (Model.View, Model, () => Model.OnClose ());
		}
		private void LaunchWalkinAppointmentDialog (SchdAppointment appointment)
		{
			string errorMessage = string.Empty;
			IPatientApptService PatientApptService = container.Resolve<IPatientApptService> ();
			IGroupPresentationModel Model = container.Resolve<IGroupPresentationModel> ();
			Model.Controller = this;
			if (Convert.ToDateTime (appointment.START_TIME).Date > DateTime.Today.Date) {
				Model.View.AlertUser ("You cannot create a walk-in appointment for a date in the future.\n Select today's date and try again.", "Walk-In Appointment");
			} else {
				errorMessage = this.dataAccessService.CheckSlotsAvailable (appointment.START_TIME, appointment.END_TIME, appointment.RESOURCENAME);

				if (errorMessage != string.Empty) {
					if (Model.View.ConfirmUser (errorMessage, "Walk In Appointment")) {
						Model.GetWalkInAppointment (appointment);
						Model.Command = "WalkIn Appointment";
						Model.PaneTitle = "Walk In Appointment";
						Model.OnPropertyChanged ("PaneTitle");
						Model.IsNoteEnabled = true;
						Model.IsDurationEnabled = false;
						PatientApptService.ShowDialog (Model.View, Model, () => Model.OnClose ());
					}
				}
			}
		}
    }
}