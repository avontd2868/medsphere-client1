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
	public partial class ReportsView : Window, IReportsView
    {
		public ReportsView()
        {
            InitializeComponent();
        }

		public TextBox ReportsTextBox
		{
			get { return this.ReportTextBox; }
		}
	}
}
