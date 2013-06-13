using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClinSchd.Infrastructure.Models
{
	public class ValidationMessage
	{
		public bool IsValid { get; set; }
		public string Title { get; set; }
		public string Message { get; set; }

	}
}
