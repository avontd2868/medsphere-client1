using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Modules.Management.AddAccessType.Controllers
{
    public interface IManagementAddAccessTypeController
    {
		IAddAccessTypePresentationModel Model { get; }
		void Run();
    }
}