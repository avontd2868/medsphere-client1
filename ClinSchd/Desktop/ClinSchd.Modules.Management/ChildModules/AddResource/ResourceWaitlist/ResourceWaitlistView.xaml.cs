using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Telerik.Windows.Controls;

using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Modules.Management.AddResource
{
    /// <summary>
	/// Interaction logic for AddResourceView.xaml
    /// </summary>
	public partial class ResourceWaitlistView : Window, IResourceWaitlistView
    {
		public ResourceWaitlistView()
        {
            InitializeComponent();
        }

		public ResourceWaitlistPresentationModel Model
        {
            get
            {
				return this.DataContext as ResourceWaitlistPresentationModel;
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

		private void RPMSClinicName_SelectionChanged (object sender, SelectionChangedEventArgs e)
		{
			if (this.Model.HospitalLocationID != null) {
				this.Model.LoadClinicSetupParemters ();
			}
		}

		private void OK_Click (object sender, RoutedEventArgs e)
		{
			this.Close ();
		}

	}
}
