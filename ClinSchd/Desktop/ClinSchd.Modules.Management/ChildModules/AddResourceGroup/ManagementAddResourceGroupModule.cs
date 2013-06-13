using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using System.Collections.Generic;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.Management.AddResourceGroup;
using ClinSchd.Modules.Management.AddResourceGroup.Controllers;
using ClinSchd.Modules.Management.AddResourceGroup.Services;

namespace ClinSchd.Modules.Management.AddResourceGroup
{
    public class ManagementAddResourceGroupModule : IModule
    {
        private readonly IUnityContainer container;
		private readonly IEventAggregator eventAggregator;
		private readonly IDataAccessService dataAccessService;
		private IManagementAddResourceGroupController controller;

		public ManagementAddResourceGroupModule (IUnityContainer container, IDataAccessService dataAccessService, IEventAggregator eventAggregator)
        {
            this.container = container;
			this.eventAggregator = eventAggregator;
			this.dataAccessService = dataAccessService;
		}

		public void Initialize()
		{
			this.RegisterViewsAndServices();
			this.eventAggregator.GetEvent<LaunchAddResourceGroupDialogEvent> ().Subscribe (LaunchAddResourceGroupDialog, ThreadOption.UIThread, true);
		}

		public void LaunchAddResourceGroupDialog (string Title)
		{
			controller = this.container.Resolve<IManagementAddResourceGroupController> ();

			if (Title == "Add Clinic Group") {
				controller.Model.PaneTitle = Title;
				controller.Model.ClinicGroupID = string.Empty;
				controller.Model.ClinicGroupName = string.Empty;
			} else {
				controller.Model.PaneTitle = "Edit Clinic Group";
				controller.Model.ClinicGroupID = Title;
				controller.Model.ClinicGroupName = GetGroupName (Title);
			}
			controller.Model.OnPropertyChanged ("PaneTitle");
			controller.Model.OnPropertyChanged ("ClinicGroupName");

			if (controller.Model.ValidationMessage.IsValid) {
				controller.Run ();
			} else {
				controller.Model.View.AlertUser (controller.Model.ValidationMessage.Message, controller.Model.ValidationMessage.Title);
			}
		}

		private string GetGroupName (string groupID)
		{
			string groupName = string.Empty;
			IList<NameValue> g = this.dataAccessService.GetResourceGroupList ();
			foreach (NameValue group in g) {
				if (groupID == group.Value)
				{
					groupName = group.Name;
					break;
				}
			}
			return groupName;
		}

        protected void RegisterViewsAndServices()
        {
			this.container.RegisterType<IManagementAddResourceGroupController, ManagementAddResourceGroupController>();
			this.container.RegisterType<IAddResourceGroupView, AddResourceGroupView>();
			this.container.RegisterType<IAddResourceGroupPresentationModel, AddResourceGroupPresentationModel>();
			this.container.RegisterType<IManagementAddResourceGroupService, ManagementAddResourceGroupService>();
        }
    }
}
