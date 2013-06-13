using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClinSchd.Modules.ResourceSelection.Services;

namespace ClinSchd.Modules.ResourceSelection.Tests.Services
{
    [TestClass]
    public class ResourceSelectionServiceFixture
    {
        [TestMethod]
        public void HavingACurrentCultureDifferentThanEnglishShouldNotThrows()
        {
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("es-AR");

			ResourceSelectionService ResourceSelectionService = new ResourceSelectionService ();

            Thread.CurrentThread.CurrentCulture = currentCulture;
        }
    }
}
