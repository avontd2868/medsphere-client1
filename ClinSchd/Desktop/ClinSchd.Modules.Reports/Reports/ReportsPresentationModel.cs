using System;
using System.Collections.Generic;
using System.ComponentModel;

using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Presentation.Commands;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Reports.Controllers;
using Microsoft.Practices.Composite.Regions;
using System.Windows.Controls;
using System.Windows;
using ClinSchd.Modules.Reports.Helper_Classes;
using System.Windows.Media;
using System.Globalization;
using Microsoft.Practices.Unity;

namespace ClinSchd.Modules.Reports.Reports
{
    public class ReportsPresentationModel : AsyncValidatableModel, IReportsPresentationModel
    {
		private readonly IRegionManager regionManager;


		public ReportsPresentationModel(
			IReportsView view,
			ReportsModel model,
			IRegionManager regionManager,
			ServerPrinter serverPrinter)
        {
            View = view;
            View.DataContext = this;
			Model = model;
			this.regionManager = regionManager;
			this.selectedServerPrinter = serverPrinter;

			OKCommand = new DelegateCommand<string> (ExecuteOKCommand, CanExecuteOKCommand);
			PrintCommand = new DelegateCommand<string> (ExecutePrintCommand, CanExecutePrintCommand);

			//regionManager.Regions[RegionNames.TaskGroupRegion].Add (View);
			(View as Window).Show ();
		}

		#region IReportsPresentationModel Members

		public IReportsView View { get; private set; }
		public ReportsModel Model { get; set; }
		public void OnClose ()
		{
			//regionManager.Regions[RegionNames.TaskGroupRegion].Remove (View);
		}

		public DelegateCommand<string> OKCommand { get; private set; }
		public DelegateCommand<string> PrintCommand { get; private set; }

		public void ExecuteOKCommand (string title)
		{
			(View as Window).Close ();
		}

		double lineHeight;
		double rowsPerPage;
		double margin;
		public void ExecutePrintCommand (string printType)
		{
			if (printType == "Local") {
				lineHeight = View.ReportsTextBox.FontFamily.LineSpacing * View.ReportsTextBox.FontSize;
				margin = 3 * lineHeight;
				PrintDialog dialog = new PrintDialog ();
				if (dialog.ShowDialog () == true) {
					Size pageSize = new Size (dialog.PrintableAreaWidth,
						dialog.PrintableAreaHeight);
					rowsPerPage = ReportPaginator.RowsPerPage (pageSize.Height, lineHeight, margin);
					PrepareTextBoxForPrinting (View.ReportsTextBox,pageSize);
					var paginator = new ReportPaginator (View.ReportsTextBox,
					  pageSize, lineHeight, margin);
					dialog.PrintDocument (paginator, Model.ReportDisplayName);
					RestoreTextBoxFromPrinting (View.ReportsTextBox);
				}
			}
			else if (printType == "Server") {
				//TODO: call RPC to print server side using selected server printer.
			}
		}

		private double previousGridHeight;
		private Thickness previousBorderThickness;
		private Thickness previousMargin;
		private int previousMaxLines;
		private void PrepareTextBoxForPrinting (TextBox _TextBox, Size PageSize)
		{
			previousGridHeight = ((Grid)_TextBox.Parent).Height;
			((Grid)_TextBox.Parent).Height = rowsPerPage * lineHeight + 36 + 2*margin;
			previousBorderThickness = _TextBox.BorderThickness;
			_TextBox.BorderThickness = new Thickness (0);
			previousMargin = _TextBox.Margin;
			_TextBox.Margin = new Thickness(margin);
			_TextBox.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
			previousMaxLines = _TextBox.MaxLines;
		}

		private void RestoreTextBoxFromPrinting (TextBox _TextBox)
		{
			((Grid)_TextBox.Parent).Height = previousGridHeight;
			_TextBox.BorderThickness = previousBorderThickness;
			_TextBox.Margin = previousMargin;
			_TextBox.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
			_TextBox.Height = double.NaN;
			_TextBox.VerticalAlignment = VerticalAlignment.Stretch;
			_TextBox.MaxLines = previousMaxLines;
		}

		public bool CanExecuteOKCommand (string title)
		{
			return true;
		}

		public bool CanExecutePrintCommand (string printType)
		{
			return true;
		}

		public IList<NameValue> ServerPrinterList 
		{
			get
			{
				return Model.ServerPrinterList;
			}
		}

		private ServerPrinter selectedServerPrinter;
		
		public ServerPrinter SelectedServerPrinter
		{
			get
			{
				if (selectedServerPrinter.Name == null) {
					foreach (NameValue item in ServerPrinterList) {
						selectedServerPrinter.Name = item.Value;
						break;
					}
				}
				return selectedServerPrinter;
			}
			set
			{
				selectedServerPrinter = value;
			}
		}

		#endregion
    }
}