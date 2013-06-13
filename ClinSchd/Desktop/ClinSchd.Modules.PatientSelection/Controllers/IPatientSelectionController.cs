using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClinSchd.Modules.PatientSelection.PatientSelection;
using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Modules.PatientSelection.Controllers
{
    public interface IPatientSelectionController
    {
		IPatientSelectionPresentationModel Model { get; }
		void Run();
    }
}