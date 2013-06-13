using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClinSchd.Modules.PatientAppt.Services;

namespace ClinSchd.Modules.PatientAppt.Tests.Services
{
    [TestClass]
    public class PatientApptServiceFixture
    {
        [TestMethod]
        public void HavingACurrentCultureDifferentThanEnglishShouldNotThrows()
        {
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("es-AR");

            PatientApptService PatientApptService = new PatientApptService();

            Thread.CurrentThread.CurrentCulture = currentCulture;
        }
    }
}
