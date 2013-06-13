using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClinSchd.Modules.PatientSelection.Services;

namespace ClinSchd.Modules.PatientSelection.Tests.Services
{
    [TestClass]
    public class PatientSelectionServiceFixture
    {
        [TestMethod]
        public void HavingACurrentCultureDifferentThanEnglishShouldNotThrows()
        {
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("es-AR");

            PatientSelectionService PatientSelectionService = new PatientSelectionService();

            Thread.CurrentThread.CurrentCulture = currentCulture;
        }
    }
}
