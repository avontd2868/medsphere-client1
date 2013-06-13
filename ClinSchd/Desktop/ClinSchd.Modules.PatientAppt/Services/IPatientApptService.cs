using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.PatientAppt.Group;
using ClinSchd.Modules.PatientAppt.Controllers;
using ClinSchd.Modules.PatientAppt.Services;

namespace ClinSchd.Modules.PatientAppt.Services
{
	public interface IPatientApptService
    {
		void ShowDialog<GroupPresentationModel>
		(IGroupView view, GroupPresentationModel viewModel, Action onDialogClose);

    }
}
