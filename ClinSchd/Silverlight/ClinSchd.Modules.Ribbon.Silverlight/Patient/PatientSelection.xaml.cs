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

namespace ADT.Modules.Ribbon.Patient
{
	/// <summary>
	/// Interaction logic for PatientSelection.xaml
	/// </summary>
	public partial class PatientSelection : UserControl
	{
		public PatientSelection(string title)
		{
			this.Title = title;
			InitializeComponent();
		}

		public string Title { get; set; }
	}
}
