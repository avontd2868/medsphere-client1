using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using System.Collections;

using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.FindAppt.Properties;
using ClinSchd.Modules.FindAppt.FindAppt;

namespace ClinSchd.Modules.FindAppt.Services
{
	public class FindApptService : IFindApptService
	{
		public FindApptService ()
		{
		}

		#region IFindAppt Members

		public void ShowDialog<FindApptPresentationModel>
			(IFindApptView view, FindApptPresentationModel viewModel, Action onDialogClose)
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
