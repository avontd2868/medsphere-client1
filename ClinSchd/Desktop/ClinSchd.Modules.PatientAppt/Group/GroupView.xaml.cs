using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

using Telerik.Windows.Controls;

namespace ClinSchd.Modules.PatientAppt.Group
{
    /// <summary>
    /// Interaction logic for GroupView.xaml
    /// </summary>
	public partial class GroupView : Window, IGroupView
    {
        public GroupView()
        {
            InitializeComponent();
			LoadReportSelection ();
        }

        public GroupPresentationModel Model
        {
            get
            {
                return this.DataContext as GroupPresentationModel;
            }

            set
            {
                this.DataContext = value;
            }
        }

		public event EventHandler<EventArgs> ShowPatientAppt;
		public DateTime m_dStartTime = DateTime.Today;

		protected override void OnClosing (System.ComponentModel.CancelEventArgs e)
		{
		}

		private void CancelButton_Click (object sender, RoutedEventArgs e)
		{
			Close ();
		}

		private void OKButton_Click (object sender, RoutedEventArgs e)
		{
			this.Model.ExecuteNewAppointmentCommand (this.Model.AppointmentData.AppointmentCommand);
			if (this.Model.ValidationMessage.IsValid) {
				Close ();
			} else {
				this.Model.View.AlertUser (this.Model.ValidationMessage.Message, this.Model.ValidationMessage.Title);
			}
		}
		
		private void LoadReportSelection ()
		{
			if (ReportCombo.Items.Count == 0) {
				if (!(m_dStartTime < DateTime.Today)) {
					ReportCombo.Items.Add ("Patient Letter");
				}
				ReportCombo.Items.Add ("Patient Appointments");
				ReportCombo.Items.Add ("Patient History");
				ReportCombo.Items.Add ("Patient HS Merge");
				ReportCombo.SelectedIndex = 0;
			}
		}

		public void Refresh ()
		{
			this.AppointmentsPatientApptGrid.Rebind ();
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

		public RadComboBox ReportComboBox 
		{
			get
			{
				return this.ReportCombo;
			}
		}

	}
}
