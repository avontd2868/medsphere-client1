using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClinSchd.Modules.ResourceSelection.ResourceSelection;
using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Modules.ResourceSelection.Controllers
{
    public interface IResourceSelectionController
    {
		IResourceSelectionPresentationModel Model { get; }
		void Run(string title);
    }
}