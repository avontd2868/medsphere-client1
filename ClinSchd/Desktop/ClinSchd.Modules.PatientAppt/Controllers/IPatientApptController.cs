using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.PatientAppt.Group;

namespace ClinSchd.Modules.PatientAppt.Controllers
{
	public interface IPatientApptController
    {
		 void Run();
    }
}