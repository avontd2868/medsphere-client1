using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Management.AddResourceGroup;

namespace ClinSchd.Modules.Management.AddResourceGroup.Services
{
	public class ManagementAddResourceGroupService : IManagementAddResourceGroupService
    {
		public ManagementAddResourceGroupService()
        {
		}

		#region IManagementAddResourceGroup Members

		public void ShowDialog<AddResourceGroupPresentationModel>
			(IAddResourceGroupView view, AddResourceGroupPresentationModel viewModel, Action onDialogClose)
		{
			view.DataContext = viewModel;
			if (onDialogClose != null) {
				view.Closed += (sender, e) => onDialogClose ();
			}
			view.ShowDialog ();
		}

        #endregion
    }
}
