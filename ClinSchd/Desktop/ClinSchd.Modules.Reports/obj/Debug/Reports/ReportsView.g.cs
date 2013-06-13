﻿#pragma checksum "..\..\..\Reports\ReportsView.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "97DAA17AB9E2DBE66A1670DE34C2EFAA"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.296
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using ClinSchd.Infrastructure;
using ClinSchd.Infrastructure.Controls;
using Microsoft.Practices.Composite.Presentation.Commands;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Carousel;
using Telerik.Windows.Controls.Docking;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Data;


namespace ClinSchd.Modules.Reports.Reports {
    
    
    /// <summary>
    /// ReportsView
    /// </summary>
    public partial class ReportsView : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 25 "..\..\..\Reports\ReportsView.xaml"
        internal System.Windows.Controls.TextBox ReportTextBox;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\..\Reports\ReportsView.xaml"
        internal Telerik.Windows.Controls.RadButton Print;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\..\Reports\ReportsView.xaml"
        internal Telerik.Windows.Controls.RadButton PrintServer;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\..\Reports\ReportsView.xaml"
        internal System.Windows.Controls.ComboBox ServerPrinterCmb;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\..\Reports\ReportsView.xaml"
        internal Telerik.Windows.Controls.RadNumericUpDown FontSizeUpDown;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\..\Reports\ReportsView.xaml"
        internal Telerik.Windows.Controls.RadButton OK;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/ClinSchd.Modules.Reports;component/reports/reportsview.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Reports\ReportsView.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.ReportTextBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 2:
            this.Print = ((Telerik.Windows.Controls.RadButton)(target));
            return;
            case 3:
            this.PrintServer = ((Telerik.Windows.Controls.RadButton)(target));
            return;
            case 4:
            this.ServerPrinterCmb = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 5:
            this.FontSizeUpDown = ((Telerik.Windows.Controls.RadNumericUpDown)(target));
            return;
            case 6:
            this.OK = ((Telerik.Windows.Controls.RadButton)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

