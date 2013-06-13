using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using System.Collections;

using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.MarkAsNoShow.Properties;
using ClinSchd.Modules.MarkAsNoShow.MarkAsNoShow;

namespace ClinSchd.Modules.MarkAsNoShow.Services
{
	public class MarkAsNoShowService : IMarkAsNoShowService
	{
		public MarkAsNoShowService ()
		{
		}

		#region IMarkAsNoShow Members

		public void ShowDialog<MarkAsNoShowPresentationModel>
			(IMarkAsNoShowView view, MarkAsNoShowPresentationModel viewModel, Action onDialogClose)
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
