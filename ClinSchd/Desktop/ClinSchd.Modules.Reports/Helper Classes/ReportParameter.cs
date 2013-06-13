using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClinSchd.Infrastructure.Models;

namespace ClinSchd.Modules.Reports.Helper_Classes
{
	public abstract class ReportParameter
	{
		protected ReportsModel parentModel;
		protected bool newlyCreated;
		private object _value;
		/// <summary>
		/// Value contained by the class (typically a string).
		/// </summary>
		public object Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
				if (!newlyCreated) {
					parentModel.InjectManualMappings (Description);
				}
			}
		}
		public string ValueType { get; set; }
		public string Description { get; set; }
		public ObservableDictionary<string, string> PossibleValues { get; set; }
	}

	public class ReportParameterCreator
	{
		private class ConcreteReportParameter : ReportParameter 
		{
			public ConcreteReportParameter (ReportsModel _parentModel)
			{
				this.parentModel = _parentModel;
				this.newlyCreated = true;
			}

			public void MarkAsOld ()
			{
				this.newlyCreated = false;
			}
		}
		private ReportsModel parentModel;
		public ReportParameterCreator (ReportsModel _parentModel)
		{
			parentModel = _parentModel;
		}

		public ReportParameter GetNewWrapper (object valueToSet, string description)
		{
			ConcreteReportParameter wrapperToReturn = new ConcreteReportParameter (parentModel);
			wrapperToReturn.Value = valueToSet;
			wrapperToReturn.Description = description;
			wrapperToReturn.MarkAsOld ();
			return wrapperToReturn;
		}
	}
}
