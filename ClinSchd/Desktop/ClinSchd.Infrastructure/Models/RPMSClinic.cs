using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClinSchd.Infrastructure.Models
{
	/// <summary>
	/// RPMS Clinic information
	/// </summary>
	public class RPMSClinic
	{
		public string PROVIDER { get; set; }
		public string HOSPITAL_LOCATION { get; set; }
		public string HOSPITAL_LOCATION_ID { get; set; }
		public string CODE { get; set; }
		public string OVERBOOK { get; set; }

		public string INACTIVE { get; set; }
		public string REACTIVE { get; set; }
		public string CREATEVISIT { get; set; }
		public string VISITSERVICE { get; set; }

	}
}
