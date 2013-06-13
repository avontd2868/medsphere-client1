using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using System.Collections;

using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.ChangeUser.Properties;
using ClinSchd.Modules.ChangeUser.ChangeUser;

namespace ClinSchd.Modules.ChangeUser.Services
{
	public class ChangeUserService : IChangeUserService
    {
		public ChangeUserService ()
        {
		}

		#region IChangeUser Members

		public void ShowDialog<ChangeUserPresentationModel>
			(IChangeUserView view, ChangeUserPresentationModel viewModel, Action onDialogClose)
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
