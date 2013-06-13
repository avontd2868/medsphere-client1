using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;

namespace ClinSchd.Infrastructure.Controls
{
	/// <summary>
	/// Interaction logic for DateTimePicker.xaml
	/// </summary>
	/// <remarks>
	/// Composite Date and Time picker control with support for RPMS date formatting.
	/// </remarks>
	public partial class DateTimePicker : UserControl
	{
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public DateTimePicker ()
		{
			InitializeComponent ();
			FixZeroTime = true;
		}

		#region DependencyProperties
		/// <summary>
		/// SelectedDateTime Dependency Property.
		/// </summary>
		public static readonly DependencyProperty SelectedDateTimeProperty =
			DependencyProperty.Register ("SelectedDateTime", typeof (DateTime?), typeof (DateTimePicker), new FrameworkPropertyMetadata (new PropertyChangedCallback (OnSelectedDateTimeChanged)));
		/// <summary>
		/// SelectedDateTimeFormatted Dependency Property.
		/// </summary>
		public static readonly DependencyProperty SelectedDateTimeFormattedProperty =
			DependencyProperty.Register ("SelectedDateTimeFormatted", typeof (string), typeof (DateTimePicker), new FrameworkPropertyMetadata (new PropertyChangedCallback (OnSelectedDateTimeFormattedChanged)));
		/// <summary>
		/// SelectedDateTimeFormattedInternal Dependency Property.
		/// </summary>
		public static readonly DependencyProperty SelectedDateTimeFormattedInternalProperty =
			DependencyProperty.Register ("SelectedDateTimeFormattedInternal", typeof (string), typeof (DateTimePicker), new FrameworkPropertyMetadata (new PropertyChangedCallback (OnSelectedDateTimeFormattedInternalChanged)));
		/// <summary>
		/// SelectedDate Dependency Property.
		/// </summary>
		public static readonly DependencyProperty SelectedDateProperty =
			DependencyProperty.Register ("SelectedDate", typeof (DateTime?), typeof (DateTimePicker), new FrameworkPropertyMetadata (new PropertyChangedCallback (OnSelectedDateChanged)));
		/// <summary>
		/// SelectedTime Dependency Property.
		/// </summary>
		public static readonly DependencyProperty SelectedTimeProperty =
			DependencyProperty.Register ("SelectedTime", typeof (TimeSpan?), typeof (DateTimePicker), new FrameworkPropertyMetadata (new PropertyChangedCallback (OnSelectedTimeChanged)));
		/// <summary>
		/// HideTimePicker Dependency Property.
		/// </summary>
		public static readonly DependencyProperty HideTimePickerProperty =
			DependencyProperty.Register ("HideTimePicker", typeof (bool), typeof (DateTimePicker), new PropertyMetadata (false, OnHideTimePickerChanged));
		/// <summary>
		/// TruncateSeconds Dependency Property.
		/// </summary>
		public static readonly DependencyProperty TruncateSecondsProperty =
			DependencyProperty.Register ("TruncateSeconds", typeof (bool), typeof (DateTimePicker), new PropertyMetadata (false, new PropertyChangedCallback (OnTruncateSecondsChanged)));
		/// <summary>
		/// FixZeroTime Dependency Property.
		/// </summary>
		public static readonly DependencyProperty FixZeroTimeProperty =
			DependencyProperty.Register ("FixZeroTime", typeof (bool), typeof (DateTimePicker), new PropertyMetadata (false, new PropertyChangedCallback (OnFixZeroTimeChanged)));
		#endregion

		#region Fields
		private bool userEnteredText;
		#endregion

		#region Properties
		/// <summary>
		/// DateTime currently selected by the picker controls.
		/// Setting this property will initialize the controls with the provided date and time.
		/// </summary>
		public DateTime? SelectedDateTime
		{
			get { return (DateTime?)GetValue (SelectedDateTimeProperty); }
			set { SetValue (SelectedDateTimeProperty, value); }
		}

		/// <summary>
		/// RPMS-formatted DateTime currently selected by the picker controls.
		/// Retrieves or sets the selected DateTime using an RPMS-formatted string representation of the date and time.
		/// (MON DD YYYY@HH:MM:SS)
		/// </summary>
		public string SelectedDateTimeFormatted
		{
			get { return (string)GetValue (SelectedDateTimeFormattedProperty); }
			set { SetValue (SelectedDateTimeFormattedProperty, value); }
		}

		/// <summary>
		/// RPMS-formatted (internal) DateTime currently selected by the picker controls.
		/// Retrieves or sets the selected DateTime using an RPMS-formatted (internal) string representation of the date and time.
		/// (YYYMMDD.HHMMSS)
		/// </summary>
		public string SelectedDateTimeFormattedInternal
		{
			get { return (string)GetValue (SelectedDateTimeFormattedInternalProperty); }
			set { SetValue (SelectedDateTimeFormattedInternalProperty, value); }
		}

		/// <summary>
		/// The DateTime (date component only) currently selected by the picker controls.
		/// Preserved for backwards compatibility with existing RadDatePicker declarations.
		/// </summary>
		public DateTime? SelectedDate
		{
			get { return (DateTime?)GetValue (SelectedDateProperty); }
			set { SetValue (SelectedDateProperty, value); }
		}

		/// <summary>
		/// The TimeSpan currently selected by the picker controls.
		/// </summary>
		public TimeSpan? SelectedTime
		{
			get { return (TimeSpan?)GetValue (SelectedTimeProperty); }
			set { SetValue (SelectedTimeProperty, value); }
		}

		/// <summary>
		/// Hide or show the Time Picker and "@" sign divider.
		/// </summary>
		public bool HideTimePicker
		{
			get { return (bool)GetValue (HideTimePickerProperty); }
			set { SetValue (HideTimePickerProperty, value); }
		}

		/// <summary>
		/// Truncate or include seconds in formatted date strings.
		/// </summary>
		public bool TruncateSeconds
		{
			get { return (bool)GetValue (TruncateSecondsProperty); }
			set { SetValue (TruncateSecondsProperty, value); }
		}

		/// <summary>
		/// Change 00:00 times to 00:01 in formatted date strings.
		/// </summary>
		public bool FixZeroTime
		{
			get { return (bool)GetValue (FixZeroTimeProperty); }
			set { SetValue (FixZeroTimeProperty, value); }
		}
		#endregion

		#region Methods

		private void InitTimePickerCulture ()
		{
			var culture = new CultureInfo ("en-US");
			if (TruncateSeconds) {
				culture.DateTimeFormat = new DateTimeFormatInfo () { ShortTimePattern = "HH:mm" };
			} else {
				culture.DateTimeFormat = new DateTimeFormatInfo () { ShortTimePattern = "HH:mm:ss" };
			}
			radTimePicker1.Culture = culture;
		}
		#endregion

		#region EventHandlers (Focus and Formatting)
		private static void OnHideTimePickerChanged (DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			DateTimePicker control = (DateTimePicker)obj;
			if ((bool)args.NewValue) {
				GridLength invisible = new GridLength (0);
				control.SeparatorColumn.Width = invisible;
				control.TimePickerColumn.Width = invisible;
			} else {
				control.SeparatorColumn.Width = GridLength.Auto;
				control.TimePickerColumn.Width = new GridLength (1, GridUnitType.Star);
			}
		}

		private void radTimePicker1_Initialized (object sender, EventArgs e)
		{
			InitTimePickerCulture ();
		}

		private void radDatePicker1_KeyUp (object sender, KeyEventArgs e)
		{
			if (userEnteredText && (e.Key == Key.Tab || (e.Key == Key.D2 && (Keyboard.IsKeyDown (Key.LeftShift) || Keyboard.IsKeyDown (Key.RightShift)) ||
				(e.Key == Key.LeftShift || e.Key == Key.RightShift) && Keyboard.IsKeyDown (Key.D2)))) {
				radTimePicker1.Focus ();
			} else if (e.Key != Key.LeftShift && e.Key != Key.RightShift && e.Key != Key.Tab) {
				userEnteredText = true;
			}
		}

		private void radTimePicker1_KeyUp (object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Back && (Keyboard.FocusedElement as TextBox).Text.Length == 0) {

				radDatePicker1.Focus ();
				userEnteredText = true;
			}
		}

		private void UserControl_LostFocus (object sender, RoutedEventArgs e)
		{
			userEnteredText = false;
		}

		private void radDatePicker1_GotFocus (object sender, RoutedEventArgs e)
		{
			TextBox tb = Keyboard.FocusedElement as TextBox;
			if (tb != null) {
				tb.SelectionStart = tb.Text.Length;
			} else {
				radDatePicker1.MoveFocus (new TraversalRequest (FocusNavigationDirection.Down));
			}
		}

		private void radTimePicker1_GotFocus (object sender, RoutedEventArgs e)
		{
			TextBox tb = Keyboard.FocusedElement as TextBox;
			if (tb != null) {
				tb.SelectionStart = tb.Text.Length;
			} else {
				radTimePicker1.MoveFocus (new TraversalRequest (FocusNavigationDirection.Down));
			}
		}
		#endregion

		#region EventHandlers
		private static void OnSelectedDateTimeChanged (DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			DateTimePicker control = (DateTimePicker)obj;
			RoutedPropertyChangedEventArgs<DateTime?> e = new RoutedPropertyChangedEventArgs<DateTime?> (
				(DateTime?)args.OldValue, (DateTime?)args.NewValue, SelectedDateTimeChangedEvent);
			control.OnSelectedDateTimeChanged (e);
			control.SelectedDate = (DateTime?)args.NewValue;
			if (((DateTime?)args.NewValue).HasValue) {
				control.SelectedTime = ((DateTime?)args.NewValue).Value.TimeOfDay;
			}
			control.SelectedDateTimeFormatted = ((DateTime)args.NewValue).ConvertToExternalDateTimeFormat (control.TruncateSeconds, control.FixZeroTime);
			control.SelectedDateTimeFormattedInternal = ((DateTime)args.NewValue).ConvertToInternalDateTimeFormat (control.TruncateSeconds, control.FixZeroTime);
		}

		private static void OnSelectedDateChanged (DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			DateTimePicker control = (DateTimePicker)obj;
			if ((DateTime?)args.NewValue != (DateTime?)args.OldValue && args.NewValue != null) {
				RoutedPropertyChangedEventArgs<DateTime?> e = new RoutedPropertyChangedEventArgs<DateTime?> (
					(DateTime?)args.OldValue, (DateTime?)args.NewValue, SelectedDateChangedEvent);
				control.OnSelectedDateChanged (e);
				TimeSpan ts;
				if (control.SelectedTime.HasValue) {
					ts = control.SelectedTime.Value;
					control.SelectedDateTime = ((DateTime?)args.NewValue).Value.Date.Add (ts);
				} else {
					control.SelectedDateTime = (DateTime?)args.NewValue;
				}
			}
		}

		private static void OnSelectedTimeChanged (DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			DateTimePicker control = (DateTimePicker)obj;
			if ((TimeSpan?)args.NewValue != (TimeSpan?)args.OldValue && args.NewValue != null) {
				RoutedPropertyChangedEventArgs<TimeSpan?> e = new RoutedPropertyChangedEventArgs<TimeSpan?> (
					(TimeSpan?)args.OldValue, (TimeSpan?)args.NewValue, SelectedTimeChangedEvent);
				control.OnSelectedTimeChanged (e);
				if (control.SelectedDateTime.HasValue && ((TimeSpan?)args.NewValue).HasValue) {
					control.SelectedDateTime = control.SelectedDateTime.Value.Date.Add (((TimeSpan?)args.NewValue).Value);
				}
			}
		}

		private static void OnSelectedDateTimeFormattedChanged (DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			DateTimePicker control = (DateTimePicker)obj;
			if ((string)args.NewValue != (string)args.OldValue && args.NewValue != null) {
				RoutedPropertyChangedEventArgs<string> e = new RoutedPropertyChangedEventArgs<string> (
					(string)args.OldValue, (string)args.NewValue, SelectedDateTimeFormattedChangedEvent);
				control.OnSelectedDateTimeFormattedChanged (e);
				//Set SelectedDateTime to proper value.
				control.SelectedDateTime = ((string)args.NewValue).ConvertToDateTime (false, control.TruncateSeconds, control.FixZeroTime);
			}
		}

		private static void OnSelectedDateTimeFormattedInternalChanged (DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			DateTimePicker control = (DateTimePicker)obj;
			if ((string)args.NewValue != (string)args.OldValue && args.NewValue != null) {
				RoutedPropertyChangedEventArgs<string> e = new RoutedPropertyChangedEventArgs<string> (
					(string)args.OldValue, (string)args.NewValue, SelectedDateTimeFormattedInternalChangedEvent);
				control.OnSelectedDateTimeFormattedInternalChanged (e);
				control.SelectedDateTime = ((string)args.NewValue).ConvertToDateTime (true, control.TruncateSeconds, control.FixZeroTime);
			}
		}

		private static void OnTruncateSecondsChanged (DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			DateTimePicker control = (DateTimePicker)obj;
			control.InitTimePickerCulture ();
		}

		private static void OnFixZeroTimeChanged (DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
		}

		/// <summary>
		/// Raises the ValueChanged event.
		/// </summary>
		/// <param name="args">Arguments associated with the ValueChanged event.</param>
		protected virtual void OnSelectedDateTimeChanged (RoutedPropertyChangedEventArgs<DateTime?> args)
		{
			RaiseEvent (args);
		}

		/// <summary>
		/// Raises the ValueChanged event.
		/// </summary>
		/// <param name="args">Arguments associated with the ValueChanged event.</param>
		protected virtual void OnSelectedDateChanged (RoutedPropertyChangedEventArgs<DateTime?> args)
		{
			RaiseEvent (args);
		}

		/// <summary>
		/// Raises the ValueChanged event.
		/// </summary>
		/// <param name="args">Arguments associated with the ValueChanged event.</param>
		protected virtual void OnSelectedTimeChanged (RoutedPropertyChangedEventArgs<TimeSpan?> args)
		{
			RaiseEvent (args);
		}

		/// <summary>
		/// Raises the ValueChanged event.
		/// </summary>
		/// <param name="args">Arguments associated with the ValueChanged event.</param>
		protected virtual void OnSelectedDateTimeFormattedChanged (RoutedPropertyChangedEventArgs<string> args)
		{
			RaiseEvent (args);
		}

		/// <summary>
		/// Raises the ValueChanged event.
		/// </summary>
		/// <param name="args">Arguments associated with the ValueChanged event.</param>
		protected virtual void OnSelectedDateTimeFormattedInternalChanged (RoutedPropertyChangedEventArgs<string> args)
		{
			RaiseEvent (args);
		}
		#endregion

		#region Events
		/// <summary>
		/// Identifies the ValueChanged routed event.
		/// </summary>
		public static readonly RoutedEvent SelectedDateTimeChangedEvent = EventManager.RegisterRoutedEvent (
			"SelectedDateTimeChanged", RoutingStrategy.Bubble,
			typeof (RoutedPropertyChangedEventHandler<DateTime?>), typeof (DateTimePicker));

		/// <summary>
		/// Identifies the ValueChanged routed event.
		/// </summary>
		public static readonly RoutedEvent SelectedDateChangedEvent = EventManager.RegisterRoutedEvent (
			"SelectedDateChanged", RoutingStrategy.Bubble,
			typeof (RoutedPropertyChangedEventHandler<DateTime?>), typeof (DateTimePicker));

		/// <summary>
		/// Identifies the ValueChanged routed event.
		/// </summary>
		public static readonly RoutedEvent SelectedTimeChangedEvent = EventManager.RegisterRoutedEvent (
			"SelectedTimeChanged", RoutingStrategy.Bubble,
			typeof (RoutedPropertyChangedEventHandler<TimeSpan?>), typeof (DateTimePicker));

		/// <summary>
		/// Identifies the ValueChanged routed event.
		/// </summary>
		public static readonly RoutedEvent SelectedDateTimeFormattedChangedEvent = EventManager.RegisterRoutedEvent (
			"SelectedDateTimeFormattedChanged", RoutingStrategy.Bubble,
			typeof (RoutedPropertyChangedEventHandler<string>), typeof (DateTimePicker));

		/// <summary>
		/// Identifies the ValueChanged routed event.
		/// </summary>
		public static readonly RoutedEvent SelectedDateTimeFormattedInternalChangedEvent = EventManager.RegisterRoutedEvent (
			"SelectedDateTimeFormattedInternalChanged", RoutingStrategy.Bubble,
			typeof (RoutedPropertyChangedEventHandler<string>), typeof (DateTimePicker));

		/// <summary>
		/// Occurs when the Value property changes.
		/// </summary>
		public event RoutedPropertyChangedEventHandler<DateTime?> SelectedDateTimeChanged
		{
			add { AddHandler (SelectedDateTimeChangedEvent, value); }
			remove { RemoveHandler (SelectedDateTimeChangedEvent, value); }
		}

		/// <summary>
		/// Occurs when the Value property changes.
		/// </summary>
		public event RoutedPropertyChangedEventHandler<DateTime?> SelectedDateChanged
		{
			add { AddHandler (SelectedDateChangedEvent, value); }
			remove { RemoveHandler (SelectedDateChangedEvent, value); }
		}

		/// <summary>
		/// Occurs when the Value property changes.
		/// </summary>
		public event RoutedPropertyChangedEventHandler<TimeSpan?> SelectedTimeChanged
		{
			add { AddHandler (SelectedTimeChangedEvent, value); }
			remove { RemoveHandler (SelectedTimeChangedEvent, value); }
		}

		/// <summary>
		/// Occurs when the Value property changes.
		/// </summary>
		public event RoutedPropertyChangedEventHandler<string> SelectedDateTimeFormattedChanged
		{
			add { AddHandler (SelectedDateTimeFormattedChangedEvent, value); }
			remove { RemoveHandler (SelectedDateTimeFormattedChangedEvent, value); }
		}

		/// <summary>
		/// Occurs when the Value property changes.
		/// </summary>
		public event RoutedPropertyChangedEventHandler<string> SelectedDateTimeFormattedInternalChanged
		{
			add { AddHandler (SelectedDateTimeFormattedInternalChangedEvent, value); }
			remove { RemoveHandler (SelectedDateTimeFormattedInternalChangedEvent, value); }
		}
		#endregion
	}
}
