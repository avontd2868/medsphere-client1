using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ClinSchd.Infrastructure.Models
{
	public class PassthroughAsyncModel : AsyncValidatableModel
	{
		public void InvokeAsync (WorkMethod methodToInvoke, WorkCompletedMethod callbackMethod)
		{
			this.DoWorkAsync ((s,args) => {methodToInvoke.Invoke(s,args);}, callbackMethod);
		}

		public delegate void WorkMethod (object s, DoWorkEventArgs args);

	}
}
