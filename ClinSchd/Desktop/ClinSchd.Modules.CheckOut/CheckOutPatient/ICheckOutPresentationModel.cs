using System;
using System.Collections.Generic;

using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Events;

using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.CheckOut.Controllers;

namespace ClinSchd.Modules.CheckOut.CheckOut
{
	public interface ICheckOutPresentationModel
	{
		ICheckOutView View { get; }
		ValidationMessage ValidationMessage { get; set; }
		CheckOutPatient CheckOut { get; set; }
		void GetCheckOutAppointment (SchdAppointment newAppointment);
		void OnClose();
	}
}
