﻿using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;

using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Infrastructure
{
	public class LaunchResourceAvailibilityDialogEvent : CompositePresentationEvent<CompositePresentationEvent<SchdResource>>
	{
	}
}