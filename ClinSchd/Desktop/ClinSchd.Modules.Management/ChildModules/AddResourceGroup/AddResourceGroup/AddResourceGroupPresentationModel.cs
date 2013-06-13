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
using ClinSchd.Modules.Management.AddResourceGroup.Services;
using System.Collections.ObjectModel;

namespace ClinSchd.Modules.Management.AddResourceGroup
{
	public class AddResourceGroupPresentationModel : IAddResourceGroupPresentationModel, INotifyPropertyChanged
    {
		private readonly IManagementAddResourceGroupService ManagementAddResourceGroupService;
		private readonly IDataAccessService dataAccessService;
		private readonly IEventAggregator eventAggregator;
		public event PropertyChangedEventHandler PropertyChanged;
		private ValidationMessage validationMessage;
		private string _paneTitle = "";

		public AddResourceGroupPresentationModel(IAddResourceGroupView view,
			IManagementAddResourceGroupService ManagementAddResourceGroupService,
			IDataAccessService dataAccessService,
			IEventAggregator eventAggregator)
        {
            View = view;
            View.Model = this;
			this.ManagementAddResourceGroupService = ManagementAddResourceGroupService;
			this.dataAccessService = dataAccessService;
			this.eventAggregator = eventAggregator;
			this.ValidationMessage = new ValidationMessage ();
			InitialErrorMessage ();
		}

		public void AddEditClinicGroup ()
		{
			this.validationMessage.IsValid = true;
			this.validationMessage.Title = string.Empty;
			this.validationMessage.Message = string.Empty;

			string errorMessage = this.dataAccessService.AddEditClinicGroup (this.ClinicGroupID, this.ClinicGroupName);

			if (errorMessage != string.Empty) {
				this.validationMessage.IsValid = false;
				this.validationMessage.Title = "Add/Edit Clinic Group";
				this.validationMessage.Message = errorMessage;
				return;
			} else {
				this.eventAggregator.GetEvent<ManagementItemAddedEvent> ().Publish ("ResourceGroups");
			}
		}

		public void InitialErrorMessage ()
		{
			this.validationMessage.IsValid = true;
			this.validationMessage.Title = string.Empty;
			this.validationMessage.Message = string.Empty;
		}

		public IAddResourceGroupView View { get; private set; }
		public string ClinicGroupName { get; set; }
		public string ClinicGroupID { get; set; }
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
