using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.DataAccess.DataAccess;
using ClinSchd.Modules.DataAccess.Controllers;
using ClinSchd.Modules.DataAccess.Services;
using ClinSchd.Modules.DataAccess.Tests.Mocks;

namespace ClinSchd.Modules.DataAccess.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class DataAccessModuleFixture
    {
        private MockUnityResolver container;
        private MockDataAccessController controller;
    }
}
