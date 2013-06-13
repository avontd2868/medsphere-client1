using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

using Telerik.Windows.Controls;

using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Modules.MarkAsNoShow.MarkAsNoShow
{
	/// <summary>
	/// Interaction logic for ChangeDivisionView.xaml
	/// </summary>
	public partial class MarkAsNoShowView : Window, IMarkAsNoShowView
	{
		public MarkAsNoShowView ()
		{
			InitializeComponent();
		}

		public MarkAsNoShowPresentationModel Model
		{
			get
			{
				return this.DataContext as MarkAsNoShowPresentationModel;
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
			this.Model.ExecuteNoShowAppointmentCommand ("Mark As NoShow Appointment");
			if (this.Model.ValidationMessage.IsValid) {
				Close ();
			} else if (!this.Model.ValidationMessage.IsValid && this.Model.ValidationMessage.Title == "AutoRebook Appointment") {
				this.Model.View.AlertUser (this.Model.ValidationMessage.Message, this.Model.ValidationMessage.Title);
				Close ();
			} else {
				this.Model.View.AlertUser (this.Model.ValidationMessage.Message, this.Model.ValidationMessage.Title);
			}
		}

		private void Button_Cancel_Click(object sender, RoutedEventArgs e)
		{
			Close();
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

		private void AnyType_Checked (object sender, RoutedEventArgs e)
		{
			this.Model.AutoRebook.IsAccessTypeChecked = false;
			this.Model.OnPropertyChanged ("AutoRebook");
		}

	}
}
