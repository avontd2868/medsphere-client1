using System;
using System.Collections.Generic;
using System.ComponentModel;

using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Commands;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Regions;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Management.ResourceGroups.Services;
using System.Collections.ObjectModel;

namespace ClinSchd.Modules.Management.ResourceGroups
{
	public class ResourceGroupsPresentationModel : IResourceGroupsPresentationModel, INotifyPropertyChanged
    {
		private readonly IManagementResourceGroupsService ManagementResourceGroupsService;
		private readonly IDataAccessService dataAccessService;
		private readonly IEventAggregator eventAggregator;
		public event PropertyChangedEventHandler PropertyChanged;
		private IList<SchdResource> resourceList;
		private IList<NameValue> clinicGroupList;
		private IList<SchdGroupedResources> groupedClinics;

		public ResourceGroupsPresentationModel(IResourceGroupsView view,
			IManagementResourceGroupsService ManagementResourceGroupsService,
			IDataAccessService dataAccessService,
			IEventAggregator eventAggregator)
        {
            View = view;
            View.Model = this;
			this.ManagementResourceGroupsService = ManagementResourceGroupsService;
			this.dataAccessService = dataAccessService;
			this.eventAggregator = eventAggregator;

			LoadClinics ();
			LoadClinicGroups ();
			AddResourceGroupCommand = new DelegateCommand<string> (ExecuteAddResourceGroupCommand, CanExecuteAddResourceGroupCommand);
			EditResourceGroupCommand = new DelegateCommand<string> (ExecuteEditResourceGroupCommand, CanExecuteEditResourceGroupCommand);
			this.eventAggregator.GetEvent<ManagementItemAddedEvent> ().Subscribe (RefreshListBoxes, ThreadOption.UIThread, true);
		}

		public void LoadClinics ()
		{
			this.resourceList = this.dataAccessService.GetResources (string.Empty, true);
			List<SchdResource> rTemp = new List<SchdResource> ();
			for (int i = 0; i< this.resourceList.Count; i++)
			{
				if (this.resourceList[i].INACTIVE == "0") {
					rTemp.Add(this.resourceList[i]);
				}
			}
			this.resourceList = rTemp;
			if (ResourceList.Count > 0) {
				this.ResourceIEN = ResourceList[0].RESOURCEID;
				OnPropertyChanged ("ResourceIEN");
			}
			OnPropertyChanged ("ResourceList");
		}

		public void LoadClinicGroups ()
		{
			this.clinicGroupList = this.dataAccessService.GetResourceGroupList ();
			if (ClinicGroupList.Count > 0) {
				this.ClinicGroupIEN = ClinicGroupList[0].Value;
				OnPropertyChanged ("ClinicGroupIEN");
			}
			OnPropertyChanged ("ClinicGroupList");
		}

		public void LoadGroupedClinics ()
		{
			this.groupedClinics = this.dataAccessService.GetResourcesByGroupID (this.ClinicGroupIEN);
			if (this.SchdGroupedResources.Count > 0) {
				this.GroupedClinicIEN = SchdGroupedResources[0].RESOURCE_GROUP_ITEMID;
				OnPropertyChanged ("GroupedClinicIEN");
			}
			OnPropertyChanged ("SchdGroupedResources");
		}

		public void AddClinicToGroup ()
		{
			if (this.ClinicGroupIEN == null) {
				this.View.AlertUser ("Please select a group", "Clinic Groups");
			} else if (this.ResourceIEN == null) {
				this.View.AlertUser ("Please select a clinic", "Clinic Groups");
			} else {
				this.groupedClinics = this.dataAccessService.AddClinicToGroupByID (this.ClinicGroupIEN, this.ResourceIEN);
				if (this.SchdGroupedResources.Count > 0) {
					this.GroupedClinicIEN = SchdGroupedResources[0].RESOURCE_GROUP_ITEMID;
					OnPropertyChanged ("GroupedClinicIEN");
				}
				OnPropertyChanged ("SchdGroupedResources");
				this.eventAggregator.GetEvent<ManagementItemAddedEvent> ().Publish ("ResourceGroups");
			}
		}

		public void AddAllClinicsToGroup ()
		{
			if (this.ClinicGroupIEN != null) {
				foreach (SchdResource clinics in this.ResourceList) {
					this.groupedClinics = this.dataAccessService.AddClinicToGroupByID (this.ClinicGroupIEN, clinics.RESOURCEID);
				}
				OnPropertyChanged ("SchdGroupedResources");
			} else {
				this.View.AlertUser ("Please select a group", "Clinic Groups");
			}
		}

		public void RemoveGroupedClinic ()
		{
			if (this.ClinicGroupIEN == null) {
				this.View.AlertUser ("Please select a group", "Clinic Groups");
			} else if (this.GroupedClinicIEN == null) {
				this.View.AlertUser ("Please select a clinic", "Clinic Groups");
			} else {
				if (View.ConfirmUser ("Are you sure you want to remove this clinic?", "Clinic Groups")) {

					this.groupedClinics = this.dataAccessService.RemoveGroupedClinicByID (this.ClinicGroupIEN, this.GroupedClinicIEN);
					if (this.SchdGroupedResources.Count > 0) {
						this.GroupedClinicIEN = SchdGroupedResources[0].RESOURCE_GROUP_ITEMID;
						OnPropertyChanged ("GroupedClinicIEN");
					}
					OnPropertyChanged ("SchdGroupedResources");
					this.eventAggregator.GetEvent<ManagementItemAddedEvent> ().Publish ("ResourceGroups");
				}
			}
		}

		public void RemoveAllGroupedClinics ()
		{
			if (this.ClinicGroupIEN == null) {
				this.View.AlertUser ("Please select a group", "Clinic Groups");
			} else {
				if (View.ConfirmUser ("Are you sure you want to remove all clinics?", "Clinic Groups")) {

					foreach (SchdGroupedResources clinics in this.SchdGroupedResources) {
						this.groupedClinics = this.dataAccessService.RemoveGroupedClinicByID (this.ClinicGroupIEN, clinics.RESOURCE_GROUP_ITEMID);
					}
					OnPropertyChanged ("SchdGroupedResources");
				}
			}
		}

		public void RemoveGroupByName ()
		{
			if (this.ClinicGroupIEN == null) {
				this.View.AlertUser ("Please select a group", "Clinic Groups");
			} else {
				if (View.ConfirmUser ("Are you sure you want to remove this group?", "Clinic Groups")) {

					string groupName = string.Empty;
					foreach (NameValue c in this.ClinicGroupList) {
						if (this.ClinicGroupIEN == c.Value) {
							groupName = c.Name;
						}
					}
					this.clinicGroupList = this.dataAccessService.RemoveResourceGroup (groupName);
					if (this.ClinicGroupList.Count > 0) {
						this.ClinicGroupIEN = ClinicGroupList[0].Value;
						OnPropertyChanged ("ClinicGroupIEN");
					}
					OnPropertyChanged ("ClinicGroupList");
				}
			}
		}

		public void RefreshListBoxes (string s)
		{
			if (s == "ResourceGroups") {
				LoadClinicGroups ();
			}

			if (s == "ResourceInGroups") {
				LoadClinics ();
			}
		}

		public void ExecuteAddResourceGroupCommand (string command)
		{
			this.eventAggregator.GetEvent<ManagementItemAddedEvent> ().Subscribe (RefreshListBoxes, ThreadOption.UIThread, true);
			this.eventAggregator.GetEvent<LaunchAddResourceGroupDialogEvent> ().Publish ("Add Clinic Group");
		}

		public bool CanExecuteAddResourceGroupCommand (string title)
		{
			return true;
		}

		public void ExecuteEditResourceGroupCommand (string command)
		{
			if (this.ClinicGroupIEN == null) {
				this.View.AlertUser ("Please select a group", "Clinic Groups");
			} else {
				this.eventAggregator.GetEvent<ManagementItemAddedEvent> ().Subscribe (RefreshListBoxes, ThreadOption.UIThread, true);
				this.eventAggregator.GetEvent<LaunchAddResourceGroupDialogEvent> ().Publish (this.ClinicGroupIEN);
			}
		}

		public bool CanExecuteEditResourceGroupCommand (string title)
		{
			return true;
		}

		public DelegateCommand<string> AddResourceGroupCommand { get; private set; }
		public DelegateCommand<string> EditResourceGroupCommand { get; private set; }
		public IResourceGroupsView View { get; private set; }
		public Patient Patient { get; private set; }
		public string ResourceIEN { get; set; }
		public string ClinicGroupIEN { get; set; }
		public string GroupedClinicIEN { get; set; }

		public IList<NameValue> ClinicGroupList
		{
			get
			{
				return clinicGroupList;
			}
			private set
			{
				if (this.clinicGroupList != value) {
					this.clinicGroupList = value;
					this.OnPropertyChanged ("ClinicGroupList");
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

		public IList<SchdGroupedResources> SchdGroupedResources
		{
			get
			{
				return groupedClinics;
			}
			private set
			{
				if (this.groupedClinics != value) {
					this.groupedClinics = value;
					this.OnPropertyChanged ("SchdGroupedResources");
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
