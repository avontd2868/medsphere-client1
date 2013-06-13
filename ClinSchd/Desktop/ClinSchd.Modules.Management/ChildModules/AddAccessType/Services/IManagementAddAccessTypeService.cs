using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Management.AddAccessType;

namespace ClinSchd.Modules.Management.AddAccessType.Services
{
	public interface IManagementAddAccessTypeService
    {
		void ShowDialog<AddAccessTypePresentationModel>
		(IAddAccessTypeView view, AddAccessTypePresentationModel viewModel, Action onDialogClose);

    }
}
