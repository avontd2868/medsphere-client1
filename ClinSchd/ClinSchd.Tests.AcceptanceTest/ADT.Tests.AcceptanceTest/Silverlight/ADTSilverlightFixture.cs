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

using System.Threading;
using System.IO;
using System.Diagnostics;

using System.Windows;
using System.Windows.Forms;

using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Text;
using System.Windows.Automation.Provider;

using AcceptanceTestLibrary.Common;
using ADT.Tests.AcceptanceTest.TestEntities.Page;
using ADT.Tests.AcceptanceTest.TestEntities.Assertion;
using AcceptanceTestLibrary.ApplicationObserver;
using AcceptanceTestLibrary.Common.Silverlight;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using AcceptanceTestLibrary.ApplicationHelper;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using AcceptanceTestLibrary.TestEntityBase;
using System.Reflection;


namespace ADT.Tests.AcceptanceTest.Silverlight
{
    /// <summary>
    /// Summary description for SilverlightAcceptanceTest
    /// </summary>
#if DEBUG
    [DeploymentItem(@"..\Silverlight\ADT\bin\Debug", "Silverlight")]
    [DeploymentItem(@".\ADT.Tests.AcceptanceTest\bin\Debug")]
#else
    [DeploymentItem(@"..\Silverlight\ADT\bin\Release", "Silverlight")]
    [DeploymentItem(@".\ADT.Tests.AcceptanceTest\bin\Release")]
#endif
    [TestClass]
    public class ADTSilverlightFixture : FixtureBase<SilverlightAppLauncher>
    {
        private const int BACKTRACKLENGTH = 5;

        #region Additional test attributes
        
        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize()
        {
            string currentOutputPath = (new System.IO.DirectoryInfo(Assembly.GetExecutingAssembly().Location)).Parent.FullName;
            ADTPage<SilverlightAppLauncher>.Window = base.LaunchApplication(currentOutputPath + GetSilverlightApplication(), GetBrowserTitle())[0];
        }
        
        // Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup() 
        {
            PageBase<SilverlightAppLauncher>.DisposeWindow();
            SilverlightAppLauncher.UnloadBrowser(GetBrowserTitle());
        }
        
        #endregion

        #region Application Launch Test

        /// <summary>
        /// Tests if RI is launched in silverlight 
        /// </summary>
        [TestMethod]
        public void SilverlightApplicationLoadTest()
        {
            Assert.IsNotNull(ADTPage<SilverlightAppLauncher>.Window, "ADT is not launched.");
        }
        [TestMethod]
        public void SilverLightNewsArticleTextLoadTest()
        {
            ADTAssertion<SilverlightAppLauncher>.AssertNewsArticleText();
        }

        #endregion

        #region Position summary Module Load Test
        /// <summary>
        /// Tests if position summary details are loaded.
        /// </summary>
        [TestMethod]
        public void SilverlightApplicationPositionSummaryTest()
        {
            InvokePositionSummaryAssert();
        }

        /// <summary>
        /// Tests the number of columns from position summary view table. 
        /// </summary>
        [TestMethod]
        public void SilverlightApplicationPositionSummaryColumnCountTest()
        {
            //For now the test data is hardcoded in resource file. But if the datasource is available it will be read from the datasource
            ADTAssertion<SilverlightAppLauncher>.AssertPositionSummaryColumnCount();
        }

        /// <summary>
        /// Tests the number of rows from position summary view table. 
        /// </summary>
        [TestMethod]
        public void SilverlightApplicationPositionSummaryRowCountTest()
        {
            ADTAssertion<SilverlightAppLauncher>.AssertPositionSummaryRowCount();
        }

        /// <summary>
        /// Tests the computed value (Market value & Gain Loss %) with the value loaded in the screen
        /// </summary>
        [TestMethod]
        public void SilverlightApplicationPositionSummaryDataTest()
        {
            ADTAssertion<SilverlightAppLauncher>.AssertPositionSummaryValuesForSymbol();
        }
        #endregion

        #region Market Trend Data Test
        /// <summary>
        /// Tests if historical data textblock is loaded.
        /// </summary>
        [TestMethod]
        public void SilverlightApplicationMarketTrendTest()
        {
            ADTAssertion<SilverlightAppLauncher>.AssertHistoricalDataText();
        }

        /// <summary>
        /// Tests if historical data textblock is loaded.
        /// </summary>
        [TestMethod]
        public void SilverlightApplicationPieChartTextBlockTest()
        {
            ADTAssertion<SilverlightAppLauncher>.AssertPieChartTextBlock();
        }
        

        #endregion

        #region WatchList Module Test

        /// <summary>
        /// Tests the Watch List Grid is loaded 
        /// </summary>
        /// 
        
        [TestMethod]
        public void SilverLightWatchListGridLoadTest()
        {
            ADTAssertion<SilverlightAppLauncher>.AssertWatchListGrid();
        }
        #endregion

        #region Watch List Module Test
        
        /// <summary>
        /// Tests the AddtoWatchList Button and the text Box is loaded
        /// </summary>
        /// 
        [TestMethod]
        public void SilverLightApplicationAddtoWatchListTextBoxLoadTest()
        {
            InvokeAddtoWatchListAssert();
        }
        
        [TestMethod]
        public void SilverLightStockAddedinWatchListTextBoxTest()
        {
            InvokeStockAddedinWatchListTextBoxAssert();
        }
        [TestMethod]
        public void SilverLightStockRemovedfromWatchListTextBoxTest()
        {
            InvokeStockRemovedfromWatchListTextBoxAssert();
        }
        #endregion

        #region PositionBuySellTab Test
        [TestMethod]
        public void SilverLightPositionBuySellTabControlsLoadTest()
        {
            InvokeSilverLightPositionBuySellTabControlsLoad("Buy");            
        }

        
        [TestMethod]        
        public void SilverLightAttemptBuyStockWithValidData()
        {
            InvokeAttemptBuySellOrderWithValidData("Buy");
        }
       
        [TestMethod]
        public void SilverLightAttemptBuyStockWithInValidData()
        {
            InvokeAttemptBuySellOrderWithInValidData("Buy");
        }
        [TestMethod]
        public void SilverLightAttemptSellStockWithValidData()
        {
            InvokeAttemptBuySellOrderWithValidData("Sell");
        }
        [TestMethod]
        public void SilverLightAttemptSellStockWithInValidData()
        {
            InvokeAttemptBuySellOrderWithInValidData("Sell");
        }
        [TestMethod]
        public void SilverLightAttemptStockBuySellCancelByCancelButton()
        {
            InvokeAttemptOrderCancelByCancelButton();
        }
       [TestMethod]
        public void SilverLightProcessMultipleStockBuySellBySubmitAllButtonforValidData()
        {
            InvokeProcessMultipleStockBuySellBySubmitAllButtonforValidData();
        }
        [TestMethod]
        public void SilverLightProcessMultipleStockBuySellBySubmitAllButtonforInValidData()
        {
            InvokeProcessMultipleStockBuySellBySubmitAllButtonforInValidData();
        }
        [TestMethod]
      public void SilverLightProcessMultipleStockBuySellByCancelAllButton()
        {
            InvokeProcessMultipleStockBuySellByCancelAllButton();
        }

        #endregion

        #region private Methods

        private static string GetSilverlightApplication()
        {
            return ConfigHandler.GetValue("SilverlightAppLocation");
        }

        private static string GetSilverlightApplicationPath(int backTrackLength)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            if (!String.IsNullOrEmpty(currentDirectory) && Directory.Exists(currentDirectory))
            {
                for (int iIndex = 0; iIndex < backTrackLength; iIndex++)
                {
                    currentDirectory = Directory.GetParent(currentDirectory).ToString();
                }
            }
            return  currentDirectory + GetSilverlightApplication();
        }

        private static string GetBrowserTitle()
        {
            return new ResXConfigHandler(ConfigHandler.GetValue("ControlIdentifiersFile")).GetValue("SilverlightAppTitle");
        }

        private void InvokePositionSummaryAssert()
        {
            ADTAssertion<SilverlightAppLauncher>.AssertPositionSummaryTab();
            ADTAssertion<SilverlightAppLauncher>.AssertPositionSummaryGrid();
        }
        
        private void InvokeOrderToolBarAssert()
        {
            ADTAssertion<SilverlightAppLauncher>.AssertSubmitButton();
            ADTAssertion<SilverlightAppLauncher>.AssertSubmitAllButton();
            ADTAssertion<SilverlightAppLauncher>.AssertCancelButton();
            ADTAssertion<SilverlightAppLauncher>.AssertCancelAllButton();
        }

        private void InvokeAddtoWatchListAssert()
        {
            ADTAssertion<SilverlightAppLauncher>.AssertTextBoxBlock();
            
        }

        private void InvokeStockAddedinWatchListTextBoxAssert()
        {
            ADTAssertion<SilverlightAppLauncher>.AssertStockAddedinWatchListTextBox();
            ADTAssertion<SilverlightAppLauncher>.AssertWatchListRowCount();
        }
        private void InvokeStockRemovedfromWatchListTextBoxAssert()
        {
            ADTAssertion<SilverlightAppLauncher>.AssertStockAddedinWatchListTextBox();
            ADTAssertion<SilverlightAppLauncher>.AssertWatchListRowCount();
            ADTAssertion<SilverlightAppLauncher>.AssertStockRemovedfromWatchListTextBox();
        }
                       
        private void InvokeSilverLightPositionBuySellTabControlsLoad(string option)
        {
            //ADTAssertion<SilverlightAppLauncher>.AssertPositionSummaryTab();
            ADTAssertion<SilverlightAppLauncher>.AssertPositionBuyButtonClickTest(option);
            
            ADTAssertion<SilverlightAppLauncher>.AssertTermComboBox();
            ADTAssertion<SilverlightAppLauncher>.AssertPriceLimitTextBox();
            ADTAssertion<SilverlightAppLauncher>.AssertSellRadioButton();
            ADTAssertion<SilverlightAppLauncher>.AssertBuyRadioButton();
            ADTAssertion<SilverlightAppLauncher>.AssertSharesTextBox();
            
            ADTAssertion<SilverlightAppLauncher>.AssertOrderTypeComboBox();
            ADTAssertion<SilverlightAppLauncher>.AssertOrderCommandSubmit();
            ADTAssertion<SilverlightAppLauncher>.AssertOrderCommandCancel();
            ADTAssertion<SilverlightAppLauncher>.AssertOrderCommandSubmitAllButton();
            ADTAssertion<SilverlightAppLauncher>.AssertOrderCommandCancelAllButton();
        }

        private void InvokeAttemptBuySellOrderWithValidData(string option)
        {
            InvokeSilverLightPositionBuySellTabControlsLoad(option);
            ADTAssertion<SilverlightAppLauncher>.AssertAttemptBuySellOrderWithValidData();
        }

        private void InvokeAttemptBuySellOrderWithInValidData(string option)
        {
            InvokeSilverLightPositionBuySellTabControlsLoad(option);
            ADTAssertion<SilverlightAppLauncher>.AssertAttemptBuySellOrderWithInValidData();
        }

        private void InvokeAttemptOrderCancelByCancelButton()
        {
            InvokeSilverLightPositionBuySellTabControlsLoad("Buy");
            ADTAssertion<SilverlightAppLauncher>.AssertAttemptOrderCancelByCancelButton();
        }

        private void InvokeProcessMultipleStockBuySellBySubmitAllButtonforValidData()
        {
            InvokeSilverLightPositionBuySellTabLoad();
            ADTAssertion<SilverlightAppLauncher>.AssertProcessMultipleStockBuySellBySubmitAllButtonforValidData();
        }

        private void InvokeProcessMultipleStockBuySellBySubmitAllButtonforInValidData()
        {
            InvokeSilverLightPositionBuySellTabLoad();
            ADTAssertion<SilverlightAppLauncher>.AssertProcessMultipleStockBuySellBySubmitAllButtonforInValidData();
        }
        private void InvokeSilverLightPositionBuySellTabLoad()
        {
            ADTAssertion<SilverlightAppLauncher>.AssertPositionSummaryTab();
            ADTAssertion<SilverlightAppLauncher>.AssertPositionBuyButtonClickTest("Buy");
        }


        private void InvokeProcessMultipleStockBuySellByCancelAllButton()
        {
            InvokeSilverLightPositionBuySellTabLoad();
            ADTAssertion<SilverlightAppLauncher>.AssertProcessMultipleStockBuySellByCancelAllButton();
        }
        #endregion
    }
}
