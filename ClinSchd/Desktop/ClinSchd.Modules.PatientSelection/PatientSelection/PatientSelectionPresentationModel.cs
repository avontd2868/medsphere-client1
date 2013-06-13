using System;
using System.Collections.Generic;
using System.ComponentModel;

using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Presentation.Commands;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.PatientSelection.Controllers;
using ClinSchd.Modules.PatientSelection.Services;
using Microsoft.Practices.Unity;

namespace ClinSchd.Modules.PatientSelection.PatientSelection
{
    public class PatientSelectionPresentationModel : IPatientSelectionPresentationModel, INotifyPropertyChanged
    {
        private readonly IPatientSelectionService patientSelectionService;
		private readonly IDataAccessService dataAccessService;
		private readonly IEventAggregator eventAggregator;
		private string searchString;
		private IList<Patient> patientList;
		private Patient selectedPatient;
		[Dependency]
		public Factory<Patient> PatientFactory { get; set; }

		public PatientSelectionPresentationModel(
			IPatientSelectionView view,
			IPatientSelectionService patientSelectionService,
			IDataAccessService dataAccessService,
			IEventAggregator eventAggregator)
        {
            View = view;
            View.Model = this;
			this.eventAggregator = eventAggregator;
			this.patientSelectionService = patientSelectionService;
			this.dataAccessService = dataAccessService;

			NewAppointmentCommand = new DelegateCommand<string> (ExecuteNewAppointmentCommand, CanExecuteNewAppointmentCommand);

		}

		public void SearchStringChanged(string newSearchString)
		{
			IList<Patient> newPatientList = this.dataAccessService.
				GetPatients(newSearchString == string.Empty ? null : newSearchString, 50);
			this.SearchString = newSearchString;
			this.PatientList = newPatientList;
		}
		
		public void OnClose()
		{
			View.Close();
		}

		#region IPatientSelectionPresentationModel Members

		public IPatientSelectionView View { get; private set; }
		public SchdAppointment SelectedAppointment { get; set; }
		public string Command { get; set; }
		public DelegateCommand<string> NewAppointmentCommand { get; private set; }

		public void ExecuteNewAppointmentCommand (string patientIEN)
		{
			if (SelectedPatient != null) {
				// Forward the sender's event
				if (this.Command == "New Appointment") {
					this.eventAggregator.GetEvent<AddNewAppointmentEvent> ().Publish (this.SelectedAppointment);
				} else if (this.Command == "WalkIn Appointment") {
					this.eventAggregator.GetEvent<AddWalkInAppointmentEvent> ().Publish (this.SelectedAppointment);
				}

				// Notify anyone who cares that a patient has been selected
				this.eventAggregator.GetEvent<PatientSelectedEvent> ().Publish (SelectedPatient);
			}

			if (this.Command == "View Patient Appointments") {
				//pop up report dialog to display all appointments for the selected patient
			} else {
				this.eventAggregator.GetEvent<ViewAppointmentEvent> ().Publish (this.SelectedAppointment);
			}
		}

		public bool CanExecuteNewAppointmentCommand (string patientIEN)
		{
			return true;
		}

		public string SearchString
		{
			get
			{
				return this.searchString;
			}
			set
			{
				if (this.searchString != value)
				{
					this.searchString = value;
					this.OnPropertyChanged("SearchString");
				}
			}
		}

		public IList<Patient> PatientList
		{
			get
			{
				return patientList;
			}
			private set
			{
				if (this.patientList != value)
				{
					this.patientList = value;
					this.OnPropertyChanged("PatientList");
				}
			}
		}

		public Patient SelectedPatient
		{
			get
			{
				return selectedPatient;
			}
			set
			{
				if (this.selectedPatient != value && value != null && value.IEN != null)
				{
					this.selectedPatient = value;
					this.OnPropertyChanged("SelectedPatient");
				}
			}
		}

		#endregion

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler Handler = PropertyChanged;
			if (Handler != null) Handler(this, new PropertyChangedEventArgs(propertyName));
		}
    }
}
