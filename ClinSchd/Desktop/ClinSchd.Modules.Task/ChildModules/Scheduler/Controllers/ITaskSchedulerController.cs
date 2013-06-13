using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Modules.Task.Scheduler.Controllers
{
    public interface ITaskSchedulerController
    {
		void Run();
		void RemoveViewFromList (ISchedulerView View);
	}
}