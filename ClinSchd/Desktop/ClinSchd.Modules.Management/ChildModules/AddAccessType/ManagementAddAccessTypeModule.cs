using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.Management.AddAccessType;
using ClinSchd.Modules.Management.AddAccessType.Controllers;
using ClinSchd.Modules.Management.AddAccessType.Services;

namespace ClinSchd.Modules.Management.AddAccessType
{
    public class ManagementAddAccessTypeModule : IModule
    {
        private readonly IUnityContainer container;
		private readonly IEventAggregator eventAggregator;
		private IManagementAddAccessTypeController controller;

		public ManagementAddAccessTypeModule (IUnityContainer container, IEventAggregator eventAggregator)
        {
            this.container = container;
			this.eventAggregator = eventAggregator;
		}

		public void Initialize()
		{
			this.RegisterViewsAndServices();
			this.eventAggregator.GetEvent<LaunchAddAccessTypeDialogEvent> ().Subscribe (LaunchAddAccessTypeDialog, ThreadOption.UIThread, true);
		}

		public void LaunchAddAccessTypeDialog (string Title)
		{
			controller = this.container.Resolve<IManagementAddAccessTypeController> ();
			if (Title == "Add Access Type") {
				controller.Model.PaneTitle = Title;
				controller.Model.EditAccessTypeID = string.Empty;
			} else {
				controller.Model.PaneTitle = "Edit Access Type";
				controller.Model.EditAccessTypeID = Title;
				controller.Model.LoadEditAccessType ();
			}
			controller.Model.OnPropertyChanged ("PaneTitle");

			if (controller.Model.ValidationMessage.IsValid) {
				controller.Run ();
			} else {
				controller.Model.View.AlertUser (controller.Model.ValidationMessage.Message, controller.Model.ValidationMessage.Title);
			}
		}

        protected void RegisterViewsAndServices()
        {
			this.container.RegisterType<IManagementAddAccessTypeController, ManagementAddAccessTypeController>();
			this.container.RegisterType<IAddAccessTypeView, AddAccessTypeView>();
			this.container.RegisterType<IAddAccessTypePresentationModel, AddAccessTypePresentationModel>();
			this.container.RegisterType<IManagementAddAccessTypeService, ManagementAddAccessTypeService>();
        }
    }
}
