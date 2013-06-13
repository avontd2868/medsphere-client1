using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.FindAppt.FindAppt;

namespace ClinSchd.Modules.FindAppt.Services
{
	public interface IFindApptService
	{
		void ShowDialog<FindApptPresentationModel>
			(IFindApptView view, FindApptPresentationModel viewModel, Action onDialogClose);
	}
}
