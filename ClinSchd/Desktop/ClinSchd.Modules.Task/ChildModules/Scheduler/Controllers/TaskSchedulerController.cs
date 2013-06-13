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
using ClinSchd.Modules.Task.Scheduler;
using ClinSchd.Modules.Task.Scheduler.Services;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Events;
using ClinSchd.Modules.Task.Group;
using System.Windows.Threading;

namespace ClinSchd.Modules.Task.Scheduler.Controllers
{
    public class TaskSchedulerController : ITaskSchedulerController
    {
		private readonly IUnityContainer container;
		private readonly IEventAggregator eventAggregator;
		private readonly IRegionManager regionManager;
		private readonly IDataAccessService dataAccessService;
		private List<ISchedulerView> ChildPanes { get; set; }
		private Patient Patient;

		public TaskSchedulerController(
			IUnityContainer container,
			IDataAccessService dataAccessService,
			IEventAggregator eventAggregator,			
			IRegionManager regionManager)
        {			
			this.container = container;
            this.eventAggregator = eventAggregator;
			this.dataAccessService = dataAccessService;
			this.regionManager = regionManager;
			ChildPanes = new List<ISchedulerView> ();
		}

		public void Run()
		{
			this.container.RegisterType<ISchedulerView, SchedulerView>();
			this.container.RegisterType<ISchedulerPresentationModel, SchedulerPresentationModel>();
			this.container.RegisterType<ITaskSchedulerService, TaskSchedulerService>();

			this.eventAggregator.GetEvent<SchedulerResourcesEvent> ().Subscribe (GetResource, ThreadOption.UIThread, true);
			this.eventAggregator.GetEvent<SchedulerDisplayEvent> ().Subscribe (SchedulerDisplay, ThreadOption.UIThread, true);
			this.eventAggregator.GetEvent<CloseSchedulesEvent> ().Subscribe (CloseSchedules, ThreadOption.UIThread, true);
			this.eventAggregator.GetEvent<PatientSelectedEvent> ().Subscribe (PatientSelected, ThreadOption.UIThread, false);
		}

		public void SchedulerDisplay (SchdResource selectedResource)
		{
			bool paneAlreadyExists = false;
			foreach (ISchedulerView item in ChildPanes) {
				if (item.Model.SelectedResource.RESOURCEID == selectedResource.RESOURCEID) {
					item.Model.IsSelectedTab = true;
					paneAlreadyExists = true;
					break;
				}
			}
			if (!paneAlreadyExists) {
				ISchedulerPresentationModel presenter = this.container.Resolve<ISchedulerPresentationModel> ();
				this.regionManager.Regions[RegionNames.TaskGroupRegion].Add (presenter.View);
				presenter.Controller = this;
				presenter.SelectedResource = selectedResource;
				presenter.PatientSelected (this.Patient);
				((TimeSlotTemplateSelector)presenter.View.Scheduler.TimeSlotTemplateSelector).startTime = presenter.View.Scheduler.VisibleRangeStart.Date.AddDays (1);
				((TimeSlotTemplateSelector)presenter.View.Scheduler.TimeSlotTemplateSelector).endTime = presenter.View.Scheduler.VisibleRangeEnd;
				((TimeSlotTemplateSelector)presenter.View.Scheduler.TimeSlotTemplateSelector).ParentResource = selectedResource;
				presenter.ResourceName = selectedResource.RESOURCE_NAME;
				presenter.PaneTitle = selectedResource.RESOURCE_NAME;
				presenter.VisibleStartDate = presenter.View.Scheduler.VisibleRangeStart.Date.AddDays (1);
				presenter.VisibleEndDate = presenter.View.Scheduler.VisibleRangeEnd;

				int timeScaleIndex = 3;
				switch (presenter.SelectedResource.TIMESCALE) {
					case "10":
					timeScaleIndex = 0;
					break;
					case "15":
					timeScaleIndex = 1;
					break;
					case "20":
					timeScaleIndex = 2;
					break;
					case "30":
					timeScaleIndex = 3;
					break;
				}
				this.eventAggregator.GetEvent<SelectTimeScaleEvent> ().Publish (timeScaleIndex);
				//presenter.View.Scheduler.WeekViewDefinition.TimeSlotLength = TimeSpan.FromMinutes (int.Parse(presenter.SelectedResource.TIMESCALE));
				this.eventAggregator.GetEvent<ViewTimeScaleEvent> ().Publish (presenter.SelectedResource.TIMESCALE);


				this.dataAccessService.RefreshAvailabilitySchedule (presenter.ResourceName, presenter.VisibleStartDate.AddDays (-1), presenter.VisibleEndDate.AddDays (1));
				ChildPanes.Add (presenter.View);
			}
		}

		public void GetResource (SchdResource selectedResource)
		{
			ISchedulerPresentationModel presenter = this.container.Resolve<ISchedulerPresentationModel> ();
			presenter.SelectedResource = selectedResource;
			presenter.ResourceName = selectedResource.RESOURCE_NAME;
			presenter.PaneTitle = selectedResource.RESOURCE_NAME;
			presenter.VisibleStartDate = presenter.View.Scheduler.VisibleRangeStart.Date.AddDays (1);
			presenter.VisibleEndDate = presenter.View.Scheduler.VisibleRangeEnd;

			presenter.AppointmentList = this.dataAccessService.GetAppointments (selectedResource.RESOURCE_NAME, presenter.VisibleStartDate.ToShortDateString (), presenter.VisibleEndDate.ToShortDateString () + "@23:59");
			presenter.View.Scheduler.AppointmentsSource = new SchdAppointmentSource (presenter.AppointmentList, presenter.AppointmentCategories, true).Appointments;
		}

		public void RemoveViewFromList (ISchedulerView View)
		{
			ChildPanes.Remove (View);
		}

		private void CloseSchedules (string ClinicIEN)
		{
			List<ISchedulerView> openPanes = new List<ISchedulerView> ();
			foreach (ISchedulerView item in ChildPanes) {
				if (item.Model.SelectedResource.Clinic.HOSPITAL_LOCATION_ID == ClinicIEN) {
					openPanes.Add (item);
					break;
				}
			}
			foreach (ISchedulerView item in openPanes) {
				RemoveViewFromList (item);
				item.Close ();
			}
		}

		private void PatientSelected (ClinSchd.Infrastructure.Models.Patient selectedPatient)
		{
			this.Patient = selectedPatient;
		}
	}
}
