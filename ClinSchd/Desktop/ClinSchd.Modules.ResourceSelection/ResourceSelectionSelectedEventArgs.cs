using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Modules.PatientSelection
{
    public class ResourceSelectionSelectedEventArgs : EventArgs
    {
		public ResourceSelectionSelectedEventArgs(Patient patient)
        {
            Patient = patient;
        }

		public Patient Patient { get; set; }
    }
}
