using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

using Telerik.Windows.Controls;
using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Modules.FindAppt.FindAppt
{
	/// <summary>
	/// Interaction logic for FindApptView.xaml
	/// </summary>
	public partial class FindApptView : Window, IFindApptView
	{
		public FindApptView ()
		{
			InitializeComponent();
		}

		public FindApptPresentationModel Model
		{
			get
			{
				return this.DataContext as FindApptPresentationModel;
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
			this.Model.ExecuteFindApptAppointmentCommand ("Find Clinic Availability");
			this.SearchResultGrid.Rebind ();

			if (!this.Model.ValidationMessage.IsValid) {
				this.Model.View.AlertUser (this.Model.ValidationMessage.Message, this.Model.ValidationMessage.Title);
			} 
		}

		private void Button_Cancel_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		public void Refresh ()
		{
			this.SearchResultGrid.Rebind ();
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

		private void AccessGroup_SelectionChanged (object sender, SelectionChangedEventArgs e)
		{
			this.Model.LoadGroupedAccessTypes ();
		}

		private void calender_SelectionChanged (object sender, SelectionChangedEventArgs e)
		{
			foreach (DateTime date in e.AddedItems) {
				this.Model.SelectedDates.Insert(0, date.ToShortDateString ());
			}
		}

		private void AMRadioButton_Checked (object sender, RoutedEventArgs e)
		{
			if (this.Model != null) {
				this.Model.IsAMChecked = true;
				this.Model.IsPMChecked = false;
				this.Model.IsBothChecked = false;
			}
		}

		private void PMRadioButton_Checked (object sender, RoutedEventArgs e)
		{
			if (this.Model != null) {
				this.Model.IsAMChecked = false;
				this.Model.IsPMChecked = true;
				this.Model.IsBothChecked = false;
			}
		}

		private void BOTHRadioButton_Checked (object sender, RoutedEventArgs e)
		{
			if (this.Model != null) {
				this.Model.IsAMChecked = false;
				this.Model.IsPMChecked = false;
				this.Model.IsBothChecked = true;
			}
		}
	}
}
