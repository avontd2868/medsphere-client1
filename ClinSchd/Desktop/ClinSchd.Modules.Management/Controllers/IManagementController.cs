using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Management.Group;

namespace ClinSchd.Modules.Management.Controllers
{
	public interface IManagementController
    {
		IGroupPresentationModel Model { get; }
        void Run();
    }
}