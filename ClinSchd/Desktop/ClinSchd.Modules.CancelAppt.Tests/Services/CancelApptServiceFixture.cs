using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClinSchd.Modules.CancelAppt.Services;

namespace ClinSchd.Modules.CancelAppt.Tests.Services
{
    [TestClass]
	public class CancelApptServiceFixture
    {
        [TestMethod]
        public void HavingACurrentCultureDifferentThanEnglishShouldNotThrows()
        {
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
			Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("es-AR");

			CancelApptService CancelApptService = new CancelApptService ();

			Thread.CurrentThread.CurrentCulture = currentCulture;
		}
	}
}
