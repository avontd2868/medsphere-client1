using System;
using System.Collections.Generic;

using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Events;

using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Management.AddAccessGroup.Controllers;

namespace ClinSchd.Modules.Management.AddAccessGroup
{
	public interface IAddAccessGroupPresentationModel
    {
		IAddAccessGroupView View { get; }
		ValidationMessage ValidationMessage { get; set; }
		void OnClose ();
		string PaneTitle { get; set; }
		void OnPropertyChanged (string propertyName);
		string AccessTypeGroupName { get; set; }
		string AccessTypeGroupID { get; set; }
	}
}
