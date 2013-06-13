using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

using Telerik.Windows.Controls;

using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Modules.ChangeServer.ChangeServer
{
    /// <summary>
    /// Interaction logic for PatientSelectionView.xaml
    /// </summary>
	public partial class ChangeServerView : Window, IChangeServerView
    {
		public ChangeServerView ()
        {
            InitializeComponent();
        }

		public ChangeServerPresentationModel Model
        {
            get
            {
				return this.DataContext as ChangeServerPresentationModel;
            }

            set
            {
                this.DataContext = value;
            }
        }

		private void ChangeServer_MouseDoubleClick (object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Model.SelectedServer = new Server ();
			DialogResult = true;
			Close();
		}

		private void Button_OK_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Model.SelectedServer = new Server ();
			Close();
		}

		private void Button_Cancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Model.SelectedServer = null;
			Close();
		}

		private void ChangeServer_TextChanged (object sender, TextChangedEventArgs e)
		{
			if ( sender is TextBox )
			{
				Model.SearchStringChanged(((TextBox)sender).Text);
			}
		}

		private void textBox1_TextChanged (object sender, TextChangedEventArgs e) {

		}
	}
}
