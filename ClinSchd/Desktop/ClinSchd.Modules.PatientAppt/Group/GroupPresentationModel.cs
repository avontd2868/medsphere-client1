using System;
using System.Collections.Generic;
using System.ComponentModel;
using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.PatientAppt.Controllers;
using ClinSchd.Modules.PatientAppt.Services;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Presentation.Commands;
using System.Collections.ObjectModel;


namespace ClinSchd.Modules.PatientAppt.Group
{

	public class GroupPresentationModel : IGroupPresentationModel, INotifyPropertyChanged
    {
		private readonly IPatientApptService PatientApptService;
		private readonly IDataAccessService dataAccessService;
		public event PropertyChangedEventHandler PropertyChanged;
		private readonly IEventAggregator eventAggregator;
		private ValidationMessage validationMessage;
		private AppointmentData _appointmentDate;
		private bool _isDurationEnabled = false;
		private bool _isNoteEnabled = true;
		private string _paneTitle = "";

		public GroupPresentationModel (IGroupView view, IDataAccessService dataAccessService,
		IPatientApptService PatientApptService, IEventAggregator eventAggregator)
        {
            View = view;
            View.Model = this;
			this.PatientApptService = PatientApptService;
			this.eventAggregator = eventAggregator;
			this.dataAccessService = dataAccessService;
			this.ValidationMessage = new ValidationMessage ();
			NewAppointmentCommand = new DelegateCommand<string> (ExecuteNewAppointmentCommand, CanExecuteNewAppointmentCommand);
			PrintLetterCommand = new DelegateCommand<string> (ExecutePrintLetterCommand, CanExecutePrintLetterCommand);
			//LoadConflicts ();
        }

		public void OnClose ()
		{
			View.Close ();
		}

		public IGroupView View { get; private set; }
		public IPatientApptController Controller { get; set; }
		public CompositePresentationEvent<string> NewForwardingEvent { get; set; }
		public PatientInformation PatientInformation { get; set; }
		public DelegateCommand<string> NewAppointmentCommand { get; private set; }
		public DelegateCommand<string> PrintLetterCommand { get; private set; }
		public IList<ConflictAppointments> ConflictAppointments { get; set; }
		public SchdAppointment SchdAppointment { get; set; }
		public string Command { get; set; }
		public bool IsNoteEnabled
		{
			get { return _isNoteEnabled; }
			set 
			{
				if (this._isNoteEnabled != value) {
					this._isNoteEnabled = value;
					this.OnPropertyChanged ("IsNoteEnabled");
				}
			}
		}

		public bool IsDurationEnabled
		{
			get { return _isDurationEnabled; }
			set 
			{
				if (this._isDurationEnabled != value) {
					this._isDurationEnabled = value;
					this.OnPropertyChanged ("IsDurationEnabled");
				}

			}
		}

		public AppointmentData AppointmentData
		{
			get { return _appointmentDate; }
			set { _appointmentDate = value; }
		}

		public ValidationMessage ValidationMessage
		{
			get
			{
				return this.validationMessage;
			}
			set
			{
				this.validationMessage = value;
			}
		}

		public string PaneTitle
		{
			get
			{
				return _paneTitle;
			}
			set
			{
				_paneTitle = value;
			}
		}

		public void GetEditAppointment (SchdAppointment newAppointment)
		{
			if (newAppointment == null || newAppointment.APPOINTMENTID == null) {
				return;
			}
			this.SchdAppointment = newAppointment;
			this.SchdAppointment.PATIENTID = newAppointment.PATIENTID;
			AppointmentData a = new AppointmentData ();
			a.ResourceName = newAppointment.RESOURCENAME;
			a.Notes = newAppointment.NOTE;
			a.StartTime = newAppointment.START_TIME;
			a.EndTime = newAppointment.END_TIME;
			a.Duration = (Convert.ToDateTime (newAppointment.END_TIME) - Convert.ToDateTime (newAppointment.START_TIME)).TotalMinutes.ToString ();
			a.AppointmentID = newAppointment.APPOINTMENTID;
			a.AppointmentCommand = "Edit Appointment";
			this.AppointmentData = a;

			OnPropertyChanged ("AppointmentData");

			if (newAppointment.PATIENTID != null) {
				GetPatientInformation (newAppointment.PATIENTID);
			}
			LoadConflicts (false);
		}

		public void GetViewAppointment (SchdAppointment newAppointment)
		{
			if (newAppointment.APPOINTMENTID == null) {
				return;
			}
			this.SchdAppointment = newAppointment;
			this.SchdAppointment.PATIENTID = newAppointment.PATIENTID;
			AppointmentData a = new AppointmentData ();
			a.ResourceName = newAppointment.RESOURCENAME;
			a.Notes = newAppointment.NOTE;
			a.StartTime = newAppointment.START_TIME;
			a.EndTime = newAppointment.END_TIME;
			a.Duration = (Convert.ToDateTime (newAppointment.END_TIME) - Convert.ToDateTime (newAppointment.START_TIME)).TotalMinutes.ToString ();
			a.AppointmentID = newAppointment.APPOINTMENTID;
			a.AppointmentCommand = "View Appointment";
			this.AppointmentData = a;

			OnPropertyChanged ("AppointmentData");

			if (newAppointment.PATIENTID != null) {
				GetPatientInformation (newAppointment.PATIENTID);
			}
			LoadConflicts (false);
		}

		public void GetWalkInAppointment (SchdAppointment newAppointment)
		{
			GetPatientInformation (newAppointment.PATIENTID);
			this.SchdAppointment = newAppointment;
			this.SchdAppointment.PATIENTID = newAppointment.PATIENTID;
			AppointmentData a = new AppointmentData ();
			a.ResourceName = newAppointment.RESOURCENAME;
			a.Notes = "";
			a.StartTime = newAppointment.START_TIME;
			a.EndTime = newAppointment.END_TIME;
			a.Duration = (Convert.ToDateTime (newAppointment.END_TIME) - Convert.ToDateTime (newAppointment.START_TIME)).TotalMinutes.ToString ();
			a.AppointmentCommand = "WalkIn Appointment";
			this.AppointmentData = a;

			OnPropertyChanged ("AppointmentData");
		}

		public void GetNewAppointment (SchdAppointment newAppointment)
		{
			GetPatientInformation (newAppointment.PATIENTID);
			this.SchdAppointment = newAppointment;
			this.SchdAppointment.PATIENTID = newAppointment.PATIENTID;
			AppointmentData a = new AppointmentData ();
			a.ResourceName = newAppointment.RESOURCENAME;
			a.Notes = "";
			a.StartTime = newAppointment.START_TIME;
			a.EndTime = newAppointment.END_TIME;
			a.Duration = (Convert.ToDateTime (newAppointment.END_TIME) - Convert.ToDateTime (newAppointment.START_TIME)).TotalMinutes.ToString();
			a.AppointmentCommand = "New Appointment";
			this.AppointmentData = a;

			OnPropertyChanged ("AppointmentData");
			LoadConflicts (true);
		}

		public void GetPatientInformation (string patientIEN)
		{
			this.PatientInformation = this.dataAccessService.GetPatientInfo (patientIEN);
			OnPropertyChanged ("PatientInformation");
		}

		public void LoadConflicts (bool isNewAppointment)
		{
			this.ConflictAppointments = this.dataAccessService.GetConflictAppointment (this.SchdAppointment.RESOURCENAME, this.SchdAppointment.PATIENTID, this.SchdAppointment.START_TIME, this.SchdAppointment.END_TIME, isNewAppointment);
			OnPropertyChanged ("ConflictAppointments");
		}

		public void ExecuteNewAppointmentCommand (string command)
		{
			List<string> errorMessages = new List<string>();
			string errorMessage = string.Empty;
			if (command == "New Appointment") {
				this.AppointmentData.AppointmentID = "0";
				errorMessages = this.dataAccessService.AddNewAppointment (this.AppointmentData, this.PatientInformation, false);
				errorMessage = errorMessages[1];
			} else if (command == "WalkIn Appointment") {
				this.AppointmentData.AppointmentID = "WALKIN";
				errorMessage = CreateWalkInAppointment (this.AppointmentData, this.PatientInformation, false);
			} else if (command == "Edit Appointment") {
				errorMessage = this.dataAccessService.EditAppointment (this.AppointmentData);
			}

			this.validationMessage.IsValid = true;
			this.validationMessage.Title = string.Empty;
			this.validationMessage.Message = string.Empty;

			if (errorMessage == string.Empty) {
				this.eventAggregator.GetEvent<RefreshScheduler> ().Publish (command);
			} else {
				this.validationMessage.IsValid = false;
				this.validationMessage.Title = command;
				this.validationMessage.Message = errorMessage;
				return;
			}
		}

		public string CreateWalkInAppointment (AppointmentData a, PatientInformation p, bool bMakeChartRequest)
		{
			string errorMessage = string.Empty;
			List<string> errorMessages = new List<string> ();
			errorMessages = this.dataAccessService.AddNewAppointment (a, p, bMakeChartRequest);
			if (errorMessages[0] != "0" && errorMessages[1] == string.Empty) {
				//prompt check in dialog
				SchdAppointment sAppt = new SchdAppointment ();
				sAppt.START_TIME = a.StartTime;
				sAppt.APPOINTMENTID = errorMessages[0].ToString ();
				sAppt.PATIENTNAME = p.Name;
				sAppt.RESOURCENAME = a.ResourceName;
				this.eventAggregator.GetEvent<LaunchCheckInDialogEvent> ().Publish (sAppt);
			} else {
				errorMessage = errorMessages[1];
			}

			return errorMessage;
		}

		public bool CanExecuteNewAppointmentCommand (string command)
		{
			return true;
		}

		public void ViewPatientAppointments (int PatientID)
		{
			object[] args = new object[2];
			args[0] = "Patient Appointments";
			args[1] = PatientID;
			this.eventAggregator.GetEvent<DisplayReportEvent> ().Publish (args);
			//DPatientApptDisplay dPa = new DPatientApptDisplay (!m_bNewAppointment);
		}		

		private void PrintPatientLetter (int PatientID)
		{
			//DPatientLetter dPa = new DPatientLetter ();
		}

		public void ViewPatientHistory (int PatientID)
		{
			//DPatientHistoryDisplay dHi = new DPatientHistoryDisplay ();
		}

		private void ViewPatientHSMerge (int PatientID)
		{
			object[] args = new object[2];
			args[0] = "Patient HS Merge";
			args[1] = PatientID;
			this.eventAggregator.GetEvent<DisplayReportEvent> ().Publish (args);
			//DPatientHSMergeTextDisplay dHs = new DPatientHSMergeTextDisplay ();
		}

		private void ExecutePrintLetterCommand (string arg)
		{
			int nPatientID = Convert.ToInt32 (PatientInformation.IEN);
			switch (View.ReportComboBox.SelectedItem.ToString ()) {
			case "Patient Letter":
				PrintPatientLetter (nPatientID);
				break;
			case "Patient Appointments":
				ViewPatientAppointments (nPatientID);
				break;
			case "Patient History":
				ViewPatientHistory (nPatientID);
				break;
			case "Patient HS Merge":
				ViewPatientHSMerge (nPatientID);
				break;
			default:
				break;
			}
		}

		private bool CanExecutePrintLetterCommand (string arg)
		{
			return true;
		}

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
