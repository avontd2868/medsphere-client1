using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ClinSchd.Infrastructure.Models
{
	public class NameValue
	{
		public NameValue ()
		{
		}
		public NameValue (string name, string value)
		{
			Name = name;
			Value = value;
		}
		public string Name { get; set; }
		public string Value { get; set; }
	}
}
