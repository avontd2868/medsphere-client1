using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Commands;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Regions;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Events;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Task.AddEditAvailability.Services;
using System.Collections.ObjectModel;

namespace ClinSchd.Modules.Task.AddEditAvailability
{
	public class AddEditAvailabilityPresentationModel : IAddEditAvailabilityPresentationModel, INotifyPropertyChanged
    {
		private readonly ITaskAddEditAvailabilityService TaskAddEditAvailabilityService;
		private readonly IDataAccessService dataAccessService;
		private readonly IEventAggregator eventAggregator;
		public event PropertyChangedEventHandler PropertyChanged;
		private IList<NameValue> accessGroupList;
		private IList<FindGroupedAccessTypes> schdGroupedAccessTypes;
		private string accessGroupIEN = "0";
		private ValidationMessage validationMessage;
		private SchdAvailability schdAvailability;

		public AddEditAvailabilityPresentationModel (IAddEditAvailabilityView view,
			ITaskAddEditAvailabilityService TaskAddEditAvailabilityService,
			IDataAccessService dataAccessService,
			IEventAggregator eventAggregator)
        {
            View = view;
            View.Model = this;
			this.TaskAddEditAvailabilityService = TaskAddEditAvailabilityService;
			this.dataAccessService = dataAccessService;
			this.eventAggregator = eventAggregator;
			this.ValidationMessage = new ValidationMessage ();
			this.SchdAvailability = new SchdAvailability ();
			InitialErrorMessage ();
			LoadAccessTypeGroups ();
			LoadGroupedAccessTypes ();

			AddEditAvailabilityCommand = new DelegateCommand<string> (ExecuteAddEditAvailabilityCommand, CanExecuteAddEditAvailabilityCommand);
		}

		public void InitialErrorMessage ()
		{
			this.validationMessage.IsValid = true;
			this.validationMessage.Title = string.Empty;
			this.validationMessage.Message = string.Empty;
		}

		public void ExecuteAddEditAvailabilityCommand (string command)
		{
			this.validationMessage.IsValid = true;
			this.validationMessage.Title = string.Empty;
			this.validationMessage.Message = string.Empty;

			string sError = string.Empty;
			this.schdAvailability.ACCESSTYPEID = this.GroupedAccessTypeIEN;
			if (this.schdAvailability.SLOTS < 1) {
				this.schdAvailability.SLOTS = 1;
			}

			if (this.schdAvailability.APPOINTMENTID != null) {
				sError = this.dataAccessService.DeleteAvailability (this.schdAvailability);
			}

			if (sError == string.Empty) {
				sError = this.dataAccessService.AddNewAvailability (this.SchdAvailability);
			} 

			if (sError != string.Empty) {
				this.validationMessage.IsValid = false;
				this.validationMessage.Title = "Access Block";
				this.validationMessage.Message = sError;
				return;
			} else {
				this.eventAggregator.GetEvent<RefreshAvailability> ().Publish (command);
				this.eventAggregator.GetEvent<RefreshScheduler> ().Publish (command);
			}
		}

		public bool CanExecuteAddEditAvailabilityCommand (string command)
		{
			return true;
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
				if (this.SchdAvailability.ACCESSTYPEID != null) {
					this.GroupedAccessTypeIEN = this.SchdAvailability.ACCESSTYPEID;
				} else {
					this.GroupedAccessTypeIEN = SchdGroupedAccessTypes[0].ACCESS_TYPE_ID;
				}
				OnPropertyChanged ("GroupedAccessTypeIEN");
			}
			OnPropertyChanged ("SchdGroupedAccessTypes");
		}

		public IAddEditAvailabilityView View { get; private set; }
		public DelegateCommand<string> AddEditAvailabilityCommand { get; set; }
		public string GroupedAccessTypeIEN { get; set; }
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

		public SchdAvailability SchdAvailability
		{
			get
			{
				return schdAvailability;
			}
			set
			{
				if (this.schdAvailability != value) {
					this.schdAvailability = value;
					this.OnPropertyChanged ("SchdAvailability");
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
