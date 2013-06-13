using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClinSchd.Modules.ChangeDivision.ChangeDivision;
using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Modules.ChangeDivision.Controllers
{
	public interface IChangeDivisionController
    {
		void Run();
    }
}