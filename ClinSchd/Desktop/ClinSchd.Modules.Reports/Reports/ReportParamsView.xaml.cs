using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

using Telerik.Windows.Controls;

using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Modules.Reports.Reports
{
    /// <summary>
    /// Interaction logic for ReportsView.xaml
    /// </summary>
	public partial class ReportParamsView : RadPane, IReportParamsView
    {
		public ReportParamsView()
        {
            InitializeComponent();
        }

		public Grid ParamControlsGrid
		{
			get { return this.paramControlsGrid; }
		}
	}
}
