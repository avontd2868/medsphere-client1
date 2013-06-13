using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using System.Collections.Generic;
using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Infrastructure
{
	public class LaunchFindApptDialogEvent : CompositePresentationEvent<IList<SchdResource>>
	{
	}
}
