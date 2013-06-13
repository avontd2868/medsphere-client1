using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using System.Collections;

using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.UserLogin.Properties;
using ClinSchd.Modules.UserLogin.UserLogin;

namespace ClinSchd.Modules.UserLogin.Services
{
	public class UserLoginService : IUserLoginService
    {
		public UserLoginService ()
        {
		}

		#region IUserLogin Members

		public void ShowDialog<UserLoginPresentationModel>
			(IUserLoginView view, UserLoginPresentationModel viewModel, Action onDialogClose)
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
