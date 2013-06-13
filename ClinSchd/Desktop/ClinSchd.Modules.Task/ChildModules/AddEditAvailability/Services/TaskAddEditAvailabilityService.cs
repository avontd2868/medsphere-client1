using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Task.AddEditAvailability;

namespace ClinSchd.Modules.Task.AddEditAvailability.Services
{
	public class TaskAddEditAvailabilityService : ITaskAddEditAvailabilityService
    {
		public TaskAddEditAvailabilityService ()
        {
		}

		#region ITaskAddEditAvailability Members

		public void ShowDialog<AddEditAvailabilityPresentationModel>
			(IAddEditAvailabilityView view, AddEditAvailabilityPresentationModel viewModel, Action onDialogClose)
		{
			view.DataContext = viewModel;
			if (onDialogClose != null) {
				view.Closed += (sender, e) => onDialogClose ();
			}
			view.ShowDialog ();
		}

        #endregion
    }
}
