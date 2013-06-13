using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.PatientAppt.Properties;
using ClinSchd.Modules.PatientAppt.Group;

namespace ClinSchd.Modules.PatientAppt.Services
{
    public class PatientApptService : IPatientApptService
    {
		public PatientApptService ()
        {
		}

		#region IPatientAppt Members

		public void ShowDialog<GroupPresentationModel>
		(IGroupView view, GroupPresentationModel viewModel, Action onDialogClose)
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
