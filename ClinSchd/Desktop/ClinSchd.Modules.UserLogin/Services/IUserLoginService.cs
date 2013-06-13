using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.UserLogin.UserLogin;

namespace ClinSchd.Modules.UserLogin.Services
{
	public interface IUserLoginService
	{
		void ShowDialog<UserLoginPresentationModel>
			(IUserLoginView view, UserLoginPresentationModel viewModel, Action onDialogClose);
	}
}
