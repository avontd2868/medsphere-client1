using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClinSchd.Modules.ChangeServer.Services;

namespace ClinSchd.Modules.ChangeServer.Tests.Services
{
    [TestClass]
	public class ChangeServerServiceFixture
    {
        [TestMethod]
        public void HavingACurrentCultureDifferentThanEnglishShouldNotThrows()
        {
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("es-AR");

			ChangeServerService ChangeServerService = new ChangeServerService ();

            Thread.CurrentThread.CurrentCulture = currentCulture;
        }
    }
}
