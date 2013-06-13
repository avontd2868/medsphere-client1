using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using System.Collections;

using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.CheckOut.Properties;
using ClinSchd.Modules.CheckOut.CheckOut;

namespace ClinSchd.Modules.CheckOut.Services
{
	public class CheckOutService : ICheckOutService
	{
		public CheckOutService ()
		{
		}

		#region ICheckOut Members

		public void ShowDialog<CheckOutPresentationModel>
			(ICheckOutView view, CheckOutPresentationModel viewModel, Action onDialogClose)
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
