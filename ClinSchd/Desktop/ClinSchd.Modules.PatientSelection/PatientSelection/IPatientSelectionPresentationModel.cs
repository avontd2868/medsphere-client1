using System;
using System.Collections.Generic;

using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Events;

using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.PatientSelection.Controllers;

namespace ClinSchd.Modules.PatientSelection.PatientSelection
{
    public interface IPatientSelectionPresentationModel
    {
		IPatientSelectionView View { get; }
		SchdAppointment SelectedAppointment { get; set; }
		string Command { get; set; }
		string SearchString { get; }
		IList<Patient> PatientList { get; }
		void OnClose();
	}
}
