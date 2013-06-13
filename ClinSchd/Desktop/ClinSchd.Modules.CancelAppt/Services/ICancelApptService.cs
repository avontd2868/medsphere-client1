using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.CancelAppt.CancelAppt;

namespace ClinSchd.Modules.CancelAppt.Services
{
	public interface ICancelApptService
	{
		void ShowDialog<CancelApptPresentationModel>
			(ICancelApptView view, CancelApptPresentationModel viewModel, Action onDialogClose);
	}
}
