using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClinSchd.Modules.CheckOut.Services;

namespace ClinSchd.Modules.CheckOut.Tests.Services
{
    [TestClass]
	public class CheckOutServiceFixture
    {
        [TestMethod]
        public void HavingACurrentCultureDifferentThanEnglishShouldNotThrows()
        {
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
			Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("es-AR");

			CheckOutService CheckOutService = new CheckOutService ();

			Thread.CurrentThread.CurrentCulture = currentCulture;
		}
	}
}
