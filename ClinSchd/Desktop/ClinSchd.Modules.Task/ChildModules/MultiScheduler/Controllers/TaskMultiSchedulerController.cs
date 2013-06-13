using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Regions;
using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Task.MultiScheduler;
using ClinSchd.Modules.Task.MultiScheduler.Services;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Events;
using ClinSchd.Modules.Task.Scheduler;
using Microsoft.Practices.Composite.Presentation.Regions;
using ClinSchd.Modules.Task.Group;
using Telerik.Windows.Controls;

namespace ClinSchd.Modules.Task.MultiScheduler.Controllers
{
    public class TaskMultiSchedulerController : ITaskMultiSchedulerController
    {
		private readonly IUnityContainer container;
		private readonly IRegionManager regionManager;
        private readonly IEventAggregator eventAggregator;
		private readonly IDataAccessService dataAccessService;
		private List<IMultiSchedulerView> ChildPanes { get; set; }

		public TaskMultiSchedulerController(
			IUnityContainer container,
			IDataAccessService dataAccessService,
			IEventAggregator eventAggregator,
			IRegionManager regionManager)
        {			
			this.container = container;
            this.eventAggregator = eventAggregator;
			this.dataAccessService = dataAccessService;
			this.regionManager = regionManager;
			ChildPanes = new List<IMultiSchedulerView> ();
		}

		public void Run()
		{
			this.eventAggregator.GetEvent<SchedulerDisplayGroupEvent> ().Subscribe (SchedulerDisplayGroup, ThreadOption.UIThread, true);
		}

		public void SchedulerDisplayGroup (SchdResourceGroup selectedResourceGroup)
		{
			bool paneAlreadyExists = false;
			foreach (IMultiSchedulerView item in ChildPanes) {
				if (item.Model.SelectedResourceGroup.Name == selectedResourceGroup.Name) {
					item.Model.IsSelectedTab = true;
					paneAlreadyExists = true;
					break;
				}
			}

			if (!paneAlreadyExists) {
				IMultiSchedulerPresentationModel groupPresentationModel = this.container.Resolve<IMultiSchedulerPresentationModel> ();
				RegionManager.SetRegionManager (groupPresentationModel.View.MultiSchedulerGroupControl, this.regionManager);
				groupPresentationModel.Controller = this;
				groupPresentationModel.PaneTitle = selectedResourceGroup.Name;
				groupPresentationModel.SelectedResourceGroup = selectedResourceGroup;
				this.regionManager.Regions[RegionNames.TaskGroupRegion].Add (groupPresentationModel.View);

				foreach (SchdResource selectedResource in selectedResourceGroup.Resources) {
					ISchedulerPresentationModel presenter = this.container.Resolve<ISchedulerPresentationModel> ();
					RadPaneGroup radPaneGroup = new RadPaneGroup ();
					radPaneGroup.Items.Add (presenter.View);
					groupPresentationModel.View.MultiSchedulerGroupControl.Items.Add (radPaneGroup);
					//presenter.Controller = this;
					presenter.SelectedResource = selectedResource;
					((TimeSlotTemplateSelector)presenter.View.Scheduler.TimeSlotTemplateSelector).startTime = presenter.View.Scheduler.VisibleRangeStart.Date.AddDays (1);
					((TimeSlotTemplateSelector)presenter.View.Scheduler.TimeSlotTemplateSelector).endTime = presenter.View.Scheduler.VisibleRangeEnd;
					((TimeSlotTemplateSelector)presenter.View.Scheduler.TimeSlotTemplateSelector).ParentResource = selectedResource;
					presenter.ResourceName = selectedResource.RESOURCE_NAME;
					presenter.PaneTitle = selectedResource.RESOURCE_NAME;
					presenter.VisibleStartDate = presenter.View.Scheduler.VisibleRangeStart.Date.AddDays (1);
					presenter.VisibleEndDate = presenter.View.Scheduler.VisibleRangeEnd;
					this.eventAggregator.GetEvent<SelectTimeScaleEvent> ().Publish (3);

					presenter.AppointmentList = this.dataAccessService.GetAppointments (selectedResource.RESOURCE_NAME, presenter.VisibleStartDate.ToShortDateString (), presenter.VisibleEndDate.ToShortDateString () + "@23:59");
					presenter.View.Scheduler.AppointmentsSource = new SchdAppointmentSource (presenter.AppointmentList, presenter.AppointmentCategories, true).Appointments;
					presenter.IsSelectedTab = true;
					presenter.ViewModeChanged ("Day");
				}
				ChildPanes.Add (groupPresentationModel.View);
			}
		}

		public void RemoveViewFromList (IMultiSchedulerView View)
		{
			ChildPanes.Remove (View);
		}
	}
}
