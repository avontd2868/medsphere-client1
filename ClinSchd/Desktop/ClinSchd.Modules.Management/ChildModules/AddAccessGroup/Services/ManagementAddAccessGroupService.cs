using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Management.AddAccessGroup;

namespace ClinSchd.Modules.Management.AddAccessGroup.Services
{
	public class ManagementAddAccessGroupService : IManagementAddAccessGroupService
    {
		public ManagementAddAccessGroupService()
        {
		}

		#region IManagementAddAccessGroup Members

		public void ShowDialog<AddAccessGroupPresentationModel>
			(IAddAccessGroupView view, AddAccessGroupPresentationModel viewModel, Action onDialogClose)
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
