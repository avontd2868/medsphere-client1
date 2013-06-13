using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClinSchd.Modules.ChangeDivision.Services;

namespace ClinSchd.Modules.ChangeDivision.Tests.Services
{
    [TestClass]
	public class ChangeDivisionServiceFixture
    {
        [TestMethod]
        public void HavingACurrentCultureDifferentThanEnglishShouldNotThrows()
        {
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("es-AR");

			ChangeDivisionService ChangeDivisionService = new ChangeDivisionService ();

            Thread.CurrentThread.CurrentCulture = currentCulture;
        }
    }
}
