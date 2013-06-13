using System;
using System.Collections.Generic;
using System.ComponentModel;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Task.Controllers;
using Microsoft.Practices.Composite.Events;
using ClinSchd.Infrastructure;
using Microsoft.Practices.Composite.Presentation.Events;

namespace ClinSchd.Modules.Task.Group
{

	public class GroupPresentationModel : IGroupPresentationModel, INotifyPropertyChanged
    {
        private readonly ITaskService taskService;
        public event PropertyChangedEventHandler PropertyChanged;
		private readonly IEventAggregator eventAggregator;

		public GroupPresentationModel(IGroupView view, ITaskService taskService, IEventAggregator eventAggregator)
        {
			System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke (new Action (() => { HideLegend (); }), System.Windows.Threading.DispatcherPriority.ApplicationIdle);
            View = view;
            View.Model = this;
            this.taskService = taskService;
			this.eventAggregator = eventAggregator;
			this.eventAggregator.GetEvent<FocusSchedulerEvent> ().Subscribe (FocusScheduler, ThreadOption.UIThread, false);
        }

		public IGroupView View { get; private set; }

        public ITaskController Controller { get; set; }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

		private void FocusScheduler (object T)
		{
			View.FocusSchedulerGroup ();
		}

		public void HideLegend ()
		{
			LegendVisibility = System.Windows.Visibility.Collapsed;
			OnPropertyChanged ("LegendVisibility");
		}

		public void ShowLegend ()
		{
			LegendVisibility = System.Windows.Visibility.Visible;
			OnPropertyChanged ("LegendVisibility");
		}

		private System.Windows.Visibility legendVisibility;
		public System.Windows.Visibility LegendVisibility
		{
			get
			{
				return legendVisibility;
			}
			set
			{
				legendVisibility = value;
			}
		}
    }
}
