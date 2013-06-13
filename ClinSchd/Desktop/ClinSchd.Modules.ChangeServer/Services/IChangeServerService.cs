using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.ChangeServer.ChangeServer;

namespace ClinSchd.Modules.ChangeServer.Services
{
	public interface IChangeServerService
	{
		void ShowDialog<ChangeServerPresentationModel>
			(IChangeServerView view, ChangeServerPresentationModel viewModel, Action onDialogClose);
	}
}
