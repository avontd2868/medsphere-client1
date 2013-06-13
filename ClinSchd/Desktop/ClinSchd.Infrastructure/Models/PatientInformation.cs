using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClinSchd.Infrastructure.Models
{
	public class PatientInformation
	{
		public string IEN { get; set; }
		public string Name { get; set; }
		public string HRN { get; set; }
		public string SSN { get; set; }
		public string SSNMasked
		{
			get
			{
				if (!string.IsNullOrEmpty(SSN) && SSN.Length > 4) {
					return SSN.Substring (SSN.Length - 4);
				} else {
					return string.Empty;
				}
			}
		}

		private string dob;
		public string DOB
		{
			get { return dob; }
			set
			{
				dob = value;
				DateTime now = DateTime.Today;
				DateTime bday = DateTime.Today;
				if (DateTime.TryParse (dob, out bday)) {
					int age = now.Year - bday.Year;
					if (bday > now.AddYears (-age))
						age--;
					Age = age.ToString ();
				} else {
					Age = null;
				}
			}
		}
		public string Age { get; private set; }
		public string Street { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string Zip { get; set; }
		public string OfficePhone { get; set; }
		public string HomePhone { get; set; }
	}
}
