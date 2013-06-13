using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

using Telerik.Windows.Controls;

namespace ClinSchd.Modules.Management.Group
{
    /// <summary>
    /// Interaction logic for GroupView.xaml
    /// </summary>
	public partial class GroupView : Window, IGroupView
    {
        public GroupView()
        {
            InitializeComponent();
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

		public event EventHandler<EventArgs> ShowManagement;

		protected override void OnClosing (System.ComponentModel.CancelEventArgs e)
		{
			this.Visibility = Visibility.Hidden;
			e.Cancel = true;
		} 

		private void Close_Click (object sender, RoutedEventArgs e)
		{
			Close ();
		}

    }
}
