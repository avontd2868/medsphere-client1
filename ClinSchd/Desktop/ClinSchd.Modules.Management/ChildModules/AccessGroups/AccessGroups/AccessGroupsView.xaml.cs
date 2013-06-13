using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Telerik.Windows.Controls;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace ClinSchd.Modules.Management.AccessGroups
{
    /// <summary>
	/// Interaction logic for AccessGroupsView.xaml
    /// </summary>
	public partial class AccessGroupsView : RadPane, IAccessGroupsView
    {
		public AccessGroupsView()
        {
            InitializeComponent();
        }

		public AccessGroupsPresentationModel Model
        {
            get
            {
				return this.DataContext as AccessGroupsPresentationModel;
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

		private void NewType_Click (object sender, RoutedEventArgs e)
		{
			this.Model.ExecuteAddAccessTypeCommand (string.Empty);
		}

		private void EditType_Click (object sender, RoutedEventArgs e)
		{
			this.Model.ExecuteEditAccessTypeCommand (string.Empty);
		}

		private void NewGroup_Click (object sender, RoutedEventArgs e)
		{
			this.Model.ExecuteAddAccessGroupCommand (string.Empty);
		}

		private void EditGroup_Click (object sender, RoutedEventArgs e)
		{
			this.Model.ExecuteEditAccessGroupCommand (string.Empty);
		}

		private void RemoveGroup_Click (object sender, RoutedEventArgs e)
		{
			this.Model.RemoveGroupByID ();			
		}

		private void AccessGroupListBox_SelectionChanged (object sender, SelectionChangedEventArgs e)
		{
			this.Model.LoadGroupedAccessTypes ();
		}

		private void AddType_Click (object sender, RoutedEventArgs e)
		{
			this.Model.AddAccessTypeToGroup ();
		}

		private void AddAll_Click (object sender, RoutedEventArgs e)
		{
			this.Model.AddAllAccessTypesToGroup ();
		}

		private void RemoveType_Click (object sender, RoutedEventArgs e)
		{
			this.Model.RemoveGroupedAccessType ();
		}

		private void RemoveAll_Click (object sender, RoutedEventArgs e)
		{
			this.Model.RemoveAllGroupedAccessTypes ();
		}

	}
}
