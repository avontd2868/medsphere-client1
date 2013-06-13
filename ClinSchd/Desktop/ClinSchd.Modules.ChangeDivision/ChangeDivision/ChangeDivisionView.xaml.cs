using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

using Telerik.Windows.Controls;

using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Modules.ChangeDivision.ChangeDivision
{
    /// <summary>
	/// Interaction logic for ChangeDivisionView.xaml
    /// </summary>
	public partial class ChangeDivisionView : Window, IChangeDivisionView
    {
		public ChangeDivisionView ()
        {
            InitializeComponent();
        }

		public ChangeDivisionPresentationModel Model
        {
            get
            {
				return this.DataContext as ChangeDivisionPresentationModel;
            }

            set
            {
                this.DataContext = value;
            }
        }

		private void Button_OK_Click(object sender, RoutedEventArgs e)
		{
			this.Model.ExecuteChangeDivisionCommand ("Change Division");

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
