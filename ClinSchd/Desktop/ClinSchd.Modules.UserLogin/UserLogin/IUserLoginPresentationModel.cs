using System;
using System.Collections.Generic;

using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Events;

using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.UserLogin.Controllers;

namespace ClinSchd.Modules.UserLogin.UserLogin
{
	public interface IUserLoginPresentationModel
    {
		IUserLoginView View { get; }
		ValidationMessage ValidationMessage { get; set; }
		
		void OnClose ();
	}
}
