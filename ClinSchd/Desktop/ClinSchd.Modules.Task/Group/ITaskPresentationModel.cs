using ClinSchd.Modules.Task.Controllers;
using ClinSchd.Infrastructure.Models;
using Microsoft.Practices.Composite.Presentation.Commands;
using ClinSchd.Infrastructure;
using Telerik.Windows.Controls;
using System.Collections.Generic;

namespace ClinSchd.Modules.Task.Group
{
	public interface ITaskPresentationModel
    {
		bool IsSelectedTab { get; set; }
		SchdResource SelectedResource { get; set; }
		SchdResourceGroup SelectedResourceGroup { get; set; }
		DelegateCommand<object> TaskTabSelectedCommand { get; }
		DelegateCommand<string> ResetUnselectedTabCommand { get; }
		void ClearViewFromList ();
		void OnPropertyChanged (string propertyName);
		bool IsEnableUpdateAppointment { get; set; }
	}
}
