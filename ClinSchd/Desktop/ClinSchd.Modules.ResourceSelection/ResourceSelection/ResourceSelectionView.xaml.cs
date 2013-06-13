using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Timers;
using Telerik.Windows.Controls;
using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Modules.ResourceSelection.ResourceSelection
{
    /// <summary>
    /// Interaction logic for PatientSelectionView.xaml
    /// </summary>
	public partial class ResourceSelectionView : Window, IResourceSelectionView
    {
		public ResourceSelectionView ()
        {
            InitializeComponent();
			resourceSearchTimer = new Timer (750);
			resourceSearchTimer.Elapsed += new ElapsedEventHandler (resourceSearchTimer_Elapsed);
		}

		public ResourceSelectionPresentationModel Model
        {
            get
            {
				return this.DataContext as ResourceSelectionPresentationModel;
            }

            set
            {
                this.DataContext = value;
            }
        }

		private void PatientSelection_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			DialogResult = true;
			Model.ResetResource ();
			Close();
		}

		private void Button_OK_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Model.ResetResource ();
			Close ();
		}

		private void Button_Cancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Model.SelectedResource = null;
			Close();
		}

		private Timer resourceSearchTimer;
		private string resourceSearchString;
		private void PatientSelection_TextChanged (object sender, TextChangedEventArgs e)
		{
			if ( sender is TextBox )
			{
				resourceSearchString = ((TextBox)sender).Text;
				Model.SearchStringChanged (resourceSearchString);
			}
		}

		private void txtSearch_KeyUp (object sender, System.Windows.Input.KeyEventArgs e)
		{
			resourceSearchTimer.Stop ();
			resourceSearchTimer.Start ();
		}

		void resourceSearchTimer_Elapsed (object sender, ElapsedEventArgs e)
		{
			resourceSearchTimer.Stop ();
			Dispatcher.Invoke ((Action)(() => {
				Model.SearchStringChanged (resourceSearchString);
			}));
		}

	}
}
