using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClinSchd.Modules.FindAppt.FindAppt;
using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Modules.FindAppt.Controllers
{
	public interface IFindApptController
	{
		IFindApptPresentationModel Model { get; }
		void Run();
	}
}