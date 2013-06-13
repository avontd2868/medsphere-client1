using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ADT.Modules.DataAccess.DataAccess
{
	/// <summary>
	/// Interaction logic for DataAccessSelection.xaml
	/// </summary>
	public partial class DataAccessSelection : UserControl
	{
		public DataAccessSelection(string title)
		{
			this.Title = title;
			InitializeComponent();
		}

		public string Title { get; set; }
	}
}
