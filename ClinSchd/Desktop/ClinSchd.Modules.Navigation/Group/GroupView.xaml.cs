using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Collections.Generic;

using Telerik.Windows.Controls;
using ClinSchd.Infrastructure.Models;
using Telerik.Windows.Controls.GridView;
using System.Windows.Threading;

namespace ClinSchd.Modules.Navigation.Group
{
    /// <summary>
	/// Interaction logic for GroupView.xaml
    /// </summary>
	public partial class GroupView : RadPane, IGroupView
	{
		public GroupView ()
		{
			InitializeComponent ();
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

		private void AdmittedPatients_SelectionChanged (object sender, SelectionChangeEventArgs e)
		{
			if (e.AddedItems.Count > 0) {
				Model.SelectPatient (e.AddedItems[0] as Patient);
			}
		}

		private void PatientContextMenu_ItemClick (object sender, Telerik.Windows.RadRoutedEventArgs e)
		{
			switch ((e.Source as RadMenuItem).Header.ToString()) {
			case "Remove Patient":
				Model.RemovePatientFromWorkspace (PatientContextMenu.GetClickedElement<GridViewRow> ().Item as Patient);
				break;
			case "Clear Workspace":
				Model.AdmittedPatients.Clear ();
				break;
			default:
				break;
			}
		}

		private void PatientContextMenu_Opened (object sender, RoutedEventArgs e)
		{
			GridViewRow selectedRow = PatientContextMenu.GetClickedElement<GridViewRow> ();
			RadMenuItem RemovePatientMenuItem = (PatientContextMenu.Items[0] as RadMenuItem);
			if (selectedRow == null) {
				RemovePatientMenuItem.IsEnabled = false;
			} else {
				RemovePatientMenuItem.IsEnabled = true;
			}
		}

		private void calendar_SelectionChanged (object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count > 0) {
				Model.ViewDayRange ((DateTime)e.AddedItems[0]);
			}
		}

		private void ClinicComboBox_SelectionChanged (object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count > 0) {
				Model.Resource = e.AddedItems[0] as SchdResource;
				Model.OnPropertyChanged ("Resource");
			}
		}

		private void ProviderComboBox_SelectionChanged (object sender, SelectionChangedEventArgs e)
		{
			Model.Provider = e.AddedItems[0] as Provider;
			Model.OnPropertyChanged ("Provider");
		}

		private void ResourceGroupComboBox_SelectionChanged (object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count > 0) {
				Model.ResourceGroup = e.AddedItems[0] as SchdResourceGroup;
				Model.OnPropertyChanged ("ResourceGroup");
			}
		}
	}
}
