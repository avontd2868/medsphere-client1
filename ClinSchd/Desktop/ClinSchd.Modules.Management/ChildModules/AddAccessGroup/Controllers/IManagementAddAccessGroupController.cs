using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Modules.Management.AddAccessGroup.Controllers
{
    public interface IManagementAddAccessGroupController
    {
		IAddAccessGroupPresentationModel Model { get; }
		void Run();
    }
}