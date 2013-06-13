using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClinSchd.Modules.Ribbon.Services;

namespace ClinSchd.Modules.Ribbon.Tests.Services
{
    [TestClass]
    public class RibbonServiceFixture
    {
        [TestMethod]
        public void HavingACurrentCultureDifferentThanEnglishShouldNotThrows()
        {
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("es-AR");

            RibbonService RibbonService = new RibbonService();

            Thread.CurrentThread.CurrentCulture = currentCulture;
        }
    }
}
