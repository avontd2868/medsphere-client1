using System;
using System.Collections.Generic;

using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Events;

using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Task.AddEditAvailability.Controllers;

namespace ClinSchd.Modules.Task.AddEditAvailability
{
	public interface IAddEditAvailabilityPresentationModel
    {
		IAddEditAvailabilityView View { get; }
		ValidationMessage ValidationMessage { get; set; }
		SchdAvailability SchdAvailability { get; set; }
		void OnClose ();
		void OnPropertyChanged (string propertyName);
    }
}
