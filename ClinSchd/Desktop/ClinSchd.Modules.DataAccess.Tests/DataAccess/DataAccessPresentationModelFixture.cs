using System;
using System.Collections.Generic;
using Microsoft.Practices.Composite.Regions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Infrastructure.Models;
using ClinSchd.Modules.DataAccess.DataAccess;
using ClinSchd.Modules.DataAccess.Tests.Mocks;

namespace ClinSchd.Modules.DataAccess.Tests.DataAccess
{
    [TestClass]
    public class DataAccessPresentationModelFixture
    {
		//[TestMethod]
		//public void CanInitPresenter()
		//{
		//    MockDataAccessView view = new MockDataAccessView();
		//    MockDataAccessService DataAccessService = new MockDataAccessService();

		//    DataAccessPresentationModel presentationModel = new DataAccessPresentationModel(view, DataAccessService);
		//    Assert.AreEqual<IDataAccessView>(view, presentationModel.View);
		//}


		//[TestMethod]
		//public void ShowDataAccessDoesNothingIfDataAccessHasNoDataAccess()
		//{
		//    MockDataAccessView view = new MockDataAccessView();
		//    MockRegionManager regionManager = new MockRegionManager();
		//    regionManager.Regions.Add("MainToolBarRegion", new MockMainToolBarRegion());
		//    MockDataAccessService DataAccessService = new MockDataAccessService();
		//    DataAccessService.DataAccessDataAccesss = null;
		//    DataAccessPresentationModel presentationModel = new DataAccessPresentationModel(view, DataAccessService);
		//    presentationModel.Controller = new MockDataAccessController();

		//    presentationModel.SetTickerSymbol("InexistentDataAccess");

		//    Assert.AreEqual(0, ((MockMainToolBarRegion)regionManager.Regions["MainToolBarRegion"]).AddedViews.Count);
		//}

		//[TestMethod]
		//public void ShowDataAccessPassesDataAccessContentToView()
		//{
		//    MockDataAccessView view = new MockDataAccessView();
		//    MockDataAccessService DataAccessService = new MockDataAccessService();
		//    DataAccessService.DataAccessDataAccesss = new List<DataAccessDataAccess>() { new DataAccessDataAccess() { Title = "FUND0", Body = "My custom body text" } };
		//    DataAccessPresentationModel presentationModel = new DataAccessPresentationModel(view, DataAccessService);
		//    presentationModel.Controller = new MockDataAccessController();

		//    presentationModel.SetTickerSymbol("FUND0");

		//    Assert.AreEqual(1, view.Model.DataAccesss.Count);
		//    Assert.AreEqual("My custom body text", view.Model.DataAccesss[0].Body);
		//}

		//[TestMethod]
		//public void ViewContainsCorrectModelHeaderInfoAfterSetTickerSymbol()
		//{
		//    var view = new MockDataAccessView();
		//    var DataAccessService = new MockDataAccessService();
		//    DataAccessService.DataAccessDataAccesss = new List<DataAccessDataAccess>() { new DataAccessDataAccess() { Title = "MySymbol", IconUri = "MyPath" } };
		//    var presenter = new DataAccessPresentationModel(view, DataAccessService);
		//    presenter.Controller = new MockDataAccessController();

		//    presenter.SetTickerSymbol("MyTitle");

		//    Assert.IsNotNull(view.Model);
		//    Assert.IsNotNull(view.Model.DataAccesss);
		//    Assert.AreEqual(1, view.Model.DataAccesss.Count);
		//    Assert.AreEqual("MyPath", view.Model.DataAccesss[0].IconUri);
		//    Assert.AreEqual("MySymbol", view.Model.DataAccesss[0].Title);
		//}

		//[TestMethod]
		//public void DataAccessPresenterNotifiesControllerOnItemChange()
		//{
		//    var view = new MockDataAccessView();
		//    var DataAccessService = new MockDataAccessService();
		//    var mockController = new MockDataAccessController();
		//    DataAccessService.DataAccessDataAccesss = new List<DataAccessDataAccess>() { new DataAccessDataAccess() { Title = "MySymbol", IconUri = "MyPath" },
		//                                                             new DataAccessDataAccess() { Title = "OtherSymbol", IconUri = "OtherPath" }};
		//    var presenter = new DataAccessPresentationModel(view, DataAccessService);
		//    presenter.Controller = mockController;
		//    presenter.SetTickerSymbol("DoesNotMatter");

		//    presenter.SelectedDataAccess = DataAccessService.DataAccessDataAccesss[1];

		//    Assert.IsTrue(mockController.CurrentDataAccessDataAccessChangedCalled);
		//}

		//private class MockDataAccessService : IDataAccessService
		//{
		//    public IList<DataAccessDataAccess> DataAccessDataAccesss = new List<DataAccessDataAccess>()
		//                                                 {
		//                                                         new DataAccessDataAccess()
		//                                                             {Title = "Title", IconUri = "IconUri", Body = "Body", PublishedDate = DateTime.Now}
		//                                                 };


		//    #region IDataAccessService Members

		//    public IList<DataAccessDataAccess> GetDataAccess(string tickerSymbol)
		//    {
		//        return this.DataAccessDataAccesss;
		//    }

		//    public bool HasDataAccess(string tickerSymbol)
		//    {
		//        throw new NotImplementedException();
		//    }

		//    public event EventHandler<DataAccessEventArgs> Updated = delegate { };

		//    #endregion
		//}
    }
}