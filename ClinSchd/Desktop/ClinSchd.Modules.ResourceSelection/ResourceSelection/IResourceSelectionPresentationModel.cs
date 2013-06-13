using System;
using System.Collections.Generic;

using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Events;

using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.ResourceSelection.Controllers;

namespace ClinSchd.Modules.ResourceSelection.ResourceSelection
{
	public interface IResourceSelectionPresentationModel
    {
		IResourceSelectionView View { get; }
		CompositePresentationEvent<SchdResource> NewForwardingEvent { get; set; }

		string SearchString { get; }
		IList<SchdResource> ResourceList { get; }
		SchdResource SelectedResource { get; set; }
		void OnClose();
		string PaneTitle { get; set; }
	}
}
