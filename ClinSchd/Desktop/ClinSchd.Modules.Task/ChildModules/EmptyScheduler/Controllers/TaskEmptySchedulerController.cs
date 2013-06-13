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
using ClinSchd.Modules.Task.EmptyScheduler;
using ClinSchd.Modules.Task.EmptyScheduler.Services;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.Task.Group;

namespace ClinSchd.Modules.Task.EmptyScheduler.Controllers
{
    public class TaskEmptySchedulerController : ITaskEmptySchedulerController
    {
		private readonly IUnityContainer container;
		private readonly IEventAggregator eventAggregator;
		private readonly IRegionManager regionManager;
		private readonly Factory<SchdResource> resourceFactory;
		private readonly IDataAccessService dataAccessService;
		private List<IEmptySchedulerView> ChildPanes { get; set; }

		public TaskEmptySchedulerController(
			IUnityContainer container,
			IDataAccessService dataAccessService,
			IEventAggregator eventAggregator,			
			IRegionManager regionManager,
			Factory<SchdResource> resourceFactory)
        {			
			this.container = container;
            this.eventAggregator = eventAggregator;
			this.dataAccessService = dataAccessService;
			this.regionManager = regionManager;
			this.resourceFactory = resourceFactory;
			ChildPanes = new List<IEmptySchedulerView> ();
		}

		public void Run()
		{
			this.container.RegisterType<IEmptySchedulerView, EmptySchedulerView>();
			this.container.RegisterType<IEmptySchedulerPresentationModel, EmptySchedulerPresentationModel>();
			this.container.RegisterType<ITaskEmptySchedulerService, TaskEmptySchedulerService>();

			this.eventAggregator.GetEvent<SchedulerDisplayEmptyPaneEvent> ().Subscribe (EmptySchedulerDisplay, ThreadOption.UIThread, true);
		}

		public void EmptySchedulerDisplay (string message)
		{
			IEmptySchedulerPresentationModel presenter = this.container.Resolve<IEmptySchedulerPresentationModel> ();
			this.regionManager.Regions[RegionNames.TaskGroupRegion].Add (presenter.View);
			presenter.Controller = this;
			presenter.Message = message;
			presenter.SelectedResource = resourceFactory.Create ();
			
			ChildPanes.Add (presenter.View);
		}

		public void RemoveViewFromList (IEmptySchedulerView View)
		{
			ChildPanes.Remove (View);
		}
	}
}
