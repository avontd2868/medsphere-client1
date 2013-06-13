using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.FindAppt.FindAppt;
using ClinSchd.Modules.FindAppt.Controllers;
using ClinSchd.Modules.FindAppt.Services;

namespace ClinSchd.Modules.FindAppt
{
	public class FindApptModule : IModule
    {
        private readonly IUnityContainer container;
		private readonly IEventAggregator eventAggregator;
		private IFindApptController controller;

		public FindApptModule (IUnityContainer container, IEventAggregator eventAggregator)
        {
            this.container = container;
			this.eventAggregator = eventAggregator;
		}

        public void Initialize()
        {
            this.RegisterViewsAndServices();
			this.eventAggregator.GetEvent<LaunchFindApptDialogEvent> ().Subscribe (LaunchFindApptDialog, ThreadOption.UIThread, true);
        }

		public void LaunchFindApptDialog (IList<SchdResource> selectedResources)
		{
			controller = this.container.Resolve<IFindApptController> ();
			controller.Model.SelectedResourceList = selectedResources;
			if (controller.Model.ValidationMessage.IsValid) {
				controller.Run ();
			} else {
				controller.Model.View.AlertUser (controller.Model.ValidationMessage.Message, controller.Model.ValidationMessage.Title);
				controller.Model.ValidationMessage.IsValid = true;
				controller.Model.ValidationMessage.Title = string.Empty;
				controller.Model.ValidationMessage.Message = string.Empty;
			}
		}

        protected void RegisterViewsAndServices()
        {
			this.container.RegisterType<IFindApptController, FindApptController> ();
			this.container.RegisterType<IFindApptView, FindApptView> ();
			this.container.RegisterType<IFindApptPresentationModel, FindApptPresentationModel> ();
			this.container.RegisterType<IFindApptService, FindApptService> ();
        }
    }
}
