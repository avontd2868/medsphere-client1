using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Management.AddAccessGroup;

namespace ClinSchd.Modules.Management.AddAccessGroup.Services
{
	public interface IManagementAddAccessGroupService
    {
		void ShowDialog<AddAccessGroupPresentationModel>
		(IAddAccessGroupView view, AddAccessGroupPresentationModel viewModel, Action onDialogClose);

    }
}
