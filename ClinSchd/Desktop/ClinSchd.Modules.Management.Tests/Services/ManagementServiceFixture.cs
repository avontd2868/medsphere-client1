using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClinSchd.Modules.Management.Services;

namespace ClinSchd.Modules.Management.Tests.Services
{
    [TestClass]
    public class ManagementServiceFixture
    {
        [TestMethod]
        public void HavingACurrentCultureDifferentThanEnglishShouldNotThrows()
        {
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("es-AR");

            ManagementService ManagementService = new ManagementService();

            Thread.CurrentThread.CurrentCulture = currentCulture;
        }
    }
}
