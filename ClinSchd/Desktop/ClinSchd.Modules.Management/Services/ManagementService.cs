using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Management.Properties;
using ClinSchd.Modules.Management.Group;

namespace ClinSchd.Modules.Management.Services
{
    public class ManagementService : IManagementService
    {
		public ManagementService ()
        {
		}

		#region IManagement Members

		public void ShowDialog(IGroupView view, IGroupPresentationModel viewModel, Action onDialogClose)
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
