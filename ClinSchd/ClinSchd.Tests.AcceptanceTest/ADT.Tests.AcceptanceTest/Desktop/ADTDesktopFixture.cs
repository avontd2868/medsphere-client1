//===================================================================================
// Microsoft patterns & practices
// Composite Application Guidance for Windows Presentation Foundation and Silverlight
//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===================================================================================
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
//===================================================================================
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.IO;
using System.Diagnostics;
using System.Threading;

using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Text;
using System.Windows.Automation.Provider;

using AcceptanceTestLibrary.ApplicationHelper;
using AcceptanceTestLibrary.Common;
using ADT.Tests.AcceptanceTest.TestEntities.Page;
using ADT.Tests.AcceptanceTest.TestEntities.Assertion;
using ADT.Tests.AcceptanceTest.TestEntities.Action;
using AcceptanceTestLibrary.ApplicationObserver;
using AcceptanceTestLibrary.Common.Desktop;
using System.Reflection;
using AcceptanceTestLibrary.TestEntityBase;

namespace ADT.Tests.AcceptanceTest.Desktop
{
    /// <summary>
    /// Acceptance test fixture for WPF application
    /// </summary>
#if DEBUG
    [DeploymentItem(@"..\Desktop\ADT\bin\Debug", "WPF")]
    [DeploymentItem(@".\ADT.Tests.AcceptanceTest\bin\Debug")]
#else
    [DeploymentItem(@"..\Desktop\ADT\bin\Release", "WPF")]
    [DeploymentItem(@".\ADT.Tests.AcceptanceTest\bin\Release")]
#endif
    [TestClass]
    public class ADTDesktopFixture : FixtureBase<WpfAppLauncher>
    {
        #region Additional test attributes

        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize()
        {
            string currentOutputPath = (new System.IO.DirectoryInfo(Assembly.GetExecutingAssembly().Location)).Parent.FullName;
            ADTPage<WpfAppLauncher>.Window = base.LaunchApplication(currentOutputPath + GetDesktopApplication(), GetDesktopApplicationProcess())[0];
        }

        // Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            PageBase<WpfAppLauncher>.DisposeWindow();
            Process p = WpfAppLauncher.GetCurrentAppProcess();
            base.UnloadApplication(p);
        }

        #endregion

        #region Test Methods

        #region Application Launch Test
        [TestMethod]
        public void DesktopApplicationLoadTest()
        {
            Assert.IsNotNull(ADTPage<WpfAppLauncher>.Window, "ADT is not launched.");
        }
        #endregion

        #endregion

        #region Position summary Module Load Test

        /// <summary>
        /// Tests if position summary details are loaded.
        /// </summary>
        [TestMethod]
        public void DesktopApplicationPositionSummaryTest()
        {
            InvokePositionSummaryAssert();
        }

        /// <summary>
        /// Tests the number of columns from position summary view table. 
        /// </summary>
        /// 

        [TestMethod]
        public void DesktopApplicationPositionSummaryColumnCountTest()
        {

            //For now the test data is hardcoded in resource file. But if the datasource is available it will be read from the datasource
            ADTAssertion<WpfAppLauncher>.DesktopAssertPositionSummaryColumnCount();
        }

        /// <summary>
        /// Tests the number of rows from position summary view table. 
        /// </summary>

        [TestMethod]
        public void DesktopApplicationPositionSummaryRowCountTest()
        {
            ADTAssertion<WpfAppLauncher>.AssertPositionSummaryRowCount();
        }

        /// <summary>
        /// Tests the computed value (Market value & Gain Loss %) with the value loaded in the screen
        /// </summary>
        /// 

        [TestMethod]
        public void DesktopApplicationPositionSummaryDataTest()
        {
            //For each Stock or Symbol take the old value and get the value from Web service and monitor that

            ADTAssertion<WpfAppLauncher>.AssertPositionSummaryValuesForSymbol();
        }

        #endregion

        #region Market Trend Test
        /// <summary>
        /// Tests the Historical Data Block is loaded 
        /// </summary>
        /// 

        [TestMethod]
        public void DesktopApplicationMarketTrendTest()
        {
            ADTAssertion<WpfAppLauncher>.AssertHistoricalDataText();
        }

        /// <summary>
        /// Tests the Pie Chart Data Block is loaded 
        /// </summary>
        /// 

        [TestMethod]
        public void DesktopApplicationPieChartTextLoadTest()
        {
            ADTAssertion<WpfAppLauncher>.AssertPieChartTextBlock();
        }
        #endregion

        #region PositionBuySellTab Test
        [TestMethod]
        public void DesktopPositionBuySellTabControlsLoadTest()
        {
            InvokeDesktopPositionBuySellTabControlsLoad("Buy");
        }


        [TestMethod]
        public void DesktopAttemptBuyStockWithValidData()
        {
            InvokeAttemptBuySellOrderWithValidData("Buy");
        }

        [TestMethod]
        public void DesktopAttemptBuyStockWithInValidData()
        {
            InvokeAttemptBuySellOrderWithInValidData("Buy");
        }
        [TestMethod]
        public void DesktopAttemptSellStockWithValidData()
        {
            InvokeAttemptBuySellOrderWithValidData("Sell");
        }
        [TestMethod]
        public void DesktopAttemptSellStockWithInValidData()
        {
            InvokeAttemptBuySellOrderWithInValidData("Sell");
        }
        [TestMethod]
        public void DesktopAttemptStockBuySellCancelByCancelButton()
        {
            InvokeAttemptOrderCancelByCancelButton();
        }
        [TestMethod]
        public void DesktopProcessMultipleStockBuySellBySubmitAllButtonforValidData()
        {
            InvokeProcessMultipleStockBuySellBySubmitAllButtonforValidData();
        }
        [TestMethod]
        public void DesktopProcessMultipleStockBuySellBySubmitAllButtonforInValidData()
        {
            InvokeProcessMultipleStockBuySellBySubmitAllButtonforInValidData();
        }
        [TestMethod]
        public void DesktopProcessMultipleStockBuySellByCancelAllButton()
        {
            InvokeProcessMultipleStockBuySellByCancelAllButton();
        }

        #endregion

        #region WatchList Module Test

        /// <summary>
        /// Tests the Watch List Grid is loaded 
        /// </summary>
        /// 
               [TestMethod]
        public void DesktopApplicationWatchListGridLoadTest()
        {
            ADTAssertion<WpfAppLauncher>.AssertWatchListGrid();
        }

     
        #endregion

        #region NewsArticle Module Load Test
        /// <summary>
        /// Tests the News Articles Data Block is loaded 
        /// </summary>
        /// 

        [TestMethod]
        public void DesktopApplicationNewsArticleTextLoadTest()
        {
            ADTAssertion<WpfAppLauncher>.AssertNewsArticleText();
        }
        #endregion

        #region Watch List Module Test
        
        /// <summary>
        /// Tests the AddtoWatchList Button and the text Box is loaded
        /// </summary>
        /// 

        [TestMethod]
        public void DesktopApplicationAddtoWatchListTextBoxLoadTest()
        {
            InvokeAddtoWatchListAssert();
        }


        [TestMethod]
        public void DesktopStockRemovedfromWatchListTextBoxTest()
        {
            InvokeStockRemovedfromWatchListTextBoxAssert();
        }

        /// <summary>
        /// Tests the stock added in the TextBox gets added to the Watch List Grid on Clicking the AddtoWatchList Button
        /// </summary>
        /// 
        [TestMethod]
        public void DesktopApplicationStockAddedinWatchListTextBoxTest()
        {
            InvokeStockAddedinWatchListTextBoxAssert();
        }
        #endregion

        #region private methods
        private static string GetDesktopApplicationProcess()
        {
            return ConfigHandler.GetValue("WpfAppProcessName");
        }

        private static string GetDesktopApplication()
        {
            return ConfigHandler.GetValue("WpfAppLocation");
        }

        private void InvokePositionSummaryAssert()
        {
            ADTAssertion<WpfAppLauncher>.AssertPositionSummaryTab();
            ADTAssertion<WpfAppLauncher>.AssertPositionSummaryGrid();
        }
        private void InvokeOrderToolBarAssert()
        {
            ADTAssertion<WpfAppLauncher>.AssertSubmitButton();
            ADTAssertion<WpfAppLauncher>.AssertSubmitAllButton();
            ADTAssertion<WpfAppLauncher>.AssertCancelButton();
            ADTAssertion<WpfAppLauncher>.AssertCancelAllButton();
        }
        private void InvokeAddtoWatchListAssert()
        {
            ADTAssertion<WpfAppLauncher>.AssertTextBoxBlock();
        }
        private void InvokeStockAddedinWatchListTextBoxAssert()
        {
            ADTAssertion<WpfAppLauncher>.AssertStockAddedinWatchListTextBox();
            ADTAssertion<WpfAppLauncher>.AssertWatchListRowCount();
        }

        private void InvokeStockRemovedfromWatchListTextBoxAssert()
        {
            ADTAssertion<WpfAppLauncher>.AssertStockAddedinWatchListTextBox();
            ADTAssertion<WpfAppLauncher>.AssertWatchListRowCount();
            ADTAssertion<WpfAppLauncher>.AssertStockRemovedfromWatchListTextBox();
        }

        private void InvokeDesktopPositionBuySellTabControlsLoad(string option)
        {
            ADTAssertion<WpfAppLauncher>.AssertPositionSummaryTab();
            ADTAssertion<WpfAppLauncher>.AssertPositionBuyButtonClickTest(option);

            ADTAssertion<WpfAppLauncher>.AssertTermComboBox();
            ADTAssertion<WpfAppLauncher>.AssertPriceLimitTextBox();
            ADTAssertion<WpfAppLauncher>.AssertSellRadioButton();
            ADTAssertion<WpfAppLauncher>.AssertBuyRadioButton();
            ADTAssertion<WpfAppLauncher>.AssertSharesTextBox();

            ADTAssertion<WpfAppLauncher>.AssertOrderTypeComboBox();
            ADTAssertion<WpfAppLauncher>.AssertOrderCommandSubmit();
            ADTAssertion<WpfAppLauncher>.AssertOrderCommandCancel();
            ADTAssertion<WpfAppLauncher>.AssertOrderCommandSubmitAllButton();
            ADTAssertion<WpfAppLauncher>.AssertOrderCommandCancelAllButton();
        }
        private void InvokeAttemptBuySellOrderWithValidData(string option)
        {
            InvokeDesktopPositionBuySellTabControlsLoad(option);
            ADTAssertion<WpfAppLauncher>.AssertAttemptBuySellOrderWithValidData();
        }

        private void InvokeAttemptBuySellOrderWithInValidData(string option)
        {
            InvokeDesktopPositionBuySellTabControlsLoad(option);
            ADTAssertion<WpfAppLauncher>.AssertAttemptBuySellOrderWithInValidData();
        }

        private void InvokeAttemptOrderCancelByCancelButton()
        {
            InvokeDesktopPositionBuySellTabControlsLoad("Buy");
            ADTAssertion<WpfAppLauncher>.AssertAttemptOrderCancelByCancelButton();
        }

        private void InvokeProcessMultipleStockBuySellBySubmitAllButtonforValidData()
        {
            InvokeDesktopPositionBuySellTabLoad();
            
            ADTAssertion<WpfAppLauncher>.AssertProcessMultipleStockBuySellBySubmitAllButtonforValidData();
        }

        private void InvokeDesktopPositionBuySellTabLoad()
        {
            ADTAssertion<WpfAppLauncher>.AssertPositionSummaryTab();
            ADTAssertion<WpfAppLauncher>.AssertPositionBuyButtonClickTest("Buy");
        }


        private void InvokeProcessMultipleStockBuySellBySubmitAllButtonforInValidData()
        {
            InvokeSilverLightPositionBuySellTabLoad();
            ADTAssertion<WpfAppLauncher>.AssertProcessMultipleStockBuySellBySubmitAllButtonforInValidData();
        }
        private void InvokeSilverLightPositionBuySellTabLoad()
        {
            ADTAssertion<WpfAppLauncher>.AssertPositionSummaryTab();
            ADTAssertion<WpfAppLauncher>.AssertPositionBuyButtonClickTest("Buy");
        }


        private void InvokeProcessMultipleStockBuySellByCancelAllButton()
        {
            InvokeDesktopPositionBuySellTabLoad();
            ADTAssertion<WpfAppLauncher>.AssertProcessMultipleStockBuySellByCancelAllButton();
        }
        #endregion
    }
}
