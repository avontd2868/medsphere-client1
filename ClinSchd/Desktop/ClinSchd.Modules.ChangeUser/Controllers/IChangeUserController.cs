using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClinSchd.Modules.ChangeUser.ChangeUser;
using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Modules.ChangeUser.Controllers
{
    public interface IChangeUserController
    {
		IChangeUserPresentationModel Model { get; }
		void Run();
    }
}