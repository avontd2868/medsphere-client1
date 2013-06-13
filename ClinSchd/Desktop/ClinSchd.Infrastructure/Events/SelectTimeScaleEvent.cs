﻿using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Infrastructure.Events
{
	public class SelectTimeScaleEvent : CompositePresentationEvent<int>
	{
	}
}