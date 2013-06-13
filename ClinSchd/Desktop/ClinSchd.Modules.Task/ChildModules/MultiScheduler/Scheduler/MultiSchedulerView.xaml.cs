using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

using Telerik.Windows.Controls;
using ClinSchd.Modules.Task.Group;
using Microsoft.Practices.Composite.Regions;

namespace ClinSchd.Modules.Task.MultiScheduler
{
    /// <summary>
    /// Interaction logic for GroupView.xaml
    /// </summary>
	public partial class MultiSchedulerView : RadPane, IMultiSchedulerView, ITaskView, IGroupView
    {
        public MultiSchedulerView()
        {
            InitializeComponent();
        }

        public MultiSchedulerPresentationModel Model
        {
            get
            {
                return this.DataContext as MultiSchedulerPresentationModel;
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


		#region ITaskView Members

		ITaskPresentationModel ITaskView.Model
		{
			get
			{
				return this.DataContext as MultiSchedulerPresentationModel;
			}

			set
			{
				this.DataContext = value;
			}
		}

		public RadSplitContainer MultiSchedulerGroupControl
		{
			get
			{
				return this.MultiSchedulerGroup;
			}
		}

		#endregion

		#region IGroupView Members


		GroupPresentationModel IGroupView.Model
		{
			get
			{
				throw new NotImplementedException ();
			}
			set
			{
				throw new NotImplementedException ();
			}
		}

		public void FocusSchedulerGroup ()
		{

		}

		#endregion

		private void RadPane_Unloaded (object sender, RoutedEventArgs e)
		{
			Model.ClearViewFromList ();
		}
	}
}
