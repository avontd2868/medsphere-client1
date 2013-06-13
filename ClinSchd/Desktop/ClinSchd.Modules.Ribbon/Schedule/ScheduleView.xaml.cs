using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Telerik.Windows.Controls;
using ClinSchd.Infrastructure.Models;
using System.Windows.Input;
using System.Timers;

namespace ClinSchd.Modules.Ribbon.Schedule
{
    /// <summary>
	/// Interaction logic for ScheduleView.xaml
    /// </summary>
    public partial class ScheduleView : UserControl, IScheduleView
    {
		private Timer patientSearchTimer;
		private string patientSearchString;
		public ScheduleView ()
        {
            InitializeComponent();
			patientSearchTimer = new Timer (750);
			patientSearchTimer.Elapsed += new ElapsedEventHandler (patientSearchTimer_Elapsed);
        }

		public SchedulePresentationModel Model
        {
            get
            {
				return this.DataContext as SchedulePresentationModel;
            }

            set
            {
                this.DataContext = value;
            }
        }

		public void FocusPatientSearch ()
		{
			Keyboard.Focus (patientSearchComboBox);
		}

		private void PatientSelection_TextChanged (object sender, TextChangedEventArgs e)
		{
			patientSearchString = ((RadComboBox)sender).Text.Replace (" ", "");
			if (!((RadComboBox)sender).IsDropDownOpen) {
				((RadComboBox)sender).IsDropDownOpen = true;
			}
		}

		private void radCombox2_SelectionChanged (object sender, SelectionChangedEventArgs e)
		{
			string TimeScale = "30";

			if ((((Telerik.Windows.Controls.RadComboBox)(sender))).IsDropDownOpen) {
				switch (((Telerik.Windows.Controls.RadComboBox)(sender)).SelectedIndex) {
				case 0:
					TimeScale = "10";
					break;

				case 1:
					TimeScale = "15";
					break;

				case 2:
					TimeScale = "20";
					break;

				case 3:
					TimeScale = "30";
					break;
				}

				this.Model.ExecuteViewTimeScaleCommand (TimeScale);
			}

		}

		private void RadComboBox_SelectionChanged (object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count > 0) {
				Model.SelectPatient (e.AddedItems[0] as Patient);
				timerFired ();
			
				if (sender is RadComboBox) {
					Model.BlurPatientSearch (sender as RadComboBox, (e.AddedItems[0] as Patient).Name);
				}
			}
		}

		private void RadComboBox_KeyUp (object sender, KeyEventArgs e)
		{
			patientSearchTimer.Stop ();
			if (e.Key != Key.Up && e.Key != Key.Down) {
				patientSearchTimer.Start ();
			}
		}

		void patientSearchTimer_Elapsed (object sender, ElapsedEventArgs e)
		{
			timerFired();
		}

		private void timerFired ()
		{
			patientSearchTimer.Stop ();
			Dispatcher.BeginInvoke ((Action)(() => {
				Model.SearchStringChanged (patientSearchString);
			}));
		}

		private void patientSearchComboBox_LostFocus (object sender, RoutedEventArgs e)
		{
			patientSearchComboBox.Text = string.Empty;
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
