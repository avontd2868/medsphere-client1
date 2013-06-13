using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClinSchd.Modules.DataAccess.Services;

namespace ClinSchd.Modules.DataAccess.Tests.Services
{
    [TestClass]
    public class DataAccessServiceFixture
    {
        [TestMethod]
        public void HavingACurrentCultureDifferentThanEnglishShouldNotThrows()
        {
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("es-AR");

            //DataAccessService DataAccessService = new DataAccessService();

            Thread.CurrentThread.CurrentCulture = currentCulture;
        }
    }
}
