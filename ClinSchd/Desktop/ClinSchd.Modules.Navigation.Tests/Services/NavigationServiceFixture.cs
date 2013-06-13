using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClinSchd.Modules.Navigation.Services;

namespace ClinSchd.Modules.Navigation.Tests.Services
{
    [TestClass]
    public class NavigationServiceFixture
    {
        [TestMethod]
        public void HavingACurrentCultureDifferentThanEnglishShouldNotThrows()
        {
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("es-AR");

            NavigationService navigationService = new NavigationService();

            Thread.CurrentThread.CurrentCulture = currentCulture;
        }
    }
}
