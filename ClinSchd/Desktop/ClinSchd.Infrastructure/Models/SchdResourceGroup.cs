using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClinSchd.Infrastructure.Interfaces;
using Microsoft.Practices.Composite.Presentation.Commands;
using Microsoft.Practices.Composite.Events;
using System.Windows.Markup;
using System.IO;


namespace ClinSchd.Infrastructure.Models
{
	public class SchdResourceGroup : AsyncValidatableModel
	{
		private IDataAccessService dataAccessService;
		public string Name { get; set; }
		public string IEN { get; set; }
		public IList<SchdResource> Resources { get; set; }

		public SchdResourceGroup (IDataAccessService dataAccessService)
		{
			this.dataAccessService = dataAccessService;
			this.Resources = new List<SchdResource> ();
		}

		public void GetAllResourceGroups (WorkCompletedMethod workCompletedMethod)
		{
			DoWorkAsync ((s, args) => {
				args.Result = dataAccessService.GetResourceGroups ();
			}, workCompletedMethod);
		}
	}
}
