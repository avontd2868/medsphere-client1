using System;
using System.Collections.Generic;
using System.ComponentModel;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Task.MultiScheduler.Controllers;
using ClinSchd.Modules.Task.Group;
using Microsoft.Practices.Unity;
using ClinSchd.Infrastructure;
using Microsoft.Practices.Composite.Presentation.Commands;
using Microsoft.Practices.Composite.Events;

namespace ClinSchd.Modules.Task.MultiScheduler
{

	public class MultiSchedulerPresentationModel : IMultiSchedulerPresentationModel, ITaskPresentationModel, INotifyPropertyChanged
    {
        private readonly ITaskService taskService;
		private readonly IEventAggregator eventAggregator;
        public event PropertyChangedEventHandler PropertyChanged;
		private SchdResourceGroup selectedResourceGroup;
		private SchdResource selectedResource;
		private bool _isEnableUpdateAppointment = true;

		public MultiSchedulerPresentationModel (IMultiSchedulerView view, ITaskService taskService, IEventAggregator eventAggregator)
        {
            View = view;
            View.Model = this;
            this.taskService = taskService;
			this.eventAggregator = eventAggregator;
			ResetUnselectedTabCommand = new DelegateCommand<string> (ExecuteEnableAppointmentButtonsCommand, CanExecuteEnableAppointmentButtonsCommand);
			TaskTabSelectedCommand = new DelegateCommand<object> (ExecuteSchedulerTabSelectedCommand, CanExecuteSchedulerTabSelectedCommand);
        }

		public void ExecuteSchedulerTabSelectedCommand (object resourceGroup)
		{
			SelectedResourceGroup = resourceGroup as SchdResourceGroup;
			this.eventAggregator.GetEvent<ResourceGroupSelectedEvent> ().Publish (SelectedResourceGroup);
		}

		public void ExecuteEnableAppointmentButtonsCommand (string command)
		{
			
		}

		public bool CanExecuteSchedulerTabSelectedCommand (object resource)
		{
			return true;
		}

		public bool CanExecuteEnableAppointmentButtonsCommand (string command)
		{
			return true;
		}

		public IMultiSchedulerView View { get; private set; }

        public ITaskMultiSchedulerController Controller { get; set; }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

		#region ITaskPresentationModel Members

		public bool IsSelectedTab { get; set; }
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

		private string paneTitle;
		public string PaneTitle
		{
			get
			{
				return this.paneTitle;
			}
			set
			{
				this.paneTitle = value;
				OnPropertyChanged ("PaneTitle");
			}
		}

		[Dependency]
		public Factory<SchdAppointment> SchdAppointmentFactory { get; set; }

		public DelegateCommand<object> TaskTabSelectedCommand { get; private set; }

		public DelegateCommand<string> ResetUnselectedTabCommand { get; private set; }

		public void ClearViewFromList ()
		{
			this.Controller.RemoveViewFromList (this.View);
		}

		#endregion
	}
}
