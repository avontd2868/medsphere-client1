using System;
using System.Collections.Generic;

using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Events;

using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.ChangeUser.Controllers;

namespace ClinSchd.Modules.ChangeUser.ChangeUser
{
	public interface IChangeUserPresentationModel
    {
		IChangeUserView View { get; }
		CompositePresentationEvent<User> ForwardingEvent { get; set; }

		string SearchString { get; }
		IList<User> UserList { get; }
		void OnClose();
	}
}
