using System;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Regions;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.MarkAsNoShow.Services;
using ClinSchd.Modules.MarkAsNoShow.MarkAsNoShow;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Presentation.Events;

namespace ClinSchd.Modules.MarkAsNoShow.Controllers
{
	public class MarkAsNoShowController : IMarkAsNoShowController
	{
		private readonly IMarkAsNoShowService markAsNoShowService;
		private readonly IEventAggregator eventAggregator;
		private readonly IUnityContainer container;

		public MarkAsNoShowController (IUnityContainer container,
			IMarkAsNoShowService markAsNoShowService, 
			IEventAggregator eventAggregator)
		{
			this.markAsNoShowService = markAsNoShowService;
			this.eventAggregator = eventAggregator;
			this.container = container;
		}

		public void Run ()
		{
			this.eventAggregator.GetEvent<LaunchMarkAsNoShowDialogEvent> ().Subscribe (LaunchMarkAsNoShowDialog, ThreadOption.UIThread, true);
		}

		public void LaunchMarkAsNoShowDialog (SchdAppointment appointment)
		{
			IMarkAsNoShowPresentationModel Model = container.Resolve<IMarkAsNoShowPresentationModel> ();
			if (Convert.ToDateTime (appointment.START_TIME).Date > DateTime.Today.Date) {
				Model.ValidationMessage.IsValid = false;
				Model.ValidationMessage.Title = "AutoRebook Appointment";
				Model.ValidationMessage.Message = "The appointment for " + appointment.PATIENTNAME + " is in the future.  Are you sure you want to No-Show?";

				if (!Model.View.ConfirmUser (Model.ValidationMessage.Message, Model.ValidationMessage.Title)) {
					return;
				}
			} 
			Model.NoShowAppointment (appointment);
			this.markAsNoShowService.ShowDialog (Model.View,Model, () => Model.OnClose ());
		}
	}
}
