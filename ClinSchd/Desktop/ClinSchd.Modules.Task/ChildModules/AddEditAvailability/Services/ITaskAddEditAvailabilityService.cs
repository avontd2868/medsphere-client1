using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Task.AddEditAvailability;

namespace ClinSchd.Modules.Task.AddEditAvailability.Services
{
	public interface ITaskAddEditAvailabilityService
    {
		void ShowDialog<AddEditAvailabilityPresentationModel>
		(IAddEditAvailabilityView view, AddEditAvailabilityPresentationModel viewModel, Action onDialogClose);

    }
}
