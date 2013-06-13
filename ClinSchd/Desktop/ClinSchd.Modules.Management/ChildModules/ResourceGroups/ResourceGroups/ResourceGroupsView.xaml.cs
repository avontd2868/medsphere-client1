using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Telerik.Windows.Controls;

namespace ClinSchd.Modules.Management.ResourceGroups
{
    /// <summary>
	/// Interaction logic for ResourceGroupsView.xaml
    /// </summary>
	public partial class ResourceGroupsView : RadPane, IResourceGroupsView
    {
		public ResourceGroupsView()
        {
            InitializeComponent();
        }

		public ResourceGroupsPresentationModel Model
        {
            get
            {
				return this.DataContext as ResourceGroupsPresentationModel;
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

		private void NewGroup_Click (object sender, RoutedEventArgs e)
		{
			this.Model.ExecuteAddResourceGroupCommand(string.Empty);
		}

		private void EditGroup_Click (object sender, RoutedEventArgs e)
		{
			this.Model.ExecuteEditResourceGroupCommand (string.Empty);
		}

		private void RemoveGroup_Click (object sender, RoutedEventArgs e)
		{
			this.Model.RemoveGroupByName ();
		}

		private void ClinicGroupListBox_SelectionChanged (object sender, SelectionChangedEventArgs e)
		{
			this.Model.LoadGroupedClinics ();
		}

		private void AddClinic_Click (object sender, RoutedEventArgs e)
		{
			this.Model.AddClinicToGroup ();
		}

		private void AddAll_Click (object sender, RoutedEventArgs e)
		{
			this.Model.AddAllClinicsToGroup ();
		}

		private void RemoveClinic_Click (object sender, RoutedEventArgs e)
		{
			this.Model.RemoveGroupedClinic ();
		}

		private void RemoveAll_Click (object sender, RoutedEventArgs e)
		{
			this.Model.RemoveAllGroupedClinics ();
		}
	}
}
