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

namespace ClinSchd.Modules.Task.Scheduler
{
    /// <summary>
	/// Interaction logic for SchedulerView.xaml
    /// </summary>
	public partial class SchedulerView : RadPane, ISchedulerView, ITaskView
    {
		private bool shouldUpdate;
		private SchedulerPresentationModel myModel
		{
			get
			{
				return this.Model as SchedulerPresentationModel;
			}
			set
			{
				this.Model = value;
			}
		}
		public SchedulerView()
		{
			InitializeComponent();
		}

		public ITaskPresentationModel Model
        {
            get
            {
				return this.DataContext as SchedulerPresentationModel;
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

		public void TimeScaleLoaded ()
		{
			this.Scheduler.Loaded += new RoutedEventHandler (SchedulerView_Loaded);
			//this.Scheduler.WeekViewDefinition.ViewDefinitionChanged += new EventHandler(WeekViewDefinition_ViewDefinitionChanged);
			this.Scheduler.GotFocus += new RoutedEventHandler (SchedulerView_Loaded);
		}

		//public void WeekViewDefinition_ViewDefinitionChanged (object sender, EventArgs e)
		//{
		//    List<ItemsControl> markerList = new List<ItemsControl> ();
		//    UIHelper.FindChildren<ItemsControl> (this.scheduler, "IntervalMarkersList", markerList);

		//    foreach (ItemsControl itemList in markerList) {
		//        List<Border> borderList = new List<Border> ();
		//        UIHelper.FindChildren<Border> (itemList, null, borderList);

		//        for (int index = 0; index < itemList.Items.Count; index++) {
		//            if (itemList.Items[index] is TimeSlot) {
		//                TimeSlot slot = itemList.Items[index] as TimeSlot;
		//                Label label = new Label ();
		//                label.Foreground = markerList[0].Foreground;
		//                //this.scheduler.TimeRulerHostStyle..Foreground;
		//                label.FontSize = 10;
		//                label.FontFamily = this.scheduler.FontFamily;
		//                label.FontWeight = FontWeights.Thin;
		//                label.HorizontalAlignment = HorizontalAlignment.Right;
		//                label.Content = slot.Start.Minute.ToString ("00");
		//                borderList[index + 1].Child = label;
		//            }
		//        }
		//    }
		//}

		public void SchedulerView_Loaded (object sender, RoutedEventArgs e)
		{
			List<ItemsControl> markerList = new List<ItemsControl>();
			UIHelper.FindChildren<ItemsControl>(this.scheduler, "IntervalMarkersList", markerList);

			foreach (ItemsControl itemList in markerList)
			{
				List<Border> borderList = new List<Border>();
				UIHelper.FindChildren<Border>(itemList, null, borderList);

				for (int index = 0; index < itemList.Items.Count; index++)
				{
					if (itemList.Items[index] is TimeSlot)
					{
						TimeSlot slot = itemList.Items[index] as TimeSlot;
						if (slot.Start.Minute == 0) {
							borderList[index + 1].BorderThickness = new Thickness (0,3,0,0);
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
			e.NewAppointment = new CustomAppointment ();
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
								if (!myModel.MoveAppointment (e.Appointment)) {
									e.Appointment.Start = preEditStart;
									e.Appointment.End = preEditEnd;
								}
							} else {
								e.Appointment.Start = preEditStart;
								e.Appointment.End = preEditEnd;
							}
						};
					dialogParams.Content = "Are you sure you want to move this appointment?"
						+ Environment.NewLine + Environment.NewLine + "New appointment time: " + e.Appointment.Start.ToString ();
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
							myModel.ResizeAppointment (e.Appointment);
						} else {
							e.Appointment.Start = preEditStart;
							e.Appointment.End = preEditEnd;
						}
					};
					dialogParams.Content = "Are you sure you want to change the length of this appointment?"
						+ Environment.NewLine + Environment.NewLine + "New appointment length: " + e.Appointment.End.Subtract (e.Appointment.Start).TotalMinutes + " minutes";
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
				foreach (Resource item in e.Appointment.Resources) {
					if (item.ResourceType == "Appointment Status") {
						switch (item.ResourceName) {
						case "CANCELLED":
							break;
						default:
							myModel.EditAppointmentCommand.Execute (null);
							break;
						}
						break;
					}
				}
			}
		}

		private void scheduler_AppointmentDeleting (object sender, AppointmentDeletingEventArgs e)
		{
			bool appointmentAlreadyCancelled = false;
			SchdAppointment deletedAppointment = myModel.SchdAppointmentFactory.Create ();
			foreach (Resource item in e.Appointment.Resources) {
				if (item.ResourceType == "APPOINTMENTID") {
					deletedAppointment.APPOINTMENTID = item.ResourceName;
				} else if (item.ResourceType == "Appointment Status") {
					if (item.ResourceName == "CANCELLED") {
						appointmentAlreadyCancelled = true;
						break;
					}
				}
			}
			if (!appointmentAlreadyCancelled) {
				myModel.SchdAppointment = deletedAppointment;
				myModel.CancelAppointmentCommand.Execute (null);
			}
			e.Cancel = true;
		}
		
		private void LoadNewAppointmentInfo(TimeSlot selectedTimeSlot)
		{
			myModel.SchdAppointment.START_TIME = selectedTimeSlot.Start.ToString ();
			myModel.SchdAppointment.END_TIME = selectedTimeSlot.End.ToString ();
			myModel.SchdAppointment.RESOURCENAME = myModel.ResourceName;
		}

		private void LoadExistAppointmentInfo (IAppointment selectedAppointment)
		{
			IResource selectedResource = new Resource ();
			foreach (IResource _resource in selectedAppointment.Resources) {
				if (_resource.ResourceType == "APPOINTMENTID") {
					myModel.SchdAppointment.APPOINTMENTID = _resource.ResourceName;
				}
				if (_resource.ResourceType == "PATIENTID") {
					myModel.SchdAppointment.PATIENTID = _resource.ResourceName;
				}
				if (_resource.ResourceType == "PATIENTNAME") {
					myModel.SchdAppointment.PATIENTNAME = _resource.ResourceName;
				}
				if (_resource.ResourceType == "RESOURCENAME") {
					myModel.SchdAppointment.RESOURCENAME = _resource.ResourceName;
				}
				if (_resource.ResourceType == "ACCESSTYPEID") {
					myModel.SchdAppointment.ACCESSTYPEID = _resource.ResourceName;
				}
				if (_resource.ResourceType == "NOTE") {
					myModel.SchdAppointment.NOTE = _resource.ResourceName;
				}
				if (_resource.ResourceType == "HRN") {
					myModel.SchdAppointment.HRN = _resource.ResourceName;
				}
				if (_resource.ResourceType == "START_TIME") {
					myModel.SchdAppointment.START_TIME = _resource.ResourceName;
				}
				if (_resource.ResourceType == "END_TIME") {
					myModel.SchdAppointment.END_TIME = _resource.ResourceName;
				}
				if (_resource.ResourceType == "VPROVIDER") {
					myModel.SchdAppointment.VPROVIDER = _resource.ResourceName;
				}
			}
		}

		private void SchedulerContextMenu_ItemClick (object sender, Telerik.Windows.RadRoutedEventArgs e)
		{
			RadMenuItem item = e.OriginalSource as RadMenuItem;
			string headerText = item.Header as string;
			myModel.SchdAppointment = myModel.SchdAppointmentFactory.Create ();

			switch (headerText) {
			case "Add Appointment":
				LoadNewAppointmentInfo (scheduler.SelectedTimeSlot);
				myModel.CreateAppointmentCommand.Execute ("New Appointment");
				break;
			case "Create Walk-in Appointment":
				LoadNewAppointmentInfo (scheduler.SelectedTimeSlot);
				myModel.CreateWalkInAppointmentCommand.Execute ("WalkIn Appointment");
				break;
			case "Edit Appointment":
				LoadExistAppointmentInfo (scheduler.SelectedAppointment);
				myModel.EditAppointmentCommand.Execute ("Edit Appointment");
				break;
			case "View Appointment":
				LoadExistAppointmentInfo (scheduler.SelectedAppointment);
				myModel.ViewAppointmentCommand.Execute ("View Appointment");
				break;
			case "Mark as No Show":
				LoadExistAppointmentInfo (scheduler.SelectedAppointment);
				myModel.MarkAsNoShowCommand.Execute ("Mark as No Show");
				break;
			case "Undo NoShow":
				LoadExistAppointmentInfo (scheduler.SelectedAppointment);
				myModel.UndoANoShowCommand.Execute ("Undo NoShow");
				break;
			case "Check In Patient":
				LoadExistAppointmentInfo (scheduler.SelectedAppointment);
				myModel.CheckInPatientCommand.Execute ("Check In Patient");
				break;
			case "Undo Check In Patient":
				LoadExistAppointmentInfo (scheduler.SelectedAppointment);
				myModel.UndoCheckInPatientCommand.Execute ("Undo Check In Patient");
				break;
			case "Check Out Patient":
				LoadExistAppointmentInfo (scheduler.SelectedAppointment);
				myModel.CheckOutPatientCommand.Execute ("Check Out Patient");
				break;
			case "Undo Check Out Patient":
				LoadExistAppointmentInfo (scheduler.SelectedAppointment);
				myModel.UndoCheckOutPatientCommand.Execute ("Undo Check Out Patient");
				break;
			case "Cancel Appointment":
				LoadExistAppointmentInfo (scheduler.SelectedAppointment);
				myModel.CancelAppointmentCommand.Execute ("Cancel Appointment");
				break;
			case "Undo Cancel Appointment":
				LoadExistAppointmentInfo (scheduler.SelectedAppointment);
				myModel.UndoCancelAppointmentCommand.Execute ("Undo Cancel Appointment");
				break;
			}
		}

		private void scheduler_SelectedViewStartDateChanged (object sender, EventArgs e)
		{
			myModel.RefreshAppoinmentScheduler ("");
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
				ScrollSchedulerToOffset ();				
			}
		}

		private void paneTaskScheduler_Unloaded (object sender, RoutedEventArgs e)
		{
			Model.ClearViewFromList ();
		}

		private void scheduler_MouseUp (object sender, MouseButtonEventArgs e)
		{
			if (((Telerik.Windows.Controls.RadScheduler)(sender)).IsReadOnly) {
				myModel.EnableAppointmentButtonsCommand.Execute ("Reset Buttons");
				return;
			}

			IAppointment selectedAppointment = ((Telerik.Windows.Controls.RadScheduler)(sender)).SelectedAppointment;
			myModel.SchdAppointment = myModel.SchdAppointmentFactory.Create ();

			if (selectedAppointment == null) {
				LoadNewAppointmentInfo (scheduler.SelectedTimeSlot);
				myModel.EnableAppointmentButtonsCommand.Execute ("New Appointment");
				myModel.PasteAppointmentCommand.Execute ("New Appointment");

			} else {
				((Telerik.Windows.Controls.RadScheduler)(sender)).SelectedTimeSlot = new TimeSlot (((Telerik.Windows.Controls.RadScheduler)(sender)).SelectedAppointment.Start, ((Telerik.Windows.Controls.RadScheduler)(sender)).SelectedAppointment.End);
				LoadExistAppointmentInfo (selectedAppointment);
				Patient selectedPatient = myModel.PatientFactory.Create ();
				selectedPatient.IEN = myModel.SchdAppointment.PATIENTID;
				selectedPatient.LoadPatientInfoByIEN ();
				myModel.SelectPatient (selectedPatient);
				IResource selectedResource = new Resource ();
				foreach (IResource _resource in selectedAppointment.Resources) {
					if (_resource.ResourceType == "Appointment Status") {
						selectedResource = _resource;
					}
				}
				switch (selectedResource.ResourceName) {
				case "EXIST":
					myModel.EnableAppointmentButtonsCommand.Execute ("EXIST Appointment");
					break;
				case "NOSHOW":
					myModel.EnableAppointmentButtonsCommand.Execute ("NOSHOW Appointment");
					break;
				case "CANCELLED":
					myModel.EnableAppointmentButtonsCommand.Execute ("CANCELLED Appointment");
					break;
				case "WALKIN":
					myModel.EnableAppointmentButtonsCommand.Execute ("WALKIN Appointment");
					break;
				case "CHECKOUT":
					myModel.EnableAppointmentButtonsCommand.Execute ("CHECKOUT Appointment");
					break;
				case "CHECKIN":
					myModel.EnableAppointmentButtonsCommand.Execute ("CHECKIN Appointment");
					break;
				default:
					myModel.EnableAppointmentButtonsCommand.Execute ("EXIST Appointment");
					break;
				}
				myModel.CopyAppointmentCommand.Execute ("EXIST Appointment");
				((Telerik.Windows.Controls.RadScheduler)(sender)).SelectedAppointment = null;
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
			this.RemoveFromParent ();
		}

		public void ScrollSchedulerToOffset ()
		{
			if (this.scheduler.ChildrenOfType<AppointmentsControl> ().Count > 0) {
					var appointmentControl = this.scheduler.ChildrenOfType<AppointmentsControl> ().Single (ac => ac.Name == "NotAllDayAppointmentsControl");
					var timeSlotItemHeight = appointmentControl.ChildrenOfType<TimeSlotItem> ().First ().ActualHeight;
					ScrollViewer scrollVeiwer = appointmentControl.ParentOfType<ScrollViewer> ();
					int timeSlotsToSkip = myModel.TimeSlotToSkip;
					Dispatcher.BeginInvoke (new Action (() => { Dispatcher.BeginInvoke (new Action (() => { scrollVeiwer.ScrollToVerticalOffset (timeSlotsToSkip * timeSlotItemHeight); })); }));
				}
		}
	}

	public class TimeSlotTemplateSelector : DataTemplateSelector
	{
		public DataTemplate DayWeekSlotTemplate
		{
			get;
			set;
		}
		public DataTemplate AllDaySlotTemplate
		{
			get;
			set;
		}
		public DataTemplate MonthSlotTemplate
		{
			get;
			set;
		}
		public DataTemplate TimeLineSlotTemplate
		{
			get;
			set;
		}

		public DateTime startTime;
		public DateTime endTime;

		private SchdResource parentResource;
		public SchdResource ParentResource
		{
			get
			{
				return parentResource;
			}
			set
			{
				parentResource = value;
				parentResource.IsAsync = false;
				parentResource.GetVisibleAvailabilities (startTime, endTime, (s, args) => {
					accessBlocks = (IList<SchdAvailability>)args.Result;
				});
				parentResource.IsAsync = true;
			}
		}

		public void Refresh ()
		{
			parentResource.IsAsync = false;
			parentResource.GetVisibleAvailabilities (startTime, endTime, (s, args) => {
				accessBlocks = (IList<SchdAvailability>)args.Result;
			});
			parentResource.IsAsync = true;
		}

		private DataTemplate customTemplate;
		private DataTemplate CustomTemplate
		{
			get
			{
				if (customTemplate == null) {
					FrameworkElementFactory fef = new FrameworkElementFactory (typeof (Border));
					fef.SetValue (Border.BackgroundProperty, new SolidColorBrush (Colors.White) { Opacity = 0 });
					DataTemplate dt = new DataTemplate ();
					dt.VisualTree = fef;
					customTemplate = dt;
				}
				return customTemplate;
			}
		}

		IList<SchdAvailability> accessBlocks;

		public override DataTemplate SelectTemplate (object item, DependencyObject container)
		{
			TimeSlot slot = item as TimeSlot;
			if (slot.ParentView.Scheduler != null) {
				if (slot.ParentView.Scheduler.ViewMode == SchedulerViewMode.Month) {
					return this.MonthSlotTemplate;
				}
				if (slot.ParentView.Scheduler.ViewMode == SchedulerViewMode.Timeline) {
					return this.TimeLineSlotTemplate;
				}
			}

			foreach (SchdAvailability block in accessBlocks)
			{
				if (slot.Start >= DateTime.Parse(block.StartTime) && slot.End <= DateTime.Parse(block.EndTime)) {
					FrameworkElementFactory fef = new FrameworkElementFactory (typeof (Border));
					Brush slotBrush = null;
					if (block.IsPreventAccess) {
						slotBrush = GetHatchBrush (Convert.ToByte (block.Red), Convert.ToByte (block.Green), Convert.ToByte (block.Blue));
					} else {
						slotBrush = new SolidColorBrush (Color.FromRgb (Convert.ToByte (block.Red), Convert.ToByte (block.Green), Convert.ToByte (block.Blue)));
					}
					fef.SetValue (Border.BackgroundProperty, slotBrush);
					DataTemplate dt = new DataTemplate ();					
					dt.VisualTree = fef;
					return dt;
				}
			}
			return CustomTemplate;
		}

		private VisualBrush GetHatchBrush (byte r, byte g, byte b)
		{
			VisualBrush hatchBrush = new VisualBrush ();
			hatchBrush.Viewport = new Rect (0, 0, 10, 10);
			hatchBrush.Viewbox = new Rect (0, 0, 10, 10);
			hatchBrush.ViewportUnits = BrushMappingMode.Absolute;
			hatchBrush.ViewboxUnits = BrushMappingMode.Absolute;
			hatchBrush.TileMode = TileMode.Tile;
			Canvas visual = new Canvas ();
			Rectangle rect = new Rectangle ();
			rect.Fill = new SolidColorBrush(Color.FromRgb (r, g, b));
			rect.Width = 10;
			rect.Height = 10;
			Path stroke = new Path ();
			stroke.Stroke = new SolidColorBrush(Colors.White);
			stroke.Data = Geometry.Parse ("M 0 0 l 10 10");

			visual.Children.Add (rect);
			visual.Children.Add (stroke);
			hatchBrush.Visual = visual;

			return hatchBrush;
		}
	}
}