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
using ClinSchd.Modules.CheckOut.Controllers;
using ClinSchd.Modules.CheckOut.Services;
using Microsoft.Practices.Unity;

namespace ClinSchd.Modules.CheckOut.CheckOut
{
	public class CheckOutPresentationModel : ICheckOutPresentationModel, INotifyPropertyChanged
	{
		private readonly ICheckOutService CheckOutService;
		private readonly IDataAccessService dataAccessService;
		private readonly IEventAggregator eventAggregator;
		private ValidationMessage validationMessage;
		private CheckOutPatient _CheckOutData;

		public CheckOutPresentationModel (
			ICheckOutView view,
			ICheckOutService CheckOutService,
			IDataAccessService dataAccessService,
			IEventAggregator eventAggregator)
		{
			View = view;
			View.Model = this;
			this.eventAggregator = eventAggregator;
			this.CheckOutService = CheckOutService;
			this.dataAccessService = dataAccessService;
			this.ValidationMessage = new ValidationMessage ();
			InitialErrorMessage ();

			CheckOutAppointmentCommand = new DelegateCommand<string> (ExecuteCheckOutAppointmentCommand, CanExecuteCheckOutAppointmentCommand);
		}

		public void InitialErrorMessage ()
		{
			this.validationMessage.IsValid = true;
			this.validationMessage.Title = string.Empty;
			this.validationMessage.Message = string.Empty;
		}

		public void GetCheckOutAppointment (SchdAppointment newAppointment)
		{
			CheckOutPatient CheckOutData = new CheckOutPatient ();
			if (newAppointment == null || newAppointment.APPOINTMENTID == null) {
				return;
			} else {
				CheckOutData.ApptID = newAppointment.APPOINTMENTID;
				CheckOutData.AppointmentStartTime = newAppointment.START_TIME;
				CheckOutData.PatientID = newAppointment.PATIENTID;
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
				this.validationMessage.Title = "Check Out Patient";
				this.validationMessage.Message = ex.Message;
			}

			string sProv = string.Empty;

			if (nHospLoc > 0) {
				//Get hospital locations
				DataView hv = new DataView(this.dataAccessService.GlobalDataSet.Tables["HospitalLocation"]);
				hv.Sort = "HOSPITAL_LOCATION_ID";
				int hFind = hv.Find (nHospLoc);
				DataRowView dhv = hv[hFind];

				sProv = dhv["DEFAULT_PROVIDER"].ToString ();
			} else {
				this.validationMessage.IsValid = false;
				this.validationMessage.Title = "Check Out Patient";
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

			if (newAppointment.VPROVIDER != string.Empty) {
				foreach (Provider n in Providers) {
					if (n.IEN == newAppointment.VPROVIDER)
						this.ProviderIEN = n.IEN;
				}
			} else {
				this.ProviderIEN = "0";
			}
			CheckOutData.ProviderIEN = this.ProviderIEN;
			OnPropertyChanged ("ProviderIEN");

			//default the check in time
			CheckOutData.CheckOutDateTime = DateTime.Now.ToString ();

			this._CheckOutData = CheckOutData;
			OnPropertyChanged ("CheckOut");
		}

		public void ExecuteCheckOutAppointmentCommand (string command)
		{
			this.validationMessage.IsValid = true;
			this.validationMessage.Title = string.Empty;
			this.validationMessage.Message = string.Empty;

			string errorMessage = this.dataAccessService.CheckOutPatient (this.CheckOut);

			if (errorMessage == string.Empty) {
				this.eventAggregator.GetEvent<RefreshScheduler> ().Publish (command);
			} else {
				this.validationMessage.IsValid = false;
				this.validationMessage.Title = "Check Out Patient";
				this.validationMessage.Message = errorMessage;
				return;
			}
		}

		public bool CanExecuteCheckOutAppointmentCommand (string command)
		{
			return true;
		}

		public void OnClose()
		{
			View.Close();
		}

		[Dependency]
		public Factory<Provider> ProviderFactory { get; set; }

		#region ICheckOutPresentationModel Members

		public ICheckOutView View { get; private set; }
		public string PatientName { get; set; }
		public string ProviderIEN { get; set; }
		public string PCCClinicIEN { get; set; }
		public string PCCFormIEN { get; set; }
		public string SummaryReportIEN { get; set; }
		public bool IsSummaryReportChecked { get; set; }
		public SchdAppointment SelectedAppointment { get; set; }
		public IList<Provider> Providers { get; set; }
		public IList<NameValue> SummaryReport { get; set; }
		public DelegateCommand<string> CheckOutAppointmentCommand { get; private set; }

		public CheckOutPatient CheckOut
		{
			get
			{
				return _CheckOutData;
			}
			set
			{
				_CheckOutData = value;
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
