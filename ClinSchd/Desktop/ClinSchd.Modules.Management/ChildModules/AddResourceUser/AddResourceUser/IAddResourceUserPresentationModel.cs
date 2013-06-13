using System;
using System.Collections.Generic;

using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Events;

using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Management.AddResourceUser.Controllers;

namespace ClinSchd.Modules.Management.AddResourceUser
{
	public interface IAddResourceUserPresentationModel
    {
		IAddResourceUserView View { get; }
		ValidationMessage ValidationMessage { get; set; }
		void OnClose ();
		string PaneTitle { get; set; }
		void OnPropertyChanged (string propertyName);
		string ResourceUserName { get; set; }
		string OverbookValue { get; set; }
		bool IsModifyChecked { get; set; }
		bool IsUpdateChecked { get; set; }
		ResourceUser ResourceUser { get; set; }
    }
}
