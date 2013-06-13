using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Infrastructure.Events;
using ClinSchd.Modules.DataAccess.Properties;
using ClinSchd.Infrastructure.Converters;
using System.Data;
using mscOVID.Domain;
using mscOVID.Domain.Mock;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Events;
using System.Timers;

namespace ClinSchd.Modules.DataAccess.Services
{
    public class DataAccessService : IDataAccessService
    {
		private IList<Patient> emptyList = new List<Patient>();
		private IList<SchdResource> resource_emptyList = new List<SchdResource> ();
		private IUnityContainer container;
		private IEventAggregator eventAggregator;
		private string currentUser = string.Empty;
		private string currentUserName = string.Empty;
		private DataSet m_dsGlobal = new DataSet();
		private bool m_bSchedManager;
		private string m_bCurrentDivision;
		private bool m_loadGlobalRecordsets = true;
		private OVIDDataTableRepository adt;
		private bool isLoginDialogShow = false;
		private Dictionary<string, IList<SchdAvailability>> m_globalAvailabilities;
		Timer refreshTimer;
		int TimerInterval = 120000;

        public DataAccessService(IUnityContainer container)
        {
			OVIDDataTableRepository.Container = new System.Windows.Controls.Grid ();
			adt = OVIDDataTableRepository.Instance;
			this.container = container;
			this.eventAggregator = this.container.Resolve<IEventAggregator> ();
			m_globalAvailabilities = new Dictionary<string, IList<SchdAvailability>> ();
			PassthroughAsyncModel newModel = new PassthroughAsyncModel ();
			newModel.InvokeAsync ((s, args) => { LoadGlobalRecordsets (); }, null);
			refreshTimer = new Timer (TimerInterval);
			refreshTimer.Elapsed += new ElapsedEventHandler (refreshTimer_Elapsed);
			refreshTimer.Start ();
			eventAggregator.GetEvent<RefreshScheduler> ().Subscribe (ResetSchedulerTimer);
			eventAggregator.GetEvent<RefreshAvailability> ().Subscribe (ResetSchedulerTimer);
			eventAggregator.GetEvent<ManagementItemAddedEvent> ().Subscribe (ResetSchedulerTimer);
			eventAggregator.GetEvent<LoadGlobalRecordsetsEvent> ().Subscribe (ResetSchedulerTimer);
        }

		void refreshTimer_Elapsed (object sender, ElapsedEventArgs e)
		{
			eventAggregator.GetEvent<RefreshScheduler> ().Publish (null);
			eventAggregator.GetEvent<RefreshAvailability> ().Publish (null);
			eventAggregator.GetEvent<LoadGlobalRecordsetsEvent> ().Publish (null);
		}

		private void ResetSchedulerTimer (string args)
		{
			refreshTimer.Stop ();
			refreshTimer.Interval = TimerInterval;
			refreshTimer.Start ();
		}

		// Safely extract a string from a DataRow
		private string GetCellString (DataRow row, string name)
		{
			if (row.Table.Columns.Contains (name))
				return row[name].ToString ();
			else
				return string.Empty;
		}

		public DataSet GlobalDataSet
		{
			get
			{
				return m_dsGlobal;
			}
			set
			{
				m_dsGlobal = value;
			}
		}
		public bool SchedManager
		{
			get
			{
				return m_bSchedManager;
			}
			set
			{
				m_bSchedManager = value;
			}
		}

		public string CurrentDivision
		{
			get
			{
				return m_bCurrentDivision;
			}
			set
			{
				m_bCurrentDivision = value;
			}
		}

		public bool IsLoginDialogShow
		{
			get
			{
				return isLoginDialogShow;
			}
		}

        #region IDataAccess Members
		public bool LoadGlobalRecordsets ()
		{
			if (OVIDDataTableRepository.LoginDialogShow) {
				this.eventAggregator.GetEvent<LaunchUserLoginDialogEvent>().Publish("User Login");
				return false;
			}
			//Get hospital locations
			DataTable dtHospitalLocations = new DataTable ();
			dtHospitalLocations = adt.callOVIDRPC ("BSDX HOSPITAL LOCATION", null, "HospitalLocation", m_dsGlobal);

			//Schedule User Info
			List<string> parm1 = new List<string> ();
			parm1.Add (this.CurrentUser);
			DataTable dtUser = new DataTable ();
			dtUser = adt.callOVIDRPC ("BSDX SCHEDULING USER INFO", parm1, "SchedulerUser", m_dsGlobal);

			if (dtUser.Rows.Count == 1) {
				DataRow rUser = dtUser.Rows[0];
				Object oUser = rUser["MANAGER"];
				currentUserName = rUser["USER_NAME"].ToString ();
				string sUser = oUser.ToString ();
				m_bSchedManager = (sUser == "YES") ? true : false;
				this.eventAggregator.GetEvent<IsManagerUserEvent> ().Publish (m_bSchedManager);

				m_loadGlobalRecordsets = true;
			} else {
				m_loadGlobalRecordsets = false;
			}

			//Get all divisions
			DataTable dtAllDivision = new DataTable ();
			dtAllDivision = adt.callOVIDRPC ("BSDX GET FACILITIES", parm1, "AllDivisions", m_dsGlobal);

			//Get current division
			m_bCurrentDivision = GetDefaultDivision ();
			this.eventAggregator.GetEvent<DisplayDefaultDivisionEvent> ().Publish (m_bCurrentDivision);

			//Get resources
			DataTable dtResources = new DataTable ();
			dtResources = adt.callOVIDRPC ("BSDX RESOURCES", parm1, "Resources", m_dsGlobal);

			//Get RPMS Clinics
			DataTable dtRPMSClinic = new DataTable ();
			dtRPMSClinic = adt.callOVIDRPC ("BSDX CLINIC SETUP", null, "ClinicSetupParameters", m_dsGlobal);

			//Get Providers
			DataTable dtProvider = new DataTable ();
			dtProvider = adt.callOVIDRPC ("BSDX NEW PERSON", null, "Provider", m_dsGlobal);

			//Get Clinic Stop table
			DataTable dtClinicStop = new DataTable ();
			dtClinicStop = adt.callOVIDRPC ("BSDX CLINIC STOP", null, "ClinicStop", m_dsGlobal);

			//Get Access Group
			DataTable dtAccessGroup = new DataTable ();
			dtAccessGroup = adt.callOVIDRPC ("BSDX ACCESS GROUP", null, "AccessGroups", m_dsGlobal);

			//Get Access Types
			DataTable dtAccessTypes = new DataTable ();
			dtAccessTypes = adt.callOVIDRPC ("BSDX ACCESS TYPE", null, "AccessTypes", m_dsGlobal);

			//Build Primary Key for AccessTypes table
			DataColumn[] dsa = new DataColumn[1];
			DataTable dtsa = m_dsGlobal.Tables["AccessTypes"];
			dsa[0] = dtsa.Columns["BSDX_ACCESS_TYPE_IEN"];
			m_dsGlobal.Tables["AccessTypes"].PrimaryKey = dsa;

			//Get Grouped Access Types
			DataTable dtGroupedAccessTypes = new DataTable ();
			dtGroupedAccessTypes = adt.callOVIDRPC ("BSDX GET ACCESS GROUP TYPES", null, "GroupedAccessType", m_dsGlobal);

			//Get Schedule Users
			DataTable dtScheduleUsers = new DataTable ();
			dtScheduleUsers = adt.callOVIDRPC ("BSDX SCHEDULE USER", null, "ScheduleUser", m_dsGlobal);

			//Build Primary Key for ScheduleUser table
			DataColumn[] dsr = new DataColumn[1];
			DataTable dtsr = m_dsGlobal.Tables["ScheduleUser"];
			dsr[0] = dtsr.Columns["USERID"];
			m_dsGlobal.Tables["ScheduleUser"].PrimaryKey = dsr;

			//Get Resource Users
			DataTable dtResourceUsers = new DataTable ();
			dtResourceUsers = adt.callOVIDRPC ("BSDX RESOURCE USER", null, "ResourceUser", m_dsGlobal);

			//Build Primary Key for ResourceUser table
			DataColumn[] drr = new DataColumn[1];
			DataTable dtrr = m_dsGlobal.Tables["ResourceUser"];
			drr[0] = dtrr.Columns["BSDX_RESOURCE_USER_IEN"];
			m_dsGlobal.Tables["ResourceUser"].PrimaryKey = drr;

			//Get Resource Groups
			DataTable dtResourceGroups = new DataTable ();
			dtResourceGroups = adt.callOVIDRPC ("BSDX RESOURCE GROUPS BY USER", parm1, "ResourceGroup", m_dsGlobal);

			//Get Grouped Resources
			DataTable dtGroupedResource = new DataTable ();
			dtGroupedResource = adt.callOVIDRPC ("BSDX GROUP RESOURCE", parm1, "GroupedResource", m_dsGlobal);

			return m_loadGlobalRecordsets;
		}

		private IList<Divisions> allDivisions = null;
		public IList<Divisions> GetDivisions ()
		{
			DataTable table = m_dsGlobal.Tables["AllDivisions"];

			List<Divisions> tempDivisions = new List<Divisions> ();
			foreach (DataRow row in table.Rows) {
				Divisions nameValue = new Divisions ();
				nameValue.NAME = GetCellString (row, "DIV_NAME");
				nameValue.IEN = GetCellString (row, "DIV_IEN");
				nameValue.IsDefault = (GetCellString (row, "DEFAULT") == "YES" ? true : false);
				tempDivisions.Add (nameValue);
			}
			tempDivisions.Sort (new Comparison<Divisions> ((x, y) => {
				return string.Compare (x.NAME, y.NAME, true);
			}));

			allDivisions = tempDivisions;
			return allDivisions;
		}

		public bool SetDivision (string divisionID)
		{
			List<string> parms = new List<string> ();
			parms.Add (this.CurrentUser);
			parms.Add (divisionID);

			DataTable dtDivision = new DataTable ();
			dtDivision = adt.callOVIDRPC ("BSDX SET FACILITY", parms);

			bool returnValue = false;
			if (dtDivision.Rows.Count == 1) {
				DataRow r = dtDivision.Rows[0];
				returnValue = (r["ERROR_ID"].ToString () == "1" ? true : false);
			}

			return returnValue;
		}

		public string GetCurrentDivision ()
		{
			//Get Current Division
			List<string> parms = new List<string> ();
			parms.Add (this.CurrentUser);

			DataTable dtDivision = new DataTable ();
			dtDivision = adt.callOVIDRPC ("BSDX CURRENT FACILITY", parms, "CurrentDivision", m_dsGlobal);

			string returnValue = string.Empty;
			string divisionID = string.Empty;
			if (dtDivision.Rows.Count == 1) {
				DataRow r = dtDivision.Rows[0];
				divisionID = r["CURRENT_DIV"].ToString ();
			}

			DataTable table = m_dsGlobal.Tables["AllDivisions"];

			foreach (DataRow row in table.Rows) {
				if (GetCellString (row, "DIV_IEN") == divisionID) {
					returnValue = GetCellString (row, "DIV_NAME");
				}
			}

			return returnValue;
		}

		public string GetDefaultDivision ()
		{
			string divisionName = string.Empty;
			DataTable table = m_dsGlobal.Tables["AllDivisions"];
			foreach (DataRow row in table.Rows) {
				if ((GetCellString (row, "DEFAULT") == "YES" ? true : false)) {
					divisionName = GetCellString (row, "DIV_NAME");
				}
			}

			return divisionName;
		}

		public bool OverBookAuthority (SchdAppointment a)
		{
			bool bOverbookAppointment = false;
			bool bRegularOverbook = false;
			bool bMasterOverbook = false;

			//Find the clinic IEN
			DataView rv = new DataView (GlobalDataSet.Tables["Resources"]);
			rv.Sort = "RESOURCE_NAME ASC";
			int nFindResource = rv.Find (a.RESOURCENAME);
			DataRowView drvResource = rv[nFindResource];

			string sHospLoc = drvResource["HOSPITAL_LOCATION_ID"].ToString ();
			string sResourceID = drvResource["RESOURCEID"].ToString ();

			DataView dv = new DataView (GlobalDataSet.Tables["ResourceUser"]);
			dv.Sort = "RESOURCE_NAME ASC";
			dv.RowFilter = "RESOURCE_NAME = '" + a.RESOURCENAME + "'";

			for (int j = 0; j < dv.Count; j++) {
				if (dv[j]["USERNAME"].ToString () == currentUserName) {
					bRegularOverbook = (dv[j]["OVERBOOK"].ToString () == "YES") ? true : false;
					bMasterOverbook = (dv[j]["MASTEROVERBOOK"].ToString () == "YES") ? true : false;
				}
			}

			if (bMasterOverbook) {
				bOverbookAppointment = true;
			} else {
				//Check the clinic level for the rebook authority
				bool bClinicOverbook = false;
				bClinicOverbook = ClinicOverbook (sHospLoc, Convert.ToDateTime(a.START_TIME), sResourceID);

				if (bClinicOverbook) {
					if (bRegularOverbook) {
						bOverbookAppointment = true;
					} else {
						bOverbookAppointment = false;
					}

				} else {
					bOverbookAppointment = false;
				}
			}
			return bOverbookAppointment;
		}

		public string NoShow (string sPatientIEN, string sResourceName, string sAppointmentType)
		{
			//Find the clinic IEN
			DataView rv = new DataView (GlobalDataSet.Tables["Resources"]);
			rv.Sort = "RESOURCE_NAME ASC";
			int nFindResource = rv.Find (sResourceName);
			DataRowView drvResource = rv[nFindResource];

			string sHospLoc = drvResource["HOSPITAL_LOCATION_ID"].ToString ();

			List<string> parms = new List<string> ();
			parms.Add (sPatientIEN);
			parms.Add (sHospLoc);

			DataTable table = adt.callOVIDRPC ("BSDX NOSHOW COUNT", parms);

			string sResult = string.Empty;
			if (table.Rows.Count == 1) {
				DataRow r = table.Rows[0];
				if (r["TOO_MANY"].ToString () == "1" && sAppointmentType == "AddNew") {
					sResult = " has " + r["TOTAL_NO_SHOWS"].ToString () + " no shows .\nThe maximum number of no shows is " + r["ALLOWED_NO_SHOWS"].ToString () + ".";
				} 
			}
			return sResult;
		}

		public bool ClinicOverbook (string sClinicIEN, DateTime dStartTime, string sResource)
		{
			List<string> parms = new List<string> ();
			parms.Add (sClinicIEN);
			parms.Add (dStartTime.ToString ("M/d/yyyy@HH:mm"));
			parms.Add (sResource);

			DataTable table = adt.callOVIDRPC ("BSDX OVERBOOK", parms);

			bool bResult = false;
			if (table.Rows.Count == 1) {
				DataRow r = table.Rows[0];
				bResult = (r["OVERBOOK"].ToString () == "YES") ? true : false;
			}
			return bResult;
		}

		public string IsHoliday (DateTime startDate)
		{
			string sHolidayName = string.Empty;
			List<string> parms = new List<string> ();
			parms.Add (startDate.ToString ("M/d/yyyy@HH:mm"));

			//Get holidays
			DataView dvHolidays = new DataView(adt.callOVIDRPC ("BSDX HOLIDAY", parms, "Holidays", m_dsGlobal));
			dvHolidays.Sort = "BSDX_HOLIDAY_DATE ASC";
			int nFind = dvHolidays.Find (startDate.ToShortDateString());

			if (nFind > -1) {
				DataRowView drv = dvHolidays[nFind];
				sHolidayName = drv["HOLIDAY_NAME"].ToString ();
			}

			return sHolidayName;
		}

		public string DeleteAvailability (string apptID)
		{
			List<string> parms = new List<string> ();
			parms.Add (apptID);

			DataTable table = adt.callOVIDRPC ("BSDX CANCEL AVAILABILITY", parms);

			string sErrorID = string.Empty;
			if (table.Rows.Count == 1) {
				DataRow r = table.Rows[0];
				if (r["ERRORID"].ToString () != "-1") {
					sErrorID = r["ERRORID"].ToString ();
				}
			}
			return sErrorID;
		}

		public string DeleteAvailability (SchdAvailability a)
		{
			List<string> parms = new List<string> ();
			parms.Add (a.APPOINTMENTID);

			DataTable table = adt.callOVIDRPC ("BSDX CANCEL AVAILABILITY", parms);

			string sErrorID = string.Empty;
			if (table.Rows.Count == 1) {
				DataRow r = table.Rows[0];
				if (r["ERRORID"].ToString () != "-1") {
					sErrorID = r["ERRORID"].ToString ();
				}
			}

			if (sErrorID == string.Empty) {
				GetAvailabilities (a.RESOURCENAME, a.StartTime, a.EndTime);
			}
			return sErrorID;
		}

		public string AppointmentNoShow (string apptID, bool bNoShow)
		{
			List<string> parms = new List<string> ();
			parms.Add (apptID);
			parms.Add ((bNoShow == true) ? "1" : "0");
			DataTable table = adt.callOVIDRPC ("BSDX NOSHOW", parms);

			string sErrorText = string.Empty;
			if (table.Rows.Count == 1) {
				DataRow r = table.Rows[0];
				if (r["ERRORID"].ToString () != "1") {
					sErrorText = r["ERRORTEXT"].ToString ();
				}
			}
			return sErrorText;
		}

		public string UndoCancelAppointment (string apptID)
		{
			List<string> parms = new List<string> ();
			parms.Add (apptID);
			DataTable table = adt.callOVIDRPC ("BSDX UNCANCEL APPT", parms);

			string sErrorID = string.Empty;
			if (table.Rows.Count == 1) {
				DataRow r = table.Rows[0];
				sErrorID = r["ERRORID"].ToString () == "0" ? string.Empty : r["ERRORID"].ToString ();
			}

			return sErrorID;
		}

		public string CancelAppointment (CancelAppointment cancelAppointmentData)
		{
			List<string> parms = new List<string> ();
			parms.Add (cancelAppointmentData.ApptID);
			parms.Add (cancelAppointmentData.CancelledByClinic == true ? "C" : "PC");
			parms.Add (cancelAppointmentData.ReasonIEN);
			parms.Add (cancelAppointmentData.Notes);
			DataTable table = adt.callOVIDRPC ("BSDX CANCEL APPOINTMENT", parms);

			string sErrorID = string.Empty;
			if (table.Rows.Count == 1) {
				DataRow r = table.Rows[0];
				sErrorID = r["ERRORID"].ToString () == "0" ? string.Empty : r["ERRORID"].ToString ();
			}

			return sErrorID;
		}

		public string UndoCheckOut (string apptID)
		{
			List<string> parms = new List<string> ();
			parms.Add (apptID);
			DataTable table = adt.callOVIDRPC ("BSDX CANCEL CHECKOUT APPT", parms);

			string sErrorID = string.Empty;
			if (table.Rows.Count == 1) {
				DataRow r = table.Rows[0];
				sErrorID = r["ERRORID"].ToString () == "0" ? string.Empty : r["ERRORID"].ToString ();
			}

			return sErrorID;
		}

		public string CheckOutPatient (CheckOutPatient checkOutData)
		{
			List<string> parms = new List<string> ();
			parms.Add (checkOutData.PatientID);
			parms.Add (Convert.ToDateTime (checkOutData.AppointmentStartTime).ConvertToExternalDateTimeFormat (true, true));
			parms.Add (Convert.ToDateTime (checkOutData.CheckOutDateTime).ConvertToExternalDateTimeFormat (true, true));
			parms.Add (checkOutData.ApptID);
			parms.Add (checkOutData.ProviderIEN);
			DataTable table = adt.callOVIDRPC ("BSDX CHECKOUT APPOINTMENT", parms);

			string sErrorID = string.Empty;
			if (table.Rows.Count == 1) {
				DataRow r = table.Rows[0];
				sErrorID = r["ERRORID"].ToString () == "0" ? string.Empty : r["ERRORID"].ToString ();
			}

			return sErrorID;
		}

		public string UndoCheckIn (string apptID)
		{
			List<string> parms = new List<string> ();
			parms.Add (apptID);
			parms.Add ("@");
			DataTable table = adt.callOVIDRPC ("BSDX CHECKIN APPOINTMENT", parms);

			string sErrorID = string.Empty;
			string sErrorMessage = string.Empty;
			if (table.Rows.Count == 1) {
				DataRow r = table.Rows[0];
				sErrorID = r["ERRORID"].ToString ();
				if (r["Message"].ToString () != string.Empty) {
					if (sErrorID == "0") {
						sErrorMessage = "Undo Patient Check In Successful.\n\n";
					}
					sErrorMessage += "However, the following error(s) occurred while Undo Checking In the patient:\n" + r["MESSAGE"].ToString ();
				} else {
					sErrorMessage = r["MESSAGE"].ToString ();
				}
			}

			return sErrorMessage;
		}

		public string CheckInPatient (CheckInPatient checkInData)
		{
			List<string> parms = new List<string> ();
			parms.Add (checkInData.ApptID);
			parms.Add (Convert.ToDateTime(checkInData.CheckInDateTime).ConvertToExternalDateTimeFormat(true, true));
			parms.Add (checkInData.ClinicStopIEN);
			parms.Add (checkInData.ProviderIEN);
			parms.Add (checkInData.PrintRouteSlip.ToString());
			parms.Add (checkInData.HealthSummaryLetterIEN);
			parms.Add (checkInData.PrintWellnessHandout.ToString());
			parms.Add (checkInData.PCCClinicIEN == null ? "" : checkInData.PCCClinicIEN);
			parms.Add (checkInData.PCCFormIEN == null ? "" : checkInData.PCCFormIEN);
			parms.Add (checkInData.PCCOutGuide.ToString ());
			parms.Add (checkInData.MakeChartRequest.ToString ());
			DataTable table = adt.callOVIDRPC ("BSDX CHECKIN APPOINTMENT", parms);

			string sErrorMessage = string.Empty;
			string sErrorID = string.Empty;
			if (table.Rows.Count == 1) {
				DataRow r = table.Rows[0];
				sErrorID = r["ERRORID"].ToString ();
				if (r["Message"].ToString () != string.Empty) {
					if (sErrorID == "0") {
						sErrorMessage = "Patient Check In Successful.\n\n";
					}
					sErrorMessage += "However, the following error(s) occurred while Checking In the patient:\n" + r["MESSAGE"].ToString ();
				} else {
					sErrorMessage = r["MESSAGE"].ToString ();
				}
			}

			return sErrorMessage;
		}

		public bool CheckConflictAppointment (string sResourceName, string sPatientIEN, string sStartDate, string sEndDate)
		{
			List<string> parms = new List<string> ();
			parms.Add (Convert.ToDateTime(sStartDate).ToString ("M-d-yyyy"));
			parms.Add (Convert.ToDateTime (sEndDate).ToString ("M-d-yyyy"));

			DataTable table = adt.callOVIDRPC ("BSDX ALL APPOINTMENTS", parms);
			foreach (DataRow row in table.Rows) {

				DateTime startDate = Convert.ToDateTime (GetCellString (row, "START_TIME"));
				DateTime endDate = Convert.ToDateTime (GetCellString (row, "END_TIME"));
				string resourceName = GetCellString (row, "RES_NAME");
				string patientId = GetCellString (row, "PAT_ID");
				if (TimesOverlap (Convert.ToDateTime (sStartDate), Convert.ToDateTime (sEndDate), startDate, endDate) && (patientId == sPatientIEN)) 
				{
					return true;
				}
			}
			return false;
		}

		private IList<ConflictAppointments> conflictAppointments = null;
		public IList<ConflictAppointments> GetConflictAppointment (string sResourceName, string sPatientIEN, string sStartDate, string sEndDate, bool isNewAppointment)
		{
			List<string> parms = new List<string> ();
			parms.Add (Convert.ToDateTime (sStartDate).ToString ("M-d-yyyy"));
			parms.Add (Convert.ToDateTime (sEndDate).ToString ("M-d-yyyy"));

			DataTable table = adt.callOVIDRPC ("BSDX ALL APPOINTMENTS", parms);

			List<ConflictAppointments> tempAppointments = new List<ConflictAppointments> ();
			foreach (DataRow row in table.Rows) {

				ConflictAppointments ca = new ConflictAppointments ();
				DateTime startDate = Convert.ToDateTime (GetCellString (row, "START_TIME"));
				DateTime endDate = Convert.ToDateTime (GetCellString (row, "END_TIME"));
				string resourceName = GetCellString (row, "RES_NAME");
				string patientId = GetCellString (row, "PAT_ID");
				if (TimesOverlap(Convert.ToDateTime(sStartDate), Convert.ToDateTime(sEndDate), startDate, endDate) &&(resourceName == sResourceName || patientId == sPatientIEN))
				{
					if (patientId == sPatientIEN && resourceName == sResourceName && startDate == Convert.ToDateTime (sStartDate) && endDate == Convert.ToDateTime (sEndDate) && !isNewAppointment)
						continue;

					ca.ResourceName = GetCellString (row, "RES_NAME");
					string patientID = GetCellString (row, "PAT_ID");
					PatientInformation newPatient = new PatientInformation ();
					newPatient = GetPatientInfo (patientID);
					ca.Patient = newPatient.Name;
					ca.StartTime = GetCellString (row, "START_TIME");
					ca.EndTime = GetCellString (row, "END_TIME");
					tempAppointments.Add (ca);
				}
			}
			conflictAppointments = tempAppointments;
			return conflictAppointments;
		}


		private IList<NameValue> scheduleUsers = null;
		public IList<NameValue> GetScheduleUsers ()
		{
			if (scheduleUsers == null || scheduleUsers.Count == 0) {
				DataTable table = m_dsGlobal.Tables["ScheduleUser"];

				List<NameValue> tempSchedUsers = new List<NameValue> ();
				foreach (DataRow row in table.Rows) {
					NameValue nameValue = new NameValue ();
					nameValue.Value = GetCellString (row, "USERID");
					nameValue.Name = GetCellString (row, "USERNAME");
					tempSchedUsers.Add (nameValue);
				}
				tempSchedUsers.Sort (new Comparison<NameValue> ((x, y) => {
					return string.Compare (x.Name, y.Name, true);
				}));

				scheduleUsers = tempSchedUsers;
			}
			return scheduleUsers;
		}

		private IList<NameValue> resourceGroups = null;
		public IList<NameValue> GetResourceGroupList ()
		{
			DataTable table = m_dsGlobal.Tables["ResourceGroup"];

			List<NameValue> tempResourceGroup = new List<NameValue> ();
			foreach (DataRow row in table.Rows) {
				NameValue nameValue = new NameValue ();
				nameValue.Value = GetCellString (row, "RESOURCE_GROUPID");
				nameValue.Name = GetCellString (row, "RESOURCE_GROUP");
				tempResourceGroup.Add (nameValue);
			}
			tempResourceGroup.Sort (new Comparison<NameValue> ((x, y) => {
				return string.Compare (x.Name, y.Name, true);
			}));

			resourceGroups = tempResourceGroup;
			return resourceGroups;
		}

		private IList<SchdGroupedAccessTypes> groupedAccessTypes = null;
		public IList<SchdGroupedAccessTypes> GetGroupedAccessTypes ()
		{
			DataTable table = m_dsGlobal.Tables["GroupedAccessType"];

			List<SchdGroupedAccessTypes> tempGroupedAccessType = new List<SchdGroupedAccessTypes> ();
			foreach (DataRow row in table.Rows) {
				SchdGroupedAccessTypes at = new SchdGroupedAccessTypes ();
				at.ACCESS_GROUP_TYPEID = GetCellString (row, "ACCESS_GROUP_TYPEID");
				at.ACCESS_GROUP = GetCellString (row, "ACCESS_GROUP");
				at.ACCESS_GROUP_ID = GetCellString (row, "ACCESS_GROUP_ID");
				at.ACCESS_TYPE = GetCellString (row, "ACCESS_TYPE");
				at.ACCESS_TYPE_ID = GetCellString (row, "ACCESS_TYPE_ID");
				tempGroupedAccessType.Add (at);
			}
			tempGroupedAccessType.Sort (new Comparison<SchdGroupedAccessTypes> ((x, y) => {
				return string.Compare (x.ACCESS_TYPE, y.ACCESS_TYPE, true);
			}));
			groupedAccessTypes = tempGroupedAccessType;
			return groupedAccessTypes;
		}

		private IList<SchdGroupedAccessTypes> groupedAccessType = null;
		public IList<SchdGroupedAccessTypes> GetAccessTypeByGroupID (string groupID)
		{
			DataTable table = m_dsGlobal.Tables["GroupedAccessType"];

			List<SchdGroupedAccessTypes> trmpAccessType = new List<SchdGroupedAccessTypes> ();
			foreach (DataRow row in table.Rows) {
				SchdGroupedAccessTypes at = new SchdGroupedAccessTypes ();

				if (GetCellString (row, "ACCESS_GROUP_ID") == groupID || groupID == "0") {
					at.ACCESS_GROUP_TYPEID = GetCellString (row, "ACCESS_GROUP_TYPEID");
					at.ACCESS_GROUP = GetCellString (row, "ACCESS_GROUP");
					at.ACCESS_GROUP_ID = GetCellString (row, "ACCESS_GROUP_ID");
					at.ACCESS_TYPE = GetCellString (row, "ACCESS_TYPE");
					at.ACCESS_TYPE_ID = GetCellString (row, "ACCESS_TYPE_ID");
					trmpAccessType.Add (at);
				}
			}
			trmpAccessType.Sort (new Comparison<SchdGroupedAccessTypes> ((x, y) => {
				return string.Compare (x.ACCESS_TYPE, y.ACCESS_TYPE, true);
			}));
			groupedAccessType = trmpAccessType;
			return groupedAccessType;
		}

		private IList<FindGroupedAccessTypes> findGroupedAccessType = null;
		public IList<FindGroupedAccessTypes> GetFindAccessTypeByGroupID (string groupID)
		{
			DataTable table = m_dsGlobal.Tables["GroupedAccessType"];
			DataTable tableAll = m_dsGlobal.Tables["AccessTypes"];

			List<FindGroupedAccessTypes> trmpAccessType = new List<FindGroupedAccessTypes> ();
			if (groupID == "0") {
				foreach (DataRow row in tableAll.Rows) {
					FindGroupedAccessTypes atAll = new FindGroupedAccessTypes ();
					atAll.ACCESS_TYPE = GetCellString (row, "NAME");
					atAll.ACCESS_TYPE_ID = GetCellString (row, "BSDX_ACCESS_TYPE_IEN");
					atAll.IsChecked = false;
					trmpAccessType.Add (atAll);
				}
			} else {
				foreach (DataRow row in table.Rows) {
					FindGroupedAccessTypes at = new FindGroupedAccessTypes ();

					if (GetCellString (row, "ACCESS_GROUP_ID") == groupID) {
						at.ACCESS_TYPE = GetCellString (row, "ACCESS_TYPE");
						at.ACCESS_TYPE_ID = GetCellString (row, "ACCESS_TYPE_ID");
						at.IsChecked = false;
						trmpAccessType.Add (at);
					}
				}
			}
			trmpAccessType.Sort (new Comparison<FindGroupedAccessTypes> ((x, y) => {
				return string.Compare (x.ACCESS_TYPE, y.ACCESS_TYPE, true);
			}));
			findGroupedAccessType = trmpAccessType;
			return findGroupedAccessType;
		}

		private IList<SchdGroupedResources> groupedResources = null;
		public IList<SchdGroupedResources> GetGroupedResources ()
		{
			DataTable table = m_dsGlobal.Tables["GroupedResource"];

			List<SchdGroupedResources> tempGroupedResources = new List<SchdGroupedResources> ();
			foreach (DataRow row in table.Rows) {
				SchdGroupedResources gr = new SchdGroupedResources ();
				gr.RESOURCE_GROUP = GetCellString (row, "RESOURCE_GROUP");
				gr.RESOURCE_GROUP_ITEMID = GetCellString (row, "RESOURCE_GROUP_ITEMID");
				gr.RESOURCE_GROUPID = GetCellString (row, "RESOURCE_GROUPID");
				gr.RESOURCE_ID = GetCellString (row, "RESOURCE_ID");
				gr.RESOURCE_NAME = GetCellString (row, "RESOURCE_NAME");
				tempGroupedResources.Add (gr);
			}
			tempGroupedResources.Sort (new Comparison<SchdGroupedResources> ((x, y) => {
				return string.Compare (x.RESOURCE_NAME, y.RESOURCE_NAME, true);
			}));

			groupedResources = tempGroupedResources;
			return groupedResources;
		}

		private IList<SchdGroupedResources> groupedResource = null;
		public IList<SchdGroupedResources> GetResourcesByGroupID (string groupID)
		{
			DataTable table = m_dsGlobal.Tables["GroupedResource"];

			List<SchdGroupedResources> trmpResource = new List<SchdGroupedResources> ();
			foreach (DataRow row in table.Rows) {
				SchdGroupedResources r = new SchdGroupedResources ();

				if (GetCellString (row, "RESOURCE_GROUPID") == groupID) {
					r.RESOURCE_GROUP = GetCellString (row, "RESOURCE_GROUP");
					r.RESOURCE_GROUP_ITEMID = GetCellString (row, "RESOURCE_GROUP_ITEMID");
					r.RESOURCE_GROUPID = GetCellString (row, "RESOURCE_GROUPID");
					r.RESOURCE_ID = GetCellString (row, "RESOURCE_ID");
					r.RESOURCE_NAME = GetCellString (row, "RESOURCE_NAME");
					trmpResource.Add (r);
				}
			}
			trmpResource.Sort (new Comparison<SchdGroupedResources> ((x, y) => {
				return string.Compare (x.RESOURCE_NAME, y.RESOURCE_NAME, true);
			}));
			groupedResource = trmpResource;
			return groupedResource;
		}

		private IList<SchdGroupedAccessTypes> rgroupedAccessType = null;
		public IList<SchdGroupedAccessTypes> RemoveGroupedAccessTypeByID (string groupID, string groupedAccessTypeID)
		{
			List<string> parms = new List<string> ();
			parms.Add (groupID);
			parms.Add (groupedAccessTypeID);
			DataTable table = adt.callOVIDRPC ("BSDX DELETE ACCESS GROUP ITEM", parms);

			m_dsGlobal.Tables["GroupedAccessType"].Clear ();
			//Get Grouped Access Types
			DataTable dtGroupedAccessTypes = new DataTable ();
			dtGroupedAccessTypes = adt.callOVIDRPC ("BSDX GET ACCESS GROUP TYPES", null, "GroupedAccessType", m_dsGlobal);

			rgroupedAccessType = GetAccessTypeByGroupID (groupID);
			return rgroupedAccessType;
		}

		private IList<SchdGroupedAccessTypes> agroupedAccessType = null;
		public IList<SchdGroupedAccessTypes> AddAccessTypeToGroupByID (string groupID, string groupedAccessTypeID)
		{
			List<string> parms = new List<string> ();
			parms.Add (groupID);
			parms.Add (groupedAccessTypeID);
			DataTable table = adt.callOVIDRPC ("BSDX ADD ACCESS GROUP ITEM", parms);

			m_dsGlobal.Tables["GroupedAccessType"].Clear ();
			//Get Grouped Access Types
			DataTable dtGroupedAccessTypes = new DataTable ();
			dtGroupedAccessTypes = adt.callOVIDRPC ("BSDX GET ACCESS GROUP TYPES", null, "GroupedAccessType", m_dsGlobal);

			agroupedAccessType = GetAccessTypeByGroupID (groupID);
			return agroupedAccessType;
		}

		private IList<NameValue> aGroups = null;
		public IList<NameValue> RemoveAccessTypeGroup (string groupID)
		{
			List<string> parms = new List<string> ();
			parms.Add (groupID);
			DataTable table = adt.callOVIDRPC ("BSDX DELETE ACCESS GROUP", parms);

			m_dsGlobal.Tables["AccessGroups"].Clear ();
			//Get Resource Groups
			DataTable dtAccessGroup = new DataTable ();
			dtAccessGroup = adt.callOVIDRPC ("BSDX ACCESS GROUP", null, "AccessGroups", m_dsGlobal);

			rGroups = GetAccessGroups ();
			return rGroups;
		}

		private IList<SchdGroupedResources> rgroupedChinic = null;
		public IList<SchdGroupedResources> RemoveGroupedClinicByID (string groupID, string groupedClinicID)
		{
			List<string> parms = new List<string> ();
			parms.Add (groupID);
			parms.Add (groupedClinicID);
			DataTable table = adt.callOVIDRPC ("BSDX DELETE RES GROUP ITEM", parms);

			m_dsGlobal.Tables["GroupedResource"].Clear ();
			//Get Grouped Resources
			DataTable dtGroupedResource = new DataTable ();
			List<string> parms1 = new List<string> ();
			parms1.Add (this.CurrentUser);
			dtGroupedResource = adt.callOVIDRPC ("BSDX GROUP RESOURCE", parms1, "GroupedResource", m_dsGlobal);

			rgroupedChinic = GetResourcesByGroupID (groupID);
			return rgroupedChinic;
		}

		private IList<NameValue> rGroups = null;
		public IList<NameValue> RemoveResourceGroup (string groupName)
		{
			List<string> parms = new List<string> ();
			parms.Add (groupName);
			DataTable table = adt.callOVIDRPC ("BSDX DELETE RESOURCE GROUP", parms);

			m_dsGlobal.Tables["ResourceGroup"].Clear ();
			//Get Resource Groups
			DataTable dtGroupedResource = new DataTable ();
			List<string> parms1 = new List<string> ();
			parms1.Add (this.CurrentUser);
			DataTable dtResourceGroups = new DataTable ();
			dtResourceGroups = adt.callOVIDRPC ("BSDX RESOURCE GROUPS BY USER", parms1, "ResourceGroup", m_dsGlobal);

			rGroups = GetResourceGroupList ();
			return rGroups;
		}

		private IList<SchdGroupedResources> agroupedChinic = null;
		public IList<SchdGroupedResources> AddClinicToGroupByID (string groupID, string groupedClinicID)
		{
			List<string> parms = new List<string> ();
			parms.Add (groupID);
			parms.Add (groupedClinicID);
			DataTable table = adt.callOVIDRPC ("BSDX ADD RES GROUP ITEM", parms);

			m_dsGlobal.Tables["GroupedResource"].Clear ();
			//Get Grouped Resources
			DataTable dtGroupedResource = new DataTable ();
			List<string> parms1 = new List<string> ();
			parms1.Add (this.CurrentUser);
			dtGroupedResource = adt.callOVIDRPC ("BSDX GROUP RESOURCE", parms1, "GroupedResource", m_dsGlobal);

			agroupedChinic = GetResourcesByGroupID (groupID);
			return agroupedChinic;
		}

		private IList<NameValue> accessGroups = null;
		public IList<NameValue> GetAccessGroups ()
		{
			DataTable table = m_dsGlobal.Tables["AccessGroups"];

			List<NameValue> tempAccessGroups = new List<NameValue> ();
			foreach (DataRow row in table.Rows) {
				NameValue nameValue = new NameValue ();
				nameValue.Value = GetCellString (row, "BSDX_ACCESS_GROUP_IEN");
				nameValue.Name = GetCellString (row, "NAME");
				tempAccessGroups.Add (nameValue);
			}
			tempAccessGroups.Sort (new Comparison<NameValue> ((x, y) => {
				return string.Compare (x.Name, y.Name, true);
			}));
			accessGroups = tempAccessGroups;
			return accessGroups;
		}

		private IList<ResourceUser> resourceUsers = null;
		public IList<ResourceUser> GetResourceUsers ()
		{
			DataTable table = m_dsGlobal.Tables["ResourceUser"];

			List<ResourceUser> tempResourceUsers = new List<ResourceUser> ();
			foreach (DataRow row in table.Rows) {
				ResourceUser resourceUser = new ResourceUser ();
				resourceUser.BSDX_RESOURCE_USER_IEN = GetCellString (row, "BSDX_RESOURCE_USER_IEN");
				resourceUser.RESOURCE_ID = GetCellString (row, "RESOURCE_ID");
				resourceUser.RESOURCE_NAME = GetCellString (row, "RESOURCE_NAME");
				resourceUser.OVERBOOK = GetCellString (row, "OVERBOOK");
				resourceUser.MODIFY_SCHEDULE = GetCellString (row, "MODIFY_SCHEDULE");
				resourceUser.MODIFY_APPTS = GetCellString (row, "MODIFY_APPTS");
				resourceUser.USERNAME = GetCellString (row, "USERNAME");
				resourceUser.USER_ID = GetCellString (row, "USER_ID");
				resourceUser.MASTEROVERBOOK = GetCellString (row, "MASTEROVERBOOK");
				tempResourceUsers.Add (resourceUser);
			}
			tempResourceUsers.Sort (new Comparison<ResourceUser> ((x, y) => {
				return string.Compare (x.RESOURCE_NAME, y.RESOURCE_NAME, true);
			}));
			resourceUsers = tempResourceUsers;
			return resourceUsers;
		}

		private IList<ResourceUser> rresourceUser = null;
		public IList<ResourceUser> RemoveResourceUserByID (string userID, string resourceID)
		{
			List<string> parms = new List<string> ();
			parms.Add (userID);
			DataTable table = adt.callOVIDRPC ("BSDX DELETE RESOURCEUSER", parms);

			m_dsGlobal.Tables["ResourceUser"].Clear ();
			DataTable dtResourceUsers = new DataTable ();
			dtResourceUsers = adt.callOVIDRPC ("BSDX RESOURCE USER", null, "ResourceUser", m_dsGlobal);

			rresourceUser = GetResourceUsersByResourceID (resourceID);
			return rresourceUser;
		}

		private IList<ResourceUser> resourceUser = null;
		public IList<ResourceUser> GetResourceUsersByResourceID (string resourceID)
		{
			DataTable table = m_dsGlobal.Tables["ResourceUser"];

			List<ResourceUser> trmpUser = new List<ResourceUser> ();
			foreach (DataRow row in table.Rows) {
				ResourceUser r = new ResourceUser ();

				if (GetCellString (row, "RESOURCE_ID") == resourceID) {
					r.BSDX_RESOURCE_USER_IEN = GetCellString (row, "BSDX_RESOURCE_USER_IEN");
					r.RESOURCE_ID = GetCellString (row, "RESOURCE_ID");
					r.RESOURCE_NAME = GetCellString (row, "RESOURCE_NAME");
					r.OVERBOOK = GetCellString (row, "OVERBOOK");
					r.MODIFY_SCHEDULE = GetCellString (row, "MODIFY_SCHEDULE");
					r.MODIFY_APPTS = GetCellString (row, "MODIFY_APPTS");
					r.USERNAME = GetCellString (row, "USERNAME");
					r.USER_ID = GetCellString (row, "USER_ID");
					r.MASTEROVERBOOK = GetCellString (row, "MASTEROVERBOOK");
					trmpUser.Add (r);
				}
			}
			trmpUser.Sort (new Comparison<ResourceUser> ((x, y) => {
				return string.Compare (x.RESOURCE_NAME, y.RESOURCE_NAME, true);
			}));
			resourceUser = trmpUser;
			return resourceUser;
		}

		public bool IsEnableUpdateAppointment (string sResourceID)
		{
			bool isEnableUpdateAppointment = false;
			IList<ResourceUser> users = GetResourceUsersByResourceID (sResourceID);

			foreach (ResourceUser u in users) {
				if (u.USER_ID == this.CurrentUser) {
					isEnableUpdateAppointment = u.MODIFY_APPTS == "YES" ? true : false;
				}
			}

			return isEnableUpdateAppointment;
		}

		public bool IsEnableUpdateAvailability (string sResourceID)
		{
			bool isEnableUpdateAvailability = false;
			IList<ResourceUser> users = GetResourceUsersByResourceID (sResourceID);

			foreach (ResourceUser u in users) {
				if (u.USER_ID == this.CurrentUser) {
					isEnableUpdateAvailability = u.MODIFY_SCHEDULE == "YES" ? true : false;
				}
			}

			return isEnableUpdateAvailability;
		}

		public SchdAccessType GetAccessTypeByID (string accessTypeID)
		{
			DataTable table = m_dsGlobal.Tables["AccessTypes"];

			//Build Primary Key for AccessType table
			DataColumn[] accessType_dc = new DataColumn[1];
			accessType_dc[0] = table.Columns["BSDX_ACCESS_TYPE_IEN"];
			table.PrimaryKey = accessType_dc;

			DataRow row = table.Rows.Find (accessTypeID);
			SchdAccessType newAccessType = container.Resolve<Factory<SchdAccessType>> ().Create ();

			if (row != null) {
				newAccessType.BSDX_ACCESS_TYPE_IEN = row["BSDX_ACCESS_TYPE_IEN"].ToString ();
				newAccessType.NAME = row["NAME"].ToString ();
				newAccessType.BLUE = row["BLUE"].ToString ();
				newAccessType.DEPARTMENT_NAME = row["DEPARTMENT_NAME"].ToString ();
				newAccessType.INACTIVE = row["INACTIVE"].ToString ();
				newAccessType.DISPLAY_COLOR = row["DISPLAY_COLOR"].ToString ();
				newAccessType.GREEN = row["GREEN"].ToString ();
				newAccessType.PREVENT_ACCESS = row["PREVENT_ACCESS"].ToString ();
				newAccessType.RED = row["RED"].ToString ();
			}
			return newAccessType;
		}

		private IList<SchdAccessType> accessTypes = null;
		public IList<SchdAccessType> GetAccessTypes ()
		{

			DataTable table = m_dsGlobal.Tables["AccessTypes"];

			List<SchdAccessType> tempAccessTypes = new List<SchdAccessType> ();
			foreach (DataRow row in table.Rows) {
				SchdAccessType accessTypeValue = new SchdAccessType ();
				accessTypeValue.BSDX_ACCESS_TYPE_IEN = GetCellString (row, "BSDX_ACCESS_TYPE_IEN");
				accessTypeValue.NAME = GetCellString (row, "NAME");
				accessTypeValue.BLUE = GetCellString (row, "BLUE");
				accessTypeValue.DEPARTMENT_NAME = GetCellString (row, "DEPARTMENT_NAME");
				accessTypeValue.DISPLAY_COLOR = GetCellString (row, "DISPLAY_COLOR");
				accessTypeValue.GREEN = GetCellString (row, "GREEN");
				accessTypeValue.INACTIVE = GetCellString (row, "INACTIVE");
				accessTypeValue.PREVENT_ACCESS = GetCellString (row, "PREVENT_ACCESS");
				accessTypeValue.RED = GetCellString (row, "RED");
				tempAccessTypes.Add (accessTypeValue);
			}
			tempAccessTypes.Sort (new Comparison<SchdAccessType> ((x, y) => {
				return string.Compare (x.NAME, y.NAME, true);
			}));

			accessTypes = tempAccessTypes;

			return accessTypes;
		}

		private IList<NameValue> cancellationReasons = null;
		public IList<NameValue> GetCancellationReasons ()
		{
			if (cancellationReasons == null || cancellationReasons.Count == 0) {
				DataTable table = adt.callOVIDRPC ("BSDX CANCELLATION REASONS", null, "CancellationReasons", m_dsGlobal);

				List<NameValue> tempReasons = new List<NameValue> ();
				foreach (DataRow row in table.Rows) {
					NameValue nameValue = new NameValue ();
					nameValue.Name = GetCellString (row, "NAME");
					nameValue.Value = GetCellString (row, "CANCELLATION_REASONS_IEN");
					tempReasons.Add (nameValue);
				}
				tempReasons.Sort (new Comparison<NameValue> ((x, y) => {
					return string.Compare (x.Name, y.Name, true);
				}));

				cancellationReasons = tempReasons;
			}

			return cancellationReasons;
		}

		private IList<NameValue> summaryReport = null;
		public IList<NameValue> GetSummaryReport ()
		{
			if (summaryReport == null || summaryReport.Count == 0) {
				DataTable table = adt.callOVIDRPC ("BSDX HS PWH TYPES", null, "PWHTypes", m_dsGlobal);

				List<NameValue> tempSummaryReport = new List<NameValue> ();
				foreach (DataRow row in table.Rows) {
					NameValue nameValue = new NameValue ();
					nameValue.Name = GetCellString (row, "PWH_TYPE_NAME");
					nameValue.Value = GetCellString (row, "PWH_TYPE_IEN");
					tempSummaryReport.Add (nameValue);
				}
				summaryReport = tempSummaryReport;
			}
			return summaryReport;
		}

		private IList<Provider> porviders = null;
		public IList<Provider> GetProviders ()
		{
			if (porviders == null || porviders.Count == 0) {
				DataTable table = m_dsGlobal.Tables["Provider"];

				List<Provider> tempProviders = new List<Provider> ();
				foreach (DataRow row in table.Rows) {
					Provider newProv = container.Resolve<Factory<Provider>> ().Create ();
					newProv.Name = GetCellString (row, "NAME");
					newProv.IEN = GetCellString (row, "NEW_PERSON_IEN");
					tempProviders.Add (newProv);
				}

				tempProviders.Sort (new Comparison<Provider> ((x, y) => {
					return string.Compare (x.Name, y.Name, true);
				}));

				porviders = tempProviders;
			}
			return porviders;
		}

		public DataTable GetWaitingList (string ResourceIEN)
		{
			List<string> parms = new List<string> ();
			parms.Add (ResourceIEN);
			return adt.callOVIDRPC ("BSDX WAITLIST", parms);			
		}

		public string AddEditAccessType (SchdAccessType accessType)
		{
			List<string> parms = new List<string> ();
			string parm = (accessType.BSDX_ACCESS_TYPE_IEN == null) ? "0" : accessType.BSDX_ACCESS_TYPE_IEN;
			parm = parm + "|" + accessType.NAME + "|" + accessType.INACTIVE + "|" + accessType.DISPLAY_COLOR + "|" + accessType.RED + " | " + accessType.GREEN + " | " + accessType.BLUE + "|" + accessType.PREVENT_ACCESS;
			parms.Add (parm);
			DataTable table = adt.callOVIDRPC ("BSDX ADD/EDIT ACCESS TYPE", parms);

			string sErrorID = string.Empty;
			if (table.Rows.Count == 1) {
				DataRow r = table.Rows[0];
				if (r["ERRORTEXT"].ToString () != "-1") {
					sErrorID = r["ERRORTEXT"].ToString ();
				} else {
					sErrorID = string.Empty;
				}
			} 
			
			if (sErrorID == string.Empty) {
				//Get Access Types
				DataTable dtAccessTypes = new DataTable ();
				dtAccessTypes = adt.callOVIDRPC ("BSDX ACCESS TYPE", null, "AccessTypes", m_dsGlobal);
			}

			return sErrorID;
		}

		public string AddEditResourceUser (ResourceUser resourceUser)
		{
			List<string> parms = new List<string> ();
			string parm = (resourceUser.BSDX_RESOURCE_USER_IEN == null) ? "0" : resourceUser.BSDX_RESOURCE_USER_IEN;
			parm = parm + "|" + resourceUser.OVERBOOK + "|" + resourceUser.MODIFY_SCHEDULE + "|" + resourceUser.RESOURCE_ID + "|" + resourceUser.USER_ID + "|" + resourceUser.MODIFY_APPTS + "|" + resourceUser.MASTEROVERBOOK;
			parms.Add (parm);
			DataTable table = adt.callOVIDRPC ("BSDX ADD/EDIT RESOURCEUSER", parms);

			string sErrorID = string.Empty;
			if (table.Rows.Count == 1) {
				DataRow r = table.Rows[0];
				sErrorID = r["ERRORID"].ToString ();
				if (sErrorID == "134234182") {
					sErrorID = "Please select a clinic.";
				}
			} else {
				//Get Resource Users
				DataTable dtResourceUsers = new DataTable ();
				dtResourceUsers = adt.callOVIDRPC ("BSDX RESOURCE USER", null, "ResourceUser", m_dsGlobal);
			}

			return sErrorID;
		}

		public string AddEditAccessTypeGroup (string groupID, string groupName)
		{

			List<string> parms = new List<string> ();
			string parm = (groupID == string.Empty) ? "0" : groupID;
			parm = parm + "|" + groupName;
			parms.Add (parm);
			DataTable table = adt.callOVIDRPC ("BSDX ADD/EDIT ACCESS GROUP", parms);

			string sErrorID = string.Empty;
			if (table.Rows.Count == 1) {
				DataRow r = table.Rows[0];
				sErrorID = r["ERRORTEXT"].ToString ();
			} 

			if (sErrorID == string.Empty)
			{
				//Get Access Group
				DataTable dtAccessGroup = new DataTable ();
				dtAccessGroup = adt.callOVIDRPC ("BSDX ACCESS GROUP", null, "AccessGroups", m_dsGlobal);
			}

			return sErrorID;
		}

		public string AddEditClinicGroup (string groupID, string groupName)
		{
			List<string> parms = new List<string> ();
			string parm = (groupID == string.Empty) ? "0" : groupID;
			parm = parm + "|" + groupName;
			parms.Add (parm);
			DataTable table = adt.callOVIDRPC ("BSDX ADD/EDIT RESOURCE GROUP", parms);

			string sErrorID = string.Empty;
			if (table.Rows.Count == 1) {
				DataRow r = table.Rows[0];
				sErrorID = r["ERRORTEXT"].ToString ();
			} 

			if (sErrorID == string.Empty)
			{
				//Get Resource Groups
				List<string> parm1 = new List<string> ();
				parm1.Add (this.CurrentUser);
				DataTable dtResourceGroups = new DataTable ();
				dtResourceGroups = adt.callOVIDRPC ("BSDX RESOURCE GROUPS BY USER", parm1, "ResourceGroup", m_dsGlobal);
			}

			return sErrorID;
		}

		public string AddEditClinic (SchdResource schdResource)
		{
			List<string> parms = new List<string> ();
			string parm = (schdResource.RESOURCEID == string.Empty) ? "0" : schdResource.RESOURCEID;
			parm = parm + "|" + schdResource.Clinic.HOSPITAL_LOCATION + "|" + schdResource.INACTIVE + "|" + schdResource.Clinic.HOSPITAL_LOCATION_ID + "|" + schdResource.TIMESCALE + "|" + schdResource.LETTER_TEXT + "|" + schdResource.NO_SHOW_LETTER + "|" + schdResource.CLINIC_CANCELLATION_LETTER;
			parms.Add (parm);
			DataTable table = adt.callOVIDRPC ("BSDX ADD/EDIT RESOURCE", parms);

			string sErrorID = string.Empty;
			if (table.Rows.Count == 1) {
				DataRow r = table.Rows[0];
				if (r["RESOURCEID"].ToString () == "0") {
					sErrorID = r["ERRORTEXT"].ToString ();
				} else {
					//Get resources
					List<string> parm1 = new List<string> ();
					parm1.Add (this.CurrentUser);
					DataTable dtResources = new DataTable ();
					dtResources = adt.callOVIDRPC ("BSDX RESOURCES", parm1, "Resources", m_dsGlobal);
				}
			} else {
				sErrorID =  "Unable to add new Resource.";
			}

			return sErrorID;
		}

		public string DeleteClinic (string clinicID)
		{
			List<string> parms = new List<string> ();
			parms.Add (clinicID + "|@");
			DataTable table = adt.callOVIDRPC ("BSDX ADD/EDIT RESOURCE", parms);

			string sErrorID = string.Empty;
			if (table.Rows.Count == 1) {
				DataRow r = table.Rows[0];
				if (r["ERRORTEXT"].ToString () != string.Empty) {
					sErrorID = r["ERRORTEXT"].ToString ();
				} else {
					//Get resources
					List<string> parm1 = new List<string> ();
					parm1.Add (this.CurrentUser);
					DataTable dtResources = new DataTable ();
					dtResources = adt.callOVIDRPC ("BSDX RESOURCES", parm1, "Resources", m_dsGlobal);
				}
			} 

			return sErrorID;
		}

		public string AddNewAvailability (SchdAvailability a)
		{
			IList<SchdAvailability> resourceAvailabilityList = m_globalAvailabilities[a.RESOURCENAME];
			a.ACCESSTYPENAME = AccessNameFromID (int.Parse (a.ACCESSTYPEID));
			a.IsPreventAccess = IsPreventAccessFromID (int.Parse (a.ACCESSTYPEID));

			string sErrorID = string.Empty;
			if (!a.IsPreventAccess) {
				for (int i = 0; i < resourceAvailabilityList.Count; i++) {
					DateTime sStart = Convert.ToDateTime (resourceAvailabilityList[i].StartTime.ToString ());
					DateTime sEnd = Convert.ToDateTime (resourceAvailabilityList[i].EndTime.ToString ());
					if (TimesOverlap (Convert.ToDateTime (a.StartTime), Convert.ToDateTime (a.EndTime), sStart, sEnd)) {
						if (!IsPreventAccessFromID (int.Parse (resourceAvailabilityList[i].ACCESSTYPEID.ToString ()))) {
							return "Access blocks may not overlap."; 
						}
					}
				}
			}

			List<string> parms = new List<string> ();
			parms.Add (Convert.ToDateTime(a.StartTime).ToString ("M-d-yyyy@HH:mm"));
			parms.Add (Convert.ToDateTime (a.EndTime).ToString ("M-d-yyyy@HH:mm"));
			parms.Add (a.ACCESSTYPEID);
			parms.Add (a.RESOURCENAME);
			parms.Add (a.SLOTS.ToString());
			parms.Add (a.Note);
			DataTable table = adt.callOVIDRPC ("BSDX ADD NEW AVAILABILITY", parms);

			if (table.Rows.Count == 1) {
				DataRow r = table.Rows[0];
				if (r["ERRORID"].ToString () != "-1") {
					sErrorID = r["ERRORID"].ToString ();
				}
			}
			return sErrorID;
		}

		private string AccessNameFromID (int nAccessTypeID)
		{
			string sAccessTypeName = string.Empty;
			DataView dvTypes = new DataView (GlobalDataSet.Tables["AccessTypes"]);
			dvTypes.Sort = "BSDX_ACCESS_TYPE_IEN ASC";
			int nRow = dvTypes.Find (nAccessTypeID);
			if (nRow >= 0)
			{
				DataRowView drv = dvTypes[nRow];
				sAccessTypeName = drv["NAME"].ToString ();
			}
			return sAccessTypeName;
		}

		private bool IsPreventAccessFromID (int nAccessTypeID)
		{
			bool bIsPreventAccess = false;
			DataView dvTypes = new DataView (GlobalDataSet.Tables["AccessTypes"]);
			dvTypes.Sort = "BSDX_ACCESS_TYPE_IEN ASC";
			int nRow = dvTypes.Find (nAccessTypeID);
			if (nRow >= 0) {
				DataRowView drv = dvTypes[nRow];
				string sIsPreventAccess = drv["PREVENT_ACCESS"].ToString ();
				bIsPreventAccess = (sIsPreventAccess == "YES") ? true : false;
			}
			return bIsPreventAccess;
		}

		public List<string> AddNewAppointment (AppointmentData a, PatientInformation p, bool bMakeChartRequest)
		{
			List<string> parms = new List<string> ();
			parms.Add ((Convert.ToDateTime(a.StartTime).ConvertToExternalDateTimeFormat(true, true)));
			parms.Add ((Convert.ToDateTime (a.EndTime).ConvertToExternalDateTimeFormat (true, true)));
			parms.Add (p.IEN);
			parms.Add (a.ResourceName);
			parms.Add (a.Duration);
			parms.Add (a.Notes);
			parms.Add (a.AppointmentID);
			parms.Add (bMakeChartRequest ? "1" : "0");
			DataTable table = adt.callOVIDRPC ("BSDX ADD NEW APPOINTMENT", parms);

			List<string> returnData = new List<string> ();
			if (table.Rows.Count == 1) {
				DataRow r = table.Rows[0];
				returnData.Add(r["APPOINTMENTID"].ToString());
				returnData.Add(r["ERRORID"].ToString ());
			}
			return returnData;
		}

		public string CheckSlotsAvailable (string startTime, string endTime, string resourceName)
		{
			int m_nSlots = 0;
			string sErrorID = string.Empty;
			string sAccessType = "";
			string sAvailabilityMessage = "";

			//Check available time slots
			m_nSlots = SlotsAvailable (Convert.ToDateTime (startTime), Convert.ToDateTime (endTime), resourceName, out sAccessType, out sAvailabilityMessage);

			if (m_nSlots < 1) {
				if (sAvailabilityMessage == "IsPreventAccess") {
					sErrorID = "IsPreventAccess";
				} else {
					sErrorID = "There are no slots available at the selected time.  Do you want to overbook this appointment?";
				}
			}
			return sErrorID;
		}

		public string CreateNewAppointment (AppointmentData a, PatientInformation p, bool bMakeChartRequest)
		{
			string sErrorID = string.Empty;
			return sErrorID;
		}

		public DataTable CreateAssignedTypeSchedule (string sResourceName, DateTime StartTime, DateTime EndTime)
		{
			List<string> parms = new List<string> ();
			parms.Add (StartTime.ToString ("M-d-yyyy"));
			parms.Add (EndTime.ToString ("M-d-yyyy"));
			parms.Add (sResourceName);
			DataTable rs = adt.callOVIDRPC ("BSDX TYPE BLOCKS OVERLAP", parms);

			if (rs.Rows.Count == 0)
				return rs;

			DataTable rsCopy = new DataTable ("rsCopy");
			DataColumn dCol = new DataColumn ();
			dCol.DataType = Type.GetType ("System.DateTime");
			dCol.ColumnName = "StartTime";
			dCol.ReadOnly = true;
			dCol.AllowDBNull = false;
			dCol.Unique = false;
			rsCopy.Columns.Add (dCol);

			dCol = new DataColumn ();
			dCol.DataType = Type.GetType ("System.DateTime");
			dCol.ColumnName = "EndTime";
			dCol.ReadOnly = true;
			dCol.AllowDBNull = false;
			dCol.Unique = false;
			rsCopy.Columns.Add (dCol);

			dCol = new DataColumn ();
			dCol.DataType = Type.GetType ("System.Int16");
			dCol.ColumnName = "AppointmentTypeID";
			dCol.ReadOnly = true;
			dCol.AllowDBNull = false;
			dCol.Unique = false;
			rsCopy.Columns.Add (dCol);

			dCol = new DataColumn ();
			dCol.DataType = Type.GetType ("System.Int32");
			dCol.ColumnName = "AvailabilityID";
			dCol.ReadOnly = true;
			dCol.AllowDBNull = false;
			dCol.Unique = false;
			rsCopy.Columns.Add (dCol);

			dCol = new DataColumn ();
			dCol.DataType = Type.GetType ("System.String");
			dCol.ColumnName = "ResourceName";
			dCol.ReadOnly = true;
			dCol.AllowDBNull = false;
			dCol.Unique = false;
			rsCopy.Columns.Add (dCol);

			DateTime dLastEnd;
			DateTime dStart;
			DateTime dEnd;
			DataRow rNew;

			rNew = rs.Rows[rs.Rows.Count - 1];
			dLastEnd = Convert.ToDateTime (rNew["EndTime"].ToString ());
			rNew = rs.Rows[0];
			dStart = Convert.ToDateTime (rNew["StartTime"].ToString ());

			long UNSPECIFIED_TYPE = 10;
			long UNSPECIFIED_ID = 0;

			//if first block in vgetrows starts later than StartTime,
			// then pad with a new block

			if (dStart > StartTime) {
				dEnd = dStart;
				rNew = rsCopy.NewRow ();
				rNew["StartTime"] = StartTime;
				rNew["EndTime"] = dEnd;
				rNew["AppointmentTypeID"] = UNSPECIFIED_TYPE;
				rNew["AvailabilityID"] = UNSPECIFIED_ID;
				rNew["ResourceName"] = sResourceName;
				rsCopy.Rows.Add (rNew);
			}

			//if first block start time is < StartTime then trim
			if (dStart < StartTime) {
				rNew = rs.Rows[0];
				rNew["StartTime"] = StartTime;
				dStart = StartTime;
			}

			int nAppointmentTypeID;
			int nAvailabilityID;

			dEnd = dStart;
			foreach (DataRow rEach in rs.Rows) {
				dStart = Convert.ToDateTime (rEach["StartTime"].ToString ());
				if (dStart > dEnd) {
					rNew = rsCopy.NewRow ();
					rNew["StartTime"] = dEnd;
					rNew["EndTime"] = dStart;
					rNew["AppointmentTypeID"] = 0;
					rNew["AvailabilityID"] = UNSPECIFIED_ID;
					rNew["ResourceName"] = sResourceName;
					rsCopy.Rows.Add (rNew);
				}

				dEnd = Convert.ToDateTime (rEach["EndTime"].ToString ());

				if (dEnd > EndTime)
					dEnd = EndTime;
				nAppointmentTypeID = int.Parse (rEach["AppointmentTypeID"].ToString ());
				nAvailabilityID = int.Parse (rEach["AvailabilityID"].ToString ());

				rNew = rsCopy.NewRow ();
				rNew["StartTime"] = dStart;
				rNew["EndTime"] = dEnd;
				rNew["AppointmentTypeID"] = nAppointmentTypeID;
				rNew["AvailabilityID"] = nAvailabilityID;
				rNew["ResourceName"] = sResourceName;
				rsCopy.Rows.Add (rNew);
			}

			//Pad the end if necessary
			if (dLastEnd < EndTime) {
				rNew = rsCopy.NewRow ();
				rNew["StartTime"] = dLastEnd;
				rNew["EndTime"] = EndTime;
				rNew["AppointmentTypeID"] = UNSPECIFIED_TYPE;
				rNew["AvailabilityID"] = UNSPECIFIED_ID;
				rNew["ResourceName"] = sResourceName;
				rsCopy.Rows.Add (rNew);
			}
			return rsCopy;
		}

		public void RefreshAvailabilitySchedule (string m_sResourcesName, DateTime m_dStartDate, DateTime m_dEndDate)
		{
			m_pAvArray.Clear ();

			ArrayList saryApptTypes = new ArrayList ();
			int nApptTypeID = 0;

			//Refresh Availability schedules
			DataTable rAvailabilitySchedule;
			rAvailabilitySchedule = CreateAvailabilitySchedule (m_sResourcesName, m_dStartDate, m_dEndDate, "0");
			
			//Refresh Type Schedule
			DataTable rTypeSchedule = new DataTable ();
			rTypeSchedule = CreateAssignedTypeSchedule (m_sResourcesName, m_dStartDate, m_dEndDate);
			
			DateTime dStart;
			DateTime dEnd;
			DateTime dTypeStart;
			DateTime dTypeEnd;
			int nSlots;
			string sResourceList;
			string sAccessRuleList;

			foreach (DataRow rTemp in rAvailabilitySchedule.Rows) {
				//get StartTime, EndTime and Slots 
				dStart = Convert.ToDateTime (rTemp["START_TIME"].ToString ());
				dEnd = Convert.ToDateTime (rTemp["END_TIME"].ToString ());
				string sSlots = rTemp["SLOTS"].ToString ();
				nSlots = Convert.ToInt16 (sSlots);

				sResourceList = rTemp["RESOURCE"].ToString ();
				sAccessRuleList = rTemp["ACCESS_TYPE"].ToString ();

				string sPreventAccessTypeID = "";
				if (sAccessRuleList.Contains ("^")) {
					sPreventAccessTypeID = sAccessRuleList.Substring (0, 1);
				}

				string sNote = rTemp["NOTE"].ToString ();

				if ((nSlots < -1000) || (sAccessRuleList == "")) {
					nApptTypeID = 0;
				} else {
					foreach (DataRow rType in rTypeSchedule.Rows) {

						dTypeStart = Convert.ToDateTime (rType["StartTime"].ToString ());
						dTypeEnd = Convert.ToDateTime (rType["EndTime"].ToString ());
						//if start & end times overlap, then
						string sTypeResource = rType["ResourceName"].ToString ();
						if ((dTypeStart.DayOfYear == dStart.DayOfYear) && (sResourceList == sTypeResource)) {
							if (TimesOverlap (dStart, dEnd, dTypeStart, dTypeEnd)) {
								string sTemp = "";
								if (sPreventAccessTypeID != "") {
									sTemp = sPreventAccessTypeID;
								} else {
									sTemp = rType["AppointmentTypeID"].ToString ();
								}
								nApptTypeID = Convert.ToInt16 (sTemp);
								break;
							}
						}
					}//end foreach datarow rType
				}
				AddAvailability (dStart, dEnd, nApptTypeID, nSlots, false, sResourceList, sAccessRuleList, sNote);
			}//end foreach datarow rTemp
		}

		public int AddAvailability (DateTime StartTime, DateTime EndTime, int nType, int nSlots, bool UpdateView, string sResourceList, string sAccessRuleList, string sNote)
		{
			//adds it to the object array
			//Returns the index in the array

			SchdAvailability pNewAv = new SchdAvailability ();
			pNewAv.Create (StartTime.ToString(), EndTime.ToString(), nType.ToString(), nSlots, sResourceList, sAccessRuleList);
			pNewAv.Note = sNote;

			//Look up the color and type name using the AppointmentTypes datatable
			DataTable dtType = m_dsGlobal.Tables["AccessTypes"];
			DataRow dRow = dtType.Rows.Find (nType.ToString ());
			if (dRow != null) {
				string sColor = dRow["DISPLAY_COLOR"].ToString ();
				pNewAv.DisplayColor = sColor;
				string sTemp = dRow["RED"].ToString ();
				sTemp = (sTemp == "") ? "0" : sTemp;
				int nRed = Convert.ToInt16 (sTemp);
				pNewAv.Red = nRed;
				sTemp = dRow["GREEN"].ToString ();
				sTemp = (sTemp == "") ? "0" : sTemp;
				int nGreen = Convert.ToInt16 (sTemp);
				pNewAv.Green = nGreen;
				sTemp = dRow["BLUE"].ToString ();
				sTemp = (sTemp == "") ? "0" : sTemp;
				int nBlue = Convert.ToInt16 (sTemp);
				pNewAv.Blue = nBlue;

				string sName = dRow["NAME"].ToString ();
				pNewAv.ACCESSTYPENAME = sName;

				bool bIsPreventAccess = dRow["PREVENT_ACCESS"].ToString () == "YES" ? true : false;
				pNewAv.IsPreventAccess = bIsPreventAccess;
			}

			int nIndex = 0;
			nIndex = m_pAvArray.Add (pNewAv);
			if (UpdateView == true) {
				//this.UpdateAllViews ();
			}
			return nIndex;
		}

		private ArrayList m_pAvArray = new ArrayList();
		public ArrayList AvailabilityArray
		{
			get
			{
				return this.m_pAvArray;
			}
		}

		public int SlotsAvailable (DateTime dSelStart, DateTime dSelEnd, string sResource, out string sAccessType, out string sAvailabilityMessage)
		{
			sAccessType = "";
			sAvailabilityMessage = "";
			DateTime dStart;
			DateTime dEnd;
			int nAvailableSlots = 999;
			int nSlots = 0;
			int i = 0;
			SchdAvailability pAv;
			//loop thru m_pAvArray
			//Compare the start time and end time of eachblock
			while (i < m_pAvArray.Count) {
				pAv = (SchdAvailability)m_pAvArray[i];
				dStart = DateTime.Parse(pAv.StartTime);
				dEnd = DateTime.Parse(pAv.EndTime);
				if ((sResource == pAv.RESOURCENAME) &&
					((dSelStart.Date == dStart.Date) || (dSelStart.Date == dEnd.Date))) {
					if (TimesOverlap (dStart, dEnd, dSelStart, dSelEnd)) {
						if (pAv.IsPreventAccess) {
							nAvailableSlots = -1;
							i = m_pAvArray.Count;
							sAvailabilityMessage = "IsPreventAccess";
						} else {
							nSlots = pAv.SLOTS;
							if (nSlots < 1) {
								nAvailableSlots = 0;
								break;
							}
							if (nSlots < nAvailableSlots) {
								nAvailableSlots = nSlots;
								sAccessType = pAv.ACCESSTYPENAME;
								sAvailabilityMessage = pAv.Note;
							}
						}
					}
				}
				i++;
			}
			if (nAvailableSlots == 999) {
				nAvailableSlots = 0;
			}
			return nAvailableSlots;
		}		

		public string EditAppointment (AppointmentData a)
		{
			List<string> parms = new List<string> ();
			parms.Add (a.AppointmentID);
			parms.Add (a.Notes);
			parms.Add (a.Duration);
			DataTable table = adt.callOVIDRPC ("BSDX EDIT APPOINTMENT", parms);

			string sErrorID = string.Empty;
			if (table.Rows.Count == 1) {
				DataRow r = table.Rows[0];
				if (r["ERRORID"].ToString () != "-1") {
					sErrorID = r["ERRORID"].ToString ();
				}
			}

			return sErrorID;
		}

		public IList<SchdAppointment> GetAppointments (string resourceName, string startDate, string endDate)
		{
			List<string> parms = new List<string> ();
			parms.Add (resourceName);
			parms.Add (startDate);
			parms.Add (endDate);
			DataTable table = adt.callOVIDRPC ("BSDX CREATE APPT SCHEDULE", parms);

			IList<SchdAppointment> AppointmentList = new List<SchdAppointment> ();
			foreach (DataRow row in table.Rows) {
				SchdAppointment newAppointment = container.Resolve<SchdAppointment> ();

				newAppointment.ACCESSTYPEID = row["ACCESSTYPEID"].ToString ();
				newAppointment.APPOINTMENTID = row["APPOINTMENTID"].ToString ();
				newAppointment.AUXTIME = row["AUXTIME"].ToString ();
				newAppointment.CANCELLED = row["CANCELLED"].ToString ();
				newAppointment.CHECKIN = row["CHECKIN"].ToString ();
				newAppointment.CHECKOUT = row["CHECKOUT"].ToString ();
				newAppointment.END_TIME = row["END_TIME"].ToString ();
				newAppointment.HRN = row["HRN"].ToString ();
				newAppointment.NOSHOW = row["NOSHOW"].ToString ();
				newAppointment.NOTE = row["NOTE"].ToString ();
				newAppointment.PATIENTID = row["PATIENTID"].ToString ();
				newAppointment.PATIENTNAME = row["PATIENTNAME"].ToString ();
				newAppointment.RESOURCENAME = row["RESOURCENAME"].ToString ();
				newAppointment.START_TIME = row["START_TIME"].ToString ();
				newAppointment.VPROVIDER = row["VPROVIDER"].ToString ();
				newAppointment.WALKIN = row["WALKIN"].ToString ();

				AppointmentList.Add (newAppointment);
			}
			return AppointmentList;
		}

		public IList<SchdAvailability> GetAvailabilities (string resourceName, string startDate, string endDate)
		{
			IList<SchdAvailability> AvailabilityList = new List<SchdAvailability> ();

			string dStart;
			string dEnd;
			int nKeyID;
			string sNote;
			string sResource;
			string sAccessType;
			string sSlots;
			SchdAvailability pAppointment;
			DataTable rAppointmentSchedule;

			ArrayList apptTypeIDs = new ArrayList ();

			rAppointmentSchedule = CreateAssignedSlotSchedule (resourceName, startDate, endDate, "", "0");
			foreach (DataRow r in rAppointmentSchedule.Rows) {
				nKeyID = Convert.ToInt32 (r["AVAILABILITYID"].ToString ());
				if (nKeyID > 0) {
					dStart = r["START_TIME"].ToString ();
					dEnd = r["END_TIME"].ToString ();
					sNote = r["NOTE"].ToString ();
					sResource = r["RESOURCE"].ToString ();
					sAccessType = r["ACCESS_TYPE"].ToString ();
					sSlots = r["SLOTS"].ToString ();

					pAppointment = new SchdAvailability ();
					pAppointment.Create (dStart, dEnd, sAccessType, int.Parse(sSlots), sResource);
					pAppointment.Note = sNote;
					SchdAccessType tAccess = GetAccessTypeByID (sAccessType);
					pAppointment.ACCESSTYPENAME = tAccess.NAME;
					pAppointment.Red = int.Parse (tAccess.RED);
					pAppointment.Green = int.Parse (tAccess.GREEN);
					pAppointment.Blue = int.Parse (tAccess.BLUE);
					pAppointment.IsPreventAccess = (tAccess.PREVENT_ACCESS == "YES") ? true : false;

					pAppointment.APPOINTMENTID = r["AVAILABILITYID"].ToString ();
					AvailabilityList.Add (pAppointment);
				}
			}
			if (!m_globalAvailabilities.ContainsKey (resourceName)) {
				m_globalAvailabilities.Add (resourceName, AvailabilityList);
			} else {
				m_globalAvailabilities[resourceName] = AvailabilityList;
			}
			return AvailabilityList;
		}

		private int GetTotalMinutes (DateTime dDate)
		{
			return ((dDate.Hour * 60) + dDate.Minute);
		}

		public PatientInformation GetPatientInfo (string patientIEN)
		{
			PatientInformation newPatient = new PatientInformation ();
			List<string> parms = new List<string> ();
			parms.Add (patientIEN);
			DataTable table = adt.callOVIDRPC ("BSDX GET BASIC REG INFO", parms);
			if (table.Rows.Count > 0) {
				DataRow row = table.Rows[0];
				newPatient.IEN = row["IEN"].ToString ();
				newPatient.Name = row["Name"].ToString ();
				newPatient.HRN = row["HRN"].ToString ();
				newPatient.SSN = row["SSN"].ToString ();
				newPatient.DOB = row["DOB"].ToString ();
				newPatient.Street = row["STREET"].ToString ();
				newPatient.City = row["CITY"].ToString ();
				newPatient.OfficePhone = row["OFCPHONE"].ToString ();
				newPatient.State = row["STATE"].ToString ();
				newPatient.Zip = row["ZIP"].ToString ();
				newPatient.HomePhone = row["HOMEPHONE"].ToString ();
			}
			return newPatient;
		}

		public IList<Patient> GetPatients (string searchString, int maxRows)
		{
			if (searchString == null)
				return emptyList;

			List<string> parms = new List<string> ();
			parms.Add (searchString);
			parms.Add (maxRows.ToString ());
			DataTable table = adt.callOVIDRPC ("BSDXPatientLookupRS", parms);

			List<Patient> patientList = new List<Patient> ();
			foreach (DataRow row in table.Rows) {
				Patient newPatient = container.Resolve<Factory<Patient>>().Create ();

				newPatient.IEN = row["IEN"].ToString ();
				newPatient.Name = row["Name"].ToString ();
				newPatient.HRN = row["HRN"].ToString ();
				newPatient.SSN = row["SSN"].ToString ();
				newPatient.DOB = row["DOB"].ToString ();

				patientList.Add (newPatient);
			}
			return patientList;
		}

		private IList<RPMSClinic> rpmsClinics = null;
		public IList<RPMSClinic> GetRPMSClinics ()
		{
			DataTable clinicTable = m_dsGlobal.Tables["ClinicSetupParameters"];
			DataTable hospitalLocationTable = m_dsGlobal.Tables["HospitalLocation"];
			//Build Primary Key for HospitalLocation table
			DataColumn[] hospital_dc = new DataColumn[1];
			hospital_dc[0] = hospitalLocationTable.Columns["HOSPITAL_LOCATION_ID"];
			hospitalLocationTable.PrimaryKey = hospital_dc;


			List<RPMSClinic> tempRPMSClinics = new List<RPMSClinic> ();
			foreach (DataRow row in clinicTable.Rows) {
				RPMSClinic p = new RPMSClinic ();
				p.HOSPITAL_LOCATION = GetCellString (row, "HOSPITAL_LOCATION");
				p.HOSPITAL_LOCATION_ID = GetCellString (row, "HOSPITAL_LOCATION_ID");
				p.CREATEVISIT = GetCellString (row, "CREATE_VISIT");
				p.OVERBOOK = GetCellString (row, "MAX_OVERBOOKS");
				p.VISITSERVICE = GetCellString (row, "VISIT_SERVICE_CATEGORY");

				DataRow hospitalRow = hospitalLocationTable.Rows.Find (p.HOSPITAL_LOCATION_ID);

				if (hospitalRow == null) {
					continue;
				}
				p.PROVIDER = GetCellString (hospitalRow, "DEFAULT_PROVIDER");
				p.CODE = GetCellString (hospitalRow, "STOP_CODE_NUMBER");
				p.INACTIVE = GetCellString (hospitalRow, "INACTIVATE_DATE");
				p.REACTIVE = GetCellString (hospitalRow, "REACTIVATE_DATE");
				tempRPMSClinics.Add (p);
			}
			rpmsClinics = tempRPMSClinics;
			return rpmsClinics;
		}

		public RPMSClinic GetClinicByID (string clinicIEN)
		{
			DataTable clinicTable = m_dsGlobal.Tables["ClinicSetupParameters"];
			//Build Primary Key for ClinicSetupParameters table
			DataColumn[] parameter_dc = new DataColumn[1];
			parameter_dc[0] = clinicTable.Columns["HOSPITAL_LOCATION_ID"];
			clinicTable.PrimaryKey = parameter_dc;

			DataTable hospitalLocationTable = m_dsGlobal.Tables["HospitalLocation"];
			//Build Primary Key for HospitalLocation table
			DataColumn[] hospital_dc = new DataColumn[1];
			hospital_dc[0] = hospitalLocationTable.Columns["HOSPITAL_LOCATION_ID"];
			hospitalLocationTable.PrimaryKey = hospital_dc;

			RPMSClinic p = new RPMSClinic ();
			DataRow parameterRow = clinicTable.Rows.Find (clinicIEN);
			p.HOSPITAL_LOCATION = GetCellString (parameterRow, "HOSPITAL_LOCATION");
			p.HOSPITAL_LOCATION_ID = GetCellString (parameterRow, "HOSPITAL_LOCATION_ID");
			p.CREATEVISIT = GetCellString (parameterRow, "CREATE_VISIT");
			p.OVERBOOK = GetCellString (parameterRow, "MAX_OVERBOOKS");
			p.VISITSERVICE = GetCellString (parameterRow, "VISIT_SERVICE_CATEGORY");

			DataRow hospitalRow = hospitalLocationTable.Rows.Find (p.HOSPITAL_LOCATION_ID);
			p.PROVIDER = GetCellString (hospitalRow, "DEFAULT_PROVIDER");
			p.CODE = GetCellString (hospitalRow, "STOP_CODE_NUMBER");
			p.INACTIVE = GetCellString (hospitalRow, "INACTIVATE_DATE");
			p.REACTIVE = GetCellString (hospitalRow, "REACTIVATE_DATE");

			return p;
		}

		public RPMSClinic GetRPMSClinic (string resourceID)
		{
			if (string.IsNullOrEmpty (resourceID)) {
				return null;
			}
			RPMSClinic RPMSClinic1 = new RPMSClinic ();

			//Get hospital location id
			List<string> resource_parms = new List<string> ();
			resource_parms.Add (this.CurrentUser);
			DataTable resourcesTable = new DataTable ("Resource");
			resourcesTable = adt.callOVIDRPC ("BSDX RESOURCES", resource_parms);

			//Build Primary Key for Resources table
			DataColumn[] resource_dc = new DataColumn[1];
			resource_dc[0] = resourcesTable.Columns["RESOURCEID"];
			resourcesTable.PrimaryKey = resource_dc;

			DataRow resourceRow = resourcesTable.Rows.Find (resourceID);
			string hospital_location_id = resourceRow["HOSPITAL_LOCATION_ID"].ToString ();

			if (hospital_location_id != "") {
				//Get hospital locations
				List<string> hospital_parms = new List<string> ();
				hospital_parms = null;
				DataTable hospitalTable = new DataTable ("HospitalLocation");
				hospitalTable = adt.callOVIDRPC ("BSDX HOSPITAL LOCATION", hospital_parms);

				//Build Primary Key for HospitalLocation table
				DataColumn[] hospital_dc = new DataColumn[1];
				hospital_dc[0] = hospitalTable.Columns["HOSPITAL_LOCATION_ID"];
				hospitalTable.PrimaryKey = hospital_dc;

				DataRow hospitalRow = hospitalTable.Rows.Find (hospital_location_id);

				//get into from hospital table
				RPMSClinic1.PROVIDER = hospitalRow["DEFAULT_PROVIDER"].ToString ();
				RPMSClinic1.CODE = hospitalRow["STOP_CODE_NUMBER"].ToString ();
				RPMSClinic1.INACTIVE = hospitalRow["INACTIVATE_DATE"].ToString ();
				RPMSClinic1.REACTIVE = hospitalRow["REACTIVATE_DATE"].ToString ();

				//Get Clinic Setup parameters
				List<string> clinic_parms = new List<string> ();
				clinic_parms = null;
				DataTable clinicTable = new DataTable ("ClinicSetupParameters");
				clinicTable = adt.callOVIDRPC ("BSDX CLINIC SETUP", clinic_parms);

				//Build Primary Key for Clinic Setup parameters table
				DataColumn[] clinic_dc = new DataColumn[1];
				clinic_dc[0] = clinicTable.Columns["HOSPITAL_LOCATION_ID"];
				clinicTable.PrimaryKey = clinic_dc;

				DataRow clinicRow = clinicTable.Rows.Find (hospital_location_id);

				//get into from clinic Setup parameters table
				RPMSClinic1.HOSPITAL_LOCATION = clinicRow["HOSPITAL_LOCATION"].ToString ();
				RPMSClinic1.HOSPITAL_LOCATION_ID = clinicRow["HOSPITAL_LOCATION_ID"].ToString ();
				RPMSClinic1.OVERBOOK = clinicRow["MAX_OVERBOOKS"].ToString ();
				RPMSClinic1.CREATEVISIT = clinicRow["CREATE_VISIT"].ToString ();
				RPMSClinic1.VISITSERVICE = clinicRow["VISIT_SERVICE_CATEGORY"].ToString ();

			}
			return RPMSClinic1; 
		}

		public IList<SchdResource> GetResources (string searchString, bool includeInactive)
		{
			if (searchString == null)
				return resource_emptyList;

			DataTable table = m_dsGlobal.Tables["Resources"];

			List<SchdResource> ResourceList = new List<SchdResource> ();
			foreach (DataRow row in table.Rows) {
				SchdResource newResource = container.Resolve<Factory<SchdResource>> ().Create ();		
				string resouceName = row["RESOURCE_NAME"].ToString ();
				bool isInactive = row["INACTIVE"].ToString () == "1" ? true : false;

				if ((resouceName.Length >= searchString.Length) && (resouceName.Substring (0, searchString.Length).ToUpper() == searchString.ToUpper()) && (!isInactive || includeInactive)) {
					newResource.RESOURCEID = row["RESOURCEID"].ToString ();
					newResource.RESOURCE_NAME = row["RESOURCE_NAME"].ToString ();
					newResource.CLINIC_CANCELLATION_LETTER = row["CLINIC_CANCELLATION_LETTER"].ToString ();
					newResource.OVERBOOK = row["OVERBOOK"].ToString ();
					newResource.INACTIVE = row["INACTIVE"].ToString ();
					newResource.HOSPITAL_LOCATION_ID = row["HOSPITAL_LOCATION_ID"].ToString ();
					newResource.LETTER_TEXT = row["LETTER_TEXT"].ToString ();
					newResource.MODIFY_APPOINTMENTS = row["MODIFY_APPOINTMENTS"].ToString ();
					newResource.MODIFY_SCHEDULE = row["MODIFY_SCHEDULE"].ToString ();
					newResource.NO_SHOW_LETTER = row["NO_SHOW_LETTER"].ToString ();
					newResource.TIMESCALE = row["TIMESCALE"].ToString ();
					newResource.VIEW = row["VIEW"].ToString ();

					ResourceList.Add (newResource);
				}
			}
			ResourceList.Sort (new Comparison<SchdResource> ((x, y) => {
				return string.Compare (x.RESOURCE_NAME, y.RESOURCE_NAME, true);
			}));
			return ResourceList;
		}

		public bool GetVariableAppointmentsFlag (string RESOURCEID)
		{
			List<string> parms = new List<string> ();
			parms.Add (RESOURCEID);
			DataTable varTable = new DataTable ("VariableLength");
			varTable = adt.callOVIDRPC ("BSDX CLINIC VAR APPT", parms);
			if (varTable.Rows[0][0].ToString () == "1") {
				return true;
			} else {
				return false;
			}
		}

		public SchdResource GetResource (string resourceID)
		{
			List<string> parms = new List<string> ();
			parms.Add (this.CurrentUser);
			DataTable table = new DataTable ("Resources");
			table = adt.callOVIDRPC ("BSDX RESOURCES", parms);

			//Build Primary Key for Resources table
			DataColumn[] resource_dc = new DataColumn[1];
			resource_dc[0] = table.Columns["RESOURCEID"];
			table.PrimaryKey = resource_dc;

			DataRow row = table.Rows.Find (resourceID);
			SchdResource newResource = container.Resolve<Factory<SchdResource>> ().Create ();

			if (row != null) {
				newResource.RESOURCEID = row["RESOURCEID"].ToString ();
				newResource.RESOURCE_NAME = row["RESOURCE_NAME"].ToString ();
				newResource.CLINIC_CANCELLATION_LETTER = row["CLINIC_CANCELLATION_LETTER"].ToString ();
				newResource.OVERBOOK = row["OVERBOOK"].ToString ();
				newResource.INACTIVE = row["INACTIVE"].ToString ();
				newResource.HOSPITAL_LOCATION_ID = row["HOSPITAL_LOCATION_ID"].ToString ();
				newResource.LETTER_TEXT = row["LETTER_TEXT"].ToString ();
				newResource.MODIFY_APPOINTMENTS = row["MODIFY_APPOINTMENTS"].ToString ();
				newResource.MODIFY_SCHEDULE = row["MODIFY_SCHEDULE"].ToString ();
				newResource.NO_SHOW_LETTER = row["NO_SHOW_LETTER"].ToString ();
				newResource.TIMESCALE = row["TIMESCALE"].ToString ();
				newResource.VIEW = row["VIEW"].ToString ();
			}

			return newResource;
		}

		public IList<SchdResource> GetClinicsByProvider (string providerIEN)
		{
			if (providerIEN == null)
				return resource_emptyList;

			List<string> parms = new List<string> ();
			parms.Add (providerIEN);
			DataTable table = new DataTable ("Resources");
			table = adt.callOVIDRPC ("BSDX PROVIDER CLINICS", parms);

			List<SchdResource> ResourceList = new List<SchdResource> ();
			if (table.Columns.Count > 1) {
				foreach (DataRow row in table.Rows) {
					IList<SchdResource> newResourceList = GetResources (row["CLINIC_NAME"].ToString (), false);
					foreach (SchdResource item in newResourceList) {
						if (item.HOSPITAL_LOCATION_ID == row["CLINIC_IEN"].ToString ()) {
							ResourceList.Add (item);
						}
					}
				}
			}

			return ResourceList;
		}

		public IList<Provider> GetProvidersByClinic (string clinicIEN)
		{
			List<Provider> ProviderList = new List<Provider> ();
			if (clinicIEN == null)
				return ProviderList;

			List<string> parms = new List<string> ();
			parms.Add (clinicIEN);
			DataTable table = new DataTable ("Providers");
			table = adt.callOVIDRPC ("BSDX CLINIC PROVIDERS", parms);

			foreach (DataRow row in table.Rows) {
				Provider newProvider = container.Resolve<Factory<Provider>> ().Create ();
				newProvider.IEN = row["PROVIDER_IEN"].ToString ();
				newProvider.Name = row["PROVIDER_NAME"].ToString ();

				ProviderList.Add (newProvider);
			}

			return ProviderList;
		}

		public IList<SchdResourceGroup> GetResourceGroups ()
		{
			List<SchdResourceGroup> ResourceGroupList = new List<SchdResourceGroup> ();
			List<string> parms = new List<string> ();
			parms.Add (this.CurrentUser);
			DataTable table = m_dsGlobal.Tables["GroupedResource"];
			foreach (DataRow row in table.Rows) {
				SchdResourceGroup newResourceGroup = container.Resolve<Factory<SchdResourceGroup>> ().Create ();
				bool ResourceGroupAlreadyAdded = false;
				foreach (SchdResourceGroup item in ResourceGroupList) {
					if (item.IEN == row["RESOURCE_GROUPID"].ToString ()) {
						newResourceGroup = item;
						ResourceGroupAlreadyAdded = true;
						break;
					}
				}
				if (!ResourceGroupAlreadyAdded) {
					newResourceGroup.Name = row["RESOURCE_GROUP"].ToString ();
					newResourceGroup.IEN = row["RESOURCE_GROUPID"].ToString ();
					ResourceGroupList.Add (newResourceGroup);
				}

				newResourceGroup.Resources.Add (GetResource (row["RESOURCEID"].ToString ()));
			}
			return ResourceGroupList;
		}

		public int GetSelectedType (string sResourceName, DateTime startDate, DateTime endDate)
		{
			int nAccessTypeID = 0;
			DateTime dStart;
			DateTime dEnd;
			DataTable rsOut = new DataTable ();

			rsOut = GetAvailabilitySchedule (sResourceName, startDate, endDate);
			if (rsOut.Rows.Count > 0) {
				for (int i = 0; i < rsOut.Rows.Count; i++) {
					DataRow r = rsOut.Rows[i];
					dStart = DateTime.Parse (r["START_TIME"].ToString());
					dEnd = DateTime.Parse (r["END_TIME"].ToString ());

					if (TimesOverlap (dStart, dEnd, startDate, endDate)) {
						nAccessTypeID = int.Parse (r["ACCESS_TYPE"].ToString());
						break;
					}
				}
			}
			return nAccessTypeID;
		}

		public DataTable GetAvailabilitySchedule (string ResourceNames, DateTime StartTime, DateTime EndTime)
		{
			DataTable rsOut;

			rsOut = CreateAssignedSlotSchedule (ResourceNames, StartTime.ToString (), EndTime.ToString (), "", "");

			return rsOut;
		}		

		public string AutoRebook (AutoRebookData autoRebookData)
		{
			SchdAppointment aRebook = new SchdAppointment ();
			aRebook = autoRebookData.RebookAppointment;

			int nSlots = 0;
			string sSlots = "";
			int nAccessTypeID;
			int autoRebookAccessID;
			//get accessTypeID
			autoRebookAccessID = GetSelectedType (aRebook.RESOURCENAME, Convert.ToDateTime (aRebook.START_TIME.ToString ()), Convert.ToDateTime (aRebook.END_TIME.ToString ()));

			DateTime dResult = new DateTime (); //StartTime of access block to autorebook into
			string sSearchInfo = "";

			//Find the StartTime of first availability block of this type for this clinic
			//between nMinimumDays and nMaximumDays
			string sAVStart = Convert.ToDateTime (autoRebookData.RebookAppointment.START_TIME).AddDays (autoRebookData.MinimumDays).ToString ("M/d/yyyy@H:mm");
			DateTime dtAVEnd = Convert.ToDateTime (autoRebookData.RebookAppointment.START_TIME).AddDays (autoRebookData.MinimumDays + autoRebookData.MaximumDays);
			string sAVEnd = dtAVEnd.ToString ("M/d/yyyy@H:mm");

			//Increment start day to search a week (or so) at a time
			//30 is a test increment.  Need to test different values for performance
			//int nIncrement = (autoRebookData.MaximumDays < 30) ? autoRebookData.MaximumDays : 30;
			int nIncrement = autoRebookData.MaximumDays;

			//nCount and nCountEnd are the 'moving' counters 
			//that I add to start and end to get the bracket
			//At the beginning of the DO loop, nCount and nCountEnd are already set
			bool bFinished = false;
			bool bFound = false;

			DateTime dStart = Convert.ToDateTime (autoRebookData.RebookAppointment.START_TIME).AddDays (autoRebookData.MinimumDays);
			DateTime dEnd = dStart.AddDays (nIncrement);

			do {
				List<string> parms = new List<string> ();
				parms.Add (dStart.ToString ("M/d/yyyy@H:mm"));
				parms.Add (autoRebookData.RebookAppointment.RESOURCENAME);
				parms.Add (autoRebookAccessID.ToString ());
				DataTable table = adt.callOVIDRPC ("BSDX REBOOK NEXT BLOCK", parms);

				if (table.Rows.Count > 0) {
					DataRow drNextBlockRow = table.Rows[0];
					object oNextBlock;
					oNextBlock = drNextBlockRow["NEXTBLOCK"];
					if (oNextBlock.ToString () == string.Empty)
						break;
					DateTime dNextBlock = Convert.ToDateTime (drNextBlockRow["NEXTBLOCK"].ToString ());
					if (dNextBlock > dtAVEnd) {
						break;
					}

					dStart = dNextBlock;
					dEnd = dStart.AddDays (nIncrement);
					if (dEnd > dtAVEnd)
						dEnd = dtAVEnd;
				}

				DataTable dtResult = CreateAvailabilitySchedule (autoRebookData.RebookAppointment.RESOURCENAME, dStart, dEnd, sSearchInfo);

				//Loop thru dtResult looking for a slot having the required availability type.
				//If found, set bFinished = true;	
				foreach (DataRow dr in dtResult.Rows) {

					sSlots = dr["SLOTS"].ToString ();
					if (sSlots == "")
						sSlots = "0";
					nSlots = Convert.ToInt16 (sSlots);
					if (nSlots > 0) {
						nAccessTypeID = 0;  //holds the access type id of the availability block
						if (dr["ACCESS_TYPE"].ToString () != "")
							nAccessTypeID = Convert.ToInt16 (dr["ACCESS_TYPE"].ToString ());
						if ((!autoRebookData.IsAccessTypeChecked) && (nAccessTypeID > 0)&& !IsPreventAccessFromID(nAccessTypeID)) //Match on any non-zero type
						{
							bFinished = true;
							bFound = true;
							dResult = (DateTime)dr["START_TIME"];
							break;
						}
						if (nAccessTypeID == autoRebookAccessID && !IsPreventAccessFromID(nAccessTypeID)) {
							bFinished = true;
							bFound = true;
							dResult = (DateTime)dr["START_TIME"];
							break;
						}
					}
				}
				dStart = dEnd.AddDays (1);
				dEnd = dStart.AddDays (nIncrement);
				if (dEnd > dtAVEnd)
					dEnd = dtAVEnd;
			} while (bFinished == false);

			string error = string.Empty;
			if (bFound == true) {
				AppointmentData a = new AppointmentData ();
				PatientInformation p = new PatientInformation ();
				List<string> errorMessages = new List<string> ();
				a.AppointmentID = "0";
				a.Duration = (Convert.ToDateTime (aRebook.END_TIME) - Convert.ToDateTime (aRebook.START_TIME)).TotalMinutes.ToString ();
				a.StartTime = dResult.ToString ();
				a.EndTime = dResult.AddMinutes (int.Parse (a.Duration)).ToString ();
				a.Notes = aRebook.NOTE;
				a.ResourceName = aRebook.RESOURCENAME;
				p.IEN = aRebook.PATIENTID;
				errorMessages = this.AddNewAppointment (a, p, false);
				error = errorMessages[1];
				SetAutoRebook (aRebook.APPOINTMENTID, dResult);
				DisplayAutoRebookLetter (errorMessages[0].ToString ());
				error = "A new appointment for patient " + p.Name + " has been booked on " + a.StartTime;
			} else {
				error = "Unable to rebook this patient: " + aRebook.PATIENTNAME;
			}
			return error;
		}

		private void DisplayAutoRebookLetter (string apptID)
		{
			object[] args = new object[2];
			args[0] = "Patient Auto Rebook Letter";
			args[1] = apptID + "|";
			this.eventAggregator.GetEvent<DisplayReportEvent> ().Publish (args);
		}

		private void SetAutoRebook (string apptID, DateTime dtRebookedTo)
		{
			List<string> parms = new List<string> ();
			parms.Add (apptID);
			parms.Add (dtRebookedTo.ToString ("M/d/yyyy@HH:mm"));
			DataTable table = adt.callOVIDRPC ("BSDX REBOOK SET", parms);
		}

		public DataTable CreateAvailabilitySchedule (string ResourceNames, DateTime StartTime, DateTime EndTime, string sSearchInfo)
		{
			DataTable rsOut;
			rsOut = new DataTable ("AvailabilitySchedule");
			DataTable rsSlotSchedule;
			DataTable rsApptSchedule;
			DataTable rsTemp1;

			rsSlotSchedule = CreateAssignedSlotSchedule (ResourceNames, StartTime.ToString (), EndTime.ToString (), "", sSearchInfo);
			
			if (rsSlotSchedule.Rows.Count > 0) {
				rsApptSchedule = CreateAppointmentSlotSchedule (ResourceNames, StartTime.ToString (), EndTime.ToString ());
				rsTemp1 = SubtractSlotsRS2 (rsSlotSchedule, rsApptSchedule, ResourceNames);
			} else {
				rsTemp1 = rsSlotSchedule;
							}
			rsOut = rsTemp1;
			return rsOut;
		}		

		private IList<FindAppointmentResult> findAppointments = null;
		public IList<FindAppointmentResult> FindAppointments (FindAppointmentData searchData)
		{
			DataTable rsOut = new DataTable ();
			List<FindAppointmentResult> tempSearchResult = new List<FindAppointmentResult> ();
			string sAccessTypes = string.Empty;
			string sSearchInfo = "1|" + searchData.AMPM + "|" + searchData.DAYOFWEEK;
			string m_sAmpm = searchData.AMPM;
			string m_sWeekDays = searchData.DAYOFWEEK;
			for (int j = 0; j < searchData.AccessTypes.Count; j++) {
				sAccessTypes = sAccessTypes + searchData.AccessTypes[j].ACCESS_TYPE;
				if (j < (searchData.AccessTypes.Count - 1))
					sAccessTypes = sAccessTypes + "|";
			}

			for (int i = 0; i < searchData.Resources.Count; i++) {

				DataTable dtTemp = CreateAssignedSlotSchedule (searchData.Resources[i].RESOURCE_NAME, searchData.StartDate, searchData.EndDate, sAccessTypes, sSearchInfo);
				DataTable rsTemp1 = new DataTable ();
				if (dtTemp.Rows.Count > 0) {
					DataTable dtAppt = CreateAppointmentSlotSchedule (searchData.Resources[i].RESOURCE_NAME, searchData.StartDate, searchData.EndDate);
					rsTemp1 = SubtractSlotsRS2 (dtTemp, dtAppt, searchData.Resources[i].RESOURCE_NAME);
				} else {
					rsTemp1 = dtTemp;
				}
				if (i == 0) {
					rsOut = rsTemp1;
				} else {
					rsOut = UnionBlocks (rsTemp1, rsOut);
				}

			}
			rsOut.TableName = "Result";
			rsOut.Columns.Add ("AMPM", System.Type.GetType ("System.String"), "Convert(START_TIME, 'System.String')");
			rsOut.Columns.Add ("DAYOFWEEK", System.Type.GetType ("System.String"));
			rsOut.Columns.Add ("ACCESSNAME", System.Type.GetType ("System.String"));

			DataRow drAT;
			DateTime dt;
			string sDOW;
			int nAccessTypeID;
			string sAccessType;
			foreach (DataRow dr in rsOut.Rows) {
				dt = Convert.ToDateTime (dr["START_TIME"].ToString ());
				sDOW = dt.DayOfWeek.ToString ();
				dr["DAYOFWEEK"] = sDOW;
				if (dr["ACCESS_TYPE"].ToString () != "") {
					nAccessTypeID = Convert.ToInt16 (dr["ACCESS_TYPE"].ToString ());
					drAT = m_dsGlobal.Tables["AccessTypes"].Rows.Find (nAccessTypeID);
					if (drAT != null) {
						sAccessType = drAT["NAME"].ToString ();
						dr["ACCESSNAME"] = sAccessType;
					}
				}
			}

			DataView m_dvResult = new DataView (rsOut);

			string sFilter = "(SLOTS > 0)";
			if (m_sAmpm != "") {
				if (m_sAmpm == "AM")
					sFilter = sFilter + " AND (AMPM LIKE '*AM*')";
				if (m_sAmpm == "PM")
					sFilter = sFilter + " AND (AMPM LIKE '*PM*')";
			}

			bool sOr = false;
			if (m_sWeekDays != "") {
				sFilter += " AND (";
				if (searchData.Monday) {
					sFilter = sFilter + "(DAYOFWEEK LIKE '*Monday*')";
					sOr = true;
				}
				if (searchData.Tuesday) {
					sFilter = (sOr == true) ? sFilter + " OR " : sFilter;
					sFilter = sFilter + "(DAYOFWEEK LIKE '*Tuesday*')";
					sOr = true;
				}
				if (searchData.Wednesday) {
					sFilter = (sOr == true) ? sFilter + " OR " : sFilter;
					sFilter = sFilter + "(DAYOFWEEK LIKE '*Wednesday*')";
					sOr = true;
				}
				if (searchData.Thursday) {
					sFilter = (sOr == true) ? sFilter + " OR " : sFilter;
					sFilter = sFilter + "(DAYOFWEEK LIKE '*Wednesday*')";
					sOr = true;
				}
				if (searchData.Friday) {
					sFilter = (sOr == true) ? sFilter + " OR " : sFilter;
					sFilter = sFilter + "(DAYOFWEEK LIKE '*Friday*')";
					sOr = true;
				}
				if (searchData.Saturday) {
					sFilter = (sOr == true) ? sFilter + " OR " : sFilter;
					sFilter = sFilter + "(DAYOFWEEK LIKE '*Saturday*')";
					sOr = true;
				}
				if (searchData.Sunday) {
					sFilter = (sOr == true) ? sFilter + " OR " : sFilter;
					sFilter = sFilter + "(DAYOFWEEK LIKE '*Sunday*')";
					sOr = true;
				}
				sFilter += ")";
			}

			if (searchData.AccessTypes.Count > 0) {
				sFilter += " AND (";
				sOr = false;
				foreach (FindGroupedAccessTypes sType in searchData.AccessTypes) {
					if (sOr == true)
						sFilter += " OR ";
					sOr = true;
					sFilter += "(ACCESSNAME = '" + sType.ACCESS_TYPE + "')";
				}
				sFilter += ")";
			}

			m_dvResult.RowFilter = sFilter;
			DataTable m = m_dvResult.ToTable();

			foreach (DataRow row in m.Rows) {
				FindAppointmentResult f = new FindAppointmentResult ();
				f.StartTime = GetCellString (row, "START_TIME");
				f.EndTime = GetCellString (row, "END_TIME");
				f.Slots = GetCellString (row, "SLOTS");
				f.ResourceName = GetCellString (row, "RESOURCE");
				f.AccessType = GetCellString (row, "ACCESSNAME");
				tempSearchResult.Add (f);
			}

			findAppointments = tempSearchResult;
			return findAppointments;
		}

		public DataTable CreateAssignedSlotSchedule (string sResourceName, string sStartTime, string sEndTime, string sAccessTypes, string sSearchInfo)
		{
			List<string> parms = new List<string> ();
			parms.Add (sResourceName);
			parms.Add (Convert.ToDateTime (sStartTime).ToString ("M-d-yyyy") + "@0001");
			parms.Add (Convert.ToDateTime (sEndTime).ToString ("M-d-yyyy") + "@2359");
			parms.Add (sAccessTypes);
			parms.Add (sSearchInfo);
			DataTable dtRet = adt.callOVIDRPC ("BSDX CREATE ASGND SLOT SCHED", parms);
			DataTable dtTemp = new DataTable ();
			dtTemp.Columns.Add ("START_TIME", System.Type.GetType ("System.DateTime"));
			dtTemp.Columns.Add ("END_TIME", System.Type.GetType ("System.DateTime"));
			dtTemp.Columns.Add ("SLOTS", System.Type.GetType ("System.Int16"));
			dtTemp.Columns.Add ("RESOURCE", System.Type.GetType ("System.String"));
			dtTemp.Columns.Add ("ACCESS_TYPE", System.Type.GetType ("System.String"));
			dtTemp.Columns.Add ("NOTE", System.Type.GetType ("System.String"));
			dtTemp.Columns.Add ("AVAILABILITYID", System.Type.GetType ("System.Int16"));

			foreach (DataRow r in dtRet.Rows) {
				DataRow rNew = dtTemp.NewRow ();
				rNew["START_TIME"] = Convert.ToDateTime (r[dtRet.Columns["START_TIME"]]);
				rNew["END_TIME"] = Convert.ToDateTime (r[dtRet.Columns["END_TIME"]]);
				rNew["SLOTS"] = r[dtRet.Columns["SLOTS"]];
				rNew["RESOURCE"] = r[dtRet.Columns["RESOURCE"]];
				rNew["ACCESS_TYPE"] = r[dtRet.Columns["ACCESS_TYPE"]];
				rNew["NOTE"] = r[dtRet.Columns["NOTE"]];
				rNew["AVAILABILITYID"] = r[dtRet.Columns["AVAILABILITYID"]];
				dtTemp.Rows.Add (rNew);
			}

			return dtTemp;
		}

		public DataTable CreateAppointmentSlotSchedule (string sResourceName, string startTime, string endTime)
		{
			List<string> parms1 = new List<string> ();
			parms1.Add (Convert.ToDateTime (startTime).ToString ("M-d-yyyy"));
			parms1.Add (Convert.ToDateTime (endTime).ToString ("M-d-yyyy"));
			parms1.Add (sResourceName);
			DataTable dtAppt = adt.callOVIDRPC ("BSDX APPT BLOCKS OVERLAP", parms1);

			DataTable dtCopy = new DataTable ("dtCopy");
			if (dtAppt.Rows.Count > 0) {
				//Create CDateTimeArray & load records from rsOut
				int nRC;
				nRC = dtAppt.Rows.Count;
				ArrayList cdtArray = new ArrayList ();
				cdtArray.Capacity = (nRC * 2);
				DateTime v;
				int icount = 0;

				foreach (DataRow r in dtAppt.Rows) {
					v = Convert.ToDateTime (r[dtAppt.Columns["START_TIME"]]);
					cdtArray.Add (v);
					v = Convert.ToDateTime (r[dtAppt.Columns["END_TIME"]]);
					cdtArray.Add (v);
				}
				cdtArray.Sort ();

				//Create a CTimeBlockArray and load it from rsOut
				ArrayList ctbAppointments = new ArrayList (nRC);
				SchdAvailability cTB;
				foreach (DataRow r in dtAppt.Rows) {
					cTB = new SchdAvailability ();
					cTB.StartTime = r[dtAppt.Columns["START_TIME"]].ToString();
					cTB.EndTime = r[dtAppt.Columns["END_TIME"]].ToString();
					ctbAppointments.Add (cTB);
				}

				//Create a TimeBlock Array from the data in the DateTime array
				ArrayList ctbApptSchedule = new ArrayList ();
				DateTime csStartTime = Convert.ToDateTime (startTime);
				DateTime csEndTime = Convert.ToDateTime (endTime);

				if (csStartTime == csEndTime) {
					csEndTime = csEndTime.AddDays (1).AddMinutes (-1);
				}
				ScheduleFromArray (cdtArray, csStartTime, csEndTime, ref ctbApptSchedule);

				//Find number of TimeBlocks in ctbApptSchedule that
				//overlap the TimeBlocks in ctbAppointments
				ArrayList ctbApptSchedule2 = new ArrayList ();
				SchdAvailability cTB2;
				int nSlots = 0;
				for (icount = 0; icount < ctbApptSchedule.Count; icount++) {
					cTB = (SchdAvailability)ctbApptSchedule[icount];
					nSlots = BlocksOverlap (cTB, ctbAppointments, false);
					cTB2 = new SchdAvailability ();
					cTB2.Create (cTB.StartTime, cTB.EndTime, nSlots);
					ctbApptSchedule2.Add (cTB2);
				}

				ConsolidateBlocks (ctbApptSchedule2);

				DataColumn dCol = new DataColumn ();
				dCol.DataType = Type.GetType ("System.DateTime");
				dCol.ColumnName = "START_TIME";
				dCol.ReadOnly = true;
				dCol.AllowDBNull = false;
				dCol.Unique = false;
				dtCopy.Columns.Add (dCol);

				dCol = new DataColumn ();
				dCol.DataType = Type.GetType ("System.DateTime");
				dCol.ColumnName = "END_TIME";
				dCol.ReadOnly = true;
				dCol.AllowDBNull = false;
				dCol.Unique = false;
				dtCopy.Columns.Add (dCol);

				dCol = new DataColumn ();
				dCol.DataType = Type.GetType ("System.Int16");
				dCol.ColumnName = "SLOTS";
				dCol.ReadOnly = true;
				dCol.AllowDBNull = false;
				dCol.Unique = false;
				dtCopy.Columns.Add (dCol);

				dCol = new DataColumn ();
				dCol.DataType = Type.GetType ("System.String");
				dCol.ColumnName = "RESOURCE";
				dCol.ReadOnly = true;
				dCol.AllowDBNull = false;
				dCol.Unique = false;
				dtCopy.Columns.Add (dCol);

				for (int k = 0; k < ctbApptSchedule2.Count; k++) {
					cTB = (SchdAvailability)ctbApptSchedule2[k];
					DataRow newRow;
					newRow = dtCopy.NewRow ();
					newRow["START_TIME"] = cTB.StartTime;
					newRow["END_TIME"] = cTB.EndTime;
					newRow["SLOTS"] = cTB.SLOTS;
					newRow["RESOURCE"] = sResourceName;
					dtCopy.Rows.Add (newRow);
				}
			}

			return dtCopy;
		}

		public DataTable UnionBlocks (DataTable rs1, DataTable rs2)
		{
			DataTable rsCopy;
			DataRow dr;
			if (rs1.Columns.Count >= rs2.Columns.Count) {
				rsCopy = rs1.Copy ();
				foreach (DataRow dr2 in rs2.Rows) {
					dr = rsCopy.NewRow ();
					dr.ItemArray = dr2.ItemArray;
					rsCopy.Rows.Add (dr);
				}
			} else {
				rsCopy = rs2.Copy ();
				foreach (DataRow dr2 in rs1.Rows) {
					dr = rsCopy.NewRow ();
					dr.ItemArray = dr2.ItemArray;
					rsCopy.Rows.Add (dr);
				}
			}
			return rsCopy;
		}

		public DataTable SubtractSlotsRS2 (DataTable rsBlocks1, DataTable rsBlocks2, string sResource)
		{
			if ((rsBlocks2.Rows.Count == 0) && (rsBlocks1.Rows.Count == 0))
				return rsBlocks1;

			//Create an array of the start and end times of blocks2
			ArrayList cdtArray = new ArrayList (2 * (rsBlocks1.Rows.Count + rsBlocks2.Rows.Count));

			foreach (DataRow r in rsBlocks1.Rows) {
				cdtArray.Add (r[rsBlocks1.Columns["START_TIME"]]);
				cdtArray.Add (r[rsBlocks1.Columns["END_TIME"]]);
			}

			foreach (DataRow r in rsBlocks2.Rows) {
				cdtArray.Add (r[rsBlocks2.Columns["START_TIME"]]);
				cdtArray.Add (r[rsBlocks2.Columns["END_TIME"]]);
			}

			cdtArray.Sort ();

			ArrayList ctbReturn = new ArrayList ();
			DateTime cDate = new DateTime ();
			ScheduleFromArray (cdtArray, cDate, cDate, ref ctbReturn);

			//Set up return table
			DataTable rsCopy = CreateCopyTable ();	
			long nSlots = 0;
			SchdAvailability cTB;

			for (int j = 0; j < (ctbReturn.Count - 1); j++) 
			{
				cTB = (SchdAvailability)ctbReturn[j];
				nSlots = SlotsInBlock (cTB, rsBlocks1) - SlotsInBlock (cTB, rsBlocks2);
				string sResourceList = "";
				string sAccessRuleList = "";
				string sNote = "";

				if (nSlots > 0) {
					bool bRet = ResourceRulesInBlock (cTB, rsBlocks1, ref sResourceList, ref sAccessRuleList, ref sNote);
				}
				DataRow newRow;
				newRow = rsCopy.NewRow ();
				newRow["START_TIME"] = cTB.StartTime;
				newRow["END_TIME"] = cTB.EndTime;
				newRow["SLOTS"] = nSlots;
				newRow["RESOURCE"] = sResource;
				newRow["ACCESS_TYPE"] = sAccessRuleList;
				newRow["NOTE"] = sNote;
				rsCopy.Rows.Add (newRow);
			}
			return rsCopy;
		}

		public bool ResourceRulesInBlock (SchdAvailability rTimeBlock, DataTable rsBlock, ref string sResourceList, ref string sAccessRuleList, ref string sNote)
		{
			DateTime dStart1;
			DateTime dStart2;
			DateTime dEnd1;
			DateTime dEnd2;
			string sResource;
			string sAccessRule;

			if (rsBlock.Rows.Count == 0)
				return true;

			dStart1 = DateTime.Parse(rTimeBlock.StartTime);
			dEnd1 = DateTime.Parse(rTimeBlock.EndTime);

			DataTable dtTypes = m_dsGlobal.Tables["AccessTypes"];
			DataView dvTypes = dtTypes.DefaultView;
			dvTypes.Sort = "BSDX_ACCESS_TYPE_IEN ASC";

			foreach (DataRow r in rsBlock.Rows) {
				dStart2 = Convert.ToDateTime (r[rsBlock.Columns["START_TIME"]]);
				dEnd2 = Convert.ToDateTime (r[rsBlock.Columns["END_TIME"]]);

				if (TimesOverlap (dStart1, dEnd1, dStart2, dEnd2)){
					sResource = r[rsBlock.Columns["RESOURCE"]].ToString ();
					if (sResource == "NULL")
						sResource = "";
					if (sResource != "") {
						if (sResourceList == "") {
							sResourceList += sResource;
						} else {
							sResourceList += "^" + sResource;
						}
					}
					sAccessRule = r[rsBlock.Columns["ACCESS_TYPE"]].ToString ();
					if (sAccessRule == "0")
						sAccessRule = "";
					if (sAccessRule != "") {
						if (sAccessRuleList == "") {
							sAccessRuleList += sAccessRule;
						} else {
							int nRow = dvTypes.Find (sAccessRule);
							DataRowView drv = dvTypes[nRow];
							string sIsPreventAccess = drv["PREVENT_ACCESS"].ToString ();

							if (sIsPreventAccess == "YES") {
								sAccessRule += "^" + sAccessRuleList;
								sAccessRuleList = sAccessRule;
							} else {
								sAccessRuleList += "^" + sAccessRule;
							}
						}
					}
					sNote = r[rsBlock.Columns["NOTE"]].ToString ();

				}
			}
			return true;
		}//End ResourceRulesInBlock

		public DataTable CreateCopyTable ()
		{
			DataTable dtCopy = new DataTable ("dtCopy");
			DataColumn dCol = new DataColumn ();
			dCol.DataType = Type.GetType ("System.DateTime");
			dCol.ColumnName = "START_TIME";
			dCol.ReadOnly = true;
			dCol.AllowDBNull = false;
			dCol.Unique = false;
			dtCopy.Columns.Add (dCol);

			dCol = new DataColumn ();
			dCol.DataType = Type.GetType ("System.DateTime");
			dCol.ColumnName = "END_TIME";
			dCol.ReadOnly = true;
			dCol.AllowDBNull = false;
			dCol.Unique = false;
			dtCopy.Columns.Add (dCol);

			dCol = new DataColumn ();
			dCol.DataType = Type.GetType ("System.Int16");
			dCol.ColumnName = "SLOTS";
			dCol.ReadOnly = true;
			dCol.AllowDBNull = false;
			dCol.Unique = false;
			dtCopy.Columns.Add (dCol);

			dCol = new DataColumn ();
			dCol.DataType = Type.GetType ("System.String");
			dCol.ColumnName = "RESOURCE";
			dCol.ReadOnly = true;
			dCol.AllowDBNull = true;
			dCol.Unique = false;
			dtCopy.Columns.Add (dCol);

			dCol = new DataColumn ();
			dCol.DataType = Type.GetType ("System.String");
			dCol.ColumnName = "ACCESS_TYPE";
			dCol.ReadOnly = true;
			dCol.AllowDBNull = true;
			dCol.Unique = false;
			dtCopy.Columns.Add (dCol);

			dCol = new DataColumn ();
			dCol.DataType = Type.GetType ("System.String");
			dCol.ColumnName = "NOTE";
			dCol.ReadOnly = true;
			dCol.AllowDBNull = true;
			dCol.Unique = false;
			dtCopy.Columns.Add (dCol);


			return dtCopy;
		}

		public int SlotsInBlock (SchdAvailability rTimeBlock, DataTable rsBlock)
		{
			DateTime dStart1;
			DateTime dStart2;
			DateTime dEnd1;
			DateTime dEnd2;
			int nSlots = 0;

			if (rsBlock.Rows.Count == 0)
				return nSlots;

			dStart1 = Convert.ToDateTime(rTimeBlock.StartTime);
			dEnd1 = Convert.ToDateTime(rTimeBlock.EndTime);

			foreach (DataRow r in rsBlock.Rows) {
				dStart2 = Convert.ToDateTime (r[rsBlock.Columns["START_TIME"]]);
				dEnd2 = Convert.ToDateTime (r[rsBlock.Columns["END_TIME"]]);

				if (TimesOverlap (dStart1, dEnd1, dStart2, dEnd2)){
					string sSlots = r[rsBlock.Columns["SLOTS"]].ToString ();
					nSlots = System.Convert.ToInt16 (sSlots);
					break;
				}
			}
			return nSlots;
		}//end SlotsInBlock

		public void ConsolidateBlocks (ArrayList rTBArray)
		{
			int j = 0;
			bool bDirty = false;
			SchdAvailability cBlockA;
			SchdAvailability cBlockB;
			SchdAvailability cTemp1;
			if (rTBArray.Count < 2)
				return;
			do {
				bDirty = false;
				for (j = 0; j < (rTBArray.Count - 1); j++)
				{
					cBlockA = (SchdAvailability)rTBArray[j];
					cBlockB = (SchdAvailability)rTBArray[j + 1];
					if ((cBlockA.EndTime == cBlockB.StartTime)
						&& (cBlockA.SLOTS == cBlockB.SLOTS)
						&& (cBlockA.RESOURCENAME == cBlockB.RESOURCENAME)
						&& (cBlockA.RESOURCENAME == cBlockB.RESOURCENAME)
						) {
						cTemp1 = new SchdAvailability ();
						cTemp1.StartTime = cBlockA.StartTime;
						cTemp1.EndTime = cBlockB.EndTime;
						cTemp1.SLOTS = cBlockA.SLOTS;
						cTemp1.AccessRuleList = cBlockA.AccessRuleList;
						cTemp1.RESOURCENAME = cBlockA.RESOURCENAME;
						rTBArray.Insert (j, cTemp1);
						rTBArray.RemoveRange (j + 1, 2);
						bDirty = true;
						break;
					}
				}
			}
			while (!((bDirty == false) || (rTBArray.Count == 1)));
		}

		public int BlocksOverlap (SchdAvailability rBlock, ArrayList rTBArray, bool bCountSlots)
		{
			DateTime dStart1;
			DateTime dStart2;
			DateTime dEnd1;
			DateTime dEnd2;
			int nSlots;
			int nCount = 0;
			SchdAvailability cBlock;

			dStart1 = DateTime.Parse(rBlock.StartTime);
			dEnd1 = DateTime.Parse(rBlock.EndTime);

			for (int j = 0; j < rTBArray.Count; j++) {
				cBlock = (SchdAvailability)rTBArray[j];
				dStart2 = DateTime.Parse(cBlock.StartTime);
				dEnd2 = DateTime.Parse(cBlock.EndTime);
				nSlots = cBlock.SLOTS;
				if (TimesOverlap (dStart1, dEnd1, dStart2, dEnd2)) {
					if (bCountSlots == true) {
						nCount += nSlots;
					} else {
						nCount++;
					}
				}
			}

			return nCount;
		}

		public bool TimesOverlap (DateTime dStart1, DateTime dEnd1, DateTime dStart2, DateTime dEnd2)
		{
			bool bRet = false;

			if ((dStart2 > dStart1 && dStart2 < dEnd1) 
				|| (dEnd2 > dStart1 && dEnd2 < dEnd1) 
				|| (dStart1 > dStart2 && dStart1 < dEnd2)
				|| (dEnd1 > dStart2 && dEnd1 < dEnd2)
				|| (dEnd2 == dEnd1 )
				|| (dStart2 == dStart1)) {
				bRet = true;
			}
			return bRet;
		}

		public void ScheduleFromArray (ArrayList cdtArray, DateTime dStartTime, DateTime dEndTime, ref ArrayList rTBArray)
		{
			int j = 0;
			SchdAvailability cTB;

			if (cdtArray.Count == 0)
				return;

			//If StartTime passed in, then adjust for it
			if (dStartTime.Ticks > 0) {
				if ((DateTime)cdtArray[0] > dStartTime) {
					cTB = new SchdAvailability ();
					cTB.Create (dStartTime.ToString(), ((DateTime)cdtArray[0]).ToString(), 0);
					rTBArray.Add (cTB);
				}
				if ((DateTime)cdtArray[0] < dStartTime) {
					for (j = 0; j < cdtArray.Count; j++) {
						if ((DateTime)cdtArray[j] < dStartTime)
							cdtArray[j] = dStartTime;
					}
				}
			}

			//Trim the end if necessary
			if (dEndTime.Ticks > 0) {
				for (j = 0; j < cdtArray.Count; j++) {
					if ((DateTime)cdtArray[j] > dEndTime)
						cdtArray[j] = dEndTime;
				}
			}

			//build the schedule in rTBArray
			DateTime dTemp = new DateTime ();
			DateTime dStart;
			DateTime dEnd;
			int k = 0;
			for (j = 0; j < (cdtArray.Count - 1); j++) //TODO: why minus 1?
			{
				if ((DateTime)cdtArray[j] != dTemp) {
					dStart = (DateTime)cdtArray[j];
					dTemp = dStart;
					for (k = j + 1; k < cdtArray.Count; k++) {
						dEnd = new DateTime ();
						if ((DateTime)cdtArray[k] != dStart) {
							dEnd = (DateTime)cdtArray[k];
						}
						if (dEnd.Ticks > 0) {
							cTB = new SchdAvailability ();
							cTB.Create (dStart.ToString(), dEnd.ToString(), 0);
							rTBArray.Add (cTB);
							break;
						}
					}
				}
			}

		}//end ScheduleFromArray

		public void CancelAccessBlocks (string sResourceID, string sResourceName, string sStart, string sEnd)
		{
			List<string> parms1 = new List<string> ();
			parms1.Add (sResourceID);
			parms1.Add (sStart);
			parms1.Add (sEnd);
			DataTable dtAppt = adt.callOVIDRPC ("BSDX CANCEL AV BY DATE", parms1);
			
			GetAvailabilities (sResourceName, sStart, sEnd);
		}

		public string GetReportText (string reportRPC, List<List<string>> parms)
		{
			string reportText = string.Empty;
			if (!string.IsNullOrEmpty (reportRPC)) {
				DataTable table = adt.callOVIDRPC (reportRPC, parms);
				if (table.Columns.Count > 0) {
					foreach (DataRow row in table.Rows) {
						reportText += row[0] + Environment.NewLine;
					}
				}
			} else {
				return "Report not defined";
			}
			if (reportText == string.Empty) {
				return "Empty report returned.";
			}
			return reportText;
		}

		private IList<NameValue> serverPrinters = null;
		public IList<NameValue> GetServerPrinters ()
		{
			if (serverPrinters == null || serverPrinters.Count == 0) {
				List<string> parms = new List<string> ();
				DataTable table = adt.callOVIDRPC ("BDGGIC06 DEVICE", parms);

				List<NameValue> tempserverPrinters = new List<NameValue> ();
				foreach (DataRow row in table.Rows) {
					NameValue nameValue = new NameValue ();
					nameValue.Name = GetCellString (row, "PRINTER_NAME");
					nameValue.Value = GetCellString (row, "PRINTER_IEN");
					tempserverPrinters.Add (nameValue);
				}
				serverPrinters = tempserverPrinters;
			}
			return serverPrinters;
		}

		public string CurrentUser
		{
			get
			{
				if (currentUser == string.Empty) {
					currentUser = adt.UserID;
				}
				return currentUser;
			}
		}

		public void ChangeUserServer ()
		{
			currentUser = string.Empty;
			adt.UserInfoLogout ();
			LoadGlobalRecordsets ();
		}

		public bool LoginUser(string accessCode, string verifyCode)
		{
			return adt.LoginUser(accessCode, verifyCode);
		}

		#endregion

	}
}