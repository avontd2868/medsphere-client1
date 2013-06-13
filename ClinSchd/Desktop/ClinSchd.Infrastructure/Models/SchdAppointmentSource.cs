using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Windows.Controls.Scheduler;
using Telerik.Windows.Controls;

namespace ClinSchd.Infrastructure.Models
{
	public class SchdAppointmentSource
	{
		public CustomAppointmentCollection Appointments
		{
			get;
			set;
		}

		public SchdAppointmentSource (IList<SchdAppointment> appointmentList, Dictionary<string, Category> categories, bool isEnableUpdateAppointment)
		{
			this.Appointments = new CustomAppointmentCollection ();

		   foreach (SchdAppointment newAppointment in appointmentList) {

			   CustomAppointment _appt = new CustomAppointment ();
			   _appt.Subject = newAppointment.PATIENTNAME;
			   _appt.Body = newAppointment.NOTE;
			   _appt.Start = Convert.ToDateTime (newAppointment.START_TIME);
			   _appt.End = Convert.ToDateTime (newAppointment.END_TIME);

			   Resource _resource1 = new Resource ();
			   _resource1.ResourceType = "APPOINTMENTID";
			   _resource1.ResourceName = newAppointment.APPOINTMENTID;
			   _appt.Resources.Add (_resource1);

			   Resource _resource2 = new Resource ();
			   _resource2.ResourceType = "PATIENTID";
			   _resource2.ResourceName = newAppointment.PATIENTID;
			   _appt.Resources.Add (_resource2);

			   Resource _resource3 = new Resource ();
			   _resource3.ResourceType = "PATIENTNAME";
			   _resource3.ResourceName = newAppointment.PATIENTNAME;
			   _appt.Resources.Add (_resource3);

			   Resource _resource4 = new Resource ();
			   _resource4.ResourceType = "RESOURCENAME";
			   _resource4.ResourceName = newAppointment.RESOURCENAME;
			   _appt.Resources.Add (_resource4);

			   Resource _resource5 = new Resource ();
			   _resource5.ResourceType = "ACCESSTYPEID";
			   _resource5.ResourceName = newAppointment.ACCESSTYPEID;
			   _appt.Resources.Add (_resource5);

			   Resource _resource6 = new Resource ();
			   _resource6.ResourceType = "HRN";
			   _resource6.ResourceName = newAppointment.HRN;
			   _appt.Resources.Add (_resource6);

			   Resource _resource7 = new Resource ();
			   _resource7.ResourceType = "NOTE";
			   _resource7.ResourceName = newAppointment.NOTE;
			   _appt.Resources.Add (_resource7);

			   Resource _resource8 = new Resource ();
			   _resource8.ResourceType = "START_TIME";
			   _resource8.ResourceName = newAppointment.START_TIME;
			   _appt.Resources.Add (_resource8);

			   Resource _resource9 = new Resource ();
			   _resource9.ResourceType = "END_TIME";
			   _resource9.ResourceName = newAppointment.END_TIME;
			   _appt.Resources.Add (_resource9);

			   Resource _resource10 = new Resource ();
			   _resource10.ResourceType = "VPROVIDER";
			   _resource10.ResourceName = newAppointment.VPROVIDER;
			   _appt.Resources.Add (_resource10);

			   Resource _resource = new Resource ();
			   _resource.ResourceType = "Appointment Status";

			   //Add to the category
			   if (newAppointment.NOSHOW == "1") {
				   _appt.Category = categories["PinkCategory"];
				   _resource.ResourceName = "NOSHOW";
				   if (isEnableUpdateAppointment) {
					   _appt.IsEditAppointmentEnabled = true;
					   _appt.IsNoShowAppointmentEnabled = true;
					   _appt.IsCheckedInAppointmentEnabled = false;
					   _appt.IsCheckedOutAppointmentEnabled = false;
					   _appt.IsCancelledAppointmentEnabled = false;
					   _appt.IsNotNoShowAppointmentEnabled = false;
					   _appt.IsNotCheckedInAppointmentEnabled = false;
					   _appt.IsNotCheckedOutAppointmentEnabled = false;
					   _appt.IsNotCancelledAppointmentEnabled = true;
				   }
			   } else if (newAppointment.CANCELLED == "1") {
				   _appt.Category = categories["GrayCategory"];
				   _resource.ResourceName = "CANCELLED";
				   if (isEnableUpdateAppointment) {
					   _appt.IsEditAppointmentEnabled = true;
					   _appt.IsNoShowAppointmentEnabled = false;
					   _appt.IsCheckedInAppointmentEnabled = false;
					   _appt.IsCheckedOutAppointmentEnabled = false;
					   _appt.IsCancelledAppointmentEnabled = true;
					   _appt.IsNotNoShowAppointmentEnabled = false;
					   _appt.IsNotCheckedInAppointmentEnabled = false;
					   _appt.IsNotCheckedOutAppointmentEnabled = false;
					   _appt.IsNotCancelledAppointmentEnabled = false;
				   }
			   } else if (newAppointment.WALKIN == "1" && newAppointment.CHECKOUT == String.Empty) {
				   _appt.Category = categories["BlueCategory"];
				   _resource.ResourceName = "WALKIN";
				   if (isEnableUpdateAppointment) {
					   _appt.IsEditAppointmentEnabled = true;
					   _appt.IsNoShowAppointmentEnabled = false;
					   _appt.IsCheckedInAppointmentEnabled = false;
					   _appt.IsCheckedOutAppointmentEnabled = false;
					   _appt.IsCancelledAppointmentEnabled = false;
					   _appt.IsNotNoShowAppointmentEnabled = false;
					   _appt.IsNotCheckedInAppointmentEnabled = false;
					   _appt.IsNotCheckedOutAppointmentEnabled = true;
					   _appt.IsNotCancelledAppointmentEnabled = false;
				   }
			   } else if (newAppointment.WALKIN == "1" && newAppointment.CHECKOUT != String.Empty) {
				   _appt.Category = categories["OrangeCategory"];
				   _resource.ResourceName = "CHECKOUT";
				   if (isEnableUpdateAppointment) {
					   _appt.IsEditAppointmentEnabled = true;
					   _appt.IsNoShowAppointmentEnabled = false;
					   _appt.IsCheckedInAppointmentEnabled = false;
					   _appt.IsCheckedOutAppointmentEnabled = true;
					   _appt.IsCancelledAppointmentEnabled = false;
					   _appt.IsNotNoShowAppointmentEnabled = false;
					   _appt.IsNotCheckedInAppointmentEnabled = false;
					   _appt.IsNotCheckedOutAppointmentEnabled = false;
					   _appt.IsNotCancelledAppointmentEnabled = false;
				   }
			   } else if (newAppointment.CHECKOUT != String.Empty) {
				   _appt.Category = categories["OrangeCategory"];
				   _resource.ResourceName = "CHECKOUT";
				   if (isEnableUpdateAppointment) {
					   _appt.IsEditAppointmentEnabled = true;
					   _appt.IsNoShowAppointmentEnabled = false;
					   _appt.IsCheckedInAppointmentEnabled = false;
					   _appt.IsCheckedOutAppointmentEnabled = true;
					   _appt.IsCancelledAppointmentEnabled = false;
					   _appt.IsNotNoShowAppointmentEnabled = false;
					   _appt.IsNotCheckedInAppointmentEnabled = false;
					   _appt.IsNotCheckedOutAppointmentEnabled = false;
					   _appt.IsNotCancelledAppointmentEnabled = false;
				   }
			   } else if (newAppointment.CHECKIN != String.Empty) {
				   _appt.Category = categories["GreenCategory"];
				   _resource.ResourceName = "CHECKIN";
				   if (isEnableUpdateAppointment) {
					   _appt.IsEditAppointmentEnabled = true;
					   _appt.IsNoShowAppointmentEnabled = false;
					   _appt.IsCheckedInAppointmentEnabled = true;
					   _appt.IsCheckedOutAppointmentEnabled = false;
					   _appt.IsCancelledAppointmentEnabled = false;
					   _appt.IsNotNoShowAppointmentEnabled = false;
					   _appt.IsNotCheckedInAppointmentEnabled = false;
					   _appt.IsNotCheckedOutAppointmentEnabled = true;
					   _appt.IsNotCancelledAppointmentEnabled = false;
				   }
			   } else {
				   _appt.Category = categories["WhiteCategory"];
				   _resource.ResourceName = "EXIST";
				   if (isEnableUpdateAppointment) {
					   _appt.IsEditAppointmentEnabled = true;
					   _appt.IsNoShowAppointmentEnabled = false;
					   _appt.IsCheckedInAppointmentEnabled = false;
					   _appt.IsCheckedOutAppointmentEnabled = false;
					   _appt.IsCancelledAppointmentEnabled = false;
					   _appt.IsNotNoShowAppointmentEnabled = true;
					   _appt.IsNotCheckedInAppointmentEnabled = true;
					   _appt.IsNotCheckedOutAppointmentEnabled = false;
					   _appt.IsNotCancelledAppointmentEnabled = true;
				   }
			   }
			   
			   _appt.Resources.Add (_resource);
			   this.Appointments.Add (_appt);
		   }
		}

		public SchdAppointmentSource (IList<SchdAvailability> availabilityList, bool isEnableUpdateAppointment)
		{
			this.Appointments = new CustomAppointmentCollection ();

			foreach (SchdAvailability newAppointment in availabilityList) {

					CustomAppointment _appt = new CustomAppointment ();
					if (!isEnableUpdateAppointment) {
						_appt.IsEditAccessBlockEnabled = false;
						_appt.IsDeleteAccessBlockEnabled = false;
					}

					_appt.Subject = newAppointment.ACCESSTYPENAME + ": " + newAppointment.SLOTS.ToString() + " Slot(s), " + newAppointment.Duration.ToString() + " Minutes.";
					_appt.Body = newAppointment.Note;
					_appt.Start = Convert.ToDateTime (newAppointment.StartTime);
					_appt.End = Convert.ToDateTime (newAppointment.EndTime);

					Resource _resource1 = new Resource ();
					_resource1.ResourceType = "APPOINTMENTID";
					_resource1.ResourceName = newAppointment.APPOINTMENTID;
					_appt.Resources.Add (_resource1);

					Resource _resource4 = new Resource ();
					_resource4.ResourceType = "RESOURCENAME";
					_resource4.ResourceName = newAppointment.RESOURCENAME;
					_appt.Resources.Add (_resource4);

					Resource _resource5 = new Resource ();
					_resource5.ResourceType = "ACCESSTYPEID";
					_resource5.ResourceName = newAppointment.ACCESSTYPEID;
					_appt.Resources.Add (_resource5);

					Resource _resource7 = new Resource ();
					_resource7.ResourceType = "NOTE";
					_resource7.ResourceName = newAppointment.Note;
					_appt.Resources.Add (_resource7);

					Resource _resource8 = new Resource ();
					_resource8.ResourceType = "START_TIME";
					_resource8.ResourceName = newAppointment.StartTime;
					_appt.Resources.Add (_resource8);

					Resource _resource9 = new Resource ();
					_resource9.ResourceType = "END_TIME";
					_resource9.ResourceName = newAppointment.EndTime;
					_appt.Resources.Add (_resource9);

					Resource _resource10 = new Resource ();
					_resource10.ResourceType = "SLOTS";
					_resource10.ResourceName = newAppointment.SLOTS.ToString();
					_appt.Resources.Add (_resource10);

					Resource _resource = new Resource ();
					_resource.ResourceType = "Appointment Status";

					_appt.Resources.Add (_resource);

					Category newCat = new Category ("Test", new System.Windows.Media.SolidColorBrush (System.Windows.Media.Color.FromRgb (Convert.ToByte (newAppointment.Red), Convert.ToByte (newAppointment.Green), Convert.ToByte (newAppointment.Blue))) );
					//newCat.CategoryBrush.Opacity = .1;
					_appt.Category = newCat;
					this.Appointments.Add (_appt);
			}
		}

	}
}
