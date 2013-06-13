using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

using Telerik.Windows.Controls;

using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Modules.ChangeUser.ChangeUser
{
    /// <summary>
    /// Interaction logic for PatientSelectionView.xaml
    /// </summary>
	public partial class ChangeUserView : Window, IChangeUserView
    {
		public ChangeUserView ()
        {
            InitializeComponent();
        }

		public ChangeUserPresentationModel Model
        {
            get
            {
				return this.DataContext as ChangeUserPresentationModel;
            }

            set
            {
                this.DataContext = value;
            }
        }

		private void ChangeUser_MouseDoubleClick (object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Model.SelectedUser = new User ();
			DialogResult = true;
			Close();
		}

		private void Button_OK_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Model.SelectedUser = new User ();
			Close();
		}

		private void Button_Cancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Model.SelectedUser = null;
			Close();
		}

		private void ChangeUser_TextChanged (object sender, TextChangedEventArgs e)
		{
			if ( sender is TextBox )
			{
				Model.SearchStringChanged(((TextBox)sender).Text);
			}
		}
	}
}
