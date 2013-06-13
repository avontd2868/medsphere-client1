using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.MarkAsNoShow.MarkAsNoShow;

namespace ClinSchd.Modules.MarkAsNoShow.Services
{
	public interface IMarkAsNoShowService
	{
		void ShowDialog<MarkAsNoShowPresentationModel>
			(IMarkAsNoShowView view, MarkAsNoShowPresentationModel viewModel, Action onDialogClose);
	}
}
