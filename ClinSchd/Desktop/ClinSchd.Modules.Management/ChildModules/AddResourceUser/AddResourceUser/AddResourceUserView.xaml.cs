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

namespace ClinSchd.Modules.Management.AddResourceUser
{
    /// <summary>
	/// Interaction logic for AddResourceUserView.xaml
    /// </summary>
	public partial class AddResourceUserView : Window, IAddResourceUserView
    {
		public AddResourceUserView()
        {
            InitializeComponent();
        }

		public AddResourceUserPresentationModel Model
        {
            get
            {
				return this.DataContext as AddResourceUserPresentationModel;
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
			this.Model.AddEditResourceUser ();
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

		private void Overbook_SelectionChanged (object sender, SelectionChangedEventArgs e)
		{
			switch (this.Model.OverbookValue) {
			case "0":
				this.Model.IsUpdateChecked = false;
				this.Model.OnPropertyChanged ("IsUpdateChecked");
				this.Model.IsModifyChecked = false;
				this.Model.OnPropertyChanged ("IsModifyChecked");
				break;
			case "1":
			case "2":
				this.Model.IsUpdateChecked = true;
				this.Model.OnPropertyChanged ("IsUpdateChecked");
				break;
			default:
				break;
			}
		}

		private void UpdateAppointment_Checked (object sender, RoutedEventArgs e)
		{
			if (this.Model.IsUpdateChecked == true) {
				if (this.Model.OverbookValue == "0") {
					this.Model.OverbookValue = "1";
					this.Model.OnPropertyChanged ("OverbookValue");
				}
				this.Model.IsModifyChecked = true;
				this.Model.OnPropertyChanged ("IsModifyChecked");
			} 
		}

		private void ModifyScheduler_Checked (object sender, RoutedEventArgs e)
		{
			if (this.Model.IsModifyChecked == true) {
				if (this.Model.OverbookValue == "0") {
					this.Model.OverbookValue = "1";
					this.Model.OnPropertyChanged ("OverbookValue");
				}
			}
			this.Model.IsUpdateChecked = true;
			this.Model.OnPropertyChanged ("IsUpdateChecked");
		}

	}
}
