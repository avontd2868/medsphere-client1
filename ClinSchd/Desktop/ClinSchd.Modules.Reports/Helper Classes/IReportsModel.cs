using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Modules.Reports.Helper_Classes
{
	public interface IReportsModel
	{
		List<ReportParameter> ParameterDictionary { get; set; }
		ReportParameterCreator reportParameterCreator { get; }
		Dictionary<string, ReportParameter> Params { get; }
		List<ReportParameter> HardCodedParams { get; }
		//IList<NameValue> WardLocations { get; }
		//IList<NameValue> TreatingSpecialities { get; }
		//IList<NameValue> Providers { get; }
		//IList<NameValue> Religions { get; }
		//IList<NameValue> HospitalLocations { get; }
		IList<NameValue> ServerPrinterList { get; }

		string ReportDisplayName { get; }

		string ReportType { get; }
		string ReportRPC { get; }
		List<ReportParameter> RequiredParams { get; }
		string ReportText { get; }

		void InjectManualMappings (string propertyChanged);
	}
}
