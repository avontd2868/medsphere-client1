using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;

namespace ClinSchd.Infrastructure.Models
{
	[Serializable]
	public class SchdAvailability
	{
		public string APPOINTMENTID { get; set; }
		public string StartTime { get; set; }
		public string EndTime { get; set; }
		public string RESOURCENAME { get; set; }
		public string ACCESSTYPEID { get; set; }
		public string ACCESSTYPENAME { get; set; }
		public string Note { get; set; }
		public string Command { get; set; }
		public bool IsPreventAccess { get; set; }
		public int SLOTS { get; set; }
		public int COLUMNS { get; set; }
		public int GridColumn { get; set; }
		public string AccessRuleList { get; set; }

		public string DisplayColor { get; set; }
		public int Red { get; set; }
		public int Green { get; set; }
		public int Blue { get; set; }

		public double? Duration
		{
			get
			{
				try {
					return (DateTime.Parse (EndTime).Subtract (DateTime.Parse (StartTime))).TotalMinutes;
				} catch (Exception ex) {
					if (ex is ArgumentException || ex is ArgumentNullException) {
						return null;
					} else {
						throw;
					}
				}
			}
		}

		#region Methods
		public void Create (string startTime, string endTime, string nAvailabilityType, int nSlots, string sResourceList, string sAccessRuleList)
		{
			StartTime = startTime;
			EndTime = endTime;
			ACCESSTYPEID = nAvailabilityType;
			SLOTS = nSlots;
			RESOURCENAME = sResourceList;
			AccessRuleList = sAccessRuleList;
		}
		public void Create (string startTime, string endTime, string nAvailabilityType, int nSlots, string sResourceList)
		{
			StartTime = startTime;
			EndTime = endTime;
			ACCESSTYPEID = nAvailabilityType;
			SLOTS = nSlots;
			RESOURCENAME = sResourceList;
			AccessRuleList = "";
		}
		public void Create (string startTime, string endTime, string nAvailabilityType, int nSlots)
		{
			StartTime = startTime;
			EndTime = endTime;
			ACCESSTYPEID = nAvailabilityType;
			SLOTS = nSlots;
			RESOURCENAME = "";
			AccessRuleList = "";
		}

		public void Create (string startTime, string endTime, int nSlots)
		{
			StartTime = startTime;
			EndTime = endTime;
			ACCESSTYPEID = "0";
			SLOTS = nSlots;
			RESOURCENAME = "";
			AccessRuleList = "";
		}

		#endregion //Methods

	}
}
