using System;

using ClinSchd.Infrastructure.Models;
using ClinSchd.Infrastructure.Interfaces;
using System.Windows.Controls;

namespace ClinSchd.Modules.Reports.Reports
{
	public interface IReportsView
    {
		TextBox ReportsTextBox {get;}
		object DataContext { get; set; }
    }
}
