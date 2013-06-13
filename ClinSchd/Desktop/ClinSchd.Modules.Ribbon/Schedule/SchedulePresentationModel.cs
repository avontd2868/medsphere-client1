using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Presentation.Commands;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Infrastructure.Events;
using ClinSchd.Modules.Ribbon.Controllers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace ClinSchd.Modules.Ribbon.Schedule
{

	public class SchedulePresentationModel : ISchedulePresentationModel, INotifyPropertyChanged
	{
		private readonly IRibbonService ribbonService;
		private readonly IEventAggregator eventAggregator;
		public event PropertyChangedEventHandler PropertyChanged;
		private readonly IDataAccessService dataAccessService;
		private bool _isViewAppointmentEnabled = false;
		private bool _isEditAppointmentEnabled = false;
		private bool _isNewAppointmentEnabled = false;
		private bool _isExistAppointmentEnabled = false;
		private bool _isNoShowAppointmentEnabled = false;
		private bool _isCheckedInAppointmentEnabled = false;
		private bool _isCheckedOutAppointmentEnabled = false;
		private bool _isCancelledAppointmentEnabled = false;
		private bool _isNotNoShowAppointmentEnabled = false;
		private bool _isNotCheckedInAppointmentEnabled = false;
		private bool _isNotCheckedOutAppointmentEnabled = false;
		private bool _isNotCancelledAppointmentEnabled = false;
		private bool _isDisplayWalkInEnabled = false;
		private bool _isPasteAvailability = false;
		private bool _isManagerUser = false;
		private IList<Patient> patientList;
		private string _paneTitle = "";
		private int _timeScaleIndex = 3;

		public SchedulePresentationModel (
			IScheduleView view,
			IRibbonService ribbonService,
			IDataAccessService dataAccessService,
			IEventAggregator eventAggregator)
		{
			View = view;
			View.Model = this;
			this.ribbonService = ribbonService;
			this.eventAggregator = eventAggregator;
			this.dataAccessService = dataAccessService;
			this.Resource = new SchdResource ();
			this.ResourceGroup = new SchdResourceGroup (this.dataAccessService);

			NewAppointmentCommand = new DelegateCommand<string> (ExecuteNewAppointmentCommand, CanExecuteNewAppointmentCommand);
			WalkInAppointmentCommand = new DelegateCommand<string> (ExecuteWalkInAppointmentCommand, CanExecuteWalkInAppointmentCommand);
			ViewAppointmentCommand = new DelegateCommand<string> (ExecuteViewAppointmentCommand, CanExecuteViewAppointmentCommand);
			EditAppointmentCommand = new DelegateCommand<string> (ExecuteEditAppointmentCommand, CanExecuteEditAppointmentCommand);
			SelectPatientCommand = new DelegateCommand<string> (ExecuteSelectPatientCommand, CanExecuteSelectPatientCommand);
			//ChangeServerCommand = new DelegateCommand<string> (ExecuteChangeServerCommand, CanExecuteChangeServerCommand);
			ChangeUserCommand = new DelegateCommand<string> (ExecuteChangeUserCommand, CanExecuteChangeUserCommand);
			ChangeDivisionCommand = new DelegateCommand<string> (ExecuteChangeDivisionCommand, CanExecuteChangeDivisionCommand);
			MarkAsNoShowCommand = new DelegateCommand<string> (ExecuteMarkAsNoShowCommand, CanExecuteMarkAsNoShowCommand);
			UndoANoShowCommand = new DelegateCommand<string> (ExecuteUndoANoShowCommand, CanExecuteUndoANoShowCommand);
			CheckInPatientCommand = new DelegateCommand<string> (ExecuteCheckInPatientCommand, CanExecuteCheckInPatientCommand);
			UndoCheckInPatientCommand = new DelegateCommand<string> (ExecuteUndoCheckInPatientCommand, CanExecuteUndoCheckInPatientCommand);
			CheckOutPatientCommand = new DelegateCommand<string> (ExecuteCheckOutPatientCommand, CanExecuteCheckOutPatientCommand);
			UndoCheckOutPatientCommand = new DelegateCommand<string> (ExecuteUndoCheckOutPatientCommand, CanExecuteUndoCheckOutPatientCommand);
			CancelAppointmentCommand = new DelegateCommand<string> (ExecuteCancelAppointmentCommand, CanExecuteCancelAppointmentCommand);
			UndoCancelAppointmentCommand = new DelegateCommand<string> (ExecuteUndoCancelAppointmentCommand, CanExecuteUndoCancelAppointmentCommand);
			FindAppointmentCommand = new DelegateCommand<string> (ExecuteFindAppointmentCommand, CanExecuteFindAppointmentCommand);
			SelectResourceCommand = new DelegateCommand<string> (ExecuteSelectResourceCommand, CanExecuteSelectResourceCommand);
			AvialiabilityResourceCommand = new DelegateCommand<string> (ExecuteAvialiabilityResourceCommand, CanExecuteAvialiabilityResourceCommand);
			ViewDayRangeCommand = new DelegateCommand<string> (ExecuteViewDayRangeCommand, CanExecuteViewDayRangeCommand);
			ViewTimeScaleCommand = new DelegateCommand<string> (ExecuteViewTimeScaleCommand, CanExecuteViewTimeScaleCommand);
			SchedulingManagementCommand = new DelegateCommand<string> (ExecuteSchedulingManagementCommand, CanExecuteSchedulingManagementCommand);
			CopyClipboardCommand = new DelegateCommand<string> (ExecuteHelpCommand, CanExecuteHelpCommand);
			DisplayWalkInCommand = new DelegateCommand<string> (ExecuteCopyClipboardCommand, CanExecuteCopyClipboardCommand);
			ChangeViewModeCommand = new DelegateCommand<string> (ExecuteChangeViewModeCommand, CanExecuteChangeViewModeCommand);
			CopyAppointmentCommand = new DelegateCommand<string> (ExecuteCopyAppointmentCommand, CanExecuteCopyAppointmentCommand);
			PasteAppointmentCommand = new DelegateCommand<string> (ExecutePasteAppointmentCommand, CanExecutePasteAppointmentCommand);
			HelpCommand = new DelegateCommand<string> (ExecuteHelpCommand, CanExecuteHelpCommand);
			ExitCommand = new DelegateCommand<string> (ExecuteExitCommand, CanExecuteExitCommand);
			PrintCurrentSchedulerCommand = new DelegateCommand<string> (ExecutePrintCurrentSchedulerCommand, CanExecutePrintCurrentSchedulerCommand);
			ReportPatientHSMergeCommand = new DelegateCommand<string> (ExecuteReportPatientHSMergeCommand, CanExecuteReportPatientCommand);
			ReportPatientHistoryCommand = new DelegateCommand<string> (ExecuteReportPatientHistoryCommand, CanExecuteReportPatientCommand);
			ReportPatientAppointmentCommand = new DelegateCommand<string> (ExecuteReportPatientAppointmentCommand, CanExecuteReportPatientCommand);
			ReportPatientLetterCommand = new DelegateCommand<string> (ExecuteReportPatientLetterCommand, CanExecuteReportLetterCommand);
			ReportReminderLetterCommand = new DelegateCommand<string> (ExecuteReportReminderLetterCommand, CanExecuteReportLetterCommand);
			ReportRebookLetterCommand = new DelegateCommand<string> (ExecuteReportRebookLetterCommand, CanExecuteReportLetterCommand);
			ReportCancellationLetterCommand = new DelegateCommand<string> (ExecuteReportCancellationLetterCommand, CanExecuteReportLetterCommand);
			ReportClinicSchedulerCommand = new DelegateCommand<string> (ExecuteReportClinicSchedulerCommand, CanExecuteReportLetterCommand);
			ReportClinicWalkInCommand = new DelegateCommand<string> (ExecuteReportClinicWalkInCommand, CanExecuteReportLetterCommand);

			this.eventAggregator.GetEvent<PatientSelectedEvent> ().Subscribe (PatientSelected, ThreadOption.UIThread, false);
			this.eventAggregator.GetEvent<SchedulerDisplayEvent> ().Subscribe (SchedulerDisplay, ThreadOption.UIThread, false);
			this.eventAggregator.GetEvent<ResourceSelectedEvent> ().Subscribe (ResourceSelected, ThreadOption.UIThread, false);
			this.eventAggregator.GetEvent<ResourceGroupSelectedEvent> ().Subscribe (ResourceGroupSelected, ThreadOption.UIThread, false);
			this.eventAggregator.GetEvent<EnableAppointmentButtonsEvent> ().Subscribe (EnableAppointmentButtons, ThreadOption.UIThread, false);
			this.eventAggregator.GetEvent<PassAppointmentsEvent> ().Subscribe (PassAppointments, ThreadOption.UIThread, false);
			this.eventAggregator.GetEvent<FocusPatientSearchEvent> ().Subscribe (FocusPatientSearch, ThreadOption.UIThread, false);
			this.eventAggregator.GetEvent<CopyKeysPressedEvent> ().Subscribe (CopyKeysPressed, ThreadOption.UIThread, false);
			this.eventAggregator.GetEvent<PasteKeysPressedEvent> ().Subscribe (PasteKeysPressed, ThreadOption.UIThread, false);
			this.eventAggregator.GetEvent<IsPasteAvailabilityEvent> ().Subscribe (IsPasteAvailability, ThreadOption.UIThread, false);
			this.eventAggregator.GetEvent<IsManagerUserEvent> ().Subscribe (SetManagerUser, ThreadOption.UIThread, false);
			this.eventAggregator.GetEvent<DisplayDefaultDivisionEvent> ().Subscribe (DisplayDefaultDivision, ThreadOption.UIThread, false);
			this.eventAggregator.GetEvent<SelectTimeScaleEvent> ().Subscribe (SelectTimeScale, ThreadOption.UIThread, false);


			
		}

		public void SelectTimeScale (int timeScaleIndex)
		{
			_timeScaleIndex = timeScaleIndex;
			OnPropertyChanged ("TimeScaleIndex");
		}

		public void DisplayDefaultDivision (string divisionName)
		{
			_paneTitle = "Clinical Scheduling for Division: " + divisionName;
			OnPropertyChanged ("PaneTitle");
		}

		public void PatientSelected (ClinSchd.Infrastructure.Models.Patient selectedPatient)
		{
			this.Patient = selectedPatient;
			OnPropertyChanged ("Patient");
			if (selectedPatient != null && this.SelectedAppointment != null) {
				this._isNewAppointmentEnabled = true;
			} else {
				this._isNewAppointmentEnabled = false;
			}
			OnPropertyChanged ("IsNewAppointmentEnabled");
		}

		public void ResourceSelected (SchdResource selectedResource)
		{
			this.Clinic = selectedResource.Clinic;
			if (this.Clinic != null) {
				if (this.Clinic.PROVIDER == string.Empty) {
					this.Clinic.PROVIDER = "None";
				}
				OnPropertyChanged ("Clinic");
			}			
			this.Resource = selectedResource;
			List<SchdResource> newGroupList = new List<SchdResource> ();
			newGroupList.Insert (0, this.Resource);
			this.SelectedResourceList = newGroupList;
		}

		public void ResourceGroupSelected (SchdResourceGroup selectedResourceGroup)
		{
			RPMSClinic c = new RPMSClinic ();
			c.PROVIDER = "Multiple";
			this.Clinic = c;
			OnPropertyChanged ("Clinic");
			this.ResourceGroup = selectedResourceGroup;
			this.SelectedResourceList = this.ResourceGroup.Resources;
		}

		public void SelectPatient (Patient selectedPatient)
		{
			this.eventAggregator.GetEvent<PatientSelectedEvent> ().Publish (selectedPatient);
		}

		public void PassAppointments (SchdAppointment newAppointment)
		{
			this.SelectedAppointment = newAppointment;
		}

		public void SchedulerDisplay (ClinSchd.Infrastructure.Models.SchdResource selectedResource)
		{
			this.Clinic = selectedResource.Clinic;
			OnPropertyChanged ("Clinic");
		}

		private void FocusPatientSearch (object T)
		{
			View.FocusPatientSearch ();
		}

		private void CopyKeysPressed (object T)
		{
			CopyAppointmentCommand.Execute (null);
		}

		private void PasteKeysPressed (object T)
		{
			PasteAppointmentCommand.Execute (null);
		}

		private void IsPasteAvailability (string command)
		{
			if (command == "New Availability") {
				_isPasteAvailability = true;
			} else {
				_isPasteAvailability = false;
			}
		}

		private void SetManagerUser (bool isManager)
		{
			if (isManager) {
				this._isManagerUser = true;
			} else {
				this._isManagerUser = false;
			}
			OnPropertyChanged ("IsManagerUser");
		}

		public bool IsEditAppointmentEnabled
		{
			get { return _isEditAppointmentEnabled; }
			set { _isEditAppointmentEnabled = value; }
		}

		public bool IsViewAppointmentEnabled
		{
			get { return _isViewAppointmentEnabled; }
			set { _isViewAppointmentEnabled = value; }
		}

		public bool IsNewAppointmentEnabled
		{
			get { return _isNewAppointmentEnabled; }
			set { _isNewAppointmentEnabled = value; }
		}

		public bool IsExistAppointmentEnabled
		{
			get { return _isExistAppointmentEnabled; }
			set { _isExistAppointmentEnabled = value; }
		}

		public bool IsNoShowAppointmentEnabled
		{
			get { return _isNoShowAppointmentEnabled; }
			set { _isNoShowAppointmentEnabled = value; }
		}

		public bool IsCancelledAppointmentEnabled
		{
			get { return _isCancelledAppointmentEnabled; }
			set { _isCancelledAppointmentEnabled = value; }
		}

		public bool IsCheckedInAppointmentEnabled
		{
			get { return _isCheckedInAppointmentEnabled; }
			set { _isCheckedInAppointmentEnabled = value; }
		}

		public bool IsCheckedOutAppointmentEnabled
		{
			get { return _isCheckedOutAppointmentEnabled; }
			set { _isCheckedOutAppointmentEnabled = value; }
		}

		public bool IsNotNoShowAppointmentEnabled
		{
			get { return _isNotNoShowAppointmentEnabled; }
			set { _isNotNoShowAppointmentEnabled = value; }
		}

		public bool IsNotCancelledAppointmentEnabled
		{
			get { return _isNotCancelledAppointmentEnabled; }
			set { _isNotCancelledAppointmentEnabled = value; }
		}

		public bool IsNotCheckedInAppointmentEnabled
		{
			get { return _isNotCheckedInAppointmentEnabled; }
			set { _isNotCheckedInAppointmentEnabled = value; }
		}

		public bool IsNotCheckedOutAppointmentEnabled
		{
			get { return _isNotCheckedOutAppointmentEnabled; }
			set { _isNotCheckedOutAppointmentEnabled = value; }
		}

		public bool IsDisplayWalkInEnabled
		{
			get { return _isDisplayWalkInEnabled; }
			set { _isDisplayWalkInEnabled = value; }
		}

		public Patient Patient { get; set; }
		public SchdAppointment SelectedAppointment { get; set; }
		public RPMSClinic Clinic { get; set; }
		public IScheduleView View { get; private set; }
		public SchdResourceGroup ResourceGroup { get; set; }
		public SchdResource Resource { get; set; }
		public IList<SchdResource> SelectedResourceList { get; set; }

		public DelegateCommand<string> NewAppointmentCommand { get; private set; }
		public DelegateCommand<string> WalkInAppointmentCommand { get; private set; }
		public DelegateCommand<string> ViewAppointmentCommand { get; private set; }
		public DelegateCommand<string> EditAppointmentCommand { get; private set; }
		public DelegateCommand<string> CopyAppointmentCommand { get; private set; }
		public DelegateCommand<string> PasteAppointmentCommand { get; private set; }
		public DelegateCommand<string> SelectPatientCommand { get; private set; }
		//public DelegateCommand<string> ChangeServerCommand { get; private set; }
		public DelegateCommand<string> ChangeUserCommand { get; private set; }
		public DelegateCommand<string> ChangeDivisionCommand { get; private set; }
		public DelegateCommand<string> MarkAsNoShowCommand { get; private set; }
		public DelegateCommand<string> UndoANoShowCommand { get; private set; }
		public DelegateCommand<string> CheckInPatientCommand { get; private set; }
		public DelegateCommand<string> UndoCheckInPatientCommand { get; private set; }
		public DelegateCommand<string> CheckOutPatientCommand { get; private set; }
		public DelegateCommand<string> UndoCheckOutPatientCommand { get; private set; }
		public DelegateCommand<string> SelectResourceCommand { get; private set; }
		public DelegateCommand<string> AvialiabilityResourceCommand { get; private set; }
		public DelegateCommand<string> SchedulingManagementCommand { get; private set; }
		public DelegateCommand<string> ViewDayRangeCommand { get; private set; }
		public DelegateCommand<string> ViewTimeScaleCommand { get; private set; }
		public DelegateCommand<string> DisplayWalkInCommand { get; private set; }
		public DelegateCommand<string> HelpCommand { get; private set; }
		public DelegateCommand<string> ExitCommand { get; private set; }
		public DelegateCommand<string> CancelAppointmentCommand { get; private set; }
		public DelegateCommand<string> UndoCancelAppointmentCommand { get; private set; }
		public DelegateCommand<string> FindAppointmentCommand { get; private set; }
		public DelegateCommand<string> CopyClipboardCommand { get; private set; }
		public DelegateCommand<string> ChangeViewModeCommand { get; private set; }
		public DelegateCommand<string> PrintCurrentSchedulerCommand { get; private set; }
		public DelegateCommand<string> ReportPatientHSMergeCommand { get; private set; }
		public DelegateCommand<string> ReportPatientHistoryCommand { get; private set; }
		public DelegateCommand<string> ReportPatientAppointmentCommand { get; private set; }
		public DelegateCommand<string> ReportPatientLetterCommand { get; private set; }
		public DelegateCommand<string> ReportReminderLetterCommand { get; private set; }
		public DelegateCommand<string> ReportRebookLetterCommand { get; private set; }
		public DelegateCommand<string> ReportCancellationLetterCommand { get; private set; }
		public DelegateCommand<string> ReportClinicSchedulerCommand { get; private set; }
		public DelegateCommand<string> ReportClinicWalkInCommand { get; private set; }


		public void ExecuteCopyClipboardCommand (string command)
		{
		}

		public void ExecuteNewAppointmentCommand (string command)
		{
			this.SelectedAppointment.PATIENTID = this.Patient.IEN;
			this.SelectedAppointment.PATIENTNAME = this.Patient.Name;
			this.eventAggregator.GetEvent<AddNewAppointmentEvent> ().Publish (this.SelectedAppointment);
		}

		public void ExecuteWalkInAppointmentCommand (string title)
		{
			this.SelectedAppointment.PATIENTID = this.Patient.IEN;
			this.SelectedAppointment.PATIENTNAME = this.Patient.Name;
			this.eventAggregator.GetEvent<AddWalkInAppointmentEvent> ().Publish (this.SelectedAppointment);
		}

		public void ExecuteViewAppointmentCommand (string command)
		{
			this.eventAggregator.GetEvent<ViewAppointmentEvent> ().Publish (this.SelectedAppointment);
		}

		public void ExecuteEditAppointmentCommand (string command)
		{
			this.eventAggregator.GetEvent<EditAppointmentEvent> ().Publish (this.SelectedAppointment);
		}

		public void ExecuteCopyAppointmentCommand (string command)
		{
			this.eventAggregator.GetEvent<CopyAppointmentEvent> ().Publish (this.SelectedAppointment);
		}

		public void ExecutePasteAppointmentCommand (string command)
		{
			if (_isPasteAvailability) {
				this.eventAggregator.GetEvent<PasteAvailabilityEvent> ().Publish (this.SelectedAppointment);
			} else {
				this.eventAggregator.GetEvent<PasteAppointmentEvent> ().Publish (this.SelectedAppointment);
			}
		}

		public void ExecuteSelectPatientCommand (string title)
		{
			this.eventAggregator.GetEvent<LaunchPatientSelectionDialogEvent> ().Publish (title);
		}

		//public void ExecuteChangeServerCommand (string title)
		//{
		//    this.eventAggregator.GetEvent<LaunchChangeServerDialogEvent> ().Publish (title);
		//}

		public void ExecuteChangeUserCommand (string title)
		{
			if (this.View.ConfirmUser("Are you sure you want to change the login user?", "Change User")){
				this.dataAccessService.ChangeUserServer ();
			}
		}

		public void ExecuteChangeDivisionCommand (string title)
		{
			this.eventAggregator.GetEvent<LaunchChangeDivisionDialogEvent> ().Publish (title);
		}

		public void ExecuteMarkAsNoShowCommand (string title)
		{
			this.eventAggregator.GetEvent<LaunchMarkAsNoShowDialogEvent> ().Publish (this.SelectedAppointment);
		}

		public void ExecuteUndoANoShowCommand (string command)
		{
			this.eventAggregator.GetEvent<UndoNoShowAppointmentEvent> ().Publish (command);
		}

		public void ExecuteCheckInPatientCommand (string command)
		{
			this.eventAggregator.GetEvent<LaunchCheckInDialogEvent> ().Publish (this.SelectedAppointment);
		}

		public void ExecuteUndoCheckInPatientCommand (string command)
		{
			this.eventAggregator.GetEvent<UndoCheckInPatientEvent> ().Publish (command);
		}

		public void ExecuteCheckOutPatientCommand (string command)
		{
			this.eventAggregator.GetEvent<LaunchCheckOutDialogEvent> ().Publish (this.SelectedAppointment);
		}

		public void ExecuteUndoCheckOutPatientCommand (string command)
		{
			this.eventAggregator.GetEvent<UndoCheckOutPatientEvent> ().Publish (command);
		}

		public void ExecuteCancelAppointmentCommand (string command)
		{
			this.eventAggregator.GetEvent<LaunchCancelApptDialogEvent> ().Publish (this.SelectedAppointment);
		}

		public void ExecuteUndoCancelAppointmentCommand (string command)
		{
			this.eventAggregator.GetEvent<UndoCancelAppointmentEvent> ().Publish (command);
		}

		public void ExecuteFindAppointmentCommand (string command)
		{
			this.eventAggregator.GetEvent<LaunchFindApptDialogEvent> ().Publish (this.SelectedResourceList);
		}

		public void ExecuteSelectResourceCommand (string title)
		{
			CompositePresentationEvent<ClinSchd.Infrastructure.Models.SchdResource> newforwardingEvent = this.eventAggregator.GetEvent<SchedulerResourcesEvent> ();
			this.eventAggregator.GetEvent<LaunchResourceSelectionDialogEvent> ().Publish (newforwardingEvent);
		}

		public void ExecuteAvialiabilityResourceCommand (string title)
		{
			CompositePresentationEvent<ClinSchd.Infrastructure.Models.SchdResource> newforwardingEvent = this.eventAggregator.GetEvent<AvailibilityResourcesEvent> ();
			this.eventAggregator.GetEvent<LaunchResourceAvailibilityDialogEvent> ().Publish (newforwardingEvent);
		}

		public void ExecuteViewDayRangeCommand (string title)
		{
			this.eventAggregator.GetEvent<ViewDayRangeEvent> ().Publish (title);
		}

		public void ExecuteViewTimeScaleCommand (string title)
		{
			this.eventAggregator.GetEvent<ViewTimeScaleEvent> ().Publish (title);
		}

		public void ExecuteChangeViewModeCommand (string mode)
		{
			this.eventAggregator.GetEvent<ChangeViewModeEvent> ().Publish (mode);
		}

		public void ExecuteDisplayWalkInCommand (string isDisplayWalkInEnabled)
		{
			this.eventAggregator.GetEvent<DisplayWalkInEvent> ().Publish (isDisplayWalkInEnabled);
		}

		public void ExecuteSchedulingManagementCommand (string title)
		{
			this.eventAggregator.GetEvent<LaunchSchedulingManagementDialogEvent> ().Publish (title);
		}

		public void ExecuteHelpCommand (string title)
		{
			this.eventAggregator.GetEvent<HelpEvent> ().Publish ("Help");
		}

		public void ExecuteExitCommand (string title)
		{
			this.eventAggregator.GetEvent<ExitApplicationEvent> ().Publish ("Exiting");
			Environment.Exit (1);
		}

		public void ExecutePrintCurrentSchedulerCommand (string cmd)
		{
			this.eventAggregator.GetEvent<PrintCurrentSchedulerEvent> ().Publish (null);
		}

		private void ExecuteReportPatientHSMergeCommand (string cmd)
		{
			object[] args = new object[2];
			args[0] = "Patient HS Merge";
			args[1] = Patient.IEN;
			this.eventAggregator.GetEvent<DisplayReportEvent> ().Publish (args);
		}

		private void ExecuteReportPatientHistoryCommand (string cmd)
		{
			object[] args = new object[2];
			args[0] = "Patient History Report";
			args[1] = Patient.IEN;
			this.eventAggregator.GetEvent<DisplayReportEvent> ().Publish (args);			
		}

		private void ExecuteReportPatientAppointmentCommand (string cmd)
		{
			object[] args = new object[2];
			args[0] = "Patient Appointment Letter";
			args[1] = Patient.IEN;
			this.eventAggregator.GetEvent<DisplayReportEvent> ().Publish (args);			
		}

		private void ExecuteReportPatientLetterCommand (string cmd)
		{
			object[] args = new object[3];
			args[0] = "Patient Reminder Letter Formatted";
			args[1] = this.Resource.RESOURCEID;
			args[2] = "R";
			this.eventAggregator.GetEvent<DisplayReportEvent> ().Publish (args);
		}

		private void ExecuteReportReminderLetterCommand (string cmd)
		{
			object[] args = new object[3];
			args[0] = "Patient Reminder Letter Formatted";
			args[1] = this.Resource.RESOURCEID;
			args[2] = "R";
			this.eventAggregator.GetEvent<DisplayReportEvent> ().Publish (args);
		}

		private void ExecuteReportRebookLetterCommand (string cmd)
		{
			object[] args = new object[3];
			args[0] = "Patient No Show Letter Formatted";
			args[1] = this.Resource.RESOURCEID;
			args[2] = "N";
			this.eventAggregator.GetEvent<DisplayReportEvent> ().Publish (args);
		}

		private void ExecuteReportCancellationLetterCommand (string cmd)
		{
			object[] args = new object[3];
			args[0] = "Patient Cancellation Letter Formatted";
			args[1] = this.Resource.RESOURCEID;
			args[2] = "A";
			this.eventAggregator.GetEvent<DisplayReportEvent> ().Publish (args);
		}

		private void ExecuteReportClinicSchedulerCommand (string cmd)
		{
			object[] args = new object[5];
			args[0] = "Clinic Scheduler Report";
			args[1] = this.Resource.RESOURCEID + "|";
			args[2] = DateTime.Now.AddDays (-7).ToString ("M/d/yyyy@HH:mm");
			args[3] = DateTime.Now.AddDays (7).ToString ("M/d/yyyy@HH:mm");
			args[4] = "0";
			this.eventAggregator.GetEvent<DisplayReportEvent> ().Publish (args);
		}

		private void ExecuteReportClinicWalkInCommand (string cmd)
		{
			object[] args = new object[4];
			args[0] = "Clinic WalkIn Report";
			args[1] = this.Resource.RESOURCEID + "|";
			args[2] = DateTime.Now.AddDays (-7).ToString ("M/d/yyyy@HH:mm");
			args[3] = DateTime.Now.AddDays (7).ToString ("M/d/yyyy@HH:mm");
			this.eventAggregator.GetEvent<DisplayReportEvent> ().Publish (args);
		}

		public bool CanExecuteReportLetterCommand (string cmd)
		{
			if (this.Clinic != null) {
				return true;
			} else {
				return false;
			}
		}

		public bool CanExecutePrintCurrentSchedulerCommand (string cmd)
		{
			return true;
		}

		public bool CanExecuteCopyClipboardCommand (string title)
		{
			return true;
		}

		public bool CanExecuteNewAppointmentCommand (string title)
		{
			return true;
		}

		public bool CanExecuteWalkInAppointmentCommand (string title)
		{
			return true;
		}

		public bool CanExecuteViewAppointmentCommand (string title)
		{
			return _isViewAppointmentEnabled;
		}

		public bool CanExecuteEditAppointmentCommand (string title)
		{
			return _isEditAppointmentEnabled;
		}

		public bool CanExecuteCopyAppointmentCommand (string title)
		{
			return true;
		}

		public bool CanExecutePasteAppointmentCommand (string title)
		{
			return true;
		}

		public bool CanExecuteSelectPatientCommand (string title)
		{
			return true;
		}

		//public bool CanExecuteChangeServerCommand (string title)
		//{
		//    return true;
		//}

		public bool CanExecuteChangeUserCommand (string title)
		{
			return true;
		}

		public bool CanExecuteChangeDivisionCommand (string title)
		{
			return true;
		}

		public bool CanExecuteMarkAsNoShowCommand (string title)
		{
			return true;
		}

		public bool CanExecuteUndoANoShowCommand (string title)
		{
			return true;
		}

		public bool CanExecuteCheckInPatientCommand (string title)
		{
			return true;
		}

		public bool CanExecuteUndoCheckInPatientCommand (string title)
		{
			return true;
		}

		public bool CanExecuteCheckOutPatientCommand (string title)
		{
			return true;
		}

		public bool CanExecuteUndoCheckOutPatientCommand (string title)
		{
			return true;
		}

		public bool CanExecuteCancelAppointmentCommand (string title)
		{
			return _isNotCancelledAppointmentEnabled;
		}

		public bool CanExecuteUndoCancelAppointmentCommand (string title)
		{
			return _isCancelledAppointmentEnabled;
		}

		public bool CanExecuteFindAppointmentCommand (string title)
		{
			if (this.Clinic != null) {
				return true;
			} else {
				return false;
			}
		}

		public bool CanExecuteSelectResourceCommand (string title)
		{
			return true;
		}

		public bool CanExecuteAvialiabilityResourceCommand (string title)
		{
			return true;
		}

		public bool CanExecuteViewDayRangeCommand (string title)
		{
			return true;
		}

		public bool CanExecuteViewTimeScaleCommand (string title)
		{
			return true;
		}

		public bool CanExecuteChangeViewModeCommand (string title)
		{
			return true;
		}

		public bool CanExecuteDisplayWalkInCommand (string title)
		{
			return true;
		}

		public bool CanExecuteSchedulingManagementCommand (string title)
		{
			return true;
		}

		public bool CanExecuteHelpCommand (string title)
		{
			return true;
		}

		public bool CanExecuteExitCommand (string title)
		{
			return true;
		}

		private bool CanExecuteReportPatientCommand (string cmd)
		{
			if (Patient == null) {
				return false;
			}
			return true;
		}

		private void OnPropertyChanged (string propertyName)
		{
			PropertyChangedEventHandler handler = this.PropertyChanged;
			if (handler != null) {
				handler (this, new PropertyChangedEventArgs (propertyName));
			}
		}

		public void EnableAppointmentButtons (string command)
		{
			switch (command) {
			case "Reset Buttons":
			case "New Availability":
				this._isNewAppointmentEnabled = false;
				this._isViewAppointmentEnabled = false;
				OnPropertyChanged ("IsNewAppointmentEnabled");
				this._isEditAppointmentEnabled = false;
				OnPropertyChanged ("IsEditAppointmentEnabled");
				this._isExistAppointmentEnabled = false;
				OnPropertyChanged ("IsExistAppointmentEnabled");
				this._isNoShowAppointmentEnabled = false;
				OnPropertyChanged ("IsNoShowAppointmentEnabled");
				this._isNotNoShowAppointmentEnabled = false;
				OnPropertyChanged ("IsNotNoShowAppointmentEnabled");
				this._isCheckedInAppointmentEnabled = false;
				OnPropertyChanged ("IsCheckedInAppointmentEnabled");
				this._isCheckedOutAppointmentEnabled = false;
				OnPropertyChanged ("IsCheckedOutAppointmentEnabled");
				this._isCancelledAppointmentEnabled = false;
				OnPropertyChanged ("IsCancelledAppointmentEnabled");
				this._isNotCheckedInAppointmentEnabled = false;
				OnPropertyChanged ("IsNotCheckedInAppointmentEnabled");
				this._isNotCheckedOutAppointmentEnabled = false;
				OnPropertyChanged ("IsNotCheckedOutAppointmentEnabled");
				this._isNotCancelledAppointmentEnabled = false;
				OnPropertyChanged ("IsNotCancelledAppointmentEnabled");
				break;
			case "New Appointment":
				if (this.Patient != null && this.SelectedAppointment != null) {
					this._isNewAppointmentEnabled = true;
				} else {
					this._isNewAppointmentEnabled = false;
				}
				OnPropertyChanged ("IsNewAppointmentEnabled");
				this._isEditAppointmentEnabled = false;
				this._isViewAppointmentEnabled = false;
				OnPropertyChanged ("IsEditAppointmentEnabled");
				this._isExistAppointmentEnabled = false;
				OnPropertyChanged ("IsExistAppointmentEnabled");
				this._isNoShowAppointmentEnabled = false;
				OnPropertyChanged ("IsNoShowAppointmentEnabled");
				this._isNotNoShowAppointmentEnabled = false;
				OnPropertyChanged ("IsNotNoShowAppointmentEnabled");
				this._isCheckedInAppointmentEnabled = false;
				OnPropertyChanged ("IsCheckedInAppointmentEnabled");
				this._isCheckedOutAppointmentEnabled = false;
				OnPropertyChanged ("IsCheckedOutAppointmentEnabled");
				this._isCancelledAppointmentEnabled = false;
				OnPropertyChanged ("IsCancelledAppointmentEnabled");
				this._isNotCheckedInAppointmentEnabled = false;
				OnPropertyChanged ("IsNotCheckedInAppointmentEnabled");
				this._isNotCheckedOutAppointmentEnabled = false;
				OnPropertyChanged ("IsNotCheckedOutAppointmentEnabled");
				this._isNotCancelledAppointmentEnabled = false;
				OnPropertyChanged ("IsNotCancelledAppointmentEnabled");
				break;
			case "EXIST Appointment":
				this._isNewAppointmentEnabled = false;
				OnPropertyChanged ("IsNewAppointmentEnabled");
				this._isViewAppointmentEnabled = true;
				this._isEditAppointmentEnabled = true;
				OnPropertyChanged ("IsEditAppointmentEnabled");
				this._isExistAppointmentEnabled = true;
				OnPropertyChanged ("IsExistAppointmentEnabled");
				this._isNoShowAppointmentEnabled = false;
				OnPropertyChanged ("IsNoShowAppointmentEnabled");
				this._isCheckedInAppointmentEnabled = false;
				OnPropertyChanged ("IsCheckedInAppointmentEnabled");
				this._isCheckedOutAppointmentEnabled = false;
				OnPropertyChanged ("IsCheckedOutAppointmentEnabled");
				this._isCancelledAppointmentEnabled = false;
				OnPropertyChanged ("IsCancelledAppointmentEnabled");
				this._isNotNoShowAppointmentEnabled = true;
				OnPropertyChanged ("IsNotNoShowAppointmentEnabled");
				this._isNotCheckedInAppointmentEnabled = true;
				OnPropertyChanged ("IsNotCheckedInAppointmentEnabled");
				this._isNotCheckedOutAppointmentEnabled = false;
				OnPropertyChanged ("IsNotCheckedOutAppointmentEnabled");
				this._isNotCancelledAppointmentEnabled = true;
				OnPropertyChanged ("IsNotCancelledAppointmentEnabled");
				break;
			case "NOSHOW Appointment":
				this._isNewAppointmentEnabled = false;
				OnPropertyChanged ("IsNewAppointmentEnabled");
				this._isViewAppointmentEnabled = true;
				this._isEditAppointmentEnabled = true;
				OnPropertyChanged ("IsEditAppointmentEnabled");
				this._isExistAppointmentEnabled = true;
				OnPropertyChanged ("IsExistAppointmentEnabled");
				this._isNoShowAppointmentEnabled = true;
				OnPropertyChanged ("IsNoShowAppointmentEnabled");
				this._isCheckedInAppointmentEnabled = false;
				OnPropertyChanged ("IsCheckedInAppointmentEnabled");
				this._isCheckedOutAppointmentEnabled = false;
				OnPropertyChanged ("IsCheckedOutAppointmentEnabled");
				this._isCancelledAppointmentEnabled = false;
				OnPropertyChanged ("IsCancelledAppointmentEnabled");
				this._isNotNoShowAppointmentEnabled = false;
				OnPropertyChanged ("IsNotNoShowAppointmentEnabled");
				this._isNotCheckedInAppointmentEnabled = false;
				OnPropertyChanged ("IsNotCheckedInAppointmentEnabled");
				this._isNotCheckedOutAppointmentEnabled = false;
				OnPropertyChanged ("IsNotCheckedOutAppointmentEnabled");
				this._isNotCancelledAppointmentEnabled = true;
				OnPropertyChanged ("IsNotCancelledAppointmentEnabled");
				break;
			case "CANCELLED Appointment":
				this._isNewAppointmentEnabled = false;
				OnPropertyChanged ("IsNewAppointmentEnabled");
				this._isViewAppointmentEnabled = true;
				this._isEditAppointmentEnabled = false;
				OnPropertyChanged ("IsEditAppointmentEnabled");
				this._isExistAppointmentEnabled = true;
				OnPropertyChanged ("IsExistAppointmentEnabled");
				this._isNoShowAppointmentEnabled = false;
				OnPropertyChanged ("IsNoShowAppointmentEnabled");
				this._isCheckedInAppointmentEnabled = false;
				OnPropertyChanged ("IsCheckedInAppointmentEnabled");
				this._isCheckedOutAppointmentEnabled = false;
				OnPropertyChanged ("IsCheckedOutAppointmentEnabled");
				this._isCancelledAppointmentEnabled = true;
				OnPropertyChanged ("IsCancelledAppointmentEnabled");
				this._isNotNoShowAppointmentEnabled = false;
				OnPropertyChanged ("IsNotNoShowAppointmentEnabled");
				this._isNotCheckedInAppointmentEnabled = false;
				OnPropertyChanged ("IsNotCheckedInAppointmentEnabled");
				this._isNotCheckedOutAppointmentEnabled = false;
				OnPropertyChanged ("IsNotCheckedOutAppointmentEnabled");
				this._isNotCancelledAppointmentEnabled = false;
				OnPropertyChanged ("IsNotCancelledAppointmentEnabled");
				break;
			case "WALKIN Appointment":
				this._isNewAppointmentEnabled = false;
				OnPropertyChanged ("IsNewAppointmentEnabled");
				this._isViewAppointmentEnabled = true;
				this._isEditAppointmentEnabled = true;
				OnPropertyChanged ("IsEditAppointmentEnabled");
				this._isExistAppointmentEnabled = true;
				OnPropertyChanged ("IsExistAppointmentEnabled");
				this._isNoShowAppointmentEnabled = false;
				OnPropertyChanged ("IsNoShowAppointmentEnabled");
				this._isCheckedInAppointmentEnabled = false;
				OnPropertyChanged ("IsCheckedInAppointmentEnabled");
				this._isCheckedOutAppointmentEnabled = false;
				OnPropertyChanged ("IsCheckedOutAppointmentEnabled");
				this._isCancelledAppointmentEnabled = false;
				OnPropertyChanged ("IsCancelledAppointmentEnabled");
				this._isNotNoShowAppointmentEnabled = false;
				OnPropertyChanged ("IsNotNoShowAppointmentEnabled");
				this._isNotCheckedInAppointmentEnabled = false;
				OnPropertyChanged ("IsNotCheckedInAppointmentEnabled");
				this._isNotCheckedOutAppointmentEnabled = true;
				OnPropertyChanged ("IsNotCheckedOutAppointmentEnabled");
				this._isNotCancelledAppointmentEnabled = false;
				OnPropertyChanged ("IsNotCancelledAppointmentEnabled");
				break;
			case "CHECKOUT Appointment":
				this._isNewAppointmentEnabled = false;
				OnPropertyChanged ("IsNewAppointmentEnabled");
				this._isViewAppointmentEnabled = true;
				this._isEditAppointmentEnabled = false;
				OnPropertyChanged ("IsEditAppointmentEnabled");
				this._isExistAppointmentEnabled = true;
				OnPropertyChanged ("IsExistAppointmentEnabled");
				this._isNoShowAppointmentEnabled = false;
				OnPropertyChanged ("IsNoShowAppointmentEnabled");
				this._isCheckedInAppointmentEnabled = false;
				OnPropertyChanged ("IsCheckedInAppointmentEnabled");
				this._isCheckedOutAppointmentEnabled = true;
				OnPropertyChanged ("IsCheckedOutAppointmentEnabled");
				this._isCancelledAppointmentEnabled = false;
				OnPropertyChanged ("IsCancelledAppointmentEnabled");
				this._isNotNoShowAppointmentEnabled = false;
				OnPropertyChanged ("IsNotNoShowAppointmentEnabled");
				this._isNotCheckedInAppointmentEnabled = false;
				OnPropertyChanged ("IsNotCheckedInAppointmentEnabled");
				this._isNotCheckedOutAppointmentEnabled = false;
				OnPropertyChanged ("IsNotCheckedOutAppointmentEnabled");
				this._isNotCancelledAppointmentEnabled = false;
				OnPropertyChanged ("IsNotCancelledAppointmentEnabled");
				break;
			case "CHECKIN Appointment":
				this._isNewAppointmentEnabled = false;
				OnPropertyChanged ("IsNewAppointmentEnabled");
				this._isViewAppointmentEnabled = true;
				this._isEditAppointmentEnabled = true;
				OnPropertyChanged ("IsEditAppointmentEnabled");
				this._isExistAppointmentEnabled = true;
				OnPropertyChanged ("IsExistAppointmentEnabled");
				this._isNoShowAppointmentEnabled = false;
				OnPropertyChanged ("IsNoShowAppointmentEnabled");
				this._isCheckedInAppointmentEnabled = true;
				OnPropertyChanged ("IsCheckedInAppointmentEnabled");
				this._isCheckedOutAppointmentEnabled = false;
				OnPropertyChanged ("IsCheckedOutAppointmentEnabled");
				this._isCancelledAppointmentEnabled = false;
				OnPropertyChanged ("IsCancelledAppointmentEnabled");
				this._isNotNoShowAppointmentEnabled = false;
				OnPropertyChanged ("IsNotNoShowAppointmentEnabled");
				this._isNotCheckedInAppointmentEnabled = false;
				OnPropertyChanged ("IsNotCheckedInAppointmentEnabled");
				this._isNotCheckedOutAppointmentEnabled = true;
				OnPropertyChanged ("IsNotCheckedOutAppointmentEnabled");
				this._isNotCancelledAppointmentEnabled = false;
				OnPropertyChanged ("IsNotCancelledAppointmentEnabled");
				break;
			}
		}

		public void SearchStringChanged (string newSearchString)
		{
			Patient tempPatient = new Patient (this.dataAccessService);
			tempPatient.IsAsync = false;
			tempPatient.GetPatients(newSearchString, (s, args) => {
				lock (args) {
					this.PatientList = args.Result as IList<Patient>;
				}
				OnPropertyChanged ("PatientList");
			});
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

		public int TimeScaleIndex
		{
			get
			{
				return _timeScaleIndex;
			}
			set
			{
				_timeScaleIndex = value;
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
				if (this.patientList != value) {
					this.patientList = value;
					this.OnPropertyChanged ("PatientList");
				}
			}
		}

		public bool IsManagerUser
		{
			get
			{
				return _isManagerUser;
			}
			set
			{
				if (this._isManagerUser != value) {
					this._isManagerUser = value;
					this.OnPropertyChanged ("IsManagerUser");
				}
			}
		}

		internal void BlurPatientSearch (Telerik.Windows.Controls.RadComboBox sender, string patientName)
		{
			UIElement elementWithFocus = Keyboard.FocusedElement as UIElement;
			elementWithFocus.MoveFocus (new TraversalRequest (FocusNavigationDirection.Next));
		}
	}
}
