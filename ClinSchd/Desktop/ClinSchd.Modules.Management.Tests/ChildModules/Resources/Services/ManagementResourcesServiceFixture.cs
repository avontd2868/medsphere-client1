using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClinSchd.Modules.Management.Resources.Services;

namespace ClinSchd.Modules.Management.Resources.Tests.Services
{
    [TestClass]
	public class ManagementResourcesServiceFixture
    {
        [TestMethod]
        public void HavingACurrentCultureDifferentThanEnglishShouldNotThrows()
        {
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("es-AR");

			ManagementResourcesService ManagementResourcesService = new ManagementResourcesService ();

            Thread.CurrentThread.CurrentCulture = currentCulture;
        }
    }
}
