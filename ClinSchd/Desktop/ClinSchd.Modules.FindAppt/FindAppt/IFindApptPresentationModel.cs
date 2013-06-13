using System;
using System.Collections.Generic;

using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Events;

using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.FindAppt.Controllers;

namespace ClinSchd.Modules.FindAppt.FindAppt
{
	public interface IFindApptPresentationModel
	{
		IFindApptView View { get; }
		ValidationMessage ValidationMessage { get; set; }
		IList<SchdResource> SelectedResourceList { get; set; }
		IList<string> SelectedDates { get; set; }
		void OnClose();
	}
}
