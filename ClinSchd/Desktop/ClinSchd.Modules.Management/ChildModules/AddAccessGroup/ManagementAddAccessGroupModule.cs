using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using System.Collections.Generic;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.Management.AddAccessGroup;
using ClinSchd.Modules.Management.AddAccessGroup.Controllers;
using ClinSchd.Modules.Management.AddAccessGroup.Services;

namespace ClinSchd.Modules.Management.AddAccessGroup
{
    public class ManagementAddAccessGroupModule : IModule
    {
        private readonly IUnityContainer container;
		private readonly IEventAggregator eventAggregator;
		private readonly IDataAccessService dataAccessService;
		private IManagementAddAccessGroupController controller;

		public ManagementAddAccessGroupModule (IUnityContainer container, IDataAccessService dataAccessService, IEventAggregator eventAggregator)
        {
            this.container = container;
			this.eventAggregator = eventAggregator;
			this.dataAccessService = dataAccessService;
		}

		public void Initialize()
		{
			this.RegisterViewsAndServices();
			this.eventAggregator.GetEvent<LaunchAddAccessGroupDialogEvent> ().Subscribe (LaunchAddAccessGroupDialog, ThreadOption.UIThread, true);
		}

		public void LaunchAddAccessGroupDialog (string Title)
		{
			controller = this.container.Resolve<IManagementAddAccessGroupController> ();
			if (Title == "Add Access Group") {
				controller.Model.PaneTitle = Title;
				controller.Model.AccessTypeGroupID = string.Empty;
				controller.Model.AccessTypeGroupName = string.Empty;
			} else {
				controller.Model.PaneTitle = "Edit Access Group";
				controller.Model.AccessTypeGroupID = Title;
				controller.Model.AccessTypeGroupName = GetGroupName (Title);
			}
			controller.Model.OnPropertyChanged ("PaneTitle");
			controller.Model.OnPropertyChanged ("AccessTypeGroupName");

			if (controller.Model.ValidationMessage.IsValid) {
				controller.Run ();
			} else {
				controller.Model.View.AlertUser (controller.Model.ValidationMessage.Message, controller.Model.ValidationMessage.Title);
			}
		}

		private string GetGroupName (string groupID)
		{
			string groupName = string.Empty;
			IList<NameValue> g = this.dataAccessService.GetAccessGroups ();
			foreach (NameValue group in g) {
				if (groupID == group.Value) {
					groupName = group.Name;
					break;
				}
			}
			return groupName;
		}

        protected void RegisterViewsAndServices()
        {
			this.container.RegisterType<IManagementAddAccessGroupController, ManagementAddAccessGroupController>();
			this.container.RegisterType<IAddAccessGroupView, AddAccessGroupView>();
			this.container.RegisterType<IAddAccessGroupPresentationModel, AddAccessGroupPresentationModel>();
			this.container.RegisterType<IManagementAddAccessGroupService, ManagementAddAccessGroupService>();
        }
    }
}
