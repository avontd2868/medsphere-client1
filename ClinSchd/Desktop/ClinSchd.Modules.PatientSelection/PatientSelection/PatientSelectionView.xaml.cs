using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

using Telerik.Windows.Controls;

using ClinSchd.Infrastructure.Models;
using Microsoft.Practices.Unity;

namespace ClinSchd.Modules.PatientSelection.PatientSelection
{
    /// <summary>
    /// Interaction logic for PatientSelectionView.xaml
    /// </summary>
	public partial class PatientSelectionView : Window, IPatientSelectionView
    {
		public PatientSelectionView()
        {
            InitializeComponent();
        }

		public PatientSelectionPresentationModel Model
        {
            get
            {
				return this.DataContext as PatientSelectionPresentationModel;
            }

            set
            {
                this.DataContext = value;
            }
        }

		protected override void OnClosing (System.ComponentModel.CancelEventArgs e)
		{
		} 

		private void PatientSelection_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Model.SelectedPatient = Model.PatientFactory.Create ();
			this.Model.NewAppointmentCommand.Execute (Model.SelectedPatient.IEN);
			Close();
		}

		private void Button_OK_Click(object sender, RoutedEventArgs e)
		{
			Model.SelectedPatient = Model.PatientFactory.Create ();
			this.Model.NewAppointmentCommand.Execute (Model.SelectedPatient.IEN);
			Close ();
		}

		private void Button_Cancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Model.SelectedPatient = null;
			Close();
		}

		private void PatientSelection_TextChanged(object sender, TextChangedEventArgs e)
		{
			if ( sender is TextBox )
			{
				Model.SearchStringChanged(((TextBox)sender).Text);
			}
		}
	}
}
