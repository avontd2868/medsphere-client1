using System;
using System.Collections.Generic;

using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Events;

using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Task.Controllers;
using Telerik.Windows.Controls;
using ClinSchd.Modules.Task.EmptyScheduler.Controllers;

namespace ClinSchd.Modules.Task.EmptyScheduler
{
	public interface IEmptySchedulerPresentationModel
    {
		IEmptySchedulerView View { get; }

		
		string PaneTitle { get; set; }
		string Message { get; set; }
		
		bool IsSelectedTab { get; set; }
		SchdResource SelectedResource { get; set; }
		ITaskEmptySchedulerController Controller { get; set; }
	}
}
