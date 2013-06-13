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
using ClinSchd.Modules.Management.AccessGroups.Services;
using System.Collections.ObjectModel;

namespace ClinSchd.Modules.Management.AccessGroups
{
	public class AccessGroupsPresentationModel : IAccessGroupsPresentationModel, INotifyPropertyChanged
    {
		private readonly IManagementAccessGroupsService ManagementAccessGroupsService;
		private readonly IDataAccessService dataAccessService;
		private readonly IEventAggregator eventAggregator;
		public event PropertyChangedEventHandler PropertyChanged;
		private IList<NameValue> accessGroupList;
		private IList<SchdAccessType> accessTypeList;
		private IList<SchdGroupedAccessTypes> schdGroupedAccessTypes;

		public AccessGroupsPresentationModel(IAccessGroupsView view,
			IManagementAccessGroupsService ManagementAccessGroupsService,
			IDataAccessService dataAccessService,
			IEventAggregator eventAggregator)
        {
            View = view;
            View.Model = this;
			this.ManagementAccessGroupsService = ManagementAccessGroupsService;
			this.dataAccessService = dataAccessService;
			this.eventAggregator = eventAggregator;

			LoadAccessTypes ();
			LoadAccessTypeGroups ();

			AddAccessTypeCommand = new DelegateCommand<string> (ExecuteAddAccessTypeCommand, CanExecuteAddAccessTypeCommand);
			EditAccessTypeCommand = new DelegateCommand<string> (ExecuteEditAccessTypeCommand, CanExecuteEditAccessTypeCommand);
			AddAccessGroupCommand = new DelegateCommand<string> (ExecuteAddAccessGroupCommand, CanExecuteAddAccessGroupCommand);
			EditAccessGroupCommand = new DelegateCommand<string> (ExecuteEditAccessGroupCommand, CanExecuteEditAccessGroupCommand);
		}

		public void LoadAccessTypes ()
		{
			this.AccessTypeList = this.dataAccessService.GetAccessTypes ();
			if (AccessTypeList.Count > 0) {
				this.AccessTypeIEN = AccessTypeList[0].BSDX_ACCESS_TYPE_IEN;
				OnPropertyChanged ("AccessTypeIEN");
			}
		}

		public void LoadAccessTypeGroups ()
		{
			this.AccessGroupList = this.dataAccessService.GetAccessGroups ();
			if (AccessGroupList.Count > 0) {
				this.AccessGroupIEN = AccessGroupList[0].Value;
				OnPropertyChanged ("AccessGroupIEN");
			}
		}

		public void LoadGroupedAccessTypes ()
		{
			this.schdGroupedAccessTypes = this.dataAccessService.GetAccessTypeByGroupID (this.AccessGroupIEN);
			if (this.SchdGroupedAccessTypes.Count > 0) {
				this.GroupedAccessTypeIEN = SchdGroupedAccessTypes[0].ACCESS_TYPE_ID;
				OnPropertyChanged ("GroupedAccessTypeIEN");
			}
			OnPropertyChanged ("SchdGroupedAccessTypes");
		}

		public void AddAccessTypeToGroup ()
		{
			if (this.AccessGroupIEN == null) {
				this.View.AlertUser ("Please select a access type", "Access Groups");
			} else if (this.AccessTypeIEN == null) {
				this.View.AlertUser ("Please select a group", "Access Groups");
			} else {
				this.schdGroupedAccessTypes = this.dataAccessService.AddAccessTypeToGroupByID (this.AccessGroupIEN, this.AccessTypeIEN);
				if (this.SchdGroupedAccessTypes.Count > 0) {
					this.GroupedAccessTypeIEN = SchdGroupedAccessTypes[0].ACCESS_TYPE_ID;
					OnPropertyChanged ("GroupedAccessTypeIEN");
				}
				OnPropertyChanged ("SchdGroupedAccessTypes");
			}
		}

		public void AddAllAccessTypesToGroup ()
		{
			if (this.AccessGroupIEN == null) {
				this.View.AlertUser ("Please select a group", "Access Groups");
			} else {
				foreach (SchdAccessType accessType in this.AccessTypeList) {
					this.schdGroupedAccessTypes = this.dataAccessService.AddAccessTypeToGroupByID (this.AccessGroupIEN, accessType.BSDX_ACCESS_TYPE_IEN);
				}
				OnPropertyChanged ("schdGroupedAccessTypes");
			}
		}

		public void RemoveGroupedAccessType ()
		{
			if (this.AccessGroupIEN == null) {
				this.View.AlertUser ("Please select a group", "Access Groups");
			} else if (this.GroupedAccessTypeIEN == null) {
				this.View.AlertUser ("Please select an access type", "Access Groups");
			} else {
				if (View.ConfirmUser ("Are you sure you want to remove this access type?", "Access Groups")) {

					this.schdGroupedAccessTypes = this.dataAccessService.RemoveGroupedAccessTypeByID (this.AccessGroupIEN, this.GroupedAccessTypeIEN);
					if (this.SchdGroupedAccessTypes.Count > 0) {
						this.GroupedAccessTypeIEN = SchdGroupedAccessTypes[0].ACCESS_TYPE_ID;
						OnPropertyChanged ("GroupedAccessTypeIEN");
					}
					OnPropertyChanged ("SchdGroupedAccessTypes");
				}
			}
		}

		public void RemoveAllGroupedAccessTypes ()
		{
			if (this.AccessGroupIEN == null) {
				this.View.AlertUser ("Please select a group", "Access Groups");
			} else {
				if (View.ConfirmUser ("Are you sure you want to remove all access types?", "Access Groups")) {

					foreach (SchdGroupedAccessTypes accessType in this.SchdGroupedAccessTypes) {
						this.schdGroupedAccessTypes = this.dataAccessService.RemoveGroupedAccessTypeByID (this.AccessGroupIEN, accessType.ACCESS_TYPE_ID);
					}
					OnPropertyChanged ("SchdGroupedAccessTypes");
				}
			}
		}

		public void RemoveGroupByID ()
		{
			if (this.AccessGroupIEN == null) {
				this.View.AlertUser ("Please select a group", "Access Groups");
			} else {
				if (View.ConfirmUser ("Are you sure you want to remove this group?", "Access Groups")) {

					this.accessGroupList = this.dataAccessService.RemoveAccessTypeGroup (this.AccessGroupIEN);
					if (this.AccessGroupList.Count > 0) {
						this.AccessGroupIEN = AccessGroupList[0].Value;
						OnPropertyChanged ("AccessGroupIEN");
					}
					OnPropertyChanged ("AccessGroupList");
				}
			}
		}

		public void RefreshListBoxes (string s)
		{
			if (s == "AccessTypeGroups") {
				LoadAccessTypeGroups ();
			} else {
				LoadAccessTypes ();
			}
		}

		public void ExecuteAddAccessTypeCommand (string command)
		{
			this.eventAggregator.GetEvent<ManagementItemAddedEvent> ().Subscribe (RefreshListBoxes, ThreadOption.UIThread, true);
			this.eventAggregator.GetEvent<LaunchAddAccessTypeDialogEvent> ().Publish ("Add Access Type");
		}

		public bool CanExecuteAddAccessTypeCommand (string title)
		{
			return true;
		}

		public void ExecuteEditAccessTypeCommand (string command)
		{
			this.eventAggregator.GetEvent<ManagementItemAddedEvent> ().Subscribe (RefreshListBoxes, ThreadOption.UIThread, true);
			this.eventAggregator.GetEvent<LaunchAddAccessTypeDialogEvent> ().Publish (this.AccessTypeIEN);
		}

		public bool CanExecuteEditAccessTypeCommand (string title)
		{
			return true;
		}

		public void ExecuteAddAccessGroupCommand (string command)
		{
			this.eventAggregator.GetEvent<ManagementItemAddedEvent> ().Subscribe (RefreshListBoxes, ThreadOption.UIThread, true);
			this.eventAggregator.GetEvent<LaunchAddAccessGroupDialogEvent> ().Publish ("Add Access Group");
		}

		public bool CanExecuteAddAccessGroupCommand (string title)
		{
			return true;
		}

		public void ExecuteEditAccessGroupCommand (string command)
		{
			if (this.AccessGroupIEN == null) {
				this.View.AlertUser ("Please select a group", "Access Groups");
			} else {
				this.eventAggregator.GetEvent<ManagementItemAddedEvent> ().Subscribe (RefreshListBoxes, ThreadOption.UIThread, true);
				this.eventAggregator.GetEvent<LaunchAddAccessGroupDialogEvent> ().Publish (this.AccessGroupIEN);
			}
		}

		public bool CanExecuteEditAccessGroupCommand (string title)
		{
			return true;
		}

		public IAccessGroupsView View { get; private set; }
		public Patient Patient { get; private set; }
		public DelegateCommand<string> AddAccessTypeCommand { get; private set; }
		public DelegateCommand<string> EditAccessTypeCommand { get; private set; }
		public DelegateCommand<string> AddAccessGroupCommand { get; private set; }
		public DelegateCommand<string> EditAccessGroupCommand { get; private set; }
		public string AccessTypeIEN { get; set; }
		public string AccessGroupIEN { get; set; }
		public string GroupedAccessTypeIEN { get; set; }

		public IList<SchdGroupedAccessTypes> SchdGroupedAccessTypes
		{
			get
			{
				return schdGroupedAccessTypes;
			}
			private set
			{
				if (this.schdGroupedAccessTypes != value) {
					this.schdGroupedAccessTypes = value;
					this.OnPropertyChanged ("SchdGroupedAccessTypes");
				}
			}
		}

		public IList<SchdAccessType> AccessTypeList
		{
			get
			{
				return accessTypeList;
			}
			private set
			{
				if (this.accessTypeList != value) {
					this.accessTypeList = value;
					this.OnPropertyChanged ("AccessTypeList");
				}
			}
		}

		public IList<NameValue> AccessGroupList
		{
			get
			{
				return accessGroupList;
			}
			private set
			{
				if (this.accessGroupList != value) {
					this.accessGroupList = value;
					this.OnPropertyChanged ("AccessGroupList");
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
