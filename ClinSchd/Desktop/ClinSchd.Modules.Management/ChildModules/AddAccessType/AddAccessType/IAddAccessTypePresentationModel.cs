using System;
using System.Collections.Generic;

using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Events;

using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Management.AddAccessType.Controllers;

namespace ClinSchd.Modules.Management.AddAccessType
{
	public interface IAddAccessTypePresentationModel
    {
		IAddAccessTypeView View { get; }
		ValidationMessage ValidationMessage { get; set; }
		void OnClose ();
		string PaneTitle { get; set; }
		void OnPropertyChanged (string propertyName);
		string EditAccessTypeID { get; set; }
		void LoadEditAccessType ();
    }
}
