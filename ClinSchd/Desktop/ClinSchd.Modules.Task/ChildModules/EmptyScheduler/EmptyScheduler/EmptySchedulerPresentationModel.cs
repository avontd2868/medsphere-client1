using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Presentation.Commands;
using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Regions;
using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Models;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Scheduler;
using ClinSchd.Modules.Task.Group;
using ClinSchd.Modules.Task.EmptyScheduler.Controllers;
using Microsoft.Practices.Composite.Presentation.Regions;

namespace ClinSchd.Modules.Task.EmptyScheduler
{
	public class EmptySchedulerPresentationModel : IEmptySchedulerPresentationModel, ITaskPresentationModel, INotifyPropertyChanged
    {
		private readonly IRegionManager regionManager;
		private readonly IDataAccessService dataAccessService;
		private readonly IEventAggregator eventAggregator;
		public event PropertyChangedEventHandler PropertyChanged;
		private string _paneTitle = "";
		private bool _isSelectedTab = false;
		private SchdResource selectedResource;
		private SchdResourceGroup selectedResourceGroup;
		private bool _isEnableUpdateAppointment = true;

		public EmptySchedulerPresentationModel(
			IEmptySchedulerView view,
			IRegionManager regionManager,
			IDataAccessService dataAccessService,
			IEventAggregator eventAggregator)
        {
            View = view;
            View.Model = this;
			this.regionManager = regionManager;
			this.dataAccessService = dataAccessService;
			this.eventAggregator = eventAggregator;

			ResetUnselectedTabCommand = new DelegateCommand<string> (ExecuteEnableAppointmentButtonsCommand, CanExecuteEnableAppointmentButtonsCommand);
			TaskTabSelectedCommand = new DelegateCommand<object> (ExecuteEmptySchedulerTabSelectedCommand, CanExecuteEmptySchedulerTabSelectedCommand);
		}

		public string Message { get; set; }
		public DelegateCommand<string> ResetUnselectedTabCommand { get; private set;}
		public DelegateCommand<object> TaskTabSelectedCommand { get; private set; }

		public void ExecuteEmptySchedulerTabSelectedCommand (object resource)
		{
			SelectedResource = resource as SchdResource;
			this.eventAggregator.GetEvent<ResourceSelectedEvent> ().Publish (SelectedResource);
			OnPropertyChanged ("Message");
		}

		public void ExecuteEnableAppointmentButtonsCommand (string command)
		{
		}

		public bool CanExecuteEmptySchedulerTabSelectedCommand (object resource)
		{
			return true;
		}

		public bool CanExecuteEnableAppointmentButtonsCommand (string command)
		{
			return true;
		}

		#region IEmptySchedulerPresentationModel Members

		public IEmptySchedulerView View { get; private set; }
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
		public bool IsEnableUpdateAppointment
		{
			get
			{
				return _isEnableUpdateAppointment;
			}
			set
			{
				if (this._isEnableUpdateAppointment != value) {
					this._isEnableUpdateAppointment = value;
					this.OnPropertyChanged ("IsEnableUpdateAppointment");
				}
			}
		}
		public bool IsSelectedTab
		{
			get
			{
				return _isSelectedTab;
			}
			set
			{
				_isSelectedTab = value;
				OnPropertyChanged ("IsSelectedTab");
			}
		}
		public SchdResource SelectedResource
		{
			get
			{
				return selectedResource;
			}
			set
			{
				if (this.selectedResource != value && value != null) {
					this.selectedResource = value;
				}
			}
		}
		public SchdResourceGroup SelectedResourceGroup
		{
			get
			{
				return selectedResourceGroup;
			}
			set
			{
				if (this.selectedResourceGroup != value && value != null) {
					this.selectedResourceGroup = value;
				}
			}
		}
		public ITaskEmptySchedulerController Controller { get; set; }
		public void ClearViewFromList ()
		{
			if (Controller != null) {
				Controller.RemoveViewFromList (View);
			}
		}
		#endregion

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
