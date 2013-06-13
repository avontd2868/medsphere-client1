using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClinSchd.Modules.Task.Services;

namespace ClinSchd.Modules.Task.Tests.Services
{
    [TestClass]
    public class TaskServiceFixture
    {
        [TestMethod]
        public void HavingACurrentCultureDifferentThanEnglishShouldNotThrows()
        {
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("es-AR");

            TaskService taskService = new TaskService();

            Thread.CurrentThread.CurrentCulture = currentCulture;
        }
    }
}
