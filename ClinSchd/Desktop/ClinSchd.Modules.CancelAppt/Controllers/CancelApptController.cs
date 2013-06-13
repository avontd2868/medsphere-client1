using System;

using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Regions;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Infrastructure.Events;
using ClinSchd.Modules.CancelAppt.Services;
using ClinSchd.Modules.CancelAppt.CancelAppt;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Unity;

namespace ClinSchd.Modules.CancelAppt.Controllers
{
	public class CancelApptController : ICancelApptController
	{
		private readonly IUnityContainer container;
		private readonly ICancelApptService CancelApptService;
		private readonly IEventAggregator eventAggregator;

		public CancelApptController (IUnityContainer container,
			ICancelApptService CancelApptService,
			IEventAggregator eventAggregator)
		{
			this.container = container;
			this.CancelApptService = CancelApptService;
			this.eventAggregator = eventAggregator;
		}

		public void Run()
		{			
			this.eventAggregator.GetEvent<LaunchCancelApptDialogEvent> ().Subscribe (LaunchCancelApptDialog, ThreadOption.UIThread, true);
		}

		public void LaunchCancelApptDialog (SchdAppointment appointment)
		{
			ICancelApptPresentationModel Model = container.Resolve<ICancelApptPresentationModel> ();
			Model.CancelApptAppointment (appointment);
			if (Model.ValidationMessage.IsValid) {
				this.CancelApptService.ShowDialog (
					Model.View,
					Model, () => Model.OnClose ());
			} else {
				Model.View.AlertUser (Model.ValidationMessage.Message, Model.ValidationMessage.Title);
				Model.ValidationMessage.IsValid = true;
				Model.ValidationMessage.Title = string.Empty;
				Model.ValidationMessage.Message = string.Empty;
			}
		}
	}
}
