using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Management.AddResource;

namespace ClinSchd.Modules.Management.AddResource.Services
{
	public interface IManagementAddResourceService
    {
		void ShowDialog<AddResourcePresentationModel>
		(IAddResourceView view, AddResourcePresentationModel viewModel, Action onDialogClose);

    }
}
