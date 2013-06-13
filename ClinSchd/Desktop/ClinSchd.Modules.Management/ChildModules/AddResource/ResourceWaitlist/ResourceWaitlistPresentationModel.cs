using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Commands;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Regions;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Management.AddResource.Services;
using System.Collections.ObjectModel;

namespace ClinSchd.Modules.Management.AddResource
{
	public class ResourceWaitlistPresentationModel : INotifyPropertyChanged
    {
		private readonly IManagementAddResourceService ManagementAddResourceService;
		private readonly IDataAccessService dataAccessService;
		private readonly IEventAggregator eventAggregator;
		public event PropertyChangedEventHandler PropertyChanged;
		private IList<RPMSClinic> clinicList;
		private RPMSClinic selectedClinic;
		private ValidationMessage validationMessage;
		private string _paneTitle = "";
		private bool isClinicEnabled = true;

		public DataRowCollection WaitingList { get; set; }

		public ResourceWaitlistPresentationModel (IResourceWaitlistView view,
			IDataAccessService dataAccessService,
			string resourceIEN)
		{
			View = view;
			View.Model = this;
			//this.ManagementAddResourceService = ManagementAddResourceService;
			this.dataAccessService = dataAccessService;
			//this.eventAggregator = eventAggregator;
			this.ValidationMessage = new ValidationMessage ();
			this.SchdResource = new SchdResource ();
			InitialErrorMessage ();

			this.WaitingList = this.dataAccessService.GetWaitingList (resourceIEN).Rows;
			LoadClinics ();
			if (this.WaitingList.Count > 0) {
				View.ShowDialog ();
			}
		}

		public void LoadClinics ()
		{
			this.clinicList = this.dataAccessService.GetRPMSClinics ();
			if (ClinicList.Count > 0) {
				this.HospitalLocationID = ClinicList[0].HOSPITAL_LOCATION_ID;
				this.OnPropertyChanged ("HospitalLocationID");
				this.selectedClinic = ClinicList[0];
				this.OnPropertyChanged ("SelectedClinic");
			}
			this.OnPropertyChanged ("ClinicList");
			LoadTimeScale ();
		}

		public void LoadEditClinic ()
		{
			this.SchdResource = this.dataAccessService.GetResource (this.EditClinicID);
			this.selectedClinic = this.SchdResource.Clinic;
			this.OnPropertyChanged ("SelectedClinic");
			this.HospitalLocationID = this.SelectedClinic.HOSPITAL_LOCATION_ID;
			this.OnPropertyChanged ("HospitalLocationID");
			this.TimeScaleValue = this.SchdResource.TIMESCALE;
			this.OnPropertyChanged ("TimeScaleValue");
			this.IsInactive = (this.SchdResource.INACTIVE == "1") ? true : false;
			this.OnPropertyChanged ("IsInactive");
		}

		public void LoadClinicSetupParemters ()
		{
			this.selectedClinic = this.dataAccessService.GetClinicByID (this.HospitalLocationID);
			this.OnPropertyChanged ("SelectedClinic");
		}

		public void LoadTimeScale ()
		{
			List<NameValue> timeScale = new List<NameValue>();
			timeScale.Insert (0, new NameValue ("60", "60"));
			timeScale.Insert (0, new NameValue ("30", "30"));
			timeScale.Insert (0, new NameValue ("20", "20"));
			timeScale.Insert (0, new NameValue ("15", "15"));
			timeScale.Insert (0, new NameValue ("10", "10"));
			timeScale.Insert (0, new NameValue ("5", "5"));
			TimeScaleValue = timeScale[2].Value;
			this.OnPropertyChanged ("TimeScaleValue");
			this.TimeScale = timeScale;
			this.OnPropertyChanged ("TimeScale");
		}

		public void AddEditClinic ()
		{
			this.validationMessage.IsValid = true;
			this.validationMessage.Title = string.Empty;
			this.validationMessage.Message = string.Empty;

			this.SchdResource.Clinic = this.SelectedClinic;
			this.SchdResource.RESOURCEID = this.EditClinicID;
			this.SchdResource.TIMESCALE = this.TimeScaleValue;
			if (this.IsInactive) {
				this.SchdResource.INACTIVE = "YES";

				this.eventAggregator.GetEvent<CloseSchedulesEvent> ().Publish (this.SelectedClinic.HOSPITAL_LOCATION_ID);
			} else {
				this.SchdResource.INACTIVE = "NO";
			}

			string errorMessage = this.dataAccessService.AddEditClinic (this.SchdResource);

			if (errorMessage != string.Empty) {
				this.validationMessage.IsValid = false;
				this.validationMessage.Title = "Add/Edit Clinic";
				this.validationMessage.Message = errorMessage;
				return;
			} else {
				this.eventAggregator.GetEvent<ManagementItemAddedEvent> ().Publish ("Resources");
				this.eventAggregator.GetEvent<ManagementItemAddedEvent> ().Publish ("ResourceInGroups");
			}
		}

		public void InitialErrorMessage ()
		{
			this.validationMessage.IsValid = true;
			this.validationMessage.Title = string.Empty;
			this.validationMessage.Message = string.Empty;
		}

		public IResourceWaitlistView View { get; private set; }
		public Patient Patient { get; private set; }
		public string HospitalLocationID { get; set; }
		public string EditClinicID { get; set; }
		public IList<NameValue> TimeScale { get; set; }
		public string TimeScaleValue { get; set; }
		public SchdResource SchdResource { get; set; }
		public bool IsInactive { get; set; }
		public string ReminderLetter { get; set; }
		public string RebookLetter { get; set; }
		public string CancellationLetter { get; set; }

		public bool IsClinicEnabled
		{
			get
			{
				return isClinicEnabled;
			}
			set
			{
				isClinicEnabled = value;
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

		public IList<RPMSClinic> ClinicList
		{
			get
			{
				return clinicList;
			}
			private set
			{
				if (this.clinicList != value) {
					this.clinicList = value;
					this.OnPropertyChanged ("ClinicList");
				}
			}
		}

		public RPMSClinic SelectedClinic
		{
			get
			{
				return selectedClinic;
			}
			private set
			{
				if (this.selectedClinic != value) {
					this.selectedClinic = value;
					this.OnPropertyChanged ("SelectedClinic");
				}
			}
		}

		public void OnClose ()
		{
			View.Close ();
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
