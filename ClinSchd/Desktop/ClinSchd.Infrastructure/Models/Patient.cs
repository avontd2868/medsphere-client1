using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClinSchd.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;

namespace ClinSchd.Infrastructure.Models
{
	public class Patient : AsyncValidatableModel
    {
		private IDataAccessService dataAccessService;
		public string IEN { get; set; }
		public string Name { get; set; }
		public string HRN { get; set; }
		public string SSN { get; set; }
		public string SSNMasked
		{
			get
			{
				if (!string.IsNullOrEmpty (SSN) && SSN.Length > 4) {
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

		public Patient (IDataAccessService dataAccessService)
		{
			this.dataAccessService = dataAccessService;
		}

		public void LoadPatientInfoByIEN()
		{
			PatientInformation ptInfo = dataAccessService.GetPatientInfo (IEN);
			if (ptInfo != null) {
				DOB = ptInfo.DOB;
				HRN = ptInfo.HRN;
				IEN = ptInfo.IEN;
				Name = ptInfo.Name;
				SSN = ptInfo.SSN;			
			}
		}

		public void GetPatients (string searchString, WorkCompletedMethod workCompletedMethod)
		{
			DoWorkAsync ((s, args) => {
				args.Result = this.dataAccessService.GetPatients (searchString == string.Empty ? null : searchString, 50);
			}, workCompletedMethod);
		}
	}
}
