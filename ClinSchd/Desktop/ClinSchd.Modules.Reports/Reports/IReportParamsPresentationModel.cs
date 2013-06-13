using System;
using System.Collections.Generic;

using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Events;

using ClinSchd.Modules.Reports.Helper_Classes;
using ClinSchd.Modules.Reports.Controllers;

namespace ClinSchd.Modules.Reports.Reports
{
    public interface IReportParamsPresentationModel
    {
		IReportParamsView View { get; }
		ReportsModel Model { get; set; }
		ReportsPresentationModel DisplayModel { get; set; }
		void OnClose();
	}
}
