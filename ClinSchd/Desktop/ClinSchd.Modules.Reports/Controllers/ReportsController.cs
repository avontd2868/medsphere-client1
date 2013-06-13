using System;
using System.Collections.Generic;

using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Regions;

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.Reports.Helper_Classes;
using ClinSchd.Modules.Reports.Reports;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Presentation.Events;

namespace ClinSchd.Modules.Reports.Controllers
{
    public class ReportsController : IReportsController
    {
        private readonly IUnityContainer container;
        private readonly IEventAggregator eventAggregator;

		public ReportsController(
			IUnityContainer container,
			IEventAggregator eventAggregator)
        {
			this.container = container;
            this.eventAggregator = eventAggregator;
		}
		
		public void Run()
        {		
			this.eventAggregator.GetEvent<DisplayReportEvent> ().Subscribe (DisplayReportParamCollectionWindow, ThreadOption.UIThread, true);
			this.container.RegisterInstance<ServerPrinter> (new ServerPrinter());
		}

		public void DisplayReportParamCollectionWindow (object[] args)
		{
			ReportsModel model = container.Resolve<ReportsModel> ();
			model.ReportDisplayName = args[0].ToString ();
			if (model.RequiredParams.Count > 0 && model.RequiredParams.Count + 1 != args.Length) {
				ReportParamsPresentationModel reportParamsPresentationModel = container.Resolve<ReportParamsPresentationModel> ();
				reportParamsPresentationModel.Model = model;
				reportParamsPresentationModel.NotifyPropertyChanged ("Model");
				reportParamsPresentationModel.BuildRequiredControls ();
			} else {
				Dictionary<string,ReportParameter>.Enumerator paramsEnumerator = model.Params.GetEnumerator ();
				paramsEnumerator.MoveNext ();
				for (int i = 1; i < args.Length; i++) {
					paramsEnumerator.Current.Value.Value = args[i];
					paramsEnumerator.MoveNext ();
				}
				ReportsPresentationModel reportsPresentationModel = container.Resolve<ReportsPresentationModel> ();
				model.AsyncWrapper = reportsPresentationModel.Model.AsyncWrapper;
				reportsPresentationModel.Model = model;
				reportsPresentationModel.NotifyPropertyChanged ("Model");
			}
		}
    }
}