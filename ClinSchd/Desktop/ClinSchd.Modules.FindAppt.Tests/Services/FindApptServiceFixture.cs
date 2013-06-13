using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClinSchd.Modules.FindAppt.Services;

namespace ClinSchd.Modules.FindAppt.Tests.Services
{
    [TestClass]
	public class FindApptServiceFixture
    {
        [TestMethod]
        public void HavingACurrentCultureDifferentThanEnglishShouldNotThrows()
        {
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
			Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("es-AR");

			FindApptService FindApptService = new FindApptService ();

			Thread.CurrentThread.CurrentCulture = currentCulture;
		}
	}
}
