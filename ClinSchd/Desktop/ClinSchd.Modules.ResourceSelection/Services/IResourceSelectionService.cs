using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.ResourceSelection.ResourceSelection;

namespace ClinSchd.Modules.ResourceSelection.Services
{
	public interface IResourceSelectionService
	{
		void ShowDialog<ResourceSelectionPresentationModel>
			(IResourceSelectionView view, ResourceSelectionPresentationModel viewModel, Action onDialogClose);
	}
}
