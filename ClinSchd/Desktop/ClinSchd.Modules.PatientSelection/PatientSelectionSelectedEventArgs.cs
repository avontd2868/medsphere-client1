using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Modules.PatientSelection
{
    public class PatientSelectionSelectedEventArgs : EventArgs
    {
		public PatientSelectionSelectedEventArgs(Patient patient)
        {
            Patient = patient;
        }

		public Patient Patient { get; set; }
    }
}
