using System;
using System.Collections.Generic;

using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Events;

using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Management.AddResourceGroup.Controllers;

namespace ClinSchd.Modules.Management.AddResourceGroup
{
	public interface IAddResourceGroupPresentationModel
    {
		IAddResourceGroupView View { get; }
		ValidationMessage ValidationMessage { get; set; }
		void OnClose ();
		string PaneTitle { get; set; }
		string ClinicGroupName { get; set; }
		string ClinicGroupID { get; set; }
		void OnPropertyChanged (string propertyName);

    }
}
