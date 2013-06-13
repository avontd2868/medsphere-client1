using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClinSchd.Infrastructure.Interfaces;
using Microsoft.Practices.Composite.Presentation.Commands;
using Microsoft.Practices.Composite.Events;

namespace ClinSchd.Infrastructure.Models
{
	public class SchdResource : AsyncValidatableModel
	{
		private IDataAccessService dataAccessService;
		private RPMSClinic clinic;
		public string RESOURCEID { get; set; }
		public string RESOURCE_NAME { get; set; }
		public string INACTIVE { get; set; }
		public string TIMESCALE { get; set; }
		public string HOSPITAL_LOCATION_ID { get; set; }
		public string LETTER_TEXT { get; set; }
		public string NO_SHOW_LETTER { get; set; }
		public string CLINIC_CANCELLATION_LETTER { get; set; }
		public string VIEW { get; set; }
		public string OVERBOOK { get; set; }
		public string MODIFY_SCHEDULE { get; set; }
		public string MODIFY_APPOINTMENTS { get; set; }

		public SchdResource ()
		{
		}

		public SchdResource (IDataAccessService dataAccessService)
		{
			this.dataAccessService = dataAccessService;
		}

		public RPMSClinic Clinic
		{
			get
			{
				if (clinic == null) {
					clinic = this.dataAccessService.GetRPMSClinic (RESOURCEID);
				}
				return clinic;
			}
			set
			{
				clinic = value;
			}
		}

		private bool? variableLengthAppointments;
		public bool VariableLengthAppointments
		{
			get
			{
				if (variableLengthAppointments == null) {
					variableLengthAppointments = dataAccessService.GetVariableAppointmentsFlag (RESOURCEID);
				}
				return variableLengthAppointments.Value;
			}
		}

		private IList<Provider> providers;
		public IList<Provider> Providers
		{
			get
			{
				if (providers == null) {
					providers = dataAccessService.GetProvidersByClinic (this.HOSPITAL_LOCATION_ID);
				}
				return providers;
			}
		}

		public void GetAllResources (WorkCompletedMethod workCompletedMethod)
		{
			DoWorkAsync ((s, args) => {
				args.Result = dataAccessService.GetResources (string.Empty, false);
			}, workCompletedMethod);
		}

		public void GetVisibleAppointments (DateTime startTime, DateTime endTime, WorkCompletedMethod workCompletedMethod)
		{
			DoWorkAsync( (s, args) => {
				args.Result = this.dataAccessService.GetAppointments (RESOURCE_NAME, startTime.ToShortDateString (), endTime.ToShortDateString () + "@23:59");
			}, workCompletedMethod);
		}

		public void GetVisibleAvailabilities (DateTime startTime, DateTime endTime, WorkCompletedMethod workCompletedMethod)
		{
			DoWorkAsync ((s, args) => {
				args.Result = this.dataAccessService.GetAvailabilities (RESOURCE_NAME, startTime.ToShortDateString (), endTime.ToShortDateString ());
			}, workCompletedMethod);
		}
	}
}
