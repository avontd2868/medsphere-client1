using System;
using System.Collections.Generic;

using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Events;

using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.MarkAsNoShow.Controllers;

namespace ClinSchd.Modules.MarkAsNoShow.MarkAsNoShow
{
	public interface IMarkAsNoShowPresentationModel
    {
		IMarkAsNoShowView View { get; }
		void OnClose();
		ValidationMessage ValidationMessage { get; set; }
		AutoRebookData AutoRebook { get; set; }
		void NoShowAppointment (SchdAppointment newAppointment);
		void OnPropertyChanged (string propertyName);
	}
}
