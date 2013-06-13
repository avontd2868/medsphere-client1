using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.CheckIn.CheckIn;

namespace ClinSchd.Modules.CheckIn.Services
{
	public interface ICheckInService
	{
		void ShowDialog<CheckInPresentationModel>
			(ICheckInView view, CheckInPresentationModel viewModel, Action onDialogClose);
	}
}
