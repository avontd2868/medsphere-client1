using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClinSchd.Infrastructure.Models
{
	public class ResourceUser
	{
		public string BSDX_RESOURCE_USER_IEN { get; set; }
		public string RESOURCE_NAME { get; set; }
		public string RESOURCE_ID { get; set; }
		public string OVERBOOK { get; set; }
		public string MODIFY_SCHEDULE { get; set; }
		public string MODIFY_APPTS { get; set; }
		public string USERNAME { get; set; }
		public string USER_ID { get; set; }
		public string MASTEROVERBOOK { get; set; }
	}
}
