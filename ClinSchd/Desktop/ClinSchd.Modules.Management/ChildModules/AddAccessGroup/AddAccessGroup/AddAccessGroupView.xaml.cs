using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Telerik.Windows.Controls;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections;

using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Modules.Management.AddAccessGroup
{
    /// <summary>
	/// Interaction logic for AddAccessGroupView.xaml
    /// </summary>
	public partial class AddAccessGroupView : Window, IAddAccessGroupView
    {
		public AddAccessGroupView()
        {
            InitializeComponent();
        }

		public AddAccessGroupPresentationModel Model
        {
            get
            {
				return this.DataContext as AddAccessGroupPresentationModel;
            }

            set
            {
                this.DataContext = value;
            }
        }

		protected override void OnClosing (System.ComponentModel.CancelEventArgs e)
		{
			this.Visibility = Visibility.Hidden;
			e.Cancel = true;
		}

		private void CancelButton_Click (object sender, RoutedEventArgs e)
		{
			Close ();
		}

		private void OkButton_Click (object sender, RoutedEventArgs e)
		{
			this.Model.AddEditAccessTypeGroup ();
			if (this.Model.ValidationMessage.IsValid) {
				Close ();
			} else {
				this.Model.View.AlertUser (this.Model.ValidationMessage.Message, this.Model.ValidationMessage.Title);
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

	}
}
