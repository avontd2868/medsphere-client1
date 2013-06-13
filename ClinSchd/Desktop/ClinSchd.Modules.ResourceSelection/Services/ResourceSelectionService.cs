using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using System.Collections;

using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Models;
//using ClinSchd.Modules.ResourceSelection.Properties;
using ClinSchd.Modules.ResourceSelection.ResourceSelection;

namespace ClinSchd.Modules.ResourceSelection.Services
{
    public class ResourceSelectionService : IResourceSelectionService
    {
        public ResourceSelectionService()
        {
		}

		#region IResourceSelection Members

		public void ShowDialog<ResourceSelectionPresentationModel>
			(IResourceSelectionView view, ResourceSelectionPresentationModel viewModel, Action onDialogClose)
		{
			view.DataContext = viewModel;
			if (onDialogClose != null)
			{
				view.Closed += (sender, e) => onDialogClose();
			}
			view.ShowDialog();
		}

        #endregion
    }
}
