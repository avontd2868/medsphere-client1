using System;

using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Regions;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.CheckIn.Services;
using ClinSchd.Modules.CheckIn.CheckIn;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Presentation.Events;

namespace ClinSchd.Modules.CheckIn.Controllers
{
	public class CheckInController : ICheckInController
	{
		private readonly IUnityContainer container;
		private readonly ICheckInService CheckInService;
		private readonly IEventAggregator eventAggregator;

		public CheckInController (IUnityContainer container,
			ICheckInService CheckInService,
			IEventAggregator eventAggregator)
		{
			this.container = container;
			this.CheckInService = CheckInService;
			this.eventAggregator = eventAggregator;
		}

		public void Run()
		{
			this.eventAggregator.GetEvent<LaunchCheckInDialogEvent> ().Subscribe (LaunchCheckInDialog, ThreadOption.UIThread, true);
		}	

		public void LaunchCheckInDialog (SchdAppointment appointment)
		{
			ICheckInPresentationModel Model = container.Resolve<ICheckInPresentationModel> ();
			Model.GetCheckInAppointment (appointment);
			if (Model.ValidationMessage.IsValid) {				
				this.CheckInService.ShowDialog (
				Model.View,
				Model, () => Model.OnClose ());
			} else {
				Model.View.AlertUser (Model.ValidationMessage.Message, Model.ValidationMessage.Title);
			}
		}

	}
}
