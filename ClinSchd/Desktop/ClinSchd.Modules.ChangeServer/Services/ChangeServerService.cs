using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using System.Collections;

using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.ChangeServer.Properties;
using ClinSchd.Modules.ChangeServer.ChangeServer;

namespace ClinSchd.Modules.ChangeServer.Services
{
    public class ChangeServerService : IChangeServerService
    {
		public ChangeServerService ()
        {
		}

		#region IChangeServer Members

		public void ShowDialog<ChangeServerPresentationModel>
			(IChangeServerView view, ChangeServerPresentationModel viewModel, Action onDialogClose)
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
