using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;

using ClinSchd.Infrastructure.Models;

using Telerik.Windows.Controls;

namespace ClinSchd.Modules.Navigation.ResourceTree
{
    /// <summary>
	/// Interaction logic for ResourceTreeView.xaml
    /// </summary>
	public partial class ResourceTreeView : RadTreeView, IResourceTreeView
    {
		public ResourceTreeView ()
        {
            InitializeComponent();
        }

		public ResourceTreePresentationModel Model
        {
            get
            {
				return this.DataContext as ResourceTreePresentationModel;
            }

            set
            {
                this.DataContext = value;
            }
        }

		public void Refresh()
		{
			//this.WorkingPatients.Rebind();
			//if (WorkingPatients.SelectedItem == null && WorkingPatients.Items.Count > 0)
			//    WorkingPatients.SelectedItem = WorkingPatients.Items[0];
		}

		private void RadTreeView1_ItemSelected (object sender, Telerik.Windows.RadRoutedEventArgs e)
		{
			this.Model.AdmitPatientCommand.Execute (resource1.Header.ToString());
		}

    }
}
