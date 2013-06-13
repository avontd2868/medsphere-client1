using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClinSchd.Infrastructure.Models;
using System.Data;

namespace ClinSchd.Infrastructure.Interfaces
{
	public interface IDataAccessService
    {
		//Clinical Scheduling Methods
		RPMSClinic GetRPMSClinic (string resourceID);
		IList<Patient> GetPatients (string searchString, int maxRows);
		IList<SchdResource> GetResources (string searchString, bool includeInactive);
		SchdResource GetResource (string resourceID);
		PatientInformation GetPatientInfo (string patientIEN);
		bool LoadGlobalRecordsets ();
		IList<SchdAppointment> GetAppointments (string resourceName, string startDate, string endDate);
		IList<SchdAvailability> GetAvailabilities (string resourceName, string startDate, string endDate);
		List<string> AddNewAppointment (AppointmentData a, PatientInformation p, bool bMakeChartRequest);
		string CheckSlotsAvailable (string startTime, string endTime, string resourceName);
		string CreateNewAppointment (AppointmentData a, PatientInformation p, bool bMakeChartRequest);
		string EditAppointment (AppointmentData a);
		string AddNewAvailability (SchdAvailability a);
		IList<Provider> GetProviders ();
		IList<NameValue> GetSummaryReport ();
		IList<NameValue> GetCancellationReasons ();
		IList<NameValue> GetAccessGroups ();
		IList<NameValue> GetScheduleUsers ();
		IList<NameValue> GetResourceGroupList ();
		IList<SchdAccessType> GetAccessTypes ();
		IList<ResourceUser> GetResourceUsers ();
		IList<ResourceUser> GetResourceUsersByResourceID (string resourceID);
		IList<ResourceUser> RemoveResourceUserByID (string userID, string resourceID);
		string CheckInPatient (CheckInPatient checkInData);
		string UndoCheckIn (string apptID);
		string CheckOutPatient (CheckOutPatient checkOutData);
		string UndoCheckOut (string apptID);
		string CancelAppointment (CancelAppointment cancelAppointmentData);
		string UndoCancelAppointment (string apptID);
		string AppointmentNoShow (string apptID, bool bNoShow);
		string AutoRebook (AutoRebookData autoRebookData);
		IList<SchdResource> GetClinicsByProvider (string providerIEN);
		IList<Provider> GetProvidersByClinic (string clinicIEN);
		IList<RPMSClinic> GetRPMSClinics ();
		RPMSClinic GetClinicByID (string clinicIEN);
		string AddEditClinic (SchdResource schdResource);
		string DeleteClinic (string clinicID);
		string AddEditResourceUser (ResourceUser resourceUser);
		DataSet GlobalDataSet { get; set; }
		bool SchedManager { get; set; }
		string CurrentDivision { get; set; }
		IList<SchdResourceGroup> GetResourceGroups ();
		IList<SchdGroupedResources> GetGroupedResources ();
		IList<SchdGroupedResources> GetResourcesByGroupID (string groupID);
		IList<SchdGroupedResources> RemoveGroupedClinicByID (string groupID, string groupedClinicID);
		IList<NameValue> RemoveResourceGroup (string groupName);
		IList<SchdGroupedResources> AddClinicToGroupByID (string groupID, string groupedClinicID);
		string AddEditClinicGroup (string groupID, string groupName);
		IList<SchdGroupedAccessTypes> GetGroupedAccessTypes ();
		IList<SchdGroupedAccessTypes> GetAccessTypeByGroupID (string groupID);
		IList<FindGroupedAccessTypes> GetFindAccessTypeByGroupID (string groupID);
		IList<SchdGroupedAccessTypes> RemoveGroupedAccessTypeByID (string groupID, string groupedAccessTypeID);
		IList<NameValue> RemoveAccessTypeGroup (string groupID);
		IList<SchdGroupedAccessTypes> AddAccessTypeToGroupByID (string groupID, string groupedAccessTypeID);
		string AddEditAccessTypeGroup (string groupID, string groupName);
		string AddEditAccessType (SchdAccessType accessType);
		SchdAccessType GetAccessTypeByID (string accessTypeID);
		IList<FindAppointmentResult> FindAppointments (FindAppointmentData searchData);
		bool GetVariableAppointmentsFlag (string RESOURCEID);
		string DeleteAvailability (string apptID);
		string DeleteAvailability (SchdAvailability a);
		void CancelAccessBlocks (string sResourceID, string sResourceName, string sStart, string sEnd);
		string IsHoliday (DateTime startDate);
		bool OverBookAuthority (SchdAppointment appt);
		bool ClinicOverbook (string sClinicIEN, DateTime dStartTime, string sResource);
		void RefreshAvailabilitySchedule (string m_sResourcesName, DateTime m_dStartDate, DateTime m_dEndDate);
		string NoShow (string sPatientIEN, string sClinicIEN, string sAppointmentType);
		string GetReportText (string reportRPC, List<List<string>> parms);
		IList<NameValue> GetServerPrinters ();
		DataTable GetWaitingList (string ResourceIEN);
		bool CheckConflictAppointment (string sResourceName, string sPatientIEN, string sStartDate, string sEndDate);
		IList<ConflictAppointments> GetConflictAppointment (string sResourceName, string sPatientIEN, string sStartDate, string sEndDate, bool isNewAppointment);
		string CurrentUser { get; }
		bool IsEnableUpdateAppointment (string sResourceID);
		bool IsEnableUpdateAvailability (string sResourceID);
		IList<Divisions> GetDivisions ();
		bool SetDivision (string divisionID);
		string GetCurrentDivision ();
		string GetDefaultDivision ();
		void ChangeUserServer ();

		bool LoginUser(string accessCode, string verifyCode);
	}
}
