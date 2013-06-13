using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClinSchd.Infrastructure.Interfaces;
using ClinSchd.Modules.MarkAsNoShow.MarkAsNoShow;
using ClinSchd.Modules.MarkAsNoShow.Controllers;
using ClinSchd.Modules.MarkAsNoShow.Services;
using ClinSchd.Modules.MarkAsNoShow.Tests.Mocks;

namespace ClinSchd.Modules.MarkAsNoShow.Tests
{
	/// <summary>
	/// Summary description for UnitTest1
	/// </summary>
	[TestClass]
	public class MarkAsNoShowModuleFixture
	{
		private MockUnityResolver container;
		private MockMarkAsNoShowController controller;

		[TestMethod]
		public void MarkAsNoShowModuleRegistersMarkAsNoShowViewAndMarkAsNoShowService ()
		{
			TestableMarkAsNoShowModule MarkAsNoShowModule = CreateTestableMarkAsNoShowModule ();

			MarkAsNoShowModule.InvokeRegisterViewsAndServices ();

			Assert.AreEqual (typeof (MarkAsNoShowView), container.Types[typeof (IMarkAsNoShowView)]);
			Assert.AreEqual (typeof (MarkAsNoShowController), container.Types[typeof (IMarkAsNoShowController)]);
			Assert.AreEqual (typeof (MarkAsNoShowPresentationModel), container.Types[typeof (IMarkAsNoShowPresentationModel)]);
			Assert.AreEqual (typeof (MarkAsNoShowService), container.Types[typeof (IMarkAsNoShowService)]);
#if !SILVERLIGHT
#endif
		}

		[TestMethod]
		public void InitCallsRunOnMarkAsNoShowController ()
		{
			var MarkAsNoShowModule = CreateTestableMarkAsNoShowModule ();

			MarkAsNoShowModule.Initialize ();

			Assert.IsTrue(controller.RunCalled);
		}

		private TestableMarkAsNoShowModule CreateTestableMarkAsNoShowModule ()
		{
			this.container = new MockUnityResolver();
			this.controller = new MockMarkAsNoShowController ();

			container.Bag.Add (typeof (IMarkAsNoShowController), controller);
			container.Bag.Add (typeof (IMarkAsNoShowView), null);

			return new TestableMarkAsNoShowModule (container);
		}

		internal class TestableMarkAsNoShowModule : MarkAsNoShowModule
		{
			public TestableMarkAsNoShowModule (IUnityContainer container)
				: base(container, null)
			{
			}

			public void InvokeRegisterViewsAndServices()
			{
				base.RegisterViewsAndServices();
			}
		}
	}
}
