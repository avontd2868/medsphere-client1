using System;
using System.Collections.Generic;

using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Events;

using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.ChangeDivision.Controllers;

namespace ClinSchd.Modules.ChangeDivision.ChangeDivision
{
	public interface IChangeDivisionPresentationModel
    {
		IChangeDivisionView View { get; }
		ValidationMessage ValidationMessage { get; set; }
		void GetDivisions ();
		void OnClose ();
	}
}
