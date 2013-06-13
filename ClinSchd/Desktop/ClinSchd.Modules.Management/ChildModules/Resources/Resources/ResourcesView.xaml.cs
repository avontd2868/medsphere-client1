using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Telerik.Windows.Controls;

namespace ClinSchd.Modules.Management.Resources
{
    /// <summary>
	/// Interaction logic for ResourcesView.xaml
    /// </summary>
	public partial class ResourcesView : RadPane, IResourcesView
    {
		public ResourcesView()
        {
            InitializeComponent();
        }

		public ResourcesPresentationModel Model
        {
            get
            {
				return this.DataContext as ResourcesPresentationModel;
            }

            set
            {
                this.DataContext = value;
            }
        }

		private bool bDialogResult = false;
		public bool ConfirmUser (string message, string caption)
		{
			DialogParameters confirm = new DialogParameters ();
			confirm.Header = caption;
			TextBlock er = new TextBlock ();
			er.Width = 250;
			er.TextWrapping = TextWrapping.Wrap;
			er.Text = message;
			confirm.Content = er;
			RadWindow.Confirm (confirm.Content, OnRadConfirmClosed);

			return bDialogResult;
		}

		private void OnRadConfirmClosed (object sender, WindowClosedEventArgs e)
		{
			if (e.DialogResult == true) {
				bDialogResult = true;
			}
		}   

		public void AlertUser (string message, string caption)
		{
			DialogParameters Alert = new DialogParameters ();
			Alert.Header = caption;
			TextBlock er = new TextBlock ();
			er.Width = 250;
			er.TextWrapping = TextWrapping.Wrap;
			er.Text = message;
			Alert.Content = er;
			RadWindow.Alert (Alert);
		}

		private void AddClinic_Click (object sender, RoutedEventArgs e)
		{
			this.Model.ExecuteAddResourceCommand (string.Empty);
		}

		private void EditClinic_Click (object sender, RoutedEventArgs e)
		{
			this.Model.ExecuteEditResourceCommand (string.Empty);
		}

		private void RemoveClinic_Click (object sender, RoutedEventArgs e)
		{
			this.Model.ExecuteDeleteResourceCommand (string.Empty);
		}

		private void EditUser_Click (object sender, RoutedEventArgs e)
		{
			this.Model.ExecuteEditUserCommand (string.Empty);
		}

		private void AddUser_Click (object sender, RoutedEventArgs e)
		{
			this.Model.ExecuteAddUserCommand (string.Empty);
		}

		private void ListBox_SelectionChanged (object sender, SelectionChangedEventArgs e)
		{
			this.Model.LoadResourceUsers ();
		}

		private void RemoveUser_Click (object sender, RoutedEventArgs e)
		{
			this.Model.RemoveResourceUser ();
		}

		private void RemoveAll_Click (object sender, RoutedEventArgs e)
		{
			this.Model.RemoveAllResourceUsers ();
		}

		private void ResourceUserListBox_SelectionChanged (object sender, SelectionChangedEventArgs e)
		{
			if (this.Model.ResourceUserIEN != null) {
				this.Model.ResourceUserSelectionChanged ();
			}
		}

		private void UserListBox_SelectionChanged (object sender, SelectionChangedEventArgs e)
		{
			if (this.Model.UserIEN != null) {
				this.Model.UserSelectionChanged ();
			}
		}

	}
}
