using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace ClinSchd.Modules.DataAccess.DataAccess
{
    /// <summary>
    /// Interaction logic for DataAccessView.xaml
    /// </summary>
    public partial class DataAccessView : UserControl, IDataAccessView
    {
        public DataAccessView()
        {
            InitializeComponent();
        }

        public DataAccessPresentationModel Model
        {
            get
            {
                return this.DataContext as DataAccessPresentationModel;
            }

            set
            {
                this.DataContext = value;
            }
        }

		private void ButtonTool_Click(object sender, RoutedEventArgs e)
		{
		}
	}
}
