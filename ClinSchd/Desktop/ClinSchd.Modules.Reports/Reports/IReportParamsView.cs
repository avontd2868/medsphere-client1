using System;
using System.Windows.Controls;

using ClinSchd.Infrastructure.Models;
using ClinSchd.Infrastructure.Interfaces;

namespace ClinSchd.Modules.Reports.Reports
{
	public interface IReportParamsView
    {
		Grid ParamControlsGrid { get; }
		object DataContext { get; set; }
	}
}