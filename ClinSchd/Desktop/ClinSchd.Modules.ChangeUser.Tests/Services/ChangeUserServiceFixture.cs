using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClinSchd.Modules.ChangeUser.Services;

namespace ClinSchd.Modules.ChangeUser.Tests.Services
{
    [TestClass]
	public class ChangeUserServiceFixture
    {
        [TestMethod]
        public void HavingACurrentCultureDifferentThanEnglishShouldNotThrows()
        {
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("es-AR");

			ChangeUserService ChangeUserService = new ChangeUserService ();

            Thread.CurrentThread.CurrentCulture = currentCulture;
        }
    }
}
