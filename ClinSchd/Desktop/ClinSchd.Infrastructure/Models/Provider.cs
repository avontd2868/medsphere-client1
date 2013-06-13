using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClinSchd.Infrastructure.Interfaces;
using Microsoft.Practices.Composite.Presentation.Commands;
using Microsoft.Practices.Composite.Events;

namespace ClinSchd.Infrastructure.Models
{
	public class Provider : AsyncValidatableModel
    {
		private IDataAccessService dataAccessService;
		public string IEN { get; set; }
		public string Name { get; set; }

		public Provider (IDataAccessService dataAccessService)
		{
			this.dataAccessService = dataAccessService;
		}

		private IList<SchdResource> resources;
		public IList<SchdResource> Resources
		{
			get
			{
				if (resources == null) {
					resources = dataAccessService.GetClinicsByProvider (this.IEN);
				}
				return resources;		
			}
		}

		public void GetAllProviders (WorkCompletedMethod workCompletedMethod)
		{
			DoWorkAsync ((s, args) => {
				args.Result = dataAccessService.GetProviders ();
			}, workCompletedMethod);
		}		
	}
}
