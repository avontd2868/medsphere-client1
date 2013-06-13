using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.Management.AddResource;
using ClinSchd.Modules.Management.AddResource.Controllers;
using ClinSchd.Modules.Management.AddResource.Services;

namespace ClinSchd.Modules.Management.AddResource
{
    public class ManagementAddResourceModule : IModule
    {
        private readonly IUnityContainer container;
		private readonly IEventAggregator eventAggregator;
		private IManagementAddResourceController controller;

		public ManagementAddResourceModule (IUnityContainer container, IEventAggregator eventAggregator)
        {
            this.container = container;
			this.eventAggregator = eventAggregator;
		}

		public void Initialize()
		{
			this.RegisterViewsAndServices();

			this.eventAggregator.GetEvent<LaunchAddResourceDialogEvent> ().Subscribe (LaunchAddResourceDialog, ThreadOption.UIThread, true);
		}

		public void LaunchAddResourceDialog (string Title)
		{
			controller = this.container.Resolve<IManagementAddResourceController> ();

			if (Title == "Add Clinic") {
				controller.Model.PaneTitle = Title;
				controller.Model.IsClinicEnabled = true;
				controller.Model.EditClinicID = string.Empty;
			} else {
				controller.Model.PaneTitle = "Edit Clinic";
				controller.Model.IsClinicEnabled = false;
				controller.Model.EditClinicID = Title;
				controller.Model.LoadEditClinic ();
			}
			controller.Model.OnPropertyChanged ("PaneTitle");
			controller.Model.OnPropertyChanged ("IsClinicEnabled");

			if (controller.Model.ValidationMessage.IsValid) {
				controller.Run ();
				controller.Model.ValidationMessage.IsValid = true;
				controller.Model.ValidationMessage.Title = string.Empty;
				controller.Model.ValidationMessage.Message = string.Empty;

			} else {
				controller.Model.View.AlertUser (controller.Model.ValidationMessage.Message, controller.Model.ValidationMessage.Title);
			}
		}

        protected void RegisterViewsAndServices()
        {
			this.container.RegisterType<IManagementAddResourceController, ManagementAddResourceController>();
			this.container.RegisterType<IAddResourceView, AddResourceView>();
			this.container.RegisterType<IAddResourcePresentationModel, AddResourcePresentationModel>();
			this.container.RegisterType<IManagementAddResourceService, ManagementAddResourceService>();
        }
    }
}
