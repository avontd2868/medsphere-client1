using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

using Telerik.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;

namespace ClinSchd.Modules.Task.Group
{
    /// <summary>
    /// Interaction logic for GroupView.xaml
    /// </summary>
	public partial class GroupView : RadPane, IGroupView
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

        public event EventHandler<EventArgs> ShowTask;

		private void TasksDocking_Close(object sender, Telerik.Windows.Controls.Docking.StateChangeEventArgs e)
		{
		}

		private void RadPaneGroup_SelectionChanged (object sender, RoutedEventArgs e)
		{
			for (int i = 0; i < SchedulerGroup.Items.Count; i++) {
				if (SchedulerGroup[i].IsSelected) {
					RadPane SelectedTab = (RadPane)SchedulerGroup.Items[i];
					ITaskPresentationModel SelectedModel = ((ITaskView)(SelectedTab)).Model;
					SelectedModel.IsSelectedTab = true;
					if (SelectedModel.SelectedResource != null) {
						SelectedModel.TaskTabSelectedCommand.Execute (SelectedModel.SelectedResource);
					} else if (SelectedModel.SelectedResourceGroup != null) {
						SelectedModel.TaskTabSelectedCommand.Execute (SelectedModel.SelectedResourceGroup);
					}
					if (SelectedModel is Availability.AvailabilityPresentationModel) {
						Model.HideLegend ();
					} else {
						Model.ShowLegend ();
					}
				} else {
					RadPane UnSelectedTab = (RadPane)SchedulerGroup.Items[i];
					ITaskPresentationModel UnSelectedModel = ((ITaskView)(UnSelectedTab)).Model;
					UnSelectedModel.IsSelectedTab = false;
					UnSelectedModel.ResetUnselectedTabCommand.Execute ("Reset Buttons");
					
				}
			}
		}

		public void FocusSchedulerGroup ()
		{
			Keyboard.Focus (SchedulerGroup.SelectedPane);
		}
    }
}
