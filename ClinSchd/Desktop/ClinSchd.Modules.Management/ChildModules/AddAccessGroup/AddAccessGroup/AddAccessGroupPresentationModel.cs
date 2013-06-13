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
using ClinSchd.Modules.Management.AddAccessGroup.Services;
using System.Collections.ObjectModel;

namespace ClinSchd.Modules.Management.AddAccessGroup
{
	public class AddAccessGroupPresentationModel : IAddAccessGroupPresentationModel, INotifyPropertyChanged
    {
		private readonly IManagementAddAccessGroupService ManagementAddAccessGroupService;
		private readonly IDataAccessService dataAccessService;
		private readonly IEventAggregator eventAggregator;
		public event PropertyChangedEventHandler PropertyChanged;
		private ValidationMessage validationMessage;
		private string _paneTitle = "";

		public AddAccessGroupPresentationModel(IAddAccessGroupView view,
			IManagementAddAccessGroupService ManagementAddAccessGroupService,
			IDataAccessService dataAccessService,
			IEventAggregator eventAggregator)
        {
            View = view;
            View.Model = this;
			this.ManagementAddAccessGroupService = ManagementAddAccessGroupService;
			this.dataAccessService = dataAccessService;
			this.eventAggregator = eventAggregator;
			this.ValidationMessage = new ValidationMessage ();
			InitialErrorMessage ();
		}

		public void AddEditAccessTypeGroup ()
		{
			this.validationMessage.IsValid = true;
			this.validationMessage.Title = string.Empty;
			this.validationMessage.Message = string.Empty;

			string errorMessage = this.dataAccessService.AddEditAccessTypeGroup (this.AccessTypeGroupID, this.AccessTypeGroupName);

			if (errorMessage != string.Empty) {
				this.validationMessage.IsValid = false;
				this.validationMessage.Title = "Add/Edit Access Type Group";
				this.validationMessage.Message = errorMessage;
				return;
			} else {
				this.eventAggregator.GetEvent<ManagementItemAddedEvent> ().Publish ("AccessTypeGroups");
			}
		}

		public void InitialErrorMessage ()
		{
			this.validationMessage.IsValid = true;
			this.validationMessage.Title = string.Empty;
			this.validationMessage.Message = string.Empty;
		}

		public IAddAccessGroupView View { get; private set; }
		public string AccessTypeGroupName { get; set; }
		public string AccessTypeGroupID { get; set; }
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
