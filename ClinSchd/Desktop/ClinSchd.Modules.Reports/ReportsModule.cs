using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.Reports.Controllers;
using ClinSchd.Modules.Reports.Reports;

namespace ClinSchd.Modules.Reports
{
	public class ReportsModule : IModule
	{
		private readonly IUnityContainer container;

		public ReportsModule(IUnityContainer container)
        {
            this.container = container;
		}

        public void Initialize()
        {
			this.RegisterViewsAndServices();

			IReportsController controller = this.container.Resolve<IReportsController>();
			controller.Run();
		}

        protected void RegisterViewsAndServices()
        {
			this.container.RegisterType<IReportsController, ReportsController>();
			this.container.RegisterType<IReportsView, ReportsView> ();
			this.container.RegisterType<IReportsPresentationModel, ReportsPresentationModel> ();
			this.container.RegisterType<IReportParamsPresentationModel, ReportParamsPresentationModel> ();
			this.container.RegisterType<IReportParamsView, ReportParamsView> ();
        }
	}
}