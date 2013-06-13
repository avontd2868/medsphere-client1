using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Modules.Task.MultiScheduler.Controllers
{
    public interface ITaskMultiSchedulerController
    {
		void Run();
		void RemoveViewFromList (IMultiSchedulerView View);
	}
}