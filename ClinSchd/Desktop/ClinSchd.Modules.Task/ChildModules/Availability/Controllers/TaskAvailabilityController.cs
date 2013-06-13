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
using ClinSchd.Infrastructure.Events;
using ClinSchd.Modules.Task.Availability;
using ClinSchd.Modules.Task.Availability.Services;
using ClinSchd.Infrastructure.Interfaces;

namespace ClinSchd.Modules.Task.Availability.Controllers
{
    public class TaskAvailabilityController : ITaskAvailabilityController
    {
		private readonly IUnityContainer container;
        private readonly IEventAggregator eventAggregator;
		private readonly IDataAccessService dataAccessService;
		private List<IAvailabilityView> ChildPanes { get; set; }
		private readonly IRegionManager regionManager;

		public TaskAvailabilityController (
			IUnityContainer container,
			IDataAccessService dataAccessService,
			IEventAggregator eventAggregator,
			IRegionManager regionManager)
        {			
			this.container = container;
            this.eventAggregator = eventAggregator;
			this.dataAccessService = dataAccessService;
			this.regionManager = regionManager;
			ChildPanes = new List<IAvailabilityView> ();
		}

		public void Run()
		{
			this.container.RegisterType<IAvailabilityView, AvailabilityView> ();
			this.container.RegisterType<IAvailabilityPresentationModel, AvailabilityPresentationModel> ();
			this.container.RegisterType<ITaskAvailabilityService, TaskAvailabilityService> ();

			this.eventAggregator.GetEvent<AvailibilityResourcesEvent> ().Subscribe (GetAvailibility, ThreadOption.UIThread, true);
			this.eventAggregator.GetEvent<CloseSchedulesEvent> ().Subscribe (CloseSchedules, ThreadOption.UIThread, true);
		}

		public void GetAvailibility (SchdResource selectedResource)
		{
			bool paneAlreadyExists = false;
			foreach (IAvailabilityView item in ChildPanes) {
				if (item.Model.SelectedResource.RESOURCEID == selectedResource.RESOURCEID) {
					item.Model.IsSelectedTab = true;
					paneAlreadyExists = true;
					break;
				}
			}
			if (!paneAlreadyExists) {
				IAvailabilityPresentationModel presenter = this.container.Resolve<IAvailabilityPresentationModel> ();
				this.regionManager.Regions[RegionNames.TaskGroupRegion].Add (presenter.View);
				presenter.Controller = this;
				presenter.SelectedResource = selectedResource;
				presenter.ResourceName = selectedResource.RESOURCE_NAME;
				presenter.PaneTitle = selectedResource.RESOURCE_NAME + " Availability";
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

				ChildPanes.Add (presenter.View);
			}
		}

		public void RemoveViewFromList (IAvailabilityView View)
		{
			ChildPanes.Remove (View);
		}

		private void CloseSchedules (string ClinicIEN)
		{
			List<IAvailabilityView> openPanes = new List<IAvailabilityView> ();
			foreach (IAvailabilityView item in ChildPanes) {
				if (item.Model.SelectedResource.Clinic.HOSPITAL_LOCATION_ID == ClinicIEN) {
					openPanes.Add (item);
					break;
				}
			}
			foreach (IAvailabilityView item in openPanes) {
				RemoveViewFromList (item);
				item.Close ();
			}
		}
	}
}
