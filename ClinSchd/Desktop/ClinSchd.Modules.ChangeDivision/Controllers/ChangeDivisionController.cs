using System;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Regions;
using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.ChangeDivision.Services;
using ClinSchd.Modules.ChangeDivision.ChangeDivision;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Presentation.Events;

namespace ClinSchd.Modules.ChangeDivision.Controllers
{
	public class ChangeDivisionController : IChangeDivisionController
    {
		private readonly IUnityContainer container;
		private readonly IChangeDivisionService ChangeDivisionService;
		private readonly IEventAggregator eventAggregator;

		public ChangeDivisionController (IUnityContainer container,
			IChangeDivisionService ChangeDivisionService,
			IEventAggregator eventAggregator)
		{
			this.container = container;
			this.ChangeDivisionService = ChangeDivisionService;
			this.eventAggregator = eventAggregator;
		}

		public void Run ()
		{
			this.eventAggregator.GetEvent<LaunchChangeDivisionDialogEvent> ().Subscribe (LaunchChangeDivisionDialog, ThreadOption.UIThread, true);
		}

		public void LaunchChangeDivisionDialog (string Title)
		{
			IChangeDivisionPresentationModel Model = container.Resolve<IChangeDivisionPresentationModel> ();
			Model.GetDivisions ();
			if (Model.ValidationMessage.IsValid) {
				this.ChangeDivisionService.ShowDialog (
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
