
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
using ClinSchd.Modules.CheckIn.Controllers;
using ClinSchd.Modules.CheckIn.Services;
using Microsoft.Practices.Unity;

namespace ClinSchd.Modules.CheckIn.CheckIn
{
	public class CheckInPresentationModel : ICheckInPresentationModel, INotifyPropertyChanged
	{
		private readonly ICheckInService CheckInService;
		private readonly IDataAccessService dataAccessService;
		private readonly IEventAggregator eventAggregator;
		private ValidationMessage validationMessage;
		private string isSummaryReportVisible = "Hidden";
		CheckInPatient _checkInData;

		public CheckInPresentationModel (
			ICheckInView view,
			ICheckInService CheckInService,
			IDataAccessService dataAccessService,
			IEventAggregator eventAggregator)
		{
			View = view;
			View.Model = this;
			this.eventAggregator = eventAggregator;
			this.CheckInService = CheckInService;
			this.dataAccessService = dataAccessService;
			this.ValidationMessage = new ValidationMessage ();
			InitialErrorMessage ();

			CheckInAppointmentCommand = new DelegateCommand<string> (ExecuteCheckInAppointmentCommand, CanExecuteCheckInAppointmentCommand);
		}

		public void InitialErrorMessage ()
		{
			this.validationMessage.IsValid = true;
			this.validationMessage.Title = string.Empty;
			this.validationMessage.Message = string.Empty;
		}

		public void GetCheckInAppointment (SchdAppointment newAppointment)
		{
			CheckInPatient checkInData = new CheckInPatient ();
			if (newAppointment == null || newAppointment.APPOINTMENTID == null) {
				return;
			} else {
				checkInData.ApptID = newAppointment.APPOINTMENTID;

				this.IsSummaryReportChecked = false;
				OnPropertyChanged ("IsSummaryReportChecked");
			}

			if (Convert.ToDateTime (newAppointment.START_TIME).Date > DateTime.Today.Date) {
				this.validationMessage.IsValid = false;
				this.validationMessage.Title = "Check In Patient";
				this.validationMessage.Message = "It is too early to check in " + newAppointment.PATIENTNAME;

				return;
			} else {
				this.validationMessage.IsValid = true;
				this.validationMessage.Title = string.Empty;
				this.validationMessage.Message = string.Empty;
			}

			this.PatientName = newAppointment.PATIENTNAME;
			OnPropertyChanged ("PatientName");

			//Find the default provider for the resource & load into combo box
			DataView rv = new DataView (this.dataAccessService.GlobalDataSet.Tables["Resources"]);
			rv.Sort = "RESOURCE_NAME ASC";
			int nFind = rv.Find ((string)newAppointment.RESOURCENAME);
			DataRowView drv = rv[nFind];

			string sHospLoc = drv["HOSPITAL_LOCATION_ID"].ToString ();
			sHospLoc = (sHospLoc == string.Empty) ? "0" : sHospLoc;
			int nHospLoc = 0;
			try {
				nHospLoc = Convert.ToInt32 (sHospLoc);
			} catch (Exception ex) {
				this.validationMessage.IsValid = false;
				this.validationMessage.Title = "Check In Patient";
				this.validationMessage.Message = ex.Message;
			}

			string sProv = string.Empty;
			string sProvReqd = "NO";
			string sPCC = "NO";
			string sMultCodes = "NO";
			string sStopCode = string.Empty;
			bool bProvReqd = false;
			bool bPCC = false;
			bool bMultCodes = false;

			if (nHospLoc > 0) {
				//Get hospital locations
				DataView hv = new DataView(this.dataAccessService.GlobalDataSet.Tables["HospitalLocation"]);
				hv.Sort = "HOSPITAL_LOCATION_ID";
				int hFind = hv.Find (nHospLoc);
				DataRowView dhv = hv[hFind];

				sProv = dhv["DEFAULT_PROVIDER"].ToString ();
				sStopCode = dhv["STOP_CODE_NUMBER"].ToString ();

				DataView sv = new DataView (this.dataAccessService.GlobalDataSet.Tables["ClinicStop"]);
				sv.Sort = "NAME ASC";
				if (sStopCode != string.Empty) {
					int sFind = sv.Find ((string)sStopCode);
					if (sFind > -1) {
						checkInData.ClinicStopIEN = sv[sFind].Row["CLINIC_STOP_IEN"].ToString ();
					}
				}

				DataView cv = new DataView (this.dataAccessService.GlobalDataSet.Tables["ClinicSetupParameters"]);
				cv.Sort = "HOSPITAL_LOCATION_ID";
				int cFind = cv.Find (nHospLoc);
				DataRowView dcv = cv[cFind];
				sProvReqd = dcv["VISIT_PROVIDER_REQUIRED"].ToString ();
				sPCC = dcv["GENERATE_PCCPLUS_FORMS?"].ToString ();
				sMultCodes = dcv["MULTIPLE_CLINIC_CODES_USED?"].ToString ();

				bProvReqd = (sProvReqd == "YES") ? true : false;
				bPCC = (sPCC == "YES") ? true : false;
				bMultCodes = (sMultCodes == "YES") ? true : false;
			} else {
				this.validationMessage.IsValid = false;
				this.validationMessage.Title = "Check In Patient";
				this.validationMessage.Message = "You need to select a clinic for this resource in Scheduling Management.";
				return;
			}

			//Provider processing
			Providers = this.dataAccessService.GetProviders ();

			bool isNone = false;
			foreach (Provider nameValue in Providers) {
				if (nameValue.Name == "<None>")
					isNone = true;
			}

			if (!isNone) {
				Provider newProv = ProviderFactory.Create ();
				newProv.Name = "<None>";
				newProv.IEN = "0";
				Providers.Insert (0, newProv);
			}
			OnPropertyChanged ("Providers");

			if (sProv != string.Empty) {
				foreach (Provider n in Providers) {
					if (n.Name == sProv)
						this.ProviderIEN = n.IEN;
				}
			} else {
				this.ProviderIEN = Providers[0].IEN;
			}
			checkInData.ProviderIEN = this.ProviderIEN;
			OnPropertyChanged ("ProviderIEN");

			//PCC Plus processing
			//...

			//Health Summary Report processing
			SummaryReport = this.dataAccessService.GetSummaryReport ();
			OnPropertyChanged ("SummaryReport");
			this.HealthSummaryLetterIEN = string.Empty;
			OnPropertyChanged ("HealthSummaryLetterIEN");

			//default the check in time
			checkInData.CheckInDateTime = DateTime.Now.ToString ();

			this._checkInData = checkInData;
			OnPropertyChanged ("CheckIn");
		}

		public void ExecuteCheckInAppointmentCommand (string command)
		{
			this.validationMessage.IsValid = true;
			this.validationMessage.Title = string.Empty;
			this.validationMessage.Message = string.Empty;

			string errorMessage = this.dataAccessService.CheckInPatient (this.CheckIn);

			if (errorMessage == string.Empty) {
				this.eventAggregator.GetEvent<RefreshScheduler> ().Publish (command);
			} else {
				this.validationMessage.IsValid = false;
				this.validationMessage.Title = "Check In Patient";
				this.validationMessage.Message = errorMessage;
				return;
			}
		}

		public bool CanExecuteCheckInAppointmentCommand (string command)
		{
			return true;
		}

		public void OnClose()
		{
			View.Close();
		}

		[Dependency]
		public Factory<Provider> ProviderFactory { get; set; }

		#region ICheckInPresentationModel Members

		public ICheckInView View { get; private set; }
		public string PatientName { get; set; }
		public string ProviderIEN { get; set; }
		public string PCCClinicIEN { get; set; }
		public string PCCFormIEN { get; set; }
		public string HealthSummaryLetterIEN { get; set; }
		public bool IsSummaryReportChecked { get; set; }
		public IList<Provider> Providers { get; set; }
		public SchdAppointment SelectedAppointment { get; set; }
		public IList<NameValue> SummaryReport { get; set; }
		public DelegateCommand<string> CheckInAppointmentCommand { get; private set; }

		public CheckInPatient CheckIn
		{
			get
			{
				return _checkInData;
			}
			set
			{
				_checkInData = value;
			}
		}
		public string IsSummaryReportVisible
		{
			get
			{
				return isSummaryReportVisible;
			}
			set
			{
				isSummaryReportVisible = value;
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

		#endregion

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler Handler = PropertyChanged;
			if (Handler != null) Handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
