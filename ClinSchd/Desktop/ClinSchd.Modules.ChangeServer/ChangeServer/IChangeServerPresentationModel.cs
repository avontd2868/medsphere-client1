using System;
using System.Collections.Generic;

using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Events;

using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.ChangeServer.Controllers;

namespace ClinSchd.Modules.ChangeServer.ChangeServer
{
	public interface IChangeServerPresentationModel
    {
		IChangeServerView View { get; }
//		CompositePresentationEvent<Server> ForwardingEvent { get; set; }

		string SearchString { get; }
		IList<Server> ServerList { get; }
		void OnClose();
	}
}
