using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Management.AddResourceUser;

namespace ClinSchd.Modules.Management.AddResourceUser.Services
{
	public class ManagementAddResourceUserService : IManagementAddResourceUserService
    {
		public ManagementAddResourceUserService()
        {
		}

		#region IManagementAddResourceUser Members

		public void ShowDialog<AddResourceUserPresentationModel>
			(IAddResourceUserView view, AddResourceUserPresentationModel viewModel, Action onDialogClose)
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
