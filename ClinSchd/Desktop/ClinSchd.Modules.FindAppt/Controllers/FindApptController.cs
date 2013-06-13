using System;

using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Regions;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.FindAppt.Services;
using ClinSchd.Modules.FindAppt.FindAppt;

namespace ClinSchd.Modules.FindAppt.Controllers
{
	public class FindApptController : IFindApptController
	{
		private readonly IRegionManager regionManager;
		private readonly IFindApptPresentationModel FindApptPresentationModel;
		private readonly IFindApptService FindApptService;
		private readonly IEventAggregator eventAggregator;

		public FindApptController (IRegionManager regionManager,
			IFindApptPresentationModel FindApptPresentationModel,
			IFindApptService FindApptService,
			IEventAggregator eventAggregator)
		{
			this.regionManager = regionManager;
			this.FindApptPresentationModel = FindApptPresentationModel;
			this.FindApptService = FindApptService;
			this.eventAggregator = eventAggregator;
		}

		public IFindApptPresentationModel Model { get { return this.FindApptPresentationModel; } }

		public void Run()
		{
			this.FindApptService.ShowDialog (
				this.FindApptPresentationModel.View,
				this.FindApptPresentationModel, () => FindApptPresentationModel.OnClose ());
		}
	}
}
