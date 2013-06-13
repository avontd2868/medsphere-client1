using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Management.Group;
using ClinSchd.Modules.Management.Controllers;
using ClinSchd.Modules.Management.Services;

namespace ClinSchd.Modules.Management.Services
{
	public interface IManagementService
    {
		void ShowDialog(IGroupView view, IGroupPresentationModel viewModel, Action onDialogClose);

    }
}
