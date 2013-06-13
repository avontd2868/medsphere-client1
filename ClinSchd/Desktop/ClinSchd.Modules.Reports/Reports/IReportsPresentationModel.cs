using System;
using System.Collections.Generic;

using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Events;

using ClinSchd.Modules.Reports.Helper_Classes;
using ClinSchd.Modules.Reports.Controllers;
using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Modules.Reports.Reports
{
    public interface IReportsPresentationModel
    {
		IReportsView View { get; }
		ReportsModel Model { get; set; }
		void OnClose();
		IList<NameValue> ServerPrinterList { get; }
	}
}
