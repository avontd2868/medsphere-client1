using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

using Telerik.Windows.Controls;

using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Modules.CheckOut.CheckOut
{
	/// <summary>
	/// Interaction logic for CheckOutView.xaml
	/// </summary>
	public partial class CheckOutView : Window, ICheckOutView
	{
		public CheckOutView ()
		{
			InitializeComponent();
		}

		public CheckOutPresentationModel Model
		{
			get
			{
				return this.DataContext as CheckOutPresentationModel;
			}

			set
			{
				this.DataContext = value;
			}
		}

		protected override void OnClosing (System.ComponentModel.CancelEventArgs e)
		{
		} 

		private void Button_OK_Click(object sender, RoutedEventArgs e)
		{
			this.Model.ExecuteCheckOutAppointmentCommand ("CheckOut Patient");

			if (this.Model.ValidationMessage.IsValid) {
				Close ();
			} else {
				this.Model.View.AlertUser (this.Model.ValidationMessage.Message, this.Model.ValidationMessage.Title);
			}
		}

		private void Button_Cancel_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		public void Refresh ()
		{
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
