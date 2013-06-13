using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Management.AddResourceGroup;

namespace ClinSchd.Modules.Management.AddResourceGroup.Services
{
	public interface IManagementAddResourceGroupService
    {
		void ShowDialog<AddResourceGroupPresentationModel>
		(IAddResourceGroupView view, AddResourceGroupPresentationModel viewModel, Action onDialogClose);

    }
}
