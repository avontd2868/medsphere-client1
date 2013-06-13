using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.ChangeDivision.ChangeDivision;

namespace ClinSchd.Modules.ChangeDivision.Services
{
	public interface IChangeDivisionService
	{
		void ShowDialog<ChangeDivisionPresentationModel>
			(IChangeDivisionView view, ChangeDivisionPresentationModel viewModel, Action onDialogClose);
	}
}
