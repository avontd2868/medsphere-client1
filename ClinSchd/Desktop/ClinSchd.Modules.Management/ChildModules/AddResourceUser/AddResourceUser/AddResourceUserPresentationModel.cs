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
using ClinSchd.Modules.Management.AddResourceUser.Services;
using System.Collections.ObjectModel;

namespace ClinSchd.Modules.Management.AddResourceUser
{
	public class AddResourceUserPresentationModel : IAddResourceUserPresentationModel, INotifyPropertyChanged
    {
		private readonly IManagementAddResourceUserService ManagementAddResourceUserService;
		private readonly IDataAccessService dataAccessService;
		private readonly IEventAggregator eventAggregator;
		public event PropertyChangedEventHandler PropertyChanged;
		private ResourceUser resourceUser;
		private ValidationMessage validationMessage;
		private string _paneTitle = "";
		private bool isUpdateChecked = false;
		private bool isModifyChecked = false;

		public AddResourceUserPresentationModel(IAddResourceUserView view,
			IManagementAddResourceUserService ManagementAddResourceUserService,
			IDataAccessService dataAccessService,
			IEventAggregator eventAggregator)
        {
            View = view;
            View.Model = this;
			this.ManagementAddResourceUserService = ManagementAddResourceUserService;
			this.dataAccessService = dataAccessService;
			this.eventAggregator = eventAggregator;
			this.ValidationMessage = new ValidationMessage ();
			this.ResourceUser = new ResourceUser ();
			InitialErrorMessage ();

			LoadOverBookAuthority ();
		}

		public void AddEditResourceUser ()
		{
			this.validationMessage.IsValid = true;
			this.validationMessage.Title = string.Empty;
			this.validationMessage.Message = string.Empty;

			if (this.OverbookValue == "0")
			{
				this.ResourceUser.OVERBOOK = "NO";
				this.ResourceUser.MASTEROVERBOOK = "NO";
			} else if (this.OverbookValue == "1") {
				this.ResourceUser.OVERBOOK = "YES";
				this.ResourceUser.MASTEROVERBOOK = "NO";
			} else {
				this.ResourceUser.OVERBOOK = "NO";
				this.ResourceUser.MASTEROVERBOOK = "YES";
			}
			this.ResourceUser.MODIFY_APPTS = (this.IsUpdateChecked == true) ? "YES" : "NO";
			this.ResourceUser.MODIFY_SCHEDULE = (this.IsModifyChecked == true) ? "YES" : "NO";

			string errorMessage = this.dataAccessService.AddEditResourceUser (this.ResourceUser);

			if (errorMessage != string.Empty) {
				this.validationMessage.IsValid = false;
				this.validationMessage.Title = "Add/Edit Resource User";
				this.validationMessage.Message = errorMessage;
				return;
			} else {
				this.eventAggregator.GetEvent<ManagementItemAddedEvent> ().Publish ("ResourceUser");
			}
		}

		public void LoadOverBookAuthority ()
		{
			List<NameValue> overBookAuthority = new List<NameValue> ();
			overBookAuthority.Insert (0, new NameValue ("Master", "2"));
			overBookAuthority.Insert (0, new NameValue ("Regular", "1"));
			overBookAuthority.Insert (0, new NameValue ("None", "0"));
			OverbookValue = overBookAuthority[0].Value;
			this.OnPropertyChanged ("OverbookValue");
			this.OverBookAuthority = overBookAuthority;
			this.OnPropertyChanged ("OverBookAuthority");
		}

		public void InitialErrorMessage ()
		{
			this.validationMessage.IsValid = true;
			this.validationMessage.Title = string.Empty;
			this.validationMessage.Message = string.Empty;
		}

		public IAddResourceUserView View { get; private set; }
		public string ResourceUserName { get; set; }
		public string OverbookValue { get; set; }
		public IList<NameValue> OverBookAuthority { get; set; }

		public bool IsUpdateChecked
		{
			get
			{
				return isUpdateChecked;
			}
			set
			{
				isUpdateChecked = value;
			}
		}

		public bool IsModifyChecked
		{
			get
			{
				return isModifyChecked;
			}
			set
			{
				isModifyChecked = value;
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

		public ResourceUser ResourceUser
		{
			get
			{
				return resourceUser;
			}
			set
			{
				if (this.resourceUser != value) {
					this.resourceUser = value;
					this.OnPropertyChanged ("ResourceUser");
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
