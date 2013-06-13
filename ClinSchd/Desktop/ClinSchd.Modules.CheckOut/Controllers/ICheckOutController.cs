using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClinSchd.Modules.CheckOut.CheckOut;
using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Modules.CheckOut.Controllers
{
	public interface ICheckOutController
	{
		void Run();
	}
}