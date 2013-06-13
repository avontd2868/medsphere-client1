using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using System.Collections;

using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.CancelAppt.Properties;
using ClinSchd.Modules.CancelAppt.CancelAppt;

namespace ClinSchd.Modules.CancelAppt.Services
{
	public class CancelApptService : ICancelApptService
	{
		public CancelApptService ()
		{
		}

		#region ICancelAppt Members

		public void ShowDialog<CancelApptPresentationModel>
			(ICancelApptView view, CancelApptPresentationModel viewModel, Action onDialogClose)
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
