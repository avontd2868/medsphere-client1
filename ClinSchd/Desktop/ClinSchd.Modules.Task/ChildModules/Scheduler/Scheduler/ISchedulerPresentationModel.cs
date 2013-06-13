using System;
using System.Collections.Generic;

using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Events;

using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Task.Controllers;
using Telerik.Windows.Controls;
using ClinSchd.Modules.Task.Scheduler.Controllers;

namespace ClinSchd.Modules.Task.Scheduler
{
	public interface ISchedulerPresentationModel
    {
		ISchedulerView View { get; }

		Patient Patient { get; set; }
		string PaneTitle { get; set; }
		string ResourceName { get; set; }
		int DayRange { get; set; }
		System.TimeSpan TimeScale { get; set; }
		bool IsSelectedTab { get; set; }
		SchdResource SelectedResource { get; set; }
		IList<SchdAppointment> AppointmentList { get; set; }
		DateTime VisibleStartDate { get; set; }
		DateTime VisibleEndDate { get; set; }
		Dictionary<string, Category> AppointmentCategories { get; }
		SchdAppointment SchdAppointment { get; set; }
		int TimeSlotToSkip { get; set; }
		ITaskSchedulerController Controller { get; set; }
		void ViewModeChanged (string mode);
		void PatientSelected (Patient selectedPatient);

	}
}
