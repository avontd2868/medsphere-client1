using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Management.AddResource;

namespace ClinSchd.Modules.Management.AddResource.Services
{
	public class ManagementAddResourceService : IManagementAddResourceService
    {
		public ManagementAddResourceService()
        {
		}

		#region IManagementAddResource Members

		public void ShowDialog<AddResourcePresentationModel>
			(IAddResourceView view, AddResourcePresentationModel viewModel, Action onDialogClose)
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
