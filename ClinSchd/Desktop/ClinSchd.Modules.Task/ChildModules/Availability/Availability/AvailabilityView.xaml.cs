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

namespace ClinSchd.Modules.Task.Availability
{
    /// <summary>
	/// Interaction logic for AvailabilityView.xaml
    /// </summary>
	public partial class AvailabilityView : RadPane, IAvailabilityView, ITaskView
    {
		private bool shouldUpdate;
		private AvailabilityPresentationModel myModel
		{
			get
			{
				return this.Model as AvailabilityPresentationModel;
			}
			set
			{
				this.Model = value;
			}
		}
		public AvailabilityView ()
		{
			InitializeComponent();
		}

		public ITaskPresentationModel Model
        {
            get
            {
				return this.DataContext as AvailabilityPresentationModel;
            }

            set
            {
                this.DataContext = value;
            }
        }

		public RadScheduler Scheduler
		{
			get
			{
				return this.scheduler;
			}

			set
			{
				this.scheduler = value;
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

		public void SchedulerView_Loaded (object sender, RoutedEventArgs e)
		{
			List<ItemsControl> markerList = new List<ItemsControl> ();
			UIHelper.FindChildren<ItemsControl> (this.scheduler, "IntervalMarkersList", markerList);

			foreach (ItemsControl itemList in markerList) {
				List<Border> borderList = new List<Border> ();
				UIHelper.FindChildren<Border> (itemList, null, borderList);

				for (int index = 0; index < itemList.Items.Count; index++) {
					if (itemList.Items[index] is TimeSlot) {
						TimeSlot slot = itemList.Items[index] as TimeSlot;
						if (slot.Start.Minute == 0) {
							borderList[index + 1].BorderThickness = new Thickness (0, 3, 0, 0);
						}
						//Label label = new Label();
						//label.Foreground = markerList[0].Foreground;
						////this.scheduler.TimeRulerHostStyle..Foreground;
						//label.FontSize = 10;
						//label.FontFamily = this.scheduler.FontFamily;
						//label.FontWeight = FontWeights.Thin;
						//label.HorizontalAlignment = HorizontalAlignment.Right;
						//label.Content = slot.Start.Minute.ToString("00");
						//borderList[index + 1].Child = label;
					}
				}
			}
		}

		private void scheduler_AppointmentCreating (object sender, AppointmentCreatingEventArgs e)
		{
			//turn off the default adding appointment dialog window.
			e.Cancel = true;
		}

		AppointmentEditAction appointmentEditAction;
		DateTime preEditStart;
		DateTime preEditEnd;
		private void scheduler_AppointmentEdited (object sender, AppointmentEditedEventArgs e)
		{
			DialogParameters dialogParams = new DialogParameters ();
			switch (appointmentEditAction) {
			case AppointmentEditAction.Drag:
				if (!(e.Appointment.Start == preEditStart && e.Appointment.End == preEditEnd)) {
					dialogParams.Closed = (s, args) => {
						if (args.DialogResult == true) {
							myModel.MoveResizeAvailability (e.Appointment);
						} else {
							e.Appointment.Start = preEditStart;
							e.Appointment.End = preEditEnd;
						}
					};
					dialogParams.Content = "Are you sure you want to move this access block?"
						+ Environment.NewLine + Environment.NewLine + "New access block time: " + e.Appointment.Start.ToString ();
					RadWindow.Confirm (dialogParams);
				}
				break;
			case AppointmentEditAction.Edit:
				break;
			case AppointmentEditAction.EditInline:
				break;
			case AppointmentEditAction.Resize:
				if (!(e.Appointment.Start == preEditStart && e.Appointment.End == preEditEnd)) {
					dialogParams.Closed = (s, args) => {
						if (args.DialogResult == true) {
							myModel.MoveResizeAvailability (e.Appointment);
						} else {
							e.Appointment.Start = preEditStart;
							e.Appointment.End = preEditEnd;
						}
					};
					dialogParams.Content = "Are you sure you want to change the length of this access block?"
						+ Environment.NewLine + Environment.NewLine + "New access block length: " + e.Appointment.End.Subtract (e.Appointment.Start).TotalMinutes + " minutes";
					RadWindow.Confirm (dialogParams);
				}
				break;
			default:
				break;
			}
		}

		private void scheduler_AppointmentEditing (object sender, AppointmentEditingEventArgs e)
		{
			if (e.AppointmentEditAction != AppointmentEditAction.Edit) {
				appointmentEditAction = e.AppointmentEditAction;
				preEditStart = e.Appointment.Start;
				preEditEnd = e.Appointment.End;
			} else {
				e.Cancel = true;
			}
		}

		private void scheduler_AppointmentDeleting (object sender, AppointmentDeletingEventArgs e)
		{
			SchdAvailability deletedAppointment = myModel.SchdAvailabilityFactory.Create ();
			foreach (Resource item in e.Appointment.Resources) {
				if (item.ResourceType == "APPOINTMENTID") {
					deletedAppointment.APPOINTMENTID = item.ResourceName;
				}
			}
			myModel.SchdAvailability = deletedAppointment;
			myModel.DeleteAvailability (myModel.SchdAvailability.APPOINTMENTID);

			e.Cancel = true;
		}
		
		private void LoadNewAppointmentInfo(TimeSlot selectedTimeSlot)
		{
			myModel.SchdAvailability.StartTime = selectedTimeSlot.Start.ToString ();
			myModel.SchdAvailability.EndTime = selectedTimeSlot.End.ToString ();
			myModel.SchdAvailability.RESOURCENAME = myModel.ResourceName;
		}

		private void LoadExistAppointmentInfo (IAppointment selectedAppointment)
		{
			IResource selectedResource = new Resource ();
			foreach (IResource _resource in selectedAppointment.Resources) {
			    if (_resource.ResourceType == "APPOINTMENTID") {
					myModel.SchdAvailability.APPOINTMENTID = _resource.ResourceName;
			    }
			    if (_resource.ResourceType == "RESOURCENAME") {
					myModel.SchdAvailability.RESOURCENAME = _resource.ResourceName;
			    }
			    if (_resource.ResourceType == "ACCESSTYPEID") {
					myModel.SchdAvailability.ACCESSTYPEID = _resource.ResourceName;
			    }
			    if (_resource.ResourceType == "NOTE") {
					myModel.SchdAvailability.Note = _resource.ResourceName;
			    }
			    if (_resource.ResourceType == "START_TIME") {
					myModel.SchdAvailability.StartTime = _resource.ResourceName;
			    }
			    if (_resource.ResourceType == "END_TIME") {
					myModel.SchdAvailability.EndTime = _resource.ResourceName;
			    }
				if (_resource.ResourceType == "SLOTS") {
					myModel.SchdAvailability.SLOTS = int.Parse(_resource.ResourceName);
				}
			}
		}

		private void SchedulerContextMenu_ItemClick (object sender, Telerik.Windows.RadRoutedEventArgs e)
		{
			RadMenuItem item = e.OriginalSource as RadMenuItem;
			string headerText = item.Header as string;
			myModel.SchdAvailability = myModel.SchdAvailabilityFactory.Create ();

			switch (headerText) {
			case "Add New Access Block":
				LoadNewAppointmentInfo (scheduler.SelectedTimeSlot);
				myModel.AddEditAvailabilityCommand.Execute ("Add New Access Block");
				break;
			case "Edit Access Block":
				LoadExistAppointmentInfo (scheduler.SelectedAppointment);
				myModel.AddEditAvailabilityCommand.Execute ("Edit Access Block");
				break;
			case "Delete Access Block":
				if (ConfirmUser("Are you sure you want to delete this access block?", "Access Block")){
					LoadExistAppointmentInfo (scheduler.SelectedAppointment);
					myModel.DeleteAvailability (myModel.SchdAvailability.APPOINTMENTID);
				}
				break;
			}
		}

		private void scheduler_SelectedViewStartDateChanged (object sender, EventArgs e)
		{
			myModel.RefreshAvailabilityScheduler ("");
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

		//Default the time slot to be 8AM
		private void scheduler_ActiveViewDefinitionChanged (object sender, EventArgs e)
		{
			if (this.scheduler.ActiveViewDefinition is WeekViewDefinition) {
				this.shouldUpdate = true;
			}  
		}

		private void scheduler_LayoutUpdated (object sender, EventArgs e)
		{
			if (this.shouldUpdate) {
				this.shouldUpdate = false;
				var appointmentControl = this.scheduler.ChildrenOfType<AppointmentsControl> ().Single (ac => ac.Name == "NotAllDayAppointmentsControl");

				var timeSlotItemHeight = appointmentControl.ChildrenOfType<TimeSlotItem> ().First ().ActualHeight;
				ScrollViewer scrollVeiwer = appointmentControl.ParentOfType<ScrollViewer> ();

				int timeSlotsToSkip = myModel.TimeSlotToSkip;
				scrollVeiwer.ScrollToVerticalOffset (timeSlotsToSkip * timeSlotItemHeight);
			}  

		}

		private void paneTaskAvailability_Unloaded (object sender, RoutedEventArgs e)
		{
			Model.ClearViewFromList ();
		}

		private void scheduler_MouseUp (object sender, MouseButtonEventArgs e)
		{
			if (((Telerik.Windows.Controls.RadScheduler)(sender)).IsReadOnly) {
				myModel.ExecuteResetUnselectedTabCommand ("Reset Buttons");
				return;
			}

			IAppointment selectedAppointment = ((Telerik.Windows.Controls.RadScheduler)(sender)).SelectedAppointment;
			myModel.SchdAvailability = myModel.SchdAvailabilityFactory.Create ();
			myModel.SelectedAppointment = myModel.SchdAppointmentFactory.Create ();
			if (selectedAppointment == null) {
				myModel.SelectedAppointment.START_TIME = scheduler.SelectedTimeSlot.Start.ToString ();
				myModel.SelectedAppointment.END_TIME = scheduler.SelectedTimeSlot.End.ToString ();
				myModel.SelectedAppointment.RESOURCENAME = myModel.ResourceName;

				myModel.PasteAvailabilityCommand.Execute ("New Availability");
			} else {
				LoadExistAppointmentInfo (selectedAppointment);
				((Telerik.Windows.Controls.RadScheduler)(sender)).SelectedAppointment = null;
				myModel.CopyAvailabilityCommand.Execute ("New Availability");
			}
		}

		private void paneTaskScheduler_Loaded (object sender, EventArgs e)
		{
			Dispatcher.BeginInvoke (new Action (() => {
				Dispatcher.BeginInvoke (new Action (() => {
					myModel.LoadAppointments ();
				}));
			}));
		}

		public void Close ()
		{
			RemoveFromParent ();
		}

	}
}
