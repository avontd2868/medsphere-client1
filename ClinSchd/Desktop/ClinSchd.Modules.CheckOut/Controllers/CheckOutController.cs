using System;

using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Regions;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.CheckOut.Services;
using ClinSchd.Modules.CheckOut.CheckOut;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Presentation.Events;

namespace ClinSchd.Modules.CheckOut.Controllers
{
	public class CheckOutController : ICheckOutController
	{
		private readonly IUnityContainer container;
		private readonly ICheckOutService CheckOutService;
		private readonly IEventAggregator eventAggregator;

		public CheckOutController (IUnityContainer container,
			ICheckOutService CheckOutService,
			IEventAggregator eventAggregator)
		{
			this.container = container;
			this.CheckOutService = CheckOutService;
			this.eventAggregator = eventAggregator;
		}

		public void Run()
		{
			this.eventAggregator.GetEvent<LaunchCheckOutDialogEvent> ().Subscribe (LaunchCheckOutDialog, ThreadOption.UIThread, true);
		}

		public void LaunchCheckOutDialog (SchdAppointment appointment)
		{
			ICheckOutPresentationModel Model = container.Resolve<ICheckOutPresentationModel> ();
			Model.GetCheckOutAppointment (appointment);
			if (Model.ValidationMessage.IsValid) {
				this.CheckOutService.ShowDialog (
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
