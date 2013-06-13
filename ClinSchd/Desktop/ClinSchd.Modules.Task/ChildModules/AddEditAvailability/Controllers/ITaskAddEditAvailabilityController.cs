using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Modules.Task.AddEditAvailability.Controllers
{
	public interface ITaskAddEditAvailabilityController
    {
		IAddEditAvailabilityPresentationModel Model { get; }
		void Run();
    }
}