using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClinSchd.Modules.Management.AccessGroups.Services;

namespace ClinSchd.Modules.Management.AccessGroups.Tests.Services
{
    [TestClass]
	public class ManagementAccessGroupsServiceFixture
    {
        [TestMethod]
        public void HavingACurrentCultureDifferentThanEnglishShouldNotThrows()
        {
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("es-AR");

			ManagementAccessGroupsService ManagementAccessGroupsService = new ManagementAccessGroupsService();

            Thread.CurrentThread.CurrentCulture = currentCulture;
        }
    }
}
