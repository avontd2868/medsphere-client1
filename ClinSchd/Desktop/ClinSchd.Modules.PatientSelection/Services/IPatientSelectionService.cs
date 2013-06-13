using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.PatientSelection.PatientSelection;

namespace ClinSchd.Modules.PatientSelection.Services
{
	public interface IPatientSelectionService
	{
		void ShowDialog<PatientSelectionPresentationModel>
			(IPatientSelectionView view, PatientSelectionPresentationModel viewModel, Action onDialogClose);
	}
}
