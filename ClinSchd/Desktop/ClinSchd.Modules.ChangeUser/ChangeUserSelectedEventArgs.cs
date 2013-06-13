using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Modules.ChangeUser
{
    public class ChangeUserSelectedEventArgs : EventArgs
    {
		public ChangeUserSelectedEventArgs(User user)
        {
            User = user;
        }

		public User User { get; set; }
    }
}
