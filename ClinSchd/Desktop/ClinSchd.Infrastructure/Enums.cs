using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ClinSchd.Infrastructure
{
	public class Enums
	{
		//Patient Movement Type (1=admit, 2=ward transfer, 
		//	3=discharge, 4=check-in lodger, 5=check-out lodger,6=service transfer)
		public enum MovementTypes
		{
			[DescriptionAttribute("1")]
			Admission = 1,
			[DescriptionAttribute("2")]
			WardTransfer = 2,
			[DescriptionAttribute("3")]
			Discharge = 3,
			[DescriptionAttribute("4")]
			CheckInLodger = 4,
			[DescriptionAttribute("5")]
			CheckOutLodger = 5,
			[DescriptionAttribute("6")]
			ServiceTransfer = 6
		}
	}
}
