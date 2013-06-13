using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClinSchd.Modules.CheckIn.Services;

namespace ClinSchd.Modules.CheckIn.Tests.Services
{
    [TestClass]
	public class CheckInServiceFixture
    {
        [TestMethod]
        public void HavingACurrentCultureDifferentThanEnglishShouldNotThrows()
        {
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
			Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("es-AR");

			CheckInService CheckInService = new CheckInService ();

			Thread.CurrentThread.CurrentCulture = currentCulture;
		}
	}
}
