using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClinSchd.Modules.MarkAsNoShow.Services;

namespace ClinSchd.Modules.MarkAsNoShow.Tests.Services
{
	[TestClass]
	public class MarkAsNoShowServiceFixture
	{
		[TestMethod]
		public void HavingACurrentCultureDifferentThanEnglishShouldNotThrows()
		{
			CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
			Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("es-AR");

			MarkAsNoShowService MarkAsNoShowService = new MarkAsNoShowService ();

			Thread.CurrentThread.CurrentCulture = currentCulture;
		}
	}
}
