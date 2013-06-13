using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.CheckOut.CheckOut;

namespace ClinSchd.Modules.CheckOut.Services
{
	public interface ICheckOutService
	{
		void ShowDialog<CheckOutPresentationModel>
			(ICheckOutView view, CheckOutPresentationModel viewModel, Action onDialogClose);
	}
}
