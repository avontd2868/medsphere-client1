using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.Management.AddResourceUser;
using ClinSchd.Modules.Management.AddResourceUser.Controllers;
using ClinSchd.Modules.Management.AddResourceUser.Services;

namespace ClinSchd.Modules.Management.AddResourceUser
{
    public class ManagementAddResourceUserModule : IModule
    {
        private readonly IUnityContainer container;
		private readonly IEventAggregator eventAggregator;
		private IManagementAddResourceUserController controller;

		public ManagementAddResourceUserModule (IUnityContainer container, IEventAggregator eventAggregator)
        {
            this.container = container;
			this.eventAggregator = eventAggregator;
		}

		public void Initialize()
		{
			this.RegisterViewsAndServices();

			this.eventAggregator.GetEvent<LaunchAddResourceUserDialogEvent> ().Subscribe (LaunchAddResourceUserDialog, ThreadOption.UIThread, true);
			this.eventAggregator.GetEvent<AddEditResourceUserEvent> ().Subscribe (LoadResourceUser, ThreadOption.UIThread, true);
		}

		public void LaunchAddResourceUserDialog (string Title)
		{
			controller = this.container.Resolve<IManagementAddResourceUserController> ();

			if (Title == "Add Clinic User") {
				controller.Model.PaneTitle = Title;
			} else {
				controller.Model.PaneTitle = "Edit Clinic User";
			}
			controller.Model.OnPropertyChanged ("PaneTitle");

			if (controller.Model.ValidationMessage.IsValid) {
				controller.Run ();
			} else {
				controller.Model.View.AlertUser (controller.Model.ValidationMessage.Message, controller.Model.ValidationMessage.Title);
			}
		}

		public void LoadResourceUser (ResourceUser r)
		{
			controller.Model.ResourceUserName = r.USERNAME;
			controller.Model.ResourceUser.RESOURCE_ID = r.RESOURCE_ID;
			controller.Model.ResourceUser.USER_ID = r.USER_ID;
			controller.Model.OnPropertyChanged ("ResourceUserName");
			if (controller.Model.PaneTitle != "Add Clinic User") {
				controller.Model.ResourceUser = r;
				controller.Model.OnPropertyChanged ("ResourceUser");


				if ((r.OVERBOOK == "YES") ? true : false) {
					controller.Model.OverbookValue = "1";
				} else if ((r.MASTEROVERBOOK == "YES") ? true : false) {
					controller.Model.OverbookValue = "2";
				} else {
					controller.Model.OverbookValue = "0";
				}
				controller.Model.OnPropertyChanged ("OverbookValue");
			}
			controller.Model.IsUpdateChecked = r.MODIFY_APPTS== "YES" ? true : false;
			controller.Model.OnPropertyChanged ("IsUpdateChecked");
			controller.Model.IsModifyChecked = r.MODIFY_APPTS == "YES" ? true : false;
			controller.Model.OnPropertyChanged ("IsModifyChecked");
		}

        protected void RegisterViewsAndServices()
        {
			this.container.RegisterType<IManagementAddResourceUserController, ManagementAddResourceUserController>();
			this.container.RegisterType<IAddResourceUserView, AddResourceUserView>();
			this.container.RegisterType<IAddResourceUserPresentationModel, AddResourceUserPresentationModel>();
			this.container.RegisterType<IManagementAddResourceUserService, ManagementAddResourceUserService>();
        }
    }
}
