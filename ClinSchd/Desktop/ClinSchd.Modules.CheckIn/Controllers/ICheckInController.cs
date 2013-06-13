using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClinSchd.Modules.CheckIn.CheckIn;
using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Modules.CheckIn.Controllers
{
	public interface ICheckInController
	{
		void Run();
	}
}