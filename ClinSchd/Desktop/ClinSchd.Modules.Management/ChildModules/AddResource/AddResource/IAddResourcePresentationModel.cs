using System;
using System.Collections.Generic;

using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Events;

using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Management.AddResource.Controllers;

namespace ClinSchd.Modules.Management.AddResource
{
	public interface IAddResourcePresentationModel
    {
		IAddResourceView View { get; }
		ValidationMessage ValidationMessage { get; set; }
		void OnClose ();
		string PaneTitle { get; set; }
		bool IsClinicEnabled { get; set; }
		void OnPropertyChanged (string propertyName);
		string EditClinicID { get; set; }
		void LoadEditClinic ();
    }
}
