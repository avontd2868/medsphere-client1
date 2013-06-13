using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using System.Collections;

using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.ChangeDivision.Properties;
using ClinSchd.Modules.ChangeDivision.ChangeDivision;

namespace ClinSchd.Modules.ChangeDivision.Services
{
	public class ChangeDivisionService : IChangeDivisionService
    {
		public ChangeDivisionService ()
        {
		}

		#region IChangeDivision Members

		public void ShowDialog<ChangeDivisionPresentationModel>
			(IChangeDivisionView view, ChangeDivisionPresentationModel viewModel, Action onDialogClose)
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
