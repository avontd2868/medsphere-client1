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
using ClinSchd.Modules.FindAppt.Controllers;
using ClinSchd.Modules.FindAppt.Services;

namespace ClinSchd.Modules.FindAppt.FindAppt
{
	public class FindApptPresentationModel : IFindApptPresentationModel, INotifyPropertyChanged
	{
		private readonly IFindApptService FindApptService;
		private readonly IDataAccessService dataAccessService;
		private readonly IEventAggregator eventAggregator;
		private ValidationMessage validationMessage;
		private IList<NameValue> accessGroupList;
		private IList<FindGroupedAccessTypes> schdGroupedAccessTypes;
		private string accessGroupIEN = "0";
		private IList<SchdResource> selectedResourceList;
		private IList<FindAppointmentResult> findAppointmentResult;
		private bool isMonChecked = false;
		private bool isTueChecked = false;
		private bool isWedChecked = false;
		private bool isThuChecked = false;
		private bool isFriChecked = false;
		private bool isSatChecked = false;
		private bool isSunChecked = false;
		private string preStartTime = string.Empty;
		private string preEndTime = string.Empty;

		public FindApptPresentationModel (
			IFindApptView view,
			IFindApptService FindApptService,
			IDataAccessService dataAccessService,
			IEventAggregator eventAggregator)
		{
			View = view;
			View.Model = this;
			this.eventAggregator = eventAggregator;
			this.FindApptService = FindApptService;
			this.dataAccessService = dataAccessService;
			this.ValidationMessage = new ValidationMessage ();
			this.SelectedDates = new List<string> ();
			this.IsBothChecked = true;
			InitialErrorMessage ();
			LoadAccessTypeGroups ();
			LoadGroupedAccessTypes ();

			FindApptAppointmentCommand = new DelegateCommand<string> (ExecuteFindApptAppointmentCommand, CanExecuteFindApptAppointmentCommand);
		}

		public void InitialErrorMessage ()
		{
			this.validationMessage.IsValid = true;
			this.validationMessage.Title = string.Empty;
			this.validationMessage.Message = string.Empty;
		}

		public void LoadAccessTypeGroups ()
		{
			this.AccessGroupList = this.dataAccessService.GetAccessGroups ();
			this.AccessGroupList.Insert (0, new NameValue ("<Show All Access Types>", "0"));
			this.accessGroupIEN = this.AccessGroupList[0].Value;

			OnPropertyChanged ("AccessGroupList");
			OnPropertyChanged ("AccessGroupIEN");
		}

		public void LoadGroupedAccessTypes ()
		{
			this.schdGroupedAccessTypes = this.dataAccessService.GetFindAccessTypeByGroupID (this.AccessGroupIEN);
			if (this.SchdGroupedAccessTypes.Count > 0) {
				this.GroupedAccessTypeIEN = SchdGroupedAccessTypes[0].ACCESS_TYPE_ID;
				OnPropertyChanged ("GroupedAccessTypeIEN");
			}
			OnPropertyChanged ("SchdGroupedAccessTypes");
		}

		public void ExecuteFindApptAppointmentCommand (string command)
		{
			this.validationMessage.IsValid = true;
			this.validationMessage.Title = string.Empty;
			this.validationMessage.Message = string.Empty;

			FindAppointmentData findAppointmentData = new FindAppointmentData ();
			if (this.SelectedDates.Count > 0) {
				findAppointmentData.StartDate = this.SelectedDates[this.SelectedDates.Count - 1].ToString ();
				findAppointmentData.EndDate = this.SelectedDates[0].ToString ();
			} else if (preStartTime == string.Empty) {
				findAppointmentData.StartDate = DateTime.Today.ToShortDateString ();
				findAppointmentData.EndDate = DateTime.Today.ToShortDateString ();
			} else {
				findAppointmentData.StartDate = preStartTime;
				findAppointmentData.EndDate = preEndTime;
			}
			preStartTime = findAppointmentData.StartDate;
			preEndTime = findAppointmentData.EndDate;
			findAppointmentData.Resources = this.SelectedResourceList;
			foreach (FindGroupedAccessTypes a in this.SchdGroupedAccessTypes) {
				if (a.IsChecked) {
					findAppointmentData.AccessTypes.Insert (0, a);
				}
			}

			if (IsAMChecked)
				findAppointmentData.AMPM = "AM";
			if (IsPMChecked)
				findAppointmentData.AMPM = "PM";
			if (IsBothChecked)
				findAppointmentData.AMPM = "BOTH";

			string sDayOfWeek = string.Empty;
			if (IsMonChecked) {
				sDayOfWeek += "Monday";
				findAppointmentData.Monday = true;
			} else {
				findAppointmentData.Monday = false;
			}
			if (IsTueChecked) {
				sDayOfWeek += "Tuesday";
				findAppointmentData.Tuesday = true;
			} else {
				findAppointmentData.Tuesday = false;
			}
			if (IsWedChecked){
				sDayOfWeek += "Wednesday";
				findAppointmentData.Wednesday = true;
			} else {
				findAppointmentData.Wednesday = false;
			}
			if (IsThuChecked){
				sDayOfWeek += "Thursday";
				findAppointmentData.Thursday = true;
			} else {
				findAppointmentData.Thursday = false;
			}
			if (IsFriChecked){
				sDayOfWeek += "Friday";
				findAppointmentData.Friday = true;
			} else {
				findAppointmentData.Friday = false;
			}
			if (IsSatChecked){
				sDayOfWeek += "Saturday";
				findAppointmentData.Saturday = true;
			} else {
				findAppointmentData.Saturday = false;
			}
			if (IsSunChecked){
				sDayOfWeek += "Sunday";
				findAppointmentData.Sunday = true;
			} else {
				findAppointmentData.Sunday = false;
			}
			findAppointmentData.DAYOFWEEK = sDayOfWeek;

			if (findAppointmentData.Resources != null && findAppointmentData.Resources.Count > 0) {
				IList<FindAppointmentResult> newResultList = this.dataAccessService.FindAppointments (findAppointmentData);
				List<string> s = new List<string> ();
				this.SelectedDates = s;

				this.findAppointmentResult = newResultList;
				OnPropertyChanged ("FindAppointmentResult");
				if (newResultList.Count == 0) {
					this.validationMessage.IsValid = false;
					this.validationMessage.Title = "Find Appointment";
					this.validationMessage.Message = "No available slot found.";
					return;
				}
			} else {
				this.validationMessage.IsValid = false;
				this.validationMessage.Title = "Find Appointment";
				this.validationMessage.Message = "Please open a clinic.";
				return;
			}
		}

		public bool CanExecuteFindApptAppointmentCommand (string command)
		{
			return true;
		}

		public void OnClose()
		{
			View.Close();
		}

		#region IFindApptPresentationModel Members

		public IFindApptView View { get; private set; }
		public string GroupedAccessTypeIEN { get; set; }
		public DelegateCommand<string> FindApptAppointmentCommand { get; set; }
		public IList<string> SelectedDates { get; set; }
		public bool IsAMChecked { get; set; }
		public bool IsPMChecked { get; set; }
		public bool IsBothChecked { get; set; }
		public bool IsMonChecked
		{
			get
			{
				return isMonChecked;
			}
			set
			{
				isMonChecked = value;
			}
		}
		public bool IsTueChecked
		{
			get
			{
				return isTueChecked;
			}
			set
			{
				isTueChecked = value;
			}
		}
		public bool IsWedChecked
		{
			get
			{
				return isWedChecked;
			}
			set
			{
				isWedChecked = value;
			}
		}
		public bool IsThuChecked
		{
			get
			{
				return isThuChecked;
			}
			set
			{
				isThuChecked = value;
			}
		}
		public bool IsFriChecked
		{
			get
			{
				return isFriChecked;
			}
			set
			{
				isFriChecked = value;
			}
		}
		public bool IsSatChecked
		{
			get
			{
				return isSatChecked;
			}
			set
			{
				isSatChecked = value;
			}
		}
		public bool IsSunChecked
		{
			get
			{
				return isSunChecked;
			}
			set
			{
				isSunChecked = value;
			}
		}

		public IList<SchdResource> SelectedResourceList
		{
			get
			{
				return selectedResourceList;
			}
			set
			{
				if (this.selectedResourceList != value) {
					this.selectedResourceList = value;
				}
			}
		}

		public IList<FindAppointmentResult> FindAppointmentResult
		{
			get
			{
				return findAppointmentResult;
			}
			set
			{
				if (this.findAppointmentResult != value) {
					this.findAppointmentResult = value;
					this.OnPropertyChanged ("FindAppointmentResult");
				}
			}
		}

		public string AccessGroupIEN
		{
			get
			{
				return accessGroupIEN;
			}
			set
			{
				if (this.accessGroupIEN != value) {
					this.accessGroupIEN = value;
					this.OnPropertyChanged ("AccessGroupIEN");
				}
			}
		}

		public IList<FindGroupedAccessTypes> SchdGroupedAccessTypes
		{
			get
			{
				return schdGroupedAccessTypes;
			}
			set
			{
				if (this.schdGroupedAccessTypes != value) {
					this.schdGroupedAccessTypes = value;
					this.OnPropertyChanged ("SchdGroupedAccessTypes");
				}
			}
		}

		public IList<NameValue> AccessGroupList
		{
			get
			{
				return accessGroupList;
			}
			set
			{
				if (this.accessGroupList != value) {
					this.accessGroupList = value;
					this.OnPropertyChanged ("AccessGroupList");
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

		#endregion

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler Handler = PropertyChanged;
			if (Handler != null) Handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
