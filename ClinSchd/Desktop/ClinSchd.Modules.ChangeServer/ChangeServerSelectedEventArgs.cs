using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Modules.ChangeServer
{
    public class ChangeServerSelectedEventArgs : EventArgs
    {
		public ChangeServerSelectedEventArgs(Server server)
        {
            Server = server;
        }

		public Server Server { get; set; }
    }
}
