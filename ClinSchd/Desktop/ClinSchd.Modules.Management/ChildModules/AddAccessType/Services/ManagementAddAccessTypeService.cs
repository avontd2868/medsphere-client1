using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Management.AddAccessType;

namespace ClinSchd.Modules.Management.AddAccessType.Services
{
	public class ManagementAddAccessTypeService : IManagementAddAccessTypeService
    {
		public ManagementAddAccessTypeService()
        {
		}

		#region IManagementAddAccessType Members

		public void ShowDialog<AddAccessTypePresentationModel>
			(IAddAccessTypeView view, AddAccessTypePresentationModel viewModel, Action onDialogClose)
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
