
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Modules.Management.AddResource.Controllers
{
    public interface IManagementAddResourceController
    {
		IAddResourcePresentationModel Model { get; }
		void Run();
    }
}