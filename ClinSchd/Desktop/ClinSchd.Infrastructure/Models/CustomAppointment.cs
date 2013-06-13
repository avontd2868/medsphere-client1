using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Windows.Controls.Scheduler;

namespace ClinSchd.Infrastructure.Models
{
	/// <summary>
	/// CustomAppointment
	/// </summary>
	public class CustomAppointment : Appointment
	{
		/// <summary>
		/// CustomAppointment
		/// </summary>
		public CustomAppointment()
			:base(){
		}

		private bool _isNoShowAppointmentEnabled = false;
		private bool _isCheckedInAppointmentEnabled = false;
		private bool _isCheckedOutAppointmentEnabled = false;
		private bool _isCancelledAppointmentEnabled = false;
		private bool _isNotNoShowAppointmentEnabled = false;
		private bool _isNotCheckedInAppointmentEnabled = false;
		private bool _isNotCheckedOutAppointmentEnabled = false;
		private bool _isNotCancelledAppointmentEnabled = false;
		private bool _isEditAppointmentEnabled = false;
		private bool _isEditAccessBlockEnabled = true;
		private bool _isDeleteAccessBlockEnabled = true;

		public bool IsNoShowAppointmentEnabled
		{
			get
			{
				return this._isNoShowAppointmentEnabled;
			}
			set
			{
				if (_isNoShowAppointmentEnabled != value) {
					this._isNoShowAppointmentEnabled = value;
					this.OnPropertyChanged ("IsNoShowAppointmentEnabled");
				}
			}
		}

		public bool IsCheckedInAppointmentEnabled
		{
			get
			{
				return this._isCheckedInAppointmentEnabled;
			}
			set
			{
				if (_isCheckedInAppointmentEnabled != value) {
					this._isCheckedInAppointmentEnabled = value;
					this.OnPropertyChanged ("IsCheckedInAppointmentEnabled");
				}
			}
		}

		public bool IsCheckedOutAppointmentEnabled
		{
			get
			{
				return this._isCheckedOutAppointmentEnabled;
			}
			set
			{
				if (_isCheckedOutAppointmentEnabled != value) {
					this._isCheckedOutAppointmentEnabled = value;
					this.OnPropertyChanged ("IsCheckedOutAppointmentEnabled");
				}
			}
		}

		public bool IsCancelledAppointmentEnabled
		{
			get
			{
				return this._isCancelledAppointmentEnabled;
			}
			set
			{
				if (_isCancelledAppointmentEnabled != value) {
					this._isCancelledAppointmentEnabled = value;
					this.OnPropertyChanged ("IsCancelledAppointmentEnabled");
				}
			}
		}

		public bool IsNotNoShowAppointmentEnabled
		{
			get
			{
				return this._isNotNoShowAppointmentEnabled;
			}
			set
			{
				if (_isNotNoShowAppointmentEnabled != value) {
					this._isNotNoShowAppointmentEnabled = value;
					this.OnPropertyChanged ("IsNotNoShowAppointmentEnabled");
				}
			}
		}

		public bool IsNotCheckedInAppointmentEnabled
		{
			get
			{
				return this._isNotCheckedInAppointmentEnabled;
			}
			set
			{
				if (_isNotCheckedInAppointmentEnabled != value) {
					this._isNotCheckedInAppointmentEnabled = value;
					this.OnPropertyChanged ("IsNotCheckedInAppointmentEnabled");
				}
			}
		}

		public bool IsNotCheckedOutAppointmentEnabled
		{
			get
			{
				return this._isNotCheckedOutAppointmentEnabled;
			}
			set
			{
				if (_isNotCheckedOutAppointmentEnabled != value) {
					this._isNotCheckedOutAppointmentEnabled = value;
					this.OnPropertyChanged ("IsNotCheckedOutAppointmentEnabled");
				}
			}
		}

		public bool IsNotCancelledAppointmentEnabled
		{
			get
			{
				return this._isNotCancelledAppointmentEnabled;
			}
			set
			{
				if (_isNotCancelledAppointmentEnabled != value) {
					this._isNotCancelledAppointmentEnabled = value;
					this.OnPropertyChanged ("IsNotCancelledAppointmentEnabled");
				}
			}
		}

		public bool IsEditAppointmentEnabled
		{
			get
			{
				return this._isEditAppointmentEnabled;
			}
			set
			{
				if (_isEditAppointmentEnabled != value) {
					this._isEditAppointmentEnabled = value;
					this.OnPropertyChanged ("IsEditAppointmentEnabled");
				}
			}
		}

		public bool IsEditAccessBlockEnabled
		{
			get
			{
				return this._isEditAccessBlockEnabled;
			}
			set
			{
				if (_isEditAccessBlockEnabled != value) {
					this._isEditAccessBlockEnabled = value;
					this.OnPropertyChanged ("IsEditAccessBlockEnabled");
				}
			}
		}

		public bool IsDeleteAccessBlockEnabled
		{
			get
			{
				return this._isDeleteAccessBlockEnabled;
			}
			set
			{
				if (_isDeleteAccessBlockEnabled != value) {
					this._isDeleteAccessBlockEnabled = value;
					this.OnPropertyChanged ("IsDeleteAccessBlockEnabled");
				}
			}
		}

	}
}
