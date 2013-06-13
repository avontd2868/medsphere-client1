using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClinSchd.Modules.UserLogin.UserLogin;
using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Modules.UserLogin.Controllers
{
	public interface IUserLoginController
    {
		void Run();
    }
}