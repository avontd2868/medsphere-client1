using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Management.AddResourceUser;

namespace ClinSchd.Modules.Management.AddResourceUser.Services
{
	public interface IManagementAddResourceUserService
    {
		void ShowDialog<AddResourceUserPresentationModel>
		(IAddResourceUserView view, AddResourceUserPresentationModel viewModel, Action onDialogClose);

    }
}
