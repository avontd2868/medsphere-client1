using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using System.Data;

using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Presentation.Commands;
using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Regions;
using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Infrastructure.Events;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Scheduler;
using ClinSchd.Modules.Task.Group;
using ClinSchd.Modules.Task.Scheduler.Controllers;
using Microsoft.Practices.Composite.Presentation.Regions;

namespace ClinSchd.Modules.Task.Scheduler
{
	public class SchedulerPresentationModel : ISchedulerPresentationModel, ITaskPresentationModel, INotifyPropertyChanged
    {
		private readonly IRegionManager regionManager;
		private readonly IDataAccessService dataAccessService;
		private readonly IEventAggregator eventAggregator;
		public event PropertyChangedEventHandler PropertyChanged;
		private string _paneTitle = "";
		private int _dayRange = 1;
		private TimeSpan _timeScale = TimeSpan.FromMinutes(10);
		private bool _isSelectedTab = false;
		private SchdResource selectedResource;
		private SchdResourceGroup selectedResourceGroup;
		private IList<SchdAppointment> appointmentList;
		private DateTime visibleStartDate;
		private DateTime visibleEndDate;
		private bool _isDisplayWalkInEnabled = true;
		private bool _isEnableUpdateAppointment = true;
		private string _resourceName;
		private int _timeSlotToSkip = 15;

		public SchedulerPresentationModel(
			ISchedulerView view,
			IRegionManager regionManager,
			IDataAccessService dataAccessService,
			IEventAggregator eventAggregator)
        {
            View = view;
            View.Model = this;
			this.regionManager = regionManager;
			this.dataAccessService = dataAccessService;
			this.eventAggregator = eventAggregator;

			LoadAppointmentCategories ();
			foreach (KeyValuePair<string, Category> pair in AppointmentCategories) {
				View.Scheduler.Categories.Add (pair.Value);
			}

			CreateAppointmentCommand = new DelegateCommand<string> (ExecuteCreateAppointmentCommand, CanExecuteCreateAppointmentCommand);
			CreateWalkInAppointmentCommand = new DelegateCommand<string> (ExecuteCreateWalkInAppointmentCommand, CanExecuteCreateWalkInAppointmentCommand);
			EditAppointmentCommand = new DelegateCommand<string> (ExecuteEditAppointmentCommand, CanExecuteEditAppointmentCommand);
			ViewAppointmentCommand = new DelegateCommand<string> (ExecuteViewAppointmentCommand, CanExecuteViewAppointmentCommand);
			EnableAppointmentButtonsCommand = new DelegateCommand<string> (ExecuteEnableAppointmentButtonsCommand, CanExecuteEnableAppointmentButtonsCommand);
			ResetUnselectedTabCommand = new DelegateCommand<string> (ExecuteEnableAppointmentButtonsCommand, CanExecuteEnableAppointmentButtonsCommand);
			TaskTabSelectedCommand = new DelegateCommand<object> (ExecuteSchedulerTabSelectedCommand, CanExecuteSchedulerTabSelectedCommand);
			MarkAsNoShowCommand = new DelegateCommand<string> (ExecuteMarkAsNoShowCommand, CanExecuteMarkAsNoShowCommand);
			UndoANoShowCommand = new DelegateCommand<string> (ExecuteUndoANoShowCommand, CanExecuteUndoANoShowCommand);
			CheckInPatientCommand = new DelegateCommand<string> (ExecuteCheckInPatientCommand, CanExecuteCheckInPatientCommand);
			UndoCheckInPatientCommand = new DelegateCommand<string> (ExecuteUndoCheckInPatientCommand, CanExecuteUndoCheckInPatientCommand);
			CheckOutPatientCommand = new DelegateCommand<string> (ExecuteCheckOutPatientCommand, CanExecuteCheckOutPatientCommand);
			UndoCheckOutPatientCommand = new DelegateCommand<string> (ExecuteUndoCheckOutPatientCommand, CanExecuteUndoCheckOutPatientCommand);
			CancelAppointmentCommand = new DelegateCommand<string> (ExecuteCancelAppointmentCommand, CanExecuteCancelAppointmentCommand);
			UndoCancelAppointmentCommand = new DelegateCommand<string> (ExecuteUndoCancelAppointmentCommand, CanExecuteUndoCancelAppointmentCommand);
			CopyAppointmentCommand = new DelegateCommand<string> (ExecuteCopyAppointmentCommand, CanExecuteCopyAppointmentCommand);
			PasteAppointmentCommand = new DelegateCommand<string> (ExecutePasteAppointmentCommand, CanExecutePasteAppointmentCommand);

			this.eventAggregator.GetEvent<ViewDayRangeEvent> ().Subscribe (DayRangeChanged, ThreadOption.UIThread, true);
			this.eventAggregator.GetEvent<ViewTimeScaleEvent> ().Subscribe (TimeScaleChanged, ThreadOption.UIThread, true);
			this.eventAggregator.GetEvent<ChangeViewModeEvent> ().Subscribe (ViewModeChanged, ThreadOption.UIThread, false);
			this.eventAggregator.GetEvent<DisplayWalkInEvent> ().Subscribe (DisplayWalkInChanged, ThreadOption.UIThread, true);
			this.eventAggregator.GetEvent<RefreshScheduler> ().Subscribe (RefreshAppoinmentScheduler, ThreadOption.UIThread, true);
			this.eventAggregator.GetEvent<UndoNoShowAppointmentEvent> ().Subscribe (UndoNoShowAppointment, ThreadOption.UIThread, true);
			this.eventAggregator.GetEvent<UndoCheckInPatientEvent> ().Subscribe (UndoCheckInPatient, ThreadOption.UIThread, true);
			this.eventAggregator.GetEvent<UndoCheckOutPatientEvent> ().Subscribe (UndoCheckOutPatient, ThreadOption.UIThread, true);
			this.eventAggregator.GetEvent<UndoCancelAppointmentEvent> ().Subscribe (UndoCancelAppointment, ThreadOption.UIThread, true);
			this.eventAggregator.GetEvent<PatientSelectedEvent> ().Subscribe (PatientSelected, ThreadOption.UIThread, false);
			this.eventAggregator.GetEvent<SchedulerViewDateEvent> ().Subscribe (SchedulerDateChanged, ThreadOption.UIThread, false);
			this.eventAggregator.GetEvent<CopyAppointmentEvent> ().Subscribe (CopyAppointment, ThreadOption.UIThread, false);
			this.eventAggregator.GetEvent<PasteAppointmentEvent> ().Subscribe (PasteAppointment, ThreadOption.UIThread, false);
			this.eventAggregator.GetEvent<PrintCurrentSchedulerEvent> ().Subscribe (PrintCurrentScheduler, ThreadOption.UIThread, false);
		}

		public Patient Patient { get; set; }
		public bool IsAPatientSelected { get; set; }

		public void SelectPatient (Patient selectedPatient)
		{
			patientSelectedByMe = true;
			this.eventAggregator.GetEvent<PatientSelectedEvent> ().Publish (selectedPatient);
		}

		private Dictionary<string, Category> appointmentCategories = new Dictionary<string, Category> ();

		[Dependency]
		public Factory<Patient> PatientFactory { get; set; }

		[Dependency]
		public Factory<SchdAppointment> SchdAppointmentFactory { get; set; }

		public DelegateCommand<string> CreateAppointmentCommand { get; private set; }
		public DelegateCommand<string> CreateWalkInAppointmentCommand { get; private set; }
		public DelegateCommand<string> EditAppointmentCommand { get; private set; }
		public DelegateCommand<string> ViewAppointmentCommand { get; private set; }
		public DelegateCommand<string> EnableAppointmentButtonsCommand { get; private set; }
		public DelegateCommand<string> ResetUnselectedTabCommand { get; private set;}
		public DelegateCommand<object> TaskTabSelectedCommand { get; private set; }
		public DelegateCommand<string> MarkAsNoShowCommand { get; private set; }
		public DelegateCommand<string> UndoANoShowCommand { get; private set; }
		public DelegateCommand<string> CheckInPatientCommand { get; private set; }
		public DelegateCommand<string> UndoCheckInPatientCommand { get; private set; }
		public DelegateCommand<string> CheckOutPatientCommand { get; private set; }
		public DelegateCommand<string> UndoCheckOutPatientCommand { get; private set; }
		public DelegateCommand<string> CancelAppointmentCommand { get; private set; }
		public DelegateCommand<string> UndoCancelAppointmentCommand { get; private set; }
		public DelegateCommand<string> CopyAppointmentCommand { get; private set; }
		public DelegateCommand<string> PasteAppointmentCommand { get; private set; }

		private void LoadAppointmentCategories ()
		{
			appointmentCategories.Add ("WhiteCategory", new Category ("WhiteCategory", Application.Current.Resources["WhiteCategoryBrush"] as Brush));
			appointmentCategories.Add ("BlueCategory", new Category ("BlueCategory", Application.Current.Resources["BlueCategoryBrush"] as Brush));
			appointmentCategories.Add ("YellowCategory", new Category ("YellowCategory", Application.Current.Resources["YellowCategoryBrush"] as Brush));
			appointmentCategories.Add ("GrayCategory", new Category ("GrayCategory", Application.Current.Resources["GrayCategoryBrush"] as Brush));
			appointmentCategories.Add ("PinkCategory", new Category ("PinkCategory", Application.Current.Resources["PinkCategoryBrush"] as Brush));
			appointmentCategories.Add ("GreenCategory", new Category ("GreenCategory", Application.Current.Resources["GreenCategoryBrush"] as Brush));
			appointmentCategories.Add ("OrangeCategory", new Category ("OrangeCategory", Application.Current.Resources["OrangeCategoryBrush"] as Brush));
		}

		public void LoadAppointments ()
		{
			this.SelectedResource.GetVisibleAppointments (this.VisibleStartDate, this.VisibleEndDate,
				(s, args) => {
					this.AppointmentList = args.Result as List<SchdAppointment>;
					this.View.Scheduler.AppointmentsSource = new SchdAppointmentSource (this.AppointmentList, this.AppointmentCategories, this.IsEnableUpdateAppointment).Appointments;
				});
			
		}

		private bool shouldRefreshScheduler = true;
		public void RefreshAppoinmentScheduler (string command)
		{
			if (shouldRefreshScheduler) {
				this.VisibleStartDate = this.View.Scheduler.VisibleRangeStart;
				this.VisibleEndDate = this.View.Scheduler.VisibleRangeEnd;
				
				this.dataAccessService.RefreshAvailabilitySchedule (SelectedResource.RESOURCE_NAME, this.VisibleStartDate.AddDays (-1), this.VisibleEndDate.AddDays (1));

				((TimeSlotTemplateSelector)View.Scheduler.TimeSlotTemplateSelector).startTime = this.VisibleStartDate;
				((TimeSlotTemplateSelector)View.Scheduler.TimeSlotTemplateSelector).endTime = this.VisibleEndDate;
				((TimeSlotTemplateSelector)View.Scheduler.TimeSlotTemplateSelector).Refresh ();

				this.SelectedResource.GetVisibleAppointments (this.VisibleStartDate, this.VisibleEndDate,
					(s, args) => {
						this.AppointmentList = args.Result as List<SchdAppointment>;
						this.View.Scheduler.AppointmentsSource = new SchdAppointmentSource (this.AppointmentList, this.AppointmentCategories, this.IsEnableUpdateAppointment).Appointments;
						SchedulerViewMode currentMode = View.Scheduler.ViewMode;
						View.Scheduler.ViewMode = SchedulerViewMode.Month;
						View.Scheduler.ViewMode = currentMode;
						if (View.Scheduler.VisibleRangeStart != this.VisibleStartDate) {
							shouldRefreshScheduler = false;
							View.Scheduler.FirstDayOfWeek = this.VisibleStartDate.DayOfWeek;
							View.Scheduler.SetFirstVisibleDate (this.VisibleStartDate);
						}
					});
			} else {
				shouldRefreshScheduler = true;
			}
		}

		private void PrintCurrentScheduler (object T)
		{
			if (this.IsSelectedTab) {
				PrintDialog dialog = new PrintDialog ();
				if (dialog.ShowDialog () == true) { dialog.PrintVisual (View.Scheduler, "Schedule"); }
			}
		}

		public void DisplayWalkInChanged (string isDisplayWalkInEnabled)
		{
			if (IsSelectedTab) {
				this.VisibleStartDate = this.View.Scheduler.VisibleRangeStart;
				this.VisibleEndDate = this.View.Scheduler.VisibleRangeEnd;
				this._isDisplayWalkInEnabled = isDisplayWalkInEnabled == "true" ? true : false;

				this.SelectedResource.GetVisibleAppointments (this.VisibleStartDate, this.VisibleEndDate,
					(s, args) => {
						this.AppointmentList = args.Result as List<SchdAppointment>;
						this.View.Scheduler.AppointmentsSource = new SchdAppointmentSource (this.AppointmentList, this.AppointmentCategories, this.IsEnableUpdateAppointment).Appointments;
					});
			}
		}

		public void CopyAppointment (SchdAppointment copyAppt)
		{
			this.CopySchdAppointment = copyAppt;
		}

		public void PasteAppointment (SchdAppointment pasteAppt)
		{
			if (this.IsSelectedTab && this.CopySchdAppointment != null) {
				AppointmentData a = new AppointmentData ();
				PatientInformation p = new PatientInformation ();
				a.AppointmentID = "0";
				a.StartTime = pasteAppt.START_TIME.ToString ();
				a.Duration = (Convert.ToDateTime (this.CopySchdAppointment.END_TIME.ToString ()) - Convert.ToDateTime (this.CopySchdAppointment.START_TIME.ToString ())).TotalMinutes.ToString ();
				a.EndTime = (Convert.ToDateTime (pasteAppt.START_TIME.ToString ()).AddMinutes (int.Parse (a.Duration))).ToString ();
				a.Notes = this.CopySchdAppointment.NOTE;
				a.ResourceName = this.CopySchdAppointment.RESOURCENAME;

				p.IEN = this.CopySchdAppointment.PATIENTID;
				p.Name = this.CopySchdAppointment.PATIENTNAME;

				string errorMessage = string.Empty;
				List<string> errorMessages = new List<string> ();
				bool bOverbookAppointment = false;
				bool isConflictAppointment = false;

				errorMessage = this.dataAccessService.IsHoliday (Convert.ToDateTime (pasteAppt.START_TIME));

				if (errorMessage == string.Empty || (View.ConfirmUser (Convert.ToDateTime (pasteAppt.START_TIME).ToShortDateString () + " is a holiday (" + errorMessage + ").  Are you sure you want to make this appointment?", "Paste Appointment"))) {
					//check conflict appointment
					isConflictAppointment = this.dataAccessService.CheckConflictAppointment (pasteAppt.RESOURCENAME, p.IEN, pasteAppt.START_TIME, pasteAppt.END_TIME);

					if (isConflictAppointment) {
						View.AlertUser ("Patient " + p.Name + " already had appt at " + pasteAppt.START_TIME + " in clinic " + pasteAppt.RESOURCENAME, "Paste Appointment");
						return;
					}

					//check overbook authority
					bOverbookAppointment = this.dataAccessService.OverBookAuthority (pasteAppt);

					//check slots available
					errorMessage = this.dataAccessService.CheckSlotsAvailable (pasteAppt.START_TIME, pasteAppt.END_TIME, pasteAppt.RESOURCENAME);

					if (errorMessage != string.Empty) {
						if (bOverbookAppointment) {
							if (View.ConfirmUser (errorMessage, "Paste Appointment")) {

								errorMessage = string.Empty;
								if (Convert.ToDateTime (pasteAppt.START_TIME).Date > DateTime.Now.Date) {
									errorMessage = this.dataAccessService.NoShow (p.IEN, pasteAppt.RESOURCENAME, "AddNew");
								}

								if (errorMessage != string.Empty) {
									View.AlertUser ("Unable to make a new appointment.\nPatient: " +
										p.Name + errorMessage, "Patient No-Shows");
									return;
								}
								errorMessages = this.dataAccessService.AddNewAppointment (a, p, false);
							}
						} else {
							View.AlertUser ("You do not have permission to overbook this appointment.", "New Appointment");
							return;
						}
					} else {
						errorMessages = this.dataAccessService.AddNewAppointment (a, p, false);
					}
				}
				if (errorMessages.Count > 0) {
					errorMessage = errorMessages[1];
				}
				if (errorMessage != string.Empty) {
					this.View.AlertUser (errorMessage, "Copy/Paste Appointment");
				}
				this.eventAggregator.GetEvent<RefreshScheduler> ().Publish ("Edit Appointment");
			}
		}

		public void UndoCancelAppointment (string apptID)
		{
			string errorMessage = string.Empty;
			errorMessage = this.dataAccessService.UndoCancelAppointment (this.SchdAppointment.APPOINTMENTID);

			if (errorMessage != string.Empty) {
				this.View.AlertUser (errorMessage, "Undo Cancel Appointment");
			} else {
				RefreshAppoinmentScheduler (string.Empty);
			}
		}

		public void UndoNoShowAppointment (string apptID)
		{
			string errorMessage = string.Empty;
			errorMessage = this.dataAccessService.AppointmentNoShow (this.SchdAppointment.APPOINTMENTID, false);

			if (errorMessage != string.Empty) {
				this.View.AlertUser (errorMessage, "Undo NoShow Appointment");
			} else {
				RefreshAppoinmentScheduler (string.Empty);
			}
		}

		public void UndoCheckInPatient (string apptID)
		{
			string errorMessage = string.Empty;
			errorMessage = this.dataAccessService.UndoCheckIn (this.SchdAppointment.APPOINTMENTID);

			if (errorMessage != string.Empty) {
				this.View.AlertUser (errorMessage, "Undo CheckIn Patient");
			} else {
				RefreshAppoinmentScheduler (string.Empty);
			}
		}

		public void UndoCheckOutPatient (string apptID)
		{
			string errorMessage = string.Empty;
			errorMessage = this.dataAccessService.UndoCheckOut (this.SchdAppointment.APPOINTMENTID);

			if (errorMessage != string.Empty) {
				this.View.AlertUser (errorMessage, "Undo CheckOut Patient");
			} else {
				RefreshAppoinmentScheduler (string.Empty);
			}
		}

		private void SchedulerDateChanged (DateTime newDate)
		{
			if (IsSelectedTab) {
				((TimeSlotTemplateSelector)View.Scheduler.TimeSlotTemplateSelector).startTime = newDate.AddDays (1);
				((TimeSlotTemplateSelector)View.Scheduler.TimeSlotTemplateSelector).endTime = newDate.AddDays (View.Scheduler.WeekViewDefinition.VisibleDays + 1);
				((TimeSlotTemplateSelector)View.Scheduler.TimeSlotTemplateSelector).Refresh ();
				this.View.Scheduler.FirstDayOfWeek = newDate.DayOfWeek;
				this.View.Scheduler.SetFirstVisibleDate (newDate);
			}
		}

		public void DayRangeChanged (string dayRange)
		{
			_dayRange = int.Parse (dayRange);

			if (IsSelectedTab) {
				this.View.Scheduler.WeekViewDefinition.VisibleDays = _dayRange;
			}
		}

		public void ViewModeChanged (string viewMode)
		{
			if (IsSelectedTab) {
				switch (viewMode) {
				case "Day":
					this.View.Scheduler.ViewMode = SchedulerViewMode.Day;
					this.View.Scheduler.SetFirstVisibleDate (this.VisibleStartDate);
					break;
				case "Month":
					this.View.Scheduler.ViewMode = SchedulerViewMode.Month;
					this.View.Scheduler.SetFirstVisibleDate (this.VisibleStartDate);
					break;
				default:
					this.View.Scheduler.ViewMode = SchedulerViewMode.Week;
					this.View.Scheduler.SetFirstVisibleDate (this.VisibleStartDate);
					break;
				}
			}
		}

		public void TimeScaleChanged (string timeScale)
		{
			switch (timeScale) {
			case "10":
				_timeScale = System.TimeSpan.FromMinutes (10);
				_timeSlotToSkip = 46;
				break;
			case "15":
				_timeScale = System.TimeSpan.FromMinutes (15);
				_timeSlotToSkip = 30;
				break;
			case "20":
				_timeScale = System.TimeSpan.FromMinutes (20);
				_timeSlotToSkip = 23;
				break;
			case "30":
				_timeScale = System.TimeSpan.FromMinutes (30);
				_timeSlotToSkip = 15;
				break;
			default:
				_timeScale = System.TimeSpan.FromMinutes (30);
				_timeSlotToSkip = 15;
				break;
			}
			if (IsSelectedTab) {
				this.View.Scheduler.WeekViewDefinition.TimeSlotLength = _timeScale;
				this.View.Scheduler.DayViewDefinition.TimeSlotLength = _timeScale;
			}
		}

		public void ExecuteCopyAppointmentCommand (string command)
		{
			this.eventAggregator.GetEvent<CopyAppointmentEvent> ().Publish (this.SchdAppointment);
		}

		public bool CanExecuteCopyAppointmentCommand (string command)
		{
			return true;
		}

		public void ExecutePasteAppointmentCommand (string command)
		{
			this.eventAggregator.GetEvent<IsPasteAvailabilityEvent> ().Publish (command);
		}

		public bool CanExecutePasteAppointmentCommand (string command)
		{
			return true;
		}

		public void ExecuteSchedulerTabSelectedCommand (object resource)
		{
			SelectedResource = resource as SchdResource;
			this.eventAggregator.GetEvent<ResourceSelectedEvent> ().Publish (SelectedResource);

			int selectedMin = 0;
			int selectedIndex = 3;
			if (IsSelectedTab) {
				switch (this.View.Scheduler.ViewMode)
				{
					case SchedulerViewMode.Week:
					selectedMin = this.View.Scheduler.WeekViewDefinition.TimeSlotLength.Minutes;
					break;
					case SchedulerViewMode.Day:
					selectedMin = this.View.Scheduler.DayViewDefinition.TimeSlotLength.Minutes;
					break;
					default:
					selectedMin = 30;
					break;
				}

				switch (selectedMin) {
					case 10:
					selectedIndex = 0;
					break;
					case 15:
					selectedIndex = 1;
					break;
					case 20:
					selectedIndex = 2;
					break;
					case 30:
					selectedIndex = 3;
					break;
				}
				this.eventAggregator.GetEvent<SelectTimeScaleEvent> ().Publish (selectedIndex);
			}
		}

		public void ExecuteEnableAppointmentButtonsCommand (string command)
		{
			this.eventAggregator.GetEvent<PassAppointmentsEvent> ().Publish (this.SchdAppointment);

			string errorMessage = string.Empty;
			if (command == "New Appointment") {
				//check access block
				errorMessage = this.dataAccessService.CheckSlotsAvailable (this.SchdAppointment.START_TIME, this.SchdAppointment.END_TIME, this.SchdAppointment.RESOURCENAME);

				if (errorMessage != string.Empty && errorMessage == "IsPreventAccess") {
					IsAPatientSelected = false;
					OnPropertyChanged ("IsAPatientSelected");
					command = "Reset Buttons";
				} else {
					if (Patient != null) {
						IsAPatientSelected = true;
						OnPropertyChanged ("IsAPatientSelected");
					} else {
						IsAPatientSelected = false;
						OnPropertyChanged ("IsAPatientSelected");
					}
				}
			}

			this.eventAggregator.GetEvent<EnableAppointmentButtonsEvent> ().Publish (command);

		}

		private bool patientSelectedByMe;
		public void PatientSelected (Patient selectedPatient)
		{
			if (selectedPatient != null) {
				if (!patientSelectedByMe) {
					Patient = selectedPatient;
					OnPropertyChanged ("Patient");
				} else {
					patientSelectedByMe = false;
				}
				IsAPatientSelected = true;
				OnPropertyChanged ("IsAPatientSelected");
			} else {
				IsAPatientSelected = false;
				OnPropertyChanged ("IsAPatientSelected");
			}
		}

		public void ExecuteEditAppointmentCommand (string command)
		{
			this.eventAggregator.GetEvent<EditAppointmentEvent> ().Publish (this.SchdAppointment);
		}

		public void ExecuteViewAppointmentCommand (string command)
		{
			this.eventAggregator.GetEvent<ViewAppointmentEvent> ().Publish (this.SchdAppointment);
		}

		public void ExecuteCreateAppointmentCommand (string title)
		{
			this.SchdAppointment.PATIENTID = this.Patient.IEN;
			this.SchdAppointment.PATIENTNAME = this.Patient.Name;
			this.eventAggregator.GetEvent<AddNewAppointmentEvent> ().Publish (this.SchdAppointment);
		}

		public void ExecuteCreateWalkInAppointmentCommand (string title)
		{
			this.SchdAppointment.PATIENTID = this.Patient.IEN;
			this.SchdAppointment.PATIENTNAME = this.Patient.Name;
			this.eventAggregator.GetEvent<AddWalkInAppointmentEvent> ().Publish (this.SchdAppointment);
		}

		public void ExecuteMarkAsNoShowCommand (string title)
		{
			this.eventAggregator.GetEvent<LaunchMarkAsNoShowDialogEvent> ().Publish (this.SchdAppointment);
		}

		public void ExecuteUndoANoShowCommand (string title)
		{
			UndoNoShowAppointment (this.SchdAppointment.APPOINTMENTID);
		}

		public void ExecuteCheckInPatientCommand (string command)
		{
			this.eventAggregator.GetEvent<LaunchCheckInDialogEvent> ().Publish (this.SchdAppointment);
		}

		public void ExecuteUndoCheckInPatientCommand (string command)
		{
			UndoCheckInPatient (this.SchdAppointment.APPOINTMENTID);
		}

		public void ExecuteCheckOutPatientCommand (string command)
		{
			this.eventAggregator.GetEvent<LaunchCheckOutDialogEvent> ().Publish (this.SchdAppointment);
		}

		public void ExecuteUndoCheckOutPatientCommand (string command)
		{
			UndoCheckOutPatient (this.SchdAppointment.APPOINTMENTID);
		}

		public void ExecuteCancelAppointmentCommand (string command)
		{
			this.eventAggregator.GetEvent<LaunchCancelApptDialogEvent> ().Publish (this.SchdAppointment);
		}

		public void ExecuteUndoCancelAppointmentCommand (string command)
		{
			UndoCancelAppointment (this.SchdAppointment.APPOINTMENTID);
		}

		public bool CanExecuteSchedulerTabSelectedCommand (object resource)
		{
			return true;
		}

		public bool CanExecuteEnableAppointmentButtonsCommand (string command)
		{
			return true;
		}

		public bool CanExecuteViewAppointmentCommand (string title)
		{
			return true;
		}

		public bool CanExecuteEditAppointmentCommand (string title)
		{
			return true;
		}

		public bool CanExecuteCreateAppointmentCommand (string title)
		{
			return true;
		}

		public bool CanExecuteCreateWalkInAppointmentCommand (string title)
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
			return true;
		}

		public bool CanExecuteUndoCancelAppointmentCommand (string title)
		{
			return true;
		}

		#region ISchedulerPresentationModel Members

		public ISchedulerView View { get; private set; }

		public int TimeSlotToSkip
		{
			get { return _timeSlotToSkip; }
			set { _timeSlotToSkip = value; }
		}

		public bool IsDisplayWalkInEnabled
		{
			get { return _isDisplayWalkInEnabled; }
			set { _isDisplayWalkInEnabled = value; }
		}

		public bool IsEnableUpdateAppointment
		{
			get
			{
				return _isEnableUpdateAppointment;
			}
			set
			{
				if (this._isEnableUpdateAppointment != value) {
					this._isEnableUpdateAppointment = value;
					this.OnPropertyChanged ("IsEnableUpdateAppointment");
				}
			}
		}

		public Dictionary<string, Category> AppointmentCategories
		{
			get
			{
				return appointmentCategories;
			}
		}

		public IList<SchdAppointment> AppointmentList
		{
			get
			{
				return appointmentList;
			}
			set
			{
				if (this.appointmentList != value) {
					this.appointmentList = value;
					//this.OnPropertyChanged ("AppointmentList");
				}
			}
		}

		public SchdAppointment SchdAppointment { get; set; }
		public SchdAppointment CopySchdAppointment { get; set; }

		public DateTime VisibleStartDate
		{
			get
			{
				return visibleStartDate;
			}
			set
			{
				visibleStartDate = value;
			}
		}

		public DateTime VisibleEndDate
		{
			get
			{
				return visibleEndDate;
			}
			set
			{
				visibleEndDate = value;
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

		public string ResourceName
		{
			get
			{
				return _resourceName;
			}
			set
			{
				_resourceName = value;
			}
		}

		public int DayRange
		{
			get
			{
				return _dayRange;
			}
			set
			{
				_dayRange = value;
			}
		}

		public TimeSpan TimeScale
		{
			get
			{
				return _timeScale;
			}
			set
			{
				_timeScale = value;
			}
		}

		public bool IsSelectedTab
		{
			get
			{
				return _isSelectedTab;
			}
			set
			{
				_isSelectedTab = value;
				OnPropertyChanged ("IsSelectedTab");
			}
		}

		public SchdResource SelectedResource
		{
			get
			{
				return selectedResource;
			}
			set
			{
				if (this.selectedResource != value && value != null) {
					this.selectedResource = value;
				}

				//check user's authority for updating the appointments
				if (!this.dataAccessService.IsEnableUpdateAppointment (this.selectedResource.RESOURCEID)) {
					this.View.Scheduler.IsReadOnly = true;
					this.View.Model.IsEnableUpdateAppointment = false;
				}
			}
		}

		public SchdResourceGroup SelectedResourceGroup
		{
			get
			{
				return selectedResourceGroup;
			}
			set
			{
				if (this.selectedResourceGroup != value && value != null) {
					this.selectedResourceGroup = value;
				}
			}
		}

		public ITaskSchedulerController Controller { get; set; }

		public void ClearViewFromList ()
		{
			if (Controller != null) {
				Controller.RemoveViewFromList (View);
			}
		}
		#endregion

		public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

		internal void ResizeAppointment (IAppointment newAppointment)
		{
			string errorMessage = string.Empty;
			string apptnote = string.Empty;
			string apptid = string.Empty;
			string resourceid = string.Empty;
			string resoourcename = string.Empty;
			AppointmentData a = new AppointmentData ();
			foreach (Resource item in newAppointment.Resources) {
				switch (item.ResourceType) {
				case "NOTE":
					apptnote = item.ResourceName;
					break;
				case "RESOURCENAME":
					resoourcename = item.ResourceName;
					break;
				case "APPOINTMENTID":
					apptid = item.ResourceName;
					break;
				default:
					break;
				}
			}
			a.Notes = apptnote;
			a.Duration = (newAppointment.End - newAppointment.Start).TotalMinutes.ToString ();
			a.AppointmentID = apptid;

			
			DataTable rv = this.dataAccessService.GlobalDataSet.Tables["Resources"];
			foreach (DataRow r in rv.Rows) {
				if (resoourcename == r["RESOURCE_NAME"].ToString ()) {
					resourceid = r["HOSPITAL_LOCATION_ID"].ToString ();
				}
			}

			if (!(this.dataAccessService.GetVariableAppointmentsFlag (resourceid))) {
				View.AlertUser ("Appointment could not be modified:" + Environment.NewLine + Environment.NewLine +
				"Variable Length Appointments are not enabled for this clinic in RPMS Scheduling Management.", "Error");
			} else {
				errorMessage = this.dataAccessService.EditAppointment (a);

				if (errorMessage != string.Empty) {
					View.AlertUser ("Appointment could not be modified:" + Environment.NewLine + Environment.NewLine + errorMessage, "Error");
				}				
			}
			this.eventAggregator.GetEvent<RefreshScheduler> ().Publish ("Edit Appointment");
		}

		internal bool MoveAppointment (IAppointment newAppointment)
		{			
			string errorMessage = string.Empty;
			string apptid = string.Empty;
			string apptnote = string.Empty;
			string apptresourcename = string.Empty;
			string patientien = string.Empty;
			string patientname = string.Empty;
			foreach (Resource item in newAppointment.Resources) {
				switch (item.ResourceType) {
				case "APPOINTMENTID":
					apptid = item.ResourceName;
					break;
				case "NOTE":
					apptnote = item.ResourceName;
					break;
				case "RESOURCENAME":
					apptresourcename = item.ResourceName;
					break;
				case "PATIENTID":
					patientien = item.ResourceName;
					break;
				case "PATIENTNAME":
					patientname = item.ResourceName;
					break;
				default:
					break;
				}
			}
			AppointmentData a = new AppointmentData ();
			PatientInformation p = new PatientInformation ();
			SchdAppointment Appt = new SchdAppointment ();
			a.AppointmentID = "0";
			a.StartTime = newAppointment.Start.ToString();
			a.EndTime = newAppointment.End.ToString();
			a.Duration = (newAppointment.End - newAppointment.Start).TotalMinutes.ToString ();
			a.Notes = apptnote;
			a.ResourceName = apptresourcename;
			Appt.RESOURCENAME = a.ResourceName;
			Appt.START_TIME = a.StartTime;
			Appt.END_TIME = a.EndTime;

			p.IEN = patientien;
			p.Name = patientname;


			bool bOverbookAppointment = false;
			bool isConflictAppointment = false;
			List<string> errorMessages = new List<string> ();
			errorMessage = this.dataAccessService.IsHoliday (Convert.ToDateTime (a.StartTime));

			if (errorMessage == string.Empty || (View.ConfirmUser (Convert.ToDateTime (a.StartTime).ToShortDateString () + " is a holiday (" + errorMessage + ").  Are you sure you want to make this appointment?", "Paste Appointment"))) {
				//check conflict appointment
				isConflictAppointment = this.dataAccessService.CheckConflictAppointment (a.ResourceName, p.IEN, a.StartTime, a.EndTime);

				if (isConflictAppointment) {
					View.AlertUser ("Patient " + p.Name + " already had appt at " + a.StartTime + " in clinic " + a.ResourceName, "Move Appointment");
					return false;
				}

				//check overbook authority
				bOverbookAppointment = this.dataAccessService.OverBookAuthority (Appt);

				//check slots available
				errorMessage = this.dataAccessService.CheckSlotsAvailable (a.StartTime, a.EndTime, a.ResourceName);

				CancelAppointment cancelAppointment = new CancelAppointment ();
				cancelAppointment.ApptID = apptid;
				cancelAppointment.CancelledByClinic = true;
				cancelAppointment.Notes = "Moved By Clinic";
				if (errorMessage != string.Empty) {
					if (bOverbookAppointment) {
						if (View.ConfirmUser (errorMessage, "Move Appointment")) {

							errorMessage = string.Empty;
							if (Convert.ToDateTime (a.StartTime).Date > DateTime.Now.Date) {
								errorMessage = this.dataAccessService.NoShow (p.IEN, a.ResourceName, "AddNew");
							}

							if (errorMessage != string.Empty) {
								View.AlertUser ("Unable to make a new appointment.\nPatient: " +
									p.Name + errorMessage, "Patient No-Shows");
								return false;
							}
							cancelAppointment.ReasonIEN = this.dataAccessService.GetCancellationReasons ()[0].Value;

							errorMessage = this.dataAccessService.CancelAppointment (cancelAppointment);
							if (errorMessage == string.Empty) {
								errorMessages = this.dataAccessService.AddNewAppointment (a, p, false);
								errorMessage = errorMessages[1].ToString ();
							}
						}
					} else {
						View.AlertUser ("You do not have permission to overbook this appointment.", "Move Appointment");
						return false;
					}
				}
			} else {
				return false;
			}

			if (errorMessage != string.Empty) {
				View.AlertUser ("Appointment could not be modified:" + Environment.NewLine + Environment.NewLine + errorMessage, "Error");
				return false;
			}
			this.eventAggregator.GetEvent<RefreshScheduler> ().Publish ("Edit Appointment");
			return true;
		}
	}
}
