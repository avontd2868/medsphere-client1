using System;
using System.Collections.Generic;

using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Events;
using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Task.Availability.Controllers;
using Telerik.Windows.Controls;

namespace ClinSchd.Modules.Task.Availability
{
	public interface IAvailabilityPresentationModel
    {
		IAvailabilityView View { get; }

		Patient Patient { get; set; }
		string PaneTitle { get; set; }
		string ResourceName { get; set; }
		int DayRange { get; set; }
		System.TimeSpan TimeScale { get; set; }
		bool IsSelectedTab { get; set; }
		SchdResource SelectedResource { get; set; }
		DateTime VisibleStartDate { get; set; }
		DateTime VisibleEndDate { get; set; }
		int TimeSlotToSkip { get; set; }
		ITaskAvailabilityController Controller { get; set; }
		IList<SchdAvailability> AvailabilityList { get; set; }
		SchdAvailability SchdAvailability { get; set; }
	}
}
