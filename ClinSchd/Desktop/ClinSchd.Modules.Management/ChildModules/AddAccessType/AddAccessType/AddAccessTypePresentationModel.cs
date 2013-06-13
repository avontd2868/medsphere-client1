using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Media;

using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Commands;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Regions;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Management.AddAccessType.Services;
using System.Collections.ObjectModel;

namespace ClinSchd.Modules.Management.AddAccessType
{
	public class AddAccessTypePresentationModel : IAddAccessTypePresentationModel, INotifyPropertyChanged
    {
		private readonly IManagementAddAccessTypeService ManagementAddAccessTypeService;
		private readonly IDataAccessService dataAccessService;
		private readonly IEventAggregator eventAggregator;
		public event PropertyChangedEventHandler PropertyChanged;
		private ValidationMessage validationMessage;
		private string _paneTitle = "";
		private bool isInactiveChecked = false;
		private bool isPreventChecked = false;
		private string accessTypeColor = "Gray";

		public AddAccessTypePresentationModel(IAddAccessTypeView view,
			IManagementAddAccessTypeService ManagementAddAccessTypeService,
			IDataAccessService dataAccessService,
			IEventAggregator eventAggregator)
        {
            View = view;
            View.Model = this;
			this.ManagementAddAccessTypeService = ManagementAddAccessTypeService;
			this.dataAccessService = dataAccessService;
			this.eventAggregator = eventAggregator;
			this.ValidationMessage = new ValidationMessage ();
			this.AccessType = new SchdAccessType ();
			InitialErrorMessage ();
		}

		public void LoadEditAccessType ()
		{
			this.AccessType = this.dataAccessService.GetAccessTypeByID (this.EditAccessTypeID);
			if (AccessType != null) {
				this.AccessTypeName = AccessType.NAME;
				OnPropertyChanged ("AccessTypeName");
				this.View.ATName.Foreground = new SolidColorBrush (System.Windows.Media.Color.FromRgb (Convert.ToByte (AccessType.RED), Convert.ToByte (AccessType.GREEN), Convert.ToByte (AccessType.BLUE)));
				this.View.AccessTypeColorPicker.SelectedColor = System.Windows.Media.Color.FromRgb (Convert.ToByte (AccessType.RED), Convert.ToByte (AccessType.GREEN), Convert.ToByte (AccessType.BLUE));
				this.IsInactiveChecked = AccessType.INACTIVE == "YES" ? true : false;
				OnPropertyChanged ("IsInactiveChecked");
				this.IsPreventChecked = AccessType.PREVENT_ACCESS == "YES" ? true : false;
				OnPropertyChanged ("IsPreventChecked");
			}

		}

		public void AddEditAccessType ()
		{
			this.validationMessage.IsValid = true;
			this.validationMessage.Title = string.Empty;
			this.validationMessage.Message = string.Empty;

			this.AccessType.BSDX_ACCESS_TYPE_IEN = this.EditAccessTypeID;
			this.AccessType.NAME = this.AccessTypeName;
			this.AccessType.INACTIVE = (this.isInactiveChecked == true) ? "YES" : "NO";
			this.AccessType.PREVENT_ACCESS = (this.isPreventChecked == true) ? "YES" : "NO";
			this.AccessType.DISPLAY_COLOR = this.AccessTypeColor;
			this.AccessType.RED = this.View.AccessTypeColorPicker.SelectedColor.R.ToString ();
			this.AccessType.GREEN = this.View.AccessTypeColorPicker.SelectedColor.G.ToString ();
			this.AccessType.BLUE = this.View.AccessTypeColorPicker.SelectedColor.B.ToString ();

			string errorMessage = this.dataAccessService.AddEditAccessType (this.AccessType);

			if (errorMessage != string.Empty) {
				this.validationMessage.IsValid = false;
				this.validationMessage.Title = "Add/Edit Access Type";
				this.validationMessage.Message = errorMessage;
				return;
			} else {
				this.eventAggregator.GetEvent<ManagementItemAddedEvent> ().Publish ("AccessType");
			}
		}

		public void InitialErrorMessage ()
		{
			this.validationMessage.IsValid = true;
			this.validationMessage.Title = string.Empty;
			this.validationMessage.Message = string.Empty;
		}

		public IAddAccessTypeView View { get; private set; }
		public string EditAccessTypeID { get; set; }
		public string AccessTypeName { get; set; }
		public string AccessTypeColor 
		{
			get
			{
				return accessTypeColor;
			}
			set
			{
				if (this.accessTypeColor != value) {
					this.accessTypeColor = value;
					this.OnPropertyChanged ("AccessTypeColor");
				}
			}
		}
		public bool IsInactiveChecked
		{
			get
			{
				return isInactiveChecked;
			}
			set
			{
				isInactiveChecked = value;
			}
		}
		public bool IsPreventChecked
		{
			get
			{
				return isPreventChecked;
			}
			set
			{
				isPreventChecked = value;
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
		public SchdAccessType AccessType { get; set; }

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
