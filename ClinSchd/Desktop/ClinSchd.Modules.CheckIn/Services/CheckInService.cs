using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using System.Collections;

using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.CheckIn.Properties;
using ClinSchd.Modules.CheckIn.CheckIn;

namespace ClinSchd.Modules.CheckIn.Services
{
	public class CheckInService : ICheckInService
	{
		public CheckInService ()
		{
		}

		#region ICheckIn Members

		public void ShowDialog<CheckInPresentationModel>
			(ICheckInView view, CheckInPresentationModel viewModel, Action onDialogClose)
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
