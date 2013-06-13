using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Regions;
using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.DataAccess.DataAccess;

namespace ClinSchd.Modules.DataAccess.Controllers
{
    public class DataAccessController : IDataAccessController
    {
        private readonly IRegionManager regionManager;
        private readonly IDataAccessPresentationModel dataAccessPresentationModel;
        private readonly IEventAggregator eventAggregator;
		//private readonly IRegion shellRegion;

        public DataAccessController(IRegionManager regionManager, IDataAccessPresentationModel dataAccessPresentationModel, IEventAggregator eventAggregator)
        {
            this.regionManager = regionManager;
            this.dataAccessPresentationModel = dataAccessPresentationModel;
            this.eventAggregator = eventAggregator;
            this.dataAccessPresentationModel.Controller = this;
        }

        public void Run()
        {
			this.regionManager.Regions[RegionNames.MainToolBarRegion].Add(dataAccessPresentationModel.View);
        }
    }
}
