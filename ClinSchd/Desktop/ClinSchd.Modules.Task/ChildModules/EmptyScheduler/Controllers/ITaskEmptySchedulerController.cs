using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Modules.Task.EmptyScheduler.Controllers
{
    public interface ITaskEmptySchedulerController
    {
		void Run();

		void RemoveViewFromList (IEmptySchedulerView View);
	}
}