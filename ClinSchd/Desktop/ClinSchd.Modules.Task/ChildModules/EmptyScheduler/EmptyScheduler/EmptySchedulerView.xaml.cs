using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Collections.Generic;

using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Scheduler;
using System.Collections;
using System.Collections.ObjectModel;
using ClinSchd.Infrastructure;
using System.Linq;
using System.Net;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Telerik.Windows.Controls.Calendar;
using ClinSchd.Modules.Task.Group;
using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Modules.Task.EmptyScheduler
{
    /// <summary>
	/// Interaction logic for EmptySchedulerView.xaml
    /// </summary>
	public partial class EmptySchedulerView : RadPane, IEmptySchedulerView, ITaskView
    {
		private bool shouldUpdate;
		private EmptySchedulerPresentationModel myModel
		{
			get
			{
				return this.Model as EmptySchedulerPresentationModel;
			}
			set
			{
				this.Model = value;
			}
		}
		public EmptySchedulerView()
		{
			InitializeComponent();
		}

		public ITaskPresentationModel Model
        {
            get
            {
				return this.DataContext as EmptySchedulerPresentationModel;
            }

            set
            {
                this.DataContext = value;
            }
        }

		public void Collapse()
		{
			IsHidden = true;
		}

		public void Expand()
		{
			IsHidden = false;
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

		private void paneTaskEmptyScheduler_Unloaded (object sender, RoutedEventArgs e)
		{
			Model.ClearViewFromList ();
		}		
	}
}
