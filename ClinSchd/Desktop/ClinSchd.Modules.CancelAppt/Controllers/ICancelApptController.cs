using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClinSchd.Modules.CancelAppt.CancelAppt;
using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Modules.CancelAppt.Controllers
{
	public interface ICancelApptController
	{
		void Run();
	}
}