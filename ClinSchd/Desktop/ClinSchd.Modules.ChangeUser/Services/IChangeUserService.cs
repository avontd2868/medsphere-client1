using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.ChangeUser.ChangeUser;

namespace ClinSchd.Modules.ChangeUser.Services
{
	public interface IChangeUserService
	{
		void ShowDialog<ChangeUserPresentationModel>
			(IChangeUserView view, ChangeUserPresentationModel viewModel, Action onDialogClose);
	}
}
