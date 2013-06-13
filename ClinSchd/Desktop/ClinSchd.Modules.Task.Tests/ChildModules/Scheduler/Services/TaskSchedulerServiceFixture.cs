using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClinSchd.Modules.Task.Scheduler.Services;

namespace ClinSchd.Modules.Task.Scheduler.Tests.Services
{
    [TestClass]
	public class TaskSchedulerServiceFixture
    {
        [TestMethod]
        public void HavingACurrentCultureDifferentThanEnglishShouldNotThrows()
        {
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("es-AR");

			TaskSchedulerService taskSchedulerService = new TaskSchedulerService();

            Thread.CurrentThread.CurrentCulture = currentCulture;
        }
    }
}
