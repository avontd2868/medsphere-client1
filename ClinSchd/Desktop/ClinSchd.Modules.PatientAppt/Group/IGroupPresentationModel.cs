using ClinSchd.Modules.PatientAppt.Controllers;
using ClinSchd.Infrastructure.Models;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Events;

namespace ClinSchd.Modules.PatientAppt.Group
{
	public interface IGroupPresentationModel
    {
		IGroupView View { get; }
		IPatientApptController Controller { get; set; }
		PatientInformation PatientInformation { get; set; }
		AppointmentData AppointmentData { get; set; }
		ValidationMessage ValidationMessage { get; set; }

		void GetEditAppointment (SchdAppointment newAppointment);
		void GetViewAppointment (SchdAppointment newAppointment);
		void GetWalkInAppointment (SchdAppointment newAppointment);
		void GetNewAppointment (SchdAppointment newAppointment);
		void GetPatientInformation (string patientIEN);
		string PaneTitle { get; set; }
		CompositePresentationEvent<string> NewForwardingEvent { get; set; }
		void OnClose ();
		string Command { get; set; }
		bool IsNoteEnabled { get; set; }
		bool IsDurationEnabled { get; set; }
		void OnPropertyChanged (string propertyName);
    }
}
