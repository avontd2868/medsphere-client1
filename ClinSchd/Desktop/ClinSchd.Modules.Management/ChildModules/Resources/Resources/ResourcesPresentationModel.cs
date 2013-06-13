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
using ClinSchd.Modules.Management.Resources.Services;
using System.Collections.ObjectModel;

namespace ClinSchd.Modules.Management.Resources
{
	public class ResourcesPresentationModel : IResourcesPresentationModel, INotifyPropertyChanged
    {
		private readonly IManagementResourcesService ManagementResourcesService;
		private readonly IDataAccessService dataAccessService;
		private readonly IEventAggregator eventAggregator;
		public event PropertyChangedEventHandler PropertyChanged;
		private IList<NameValue> userList;
		private NameValue selectedUser;
		private IList<ResourceUser> resourceUserList;
		private IList<SchdResource> resourceList;
		private ResourceUser selectedResourceUser;


		public ResourcesPresentationModel(IResourcesView view,
			IManagementResourcesService ManagementResourcesService,
			IDataAccessService dataAccessService,
			IEventAggregator eventAggregator)
        {
            View = view;
            View.Model = this;
			this.ManagementResourcesService = ManagementResourcesService;
			this.dataAccessService = dataAccessService;
			this.eventAggregator = eventAggregator;

			LoadResources ();
			LoadUsers ();
			AddResourceCommand = new DelegateCommand<string> (ExecuteAddResourceCommand, CanExecuteAddResourceCommand);
			EditResourceCommand = new DelegateCommand<string> (ExecuteEditResourceCommand, CanExecuteEditResourceCommand);
			DeleteResourceCommand = new DelegateCommand<string> (ExecuteDeleteResourceCommand, CanExecuteDeleteResourceCommand);
			AddUserCommand = new DelegateCommand<string> (ExecuteAddUserCommand, CanExecuteAddUserCommand);
			EdiUserCommand = new DelegateCommand<string> (ExecuteEditUserCommand, CanExecuteEditUserCommand);
		}

		public void LoadResources ()
		{
			this.resourceList = this.dataAccessService.GetResources (string.Empty, true);
			if (ResourceList.Count > 0) {
				this.ResourceIEN = ResourceList[0].RESOURCEID;
				OnPropertyChanged ("ResourceIEN");
			}
			OnPropertyChanged ("ResourceList");
		}

		public void LoadUsers ()
		{
			this.userList = this.dataAccessService.GetScheduleUsers ();
			if (Users.Count > 0) {
				this.UserIEN = Users[0].Value;
				OnPropertyChanged ("UserIEN");
				this.selectedUser = Users[0];
				OnPropertyChanged ("SelectedUser");
			}
			OnPropertyChanged ("Users");
		}

		public void LoadResourceUsers ()
		{
			this.resourceUserList = this.dataAccessService.GetResourceUsersByResourceID (this.ResourceIEN);
			if (ResourceUserList.Count > 0) {
				this.ResourceUserIEN = ResourceUserList[0].BSDX_RESOURCE_USER_IEN;
				OnPropertyChanged ("ResourceUserIEN");
				this.selectedResourceUser = ResourceUserList[0];
				OnPropertyChanged ("SelectedResourceUser");
			}
			OnPropertyChanged ("ResourceUserList");
		}

		public void UserSelectionChanged ()
		{
			foreach (NameValue n in this.Users) {
				if (this.UserIEN == n.Value) {
					this.selectedUser = n;
					OnPropertyChanged ("SelectedUser");
					break;
				}
			}
		}

		public void ResourceUserSelectionChanged ()
		{
			foreach (ResourceUser r in this.ResourceUserList) {
				if (this.ResourceUserIEN == r.BSDX_RESOURCE_USER_IEN) {
					this.selectedResourceUser = r;
					OnPropertyChanged ("SelectedResourceUser");
					break;
				}
			}
		}

		public void RemoveResourceUser ()
		{
			if (this.ResourceUserIEN == null) {
				this.View.AlertUser ("Please select a user", "Clinics and Users");
			} else if (this.ResourceIEN == null) {
				this.View.AlertUser ("Please select a clinic", "Clinics and Users");
			} else {
				if (View.ConfirmUser ("Are you sure you want to delete this user?", "Clinics and Users")) {
					this.resourceUserList = this.dataAccessService.RemoveResourceUserByID (this.ResourceUserIEN, this.ResourceIEN);
					if (ResourceUserList.Count > 0) {
						this.ResourceUserIEN = ResourceUserList[0].USER_ID;
						OnPropertyChanged ("ResourceUserIEN");
					}
					OnPropertyChanged ("ResourceUserList");
				}
			}
		}

		public void RemoveAllResourceUsers ()
		{
			if (this.ResourceIEN == null) {
				this.View.AlertUser ("Please select a clinic", "Clinics and Users");
			} else {
				if (View.ConfirmUser ("Are you sure you want to delete all users?", "Clinics and Users")) {

					foreach (ResourceUser user in this.ResourceUserList) {
						this.resourceUserList = this.dataAccessService.RemoveResourceUserByID (user.BSDX_RESOURCE_USER_IEN, this.ResourceIEN);
					}
					OnPropertyChanged ("ResourceUserList");
				}
			}
		}

		public void RefreshListBoxes (string s)
		{
			if (s == "Resources") {
				LoadResources ();
			} else {
				LoadResourceUsers ();
			}
		}

		public void ExecuteAddResourceCommand (string command)
		{
			this.eventAggregator.GetEvent<ManagementItemAddedEvent> ().Subscribe (RefreshListBoxes, ThreadOption.UIThread, true);
			this.eventAggregator.GetEvent<LaunchAddResourceDialogEvent> ().Publish ("Add Clinic");
		}

		public bool CanExecuteAddResourceCommand (string title)
		{
			return true;
		}

		public void ExecuteEditResourceCommand (string command)
		{
			if (this.ResourceIEN == null) {
				this.View.AlertUser ("Please select a clinic", "Clinics and Users");
			}else{
				this.eventAggregator.GetEvent<ManagementItemAddedEvent> ().Subscribe (RefreshListBoxes, ThreadOption.UIThread, true);
				this.eventAggregator.GetEvent<LaunchAddResourceDialogEvent> ().Publish (this.ResourceIEN);
			}
		}

		public bool CanExecuteEditResourceCommand (string title)
		{
			return true;
		}

		public void ExecuteDeleteResourceCommand (string command)
		{
			if (this.ResourceIEN == null) {
				this.View.AlertUser ("Please select a clinic", "Clinics and Users");
			} else {
				string sHospitalLocationID = string.Empty;
				foreach (SchdResource r in this.ResourceList) {
					if (this.ResourceIEN == r.RESOURCEID) {
						sHospitalLocationID = r.HOSPITAL_LOCATION_ID;
					}
				}
				if (View.ConfirmUser ("Are you sure you want to delete this clinic?", "Clinics and Users")) {
					string sError = string.Empty;
					sError = this.dataAccessService.DeleteClinic (this.ResourceIEN);


					if (sError != string.Empty) {
						this.View.AlertUser (sError, "Clinics and Users");
					} else {
						RefreshListBoxes ("Resources");
						this.eventAggregator.GetEvent<CloseSchedulesEvent> ().Publish (sHospitalLocationID);
						this.eventAggregator.GetEvent<ManagementItemAddedEvent> ().Publish (null);
					}
				}
			}
		}

		public bool CanExecuteDeleteResourceCommand (string title)
		{
			return true;
		}

		public void ExecuteAddUserCommand (string command)
		{
			if (this.SelectedUser == null) {
				this.View.AlertUser ("Please select a user", "Clinics and Users");
			} else {
				this.eventAggregator.GetEvent<ManagementItemAddedEvent> ().Subscribe (RefreshListBoxes, ThreadOption.UIThread, true);
				this.eventAggregator.GetEvent<LaunchAddResourceUserDialogEvent> ().Publish ("Add Clinic User");

				ResourceUser r = new ResourceUser ();
				r.USER_ID = this.SelectedUser.Value;
				r.USERNAME = this.SelectedUser.Name;
				r.RESOURCE_ID = this.ResourceIEN;
				this.eventAggregator.GetEvent<AddEditResourceUserEvent> ().Publish (r);
			}
		}

		public bool CanExecuteAddUserCommand (string title)
		{
			return true;
		}

		public void ExecuteEditUserCommand (string command)
		{
			if (this.ResourceUserIEN == null || this.SelectedResourceUser == null) {
				this.View.AlertUser ("Please select a user", "Clinics and Users");
			} else {
				this.eventAggregator.GetEvent<ManagementItemAddedEvent> ().Subscribe (RefreshListBoxes, ThreadOption.UIThread, true);
				this.eventAggregator.GetEvent<LaunchAddResourceUserDialogEvent> ().Publish (this.ResourceUserIEN);
				this.eventAggregator.GetEvent<AddEditResourceUserEvent> ().Publish (this.SelectedResourceUser);
			}
		}

		public bool CanExecuteEditUserCommand (string title)
		{
			return true;
		}

		public DelegateCommand<string> AddResourceCommand { get; private set; }
		public DelegateCommand<string> EditResourceCommand { get; private set; }
		public DelegateCommand<string> DeleteResourceCommand { get; private set; }
		public DelegateCommand<string> AddUserCommand { get; private set; }
		public DelegateCommand<string> EdiUserCommand { get; private set; }
		public IResourcesView View { get; private set; }
		public Patient Patient { get; private set; }
		public string ResourceIEN { get; set; }
		public string ResourceUserIEN { get; set; }
		public string UserIEN { get; set; }

		public IList<ResourceUser> ResourceUserList
		{
			get
			{
				return resourceUserList;
			}
			private set
			{
				if (this.resourceUserList != value) {
					this.resourceUserList = value;
					this.OnPropertyChanged ("ResourceUserList");
				}
			}
		}

		public IList<NameValue> Users
		{
			get
			{
				return userList;
			}
			private set
			{
				if (this.userList != value) {
					this.userList = value;
					this.OnPropertyChanged ("Users");
				}
			}
		}

		public NameValue SelectedUser
		{
			get
			{
				return selectedUser;
			}
			private set
			{
				if (this.selectedUser != value) {
					this.selectedUser = value;
					this.OnPropertyChanged ("SelectedUser");
				}
			}
		}

		public IList<SchdResource> ResourceList
		{
			get
			{
				return resourceList;
			}
			private set
			{
				if (this.resourceList != value) {
					this.resourceList = value;
					this.OnPropertyChanged ("ResourceList");
				}
			}
		}

		public ResourceUser SelectedResourceUser
		{
			get
			{
				return selectedResourceUser;
			}
			private set
			{
				if (this.selectedResourceUser != value) {
					this.selectedResourceUser = value;
					this.OnPropertyChanged ("SelectedResourceUser");
				}
			}
		}

		private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
