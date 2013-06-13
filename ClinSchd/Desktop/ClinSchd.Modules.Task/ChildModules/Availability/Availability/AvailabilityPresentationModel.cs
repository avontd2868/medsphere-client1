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

using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Presentation.Commands;
using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Regions;
using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Events;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Models;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Scheduler;
using ClinSchd.Modules.Task.Group;
using ClinSchd.Modules.Task.Availability.Controllers;
using Microsoft.Practices.Composite.Presentation.Regions;
using System.IO;
using Microsoft.Win32;
using System.Runtime.Serialization.Formatters.Binary;
using System.Data;

using IndianHealthService.ClinicalScheduling;
using System.Collections;

namespace ClinSchd.Modules.Task.Availability
{
	public class AvailabilityPresentationModel : IAvailabilityPresentationModel, ITaskPresentationModel, INotifyPropertyChanged
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
		private IList<SchdAvailability> availabilityList;
		private SchdAvailability schdAvailability;
		private DateTime visibleStartDate;
		private DateTime visibleEndDate;
		private string _resourceName;
		private ValidationMessage validationMessage;
		private int _timeSlotToSkip = 15;
		private bool _isEnableUpdateAppointment = true;

		public AvailabilityPresentationModel (
			IAvailabilityView view,
			IRegionManager regionManager,
			IDataAccessService dataAccessService,
			IEventAggregator eventAggregator)
        {
            View = view;
            View.Model = this;
			this.regionManager = regionManager;
			this.dataAccessService = dataAccessService;
			this.eventAggregator = eventAggregator;
			this.ValidationMessage = new ValidationMessage ();
			this.SelectedStartDate = DateTime.Today;
			InitialErrorMessage ();

			TaskTabSelectedCommand = new DelegateCommand<object> (ExecuteSchedulerTabSelectedCommand, CanExecuteSchedulerTabSelectedCommand);
			AddEditAvailabilityCommand = new DelegateCommand<string> (ExecuteAddEditAvailabilityCommand, CanExecuteAddEditAvailabilityCommand);
			ResetUnselectedTabCommand = new DelegateCommand<string> (ExecuteResetUnselectedTabCommand, CanExecuteResetUnselectedTabCommand);
			PasteAvailabilityCommand = new DelegateCommand<string> (ExecutePasteAvailabilityCommand, CanExecutePasteAvailabilityCommand);
			CopyAvailabilityCommand = new DelegateCommand<string> (ExecuteCopyAvailabilityCommand, CanExecuteCopyAvailabilityCommand);
			SaveTemplateCommand = new DelegateCommand<string> (ExecuteSaveTemplateCommand, CanExecuteSaveTemplateCommand);
			ApplyTemplateCommand = new DelegateCommand<string> (ExecuteApplyTemplateCommand, CanExecuteApplyTemplateCommand);


			this.eventAggregator.GetEvent<ViewDayRangeEvent> ().Subscribe (DayRangeChanged, ThreadOption.UIThread, true);
			this.eventAggregator.GetEvent<ViewTimeScaleEvent> ().Subscribe (TimeScaleChanged, ThreadOption.UIThread, true);
			this.eventAggregator.GetEvent<ChangeViewModeEvent> ().Subscribe (ViewModeChanged, ThreadOption.UIThread, false);
			this.eventAggregator.GetEvent<SchedulerViewDateEvent> ().Subscribe (SchedulerDateChanged, ThreadOption.UIThread, false);
			this.eventAggregator.GetEvent<RefreshAvailability> ().Subscribe (RefreshAvailabilityScheduler, ThreadOption.UIThread, true);
			this.eventAggregator.GetEvent<ResourceSelectedEvent> ().Subscribe (ResourceSelected, ThreadOption.UIThread, false);
			this.eventAggregator.GetEvent<ResourceGroupSelectedEvent> ().Subscribe (ResourceGroupSelected, ThreadOption.UIThread, false);
			this.eventAggregator.GetEvent<PasteAvailabilityEvent> ().Subscribe (PasteAppointment, ThreadOption.UIThread, false);
			this.eventAggregator.GetEvent<CopyAvailabilityEvent> ().Subscribe (CopyAppointment, ThreadOption.UIThread, false);
		}

		public Patient Patient { get; set; }

		public IList<SchdAppointment> AppointmentList { get; set; }

		[Dependency]
		public Factory<SchdAvailability> SchdAvailabilityFactory { get; set; }

		[Dependency]
		public Factory<SchdAppointment> SchdAppointmentFactory { get; set; }
		
		public DelegateCommand<string> ResetUnselectedTabCommand { get; private set; }
		public DelegateCommand<object> TaskTabSelectedCommand { get; private set; }
		public DelegateCommand<string> AddEditAvailabilityCommand { get; private set; }
		public DelegateCommand<string> PasteAvailabilityCommand { get; private set; }
		public DelegateCommand<string> CopyAvailabilityCommand { get; private set; }
		public DelegateCommand<string> SaveTemplateCommand { get; private set; }
		public DelegateCommand<string> ApplyTemplateCommand { get; private set; }

		public void LoadAppointments ()
		{
			this.SelectedResource.GetVisibleAvailabilities (this.VisibleStartDate, this.VisibleEndDate,
				(s, args) => {
					this.AvailabilityList = args.Result as List<SchdAvailability>;
					this.View.Scheduler.AppointmentsSource = new SchdAppointmentSource (this.AvailabilityList, this.IsEnableUpdateAppointment).Appointments;
				});
		}

		public void InitialErrorMessage ()
		{
			this.validationMessage.IsValid = true;
			this.validationMessage.Title = string.Empty;
			this.validationMessage.Message = string.Empty;
		}

		public void RefreshAvailabilityScheduler (string command)
		{
			this.VisibleStartDate = this.View.Scheduler.VisibleRangeStart;
			this.VisibleEndDate = this.View.Scheduler.VisibleRangeEnd;

			this.dataAccessService.RefreshAvailabilitySchedule (SelectedResource.RESOURCE_NAME, this.VisibleStartDate.AddDays (-1), this.VisibleEndDate.AddDays (1));

			this.SelectedResource.GetVisibleAvailabilities (this.VisibleStartDate, this.VisibleEndDate,
			(s, args) => {
				this.AvailabilityList = args.Result as List<SchdAvailability>;
				this.View.Scheduler.AppointmentsSource = new SchdAppointmentSource (this.AvailabilityList, this.IsEnableUpdateAppointment).Appointments;
			});
		}

		public void CopyAppointment (SchdAvailability copyAppt)
		{
			this.CopySchdAvailability = copyAppt;
		}

		public void PasteAppointment (SchdAppointment pasteAppt)
		{
			if (this.IsSelectedTab) {
				SchdAvailability a = new SchdAvailability ();
				a.APPOINTMENTID = null;
				a.StartTime = pasteAppt.START_TIME.ToString ();
				string Duration = (Convert.ToDateTime (this.CopySchdAvailability.EndTime.ToString ()) - Convert.ToDateTime (this.CopySchdAvailability.StartTime.ToString ())).TotalMinutes.ToString ();
				a.EndTime = (Convert.ToDateTime (pasteAppt.START_TIME.ToString ()).AddMinutes (int.Parse (Duration))).ToString ();
				a.ACCESSTYPEID = this.CopySchdAvailability.ACCESSTYPEID;
				a.Note = this.CopySchdAvailability.Note;
				a.RESOURCENAME = this.CopySchdAvailability.RESOURCENAME;
				a.SLOTS = this.CopySchdAvailability.SLOTS;

				string errorMessage = string.Empty;
				errorMessage = this.dataAccessService.AddNewAvailability (a);

				if (errorMessage != string.Empty) {
					View.AlertUser (errorMessage, "Copy/Paste Availability");
					a.StartTime = this.SchdAvailability.StartTime;
					a.EndTime = this.SchdAvailability.EndTime;
					errorMessage = this.dataAccessService.AddNewAvailability (a);
				}
				RefreshAvailabilityScheduler (string.Empty);
			}
		}

		public void ResourceSelected (SchdResource selectedResource)
		{
			this.Resource = selectedResource;
			List<SchdResource> newGroupList = new List<SchdResource> ();
			newGroupList.Insert (0, this.Resource);
			this.SelectedResourceList = newGroupList;
		}

		public void ResourceGroupSelected (SchdResourceGroup selectedResourceGroup)
		{
			this.ResourceGroup = selectedResourceGroup;
			this.SelectedResourceList = this.ResourceGroup.Resources;
		}

		private void SchedulerDateChanged (DateTime newDate)
		{
			if (IsSelectedTab) {
				this.View.Scheduler.SetFirstVisibleDate (newDate);
				this.RefreshAvailabilityScheduler (string.Empty);
			}
		}

		public void DayRangeChanged (string dayRange)
		{
			_dayRange = int.Parse (dayRange);

			if (IsSelectedTab) {
				this.View.Scheduler.WeekViewDefinition.VisibleDays = _dayRange;
			}

			//OnPropertyChanged ("DayRange");
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

			//OnPropertyChanged ("TimeScale");
		}

		public void ExecuteCopyAvailabilityCommand (string command)
		{
			this.eventAggregator.GetEvent<EnableAppointmentButtonsEvent> ().Publish (command);
			this.eventAggregator.GetEvent<CopyAvailabilityEvent> ().Publish (this.SchdAvailability);
		}

		public bool CanExecuteCopyAvailabilityCommand (string command)
		{
			return true;
		}

		public void ExecutePasteAvailabilityCommand (string command)
		{
			this.eventAggregator.GetEvent<EnableAppointmentButtonsEvent> ().Publish (command);
			this.eventAggregator.GetEvent<PassAppointmentsEvent> ().Publish (this.SelectedAppointment);
			this.eventAggregator.GetEvent<IsPasteAvailabilityEvent> ().Publish (command);
		}

		public bool CanExecutePasteAvailabilityCommand (string command)
		{
			return true;
		}

		public void ExecuteSchedulerTabSelectedCommand (object resource)
		{
			selectedResource = resource as SchdResource;
			this.eventAggregator.GetEvent<ResourceSelectedEvent> ().Publish (selectedResource);

			int selectedMin = 0;
			int selectedIndex = 3;
			if (IsSelectedTab) {
				switch (this.View.Scheduler.ViewMode) {
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

		public bool CanExecuteSchedulerTabSelectedCommand (object resource)
		{
			return true;
		}

		public void ExecuteAddEditAvailabilityCommand (string command)
		{
			this.eventAggregator.GetEvent<AddEditAvailabilityEvent> ().Publish (this.SchdAvailability);
		}

		public bool CanExecuteAddEditAvailabilityCommand (object resource)
		{
			return true;
		}

		public void ExecuteResetUnselectedTabCommand (string command)
		{
			this.eventAggregator.GetEvent<EnableAppointmentButtonsEvent> ().Publish (command);
		}

		public bool CanExecuteResetUnselectedTabCommand (string command)
		{
			return true;
		}

		private void ExecuteSaveTemplateCommand (string command)
		{
			try
			{
				Stream streamFile ;
				SaveFileDialog saveFileDialog1 = new SaveFileDialog();
 
				saveFileDialog1.Filter = "Schedule Template Files (*.bsdxa*)|*.bsdxa*|All files (*.*)|*.*" ;
				saveFileDialog1.FilterIndex = 0 ;
				saveFileDialog1.RestoreDirectory = true ;
				saveFileDialog1.AddExtension = true;
				saveFileDialog1.DefaultExt = "bsdxa4";

				foreach (SchdAvailability item in this.AvailabilityList) {
					item.GridColumn = DateTime.Parse (item.StartTime).Subtract (VisibleStartDate).Days;
				}

				if (saveFileDialog1.ShowDialog ().Value) {
					if ((streamFile = saveFileDialog1.OpenFile ()) != null) {
						BinaryFormatter formatter = new BinaryFormatter ();
						
						formatter.TypeFormat = System.Runtime.Serialization.Formatters.FormatterTypeStyle.TypesAlways;
						formatter.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;

						formatter.Serialize (streamFile, this.AvailabilityList);

						streamFile.Close ();
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error saving template:  " + ex.Message, "Availablility");
			}
		}

		private bool CanExecuteSaveTemplateCommand (string command)
		{
			return true;
		}

		private void ExecuteApplyTemplateCommand (string command)
		{
	        /*
	         * Display dialog to collect:
	         * - Number of weeks to apply template
	         * - Starting week Monday
	         * - Template path & filename
	         * 
	         * for each week,
	         *	Delete all availability during that week
	         *  apply the template
	         * 
	         */
			//DAccessTemplate dlg = new DAccessTemplate ();
			//dlg.InitializePage ();
			//if (dlg.ShowDialog (this) == DialogResult.Cancel) {
			//    return;
			//}

			try {
				//OpenFileDialog openFileDialog1 = dlg.FileDialog
				OpenFileDialog openFileDialog1 = new OpenFileDialog ();
				openFileDialog1.Filter = "Schedule Template Files (*.bsdxa*)|*.bsdxa*|All files (*.*)|*.*";
				openFileDialog1.FilterIndex = 0;
				openFileDialog1.RestoreDirectory = true;
				openFileDialog1.AddExtension = true;
				openFileDialog1.DefaultExt = "bsdxa";
				if (!openFileDialog1.ShowDialog ().Value) {
					return;
				}
				ApplyTemplateView applyTemplateView = new ApplyTemplateView ();
				applyTemplateView.DataContext = this;
				if (!applyTemplateView.ShowDialog ().Value) {
					return;
				}
				Stream streamFile;
				if ((streamFile = openFileDialog1.OpenFile ()) == null) {
					MessageBox.Show ("Unable to open template file.");
					return;
				}

				IList<SchdAvailability> cgaTemp = new List<SchdAvailability> ();

				BinaryFormatter formatter = new BinaryFormatter ();
				
				try {
					cgaTemp = (IList<SchdAvailability>)formatter.Deserialize (streamFile);
				} catch (InvalidCastException) {
					formatter.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
					streamFile.Position = 0;
					CGAppointments cgaConvertTemp = (CGAppointments)formatter.Deserialize (streamFile);
					foreach (DictionaryEntry item in cgaConvertTemp) {
						CGAppointment apptItem = item.Value as CGAppointment;
						cgaTemp.Add (new SchdAvailability () { 
							StartTime = apptItem.StartTime.ToString(),
							EndTime = apptItem.EndTime.ToString(),
							 ACCESSTYPEID = apptItem.AccessTypeID.ToString(),
							 ACCESSTYPENAME = apptItem.AccessTypeName,
							 APPOINTMENTID = apptItem.AppointmentKey.ToString(),
							 RESOURCENAME = apptItem.Resource,
							 SLOTS = apptItem.Slots});
					}
				}

				streamFile.Close ();

				DateTime dtStart = VisibleStartDate;
				if (SelectedStartDate.DayOfWeek != DayOfWeek.Monday) {
					for (int i = 1; i < 7; i++) {
						if (SelectedStartDate.AddDays (-1 * i).DayOfWeek == DayOfWeek.Monday) {
							dtStart = SelectedStartDate.AddDays (-1 * i);
							break;
						}
					}
				}
				int nWeeksToApply = Convert.ToInt32(SelectedNumberOfWeeks);
				DateTime dtEnd = dtStart.AddDays (6); // or 7?
				string sResourceID = SelectedResource.RESOURCEID;

				for (int j = 1; j < nWeeksToApply + 1; j++) {
					//Convert start and end to string
					string sStart = dtStart.ToString ("M/d/yyyy");
					string sEnd = dtEnd.ToString ("M/d/yyyy");

					//Cancel all existing access blocks in the date range
					this.dataAccessService.CancelAccessBlocks (sResourceID, SelectedResource.RESOURCE_NAME, sStart, sEnd);

					//for each CGAppointment in AVBlocks, call AddNew
					string sResource = "";
					sResource = selectedResource.RESOURCE_NAME;
					foreach (SchdAvailability a in cgaTemp) {
						//Change the resource to the current one
						a.RESOURCENAME = sResource;

						//Change the date to correspond to the GridColumn member
						int col = a.GridColumn;
						//col--;
						DateTime dBuildDate = dtStart.Date;
						dBuildDate = dBuildDate.AddDays (col);
						dBuildDate = dBuildDate.AddHours (DateTime.Parse(a.StartTime).Hour);
						dBuildDate = dBuildDate.AddMinutes (DateTime.Parse(a.StartTime).Minute);
						a.StartTime = dBuildDate.ToString();
						dBuildDate = dtStart.Date;
						dBuildDate = dBuildDate.AddDays (col);
						dBuildDate = dBuildDate.AddHours (DateTime.Parse(a.EndTime).Hour);
						dBuildDate = dBuildDate.AddMinutes (DateTime.Parse(a.EndTime).Minute);
						a.EndTime = dBuildDate.ToString();

						//Call Document to add a new appointment
						this.dataAccessService.AddNewAvailability (a);
						//this.Document.CreateAppointmentAuto (a);
					}

					//Increment start and end
					dtStart = dtStart.AddDays (7);
					dtEnd = dtStart.AddDays (6);

				}//end for
				//try {
				//    RaiseRPMSEvent ("BSDX SCHEDULE", m_Document.DocName);
				//} catch (Exception ex) {
				//    Debug.Write (ex.Message);
				//}
				//this.calendarGrid1.Invalidate ();
				//this.m_DocManager.UpdateViews ((string)this.m_Document.Resources[0], "");
				RefreshAvailabilityScheduler (null);
			} catch (Exception ex) {
				MessageBox.Show ("Error loading template:  " + ex.Message, "Availablility");
			}		    
		}

		private bool CanExecuteApplyTemplateCommand (string command)
		{
			return true;
		}

		public void DeleteAvailability (string apptID)
		{
			string errorMessage = string.Empty;
			errorMessage = this.dataAccessService.DeleteAvailability (apptID);

			if (errorMessage != string.Empty) {
				this.View.AlertUser (errorMessage, "Delete Availability");
			} else {
				RefreshAvailabilityScheduler (string.Empty);
				this.eventAggregator.GetEvent<RefreshScheduler> ().Publish (string.Empty);
			}
		}

		public void ClearViewFromList ()
		{
			if (Controller != null) {
				Controller.RemoveViewFromList (View);
			}
		}

		#region IAvailabilityPresentationModel Members

		public IAvailabilityView View { get; private set; }

		public int TimeSlotToSkip
		{
			get { return _timeSlotToSkip; }
			set { _timeSlotToSkip = value; }
		}
		public ITaskAvailabilityController Controller { get; set; }
		public SchdResourceGroup ResourceGroup { get; set; }
		public SchdResource Resource { get; set; }
		public IList<SchdResource> SelectedResourceList { get; set; }
		public SchdAppointment SelectedAppointment { get; set; }
		public SchdAvailability CopySchdAvailability { get; set; }
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
		public IList<SchdAvailability> AvailabilityList
		{
			get
			{
				return availabilityList;
			}
			set
			{
				if (this.availabilityList != value) {
					this.availabilityList = value;
					//this.OnPropertyChanged ("AvailabilityList");
				}
			}
		}

		public SchdAvailability SchdAvailability
		{
			get
			{
				return schdAvailability;
			}
			set
			{
				schdAvailability = value;
			}
		}

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

		#endregion

		public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

		public void MoveResizeAvailability (IAppointment newAppointment)
		{
			string errorMessage = string.Empty;
			string apptid = string.Empty;
			string apptnote = string.Empty;
			string apptresourcename = string.Empty;
			string accesstypeid = string.Empty;
			string slots = string.Empty;
			string preStartTime = string.Empty;
			string preEndTime = string.Empty;
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
				case "ACCESSTYPEID":
					accesstypeid = item.ResourceName;
					break;
				case "SLOTS":
					slots = item.ResourceName;
					break;
				case "START_TIME":
					preStartTime = item.ResourceName;
					break;
				case "END_TIME":
					preEndTime = item.ResourceName;
					break;
				default:
					break;
				}
			}
			SchdAvailability a = new SchdAvailability ();
			a.APPOINTMENTID = null;
			a.StartTime = newAppointment.Start.ToString ();
			a.EndTime = newAppointment.End.ToString ();
			a.ACCESSTYPEID = accesstypeid;
			a.Note = apptnote;
			a.RESOURCENAME = apptresourcename;
			a.SLOTS = int.Parse (slots);

			errorMessage = this.dataAccessService.DeleteAvailability (apptid);

			if (errorMessage == string.Empty) {
				this.SelectedResource.IsAsync = false;
				RefreshAvailabilityScheduler (string.Empty);
				this.SelectedResource.IsAsync = true;
				errorMessage = this.dataAccessService.AddNewAvailability (a);
			} 
			if (errorMessage != string.Empty) {
				View.AlertUser ("Access Block could not be modified:" + Environment.NewLine + Environment.NewLine + errorMessage, "Error");
				a.StartTime = preStartTime;
				a.EndTime = preEndTime;
				errorMessage = this.dataAccessService.AddNewAvailability (a);
			}
			RefreshAvailabilityScheduler (string.Empty);
		}

		public Double SelectedNumberOfWeeks { get; set; }
		public DateTime SelectedStartDate { get; set; }
    }
}
