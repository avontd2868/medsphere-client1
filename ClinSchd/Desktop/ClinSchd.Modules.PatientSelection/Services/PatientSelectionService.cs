using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using System.Collections;

using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.PatientSelection.Properties;
using ClinSchd.Modules.PatientSelection.PatientSelection;

namespace ClinSchd.Modules.PatientSelection.Services
{
    public class PatientSelectionService : IPatientSelectionService
    {
        public PatientSelectionService()
        {
        }

        #region IPatientSelection Members

		public void ShowDialog<PatientSelectionPresentationModel>
			(IPatientSelectionView view, PatientSelectionPresentationModel viewModel, Action onDialogClose)
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
