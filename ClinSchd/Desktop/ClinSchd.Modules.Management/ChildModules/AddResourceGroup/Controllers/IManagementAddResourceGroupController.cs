using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Modules.Management.AddResourceGroup.Controllers
{
    public interface IManagementAddResourceGroupController
    {
		IAddResourceGroupPresentationModel Model { get; }
		void Run();
    }
}