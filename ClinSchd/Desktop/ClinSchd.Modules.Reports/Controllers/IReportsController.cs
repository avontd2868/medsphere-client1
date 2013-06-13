using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Reports.Reports;

namespace ClinSchd.Modules.Reports.Controllers
{
    public interface IReportsController
    {
		void Run();
    }
}