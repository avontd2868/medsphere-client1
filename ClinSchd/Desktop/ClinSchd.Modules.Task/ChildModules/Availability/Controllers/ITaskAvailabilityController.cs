using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Modules.Task.Availability.Controllers
{
	public interface ITaskAvailabilityController
    {
		void Run();
		void RemoveViewFromList (IAvailabilityView View);
	}
}