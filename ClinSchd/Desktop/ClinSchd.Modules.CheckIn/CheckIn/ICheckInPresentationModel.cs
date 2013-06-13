using System;
using System.Collections.Generic;

using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Events;

using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.CheckIn.Controllers;

namespace ClinSchd.Modules.CheckIn.CheckIn
{
	public interface ICheckInPresentationModel
	{
		ICheckInView View { get; }
		ValidationMessage ValidationMessage { get; set; }
		CheckInPatient CheckIn { get; set; }
		void GetCheckInAppointment (SchdAppointment newAppointment);

		void OnClose();
	}
}
