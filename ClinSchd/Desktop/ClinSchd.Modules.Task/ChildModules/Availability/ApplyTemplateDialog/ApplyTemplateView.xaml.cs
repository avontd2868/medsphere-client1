using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

using Telerik.Windows.Controls;

using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Modules.Task.Availability
{
	/// <summary>
	/// Interaction logic for ApplyTemplateView.xaml
	/// </summary>
	public partial class ApplyTemplateView : Window
	{
		public ApplyTemplateView ()
		{
			InitializeComponent();
		}

		protected override void OnClosing (System.ComponentModel.CancelEventArgs e)
		{
		} 

		private void Button_OK_Click(object sender, RoutedEventArgs e)
		{
			//this.Model.ExecuteCheckInAppointmentCommand ("CheckIn Patient");

			//if (this.Model.ValidationMessage.IsValid) {
				this.DialogResult = true;
				Close ();
			//} else {
				//this.Model.View.AlertUser (this.Model.ValidationMessage.Message, this.Model.ValidationMessage.Title);
			//}
		}

		private void Button_Cancel_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
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

		private void RadNumericUpDown_Initialized (object sender, EventArgs e)
		{
			((RadNumericUpDown)sender).NumberFormatInfo.NumberDecimalDigits = 0;
		}

	}
}
