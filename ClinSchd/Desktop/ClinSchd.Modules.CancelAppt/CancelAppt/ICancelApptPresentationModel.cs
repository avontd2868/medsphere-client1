using System;
using System.Collections.Generic;

using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Events;

using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.CancelAppt.Controllers;

namespace ClinSchd.Modules.CancelAppt.CancelAppt
{
	public interface ICancelApptPresentationModel
	{
		ICancelApptView View { get; }
		ValidationMessage ValidationMessage { get; set; }
		CancelAppointment CancelAppointment { get; set; }
		AutoRebookData AutoRebook { get; set; }
		void CancelApptAppointment (SchdAppointment newAppointment);
		void OnPropertyChanged (string propertyName);
		void OnClose();
	}
}
