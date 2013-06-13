using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClinSchd.Modules.MarkAsNoShow.MarkAsNoShow;
using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Modules.MarkAsNoShow.Controllers
{
	public interface IMarkAsNoShowController
    {
		void Run();
    }
}