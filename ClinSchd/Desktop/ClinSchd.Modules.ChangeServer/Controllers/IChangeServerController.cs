using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClinSchd.Modules.ChangeServer.ChangeServer;
using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Modules.ChangeServer.Controllers
{
    public interface IChangeServerController
    {
		IChangeServerPresentationModel Model { get; }
		void Run();
    }
}