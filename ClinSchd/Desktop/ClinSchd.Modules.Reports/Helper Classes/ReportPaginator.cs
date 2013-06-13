using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Globalization;
using System.Windows.Controls;

namespace ClinSchd.Modules.Reports.Helper_Classes
{
	class ReportPaginator : DocumentPaginator
	{
		private int _RowsPerPage;
		private double _RowHeight;
		private Size _PageSize;
		private TextBox _TextBox;
		private double _Margin;

		public ReportPaginator (TextBox textBox, Size pageSize, double rowHeight,double margin)
		{
			_RowHeight = rowHeight;
			_Margin = margin;
			PageSize = pageSize;
			_TextBox = textBox;			
		}

		public override DocumentPage GetPage (int pageNumber)
		{
			int currentRow = _RowsPerPage * pageNumber;
			if (_TextBox.LineCount < currentRow + _RowsPerPage) {
				_TextBox.MaxLines = _TextBox.LineCount - currentRow;
				_TextBox.VerticalAlignment = VerticalAlignment.Top;
			}
			_TextBox.ScrollToHome ();
			for (int i = 0; i <= currentRow; i++) {
				_TextBox.LineDown ();
			}
			return new DocumentPage (_TextBox);
		}

		public override bool IsPageCountValid
		{
			get { return true; }
		}

		public override int PageCount
		{
			get { return (int)Math.Ceiling(_TextBox.LineCount / (double)_RowsPerPage); }
		}

		public override System.Windows.Size PageSize
		{
			get { return _PageSize; }
			set
			{
				_PageSize = value;

				_RowsPerPage = RowsPerPage (PageSize.Height, _RowHeight, _Margin);

				//Can't print anything if you can't fit a row on a page
				Debug.Assert (_RowsPerPage > 0);
			}
		}

		public override IDocumentPaginatorSource Source
		{
			get { return null; }
		}

		public static int RowsPerPage (double pageHeight, double rowHeight, double margin)
		{
			return (int)Math.Floor ((pageHeight - (2 * margin)) / rowHeight);
		}
	}
}
