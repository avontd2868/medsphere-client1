﻿#pragma checksum "..\..\..\Group\GroupView.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "B75FE81365B05D86179B26CACC0B50D5"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.296
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using ClinSchd.Infrastructure.Models;
using Microsoft.Practices.Composite.Presentation.Commands;
using Microsoft.Practices.Composite.Presentation.Regions;
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
using Telerik.Windows.Controls.Scheduler;
using Telerik.Windows.Data;


namespace ClinSchd.Modules.Task.Group {
    
    
    /// <summary>
    /// GroupView
    /// </summary>
    public partial class GroupView : Telerik.Windows.Controls.RadPane, System.Windows.Markup.IComponentConnector {
        
        
        #line 14 "..\..\..\Group\GroupView.xaml"
        internal Telerik.Windows.Controls.RadDocking TasksDocking;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\..\Group\GroupView.xaml"
        internal Telerik.Windows.Controls.RadPaneGroup SchedulerGroup;
        
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
            System.Uri resourceLocater = new System.Uri("/ClinSchd.Modules.Task;component/group/groupview.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Group\GroupView.xaml"
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
            this.TasksDocking = ((Telerik.Windows.Controls.RadDocking)(target));
            
            #line 14 "..\..\..\Group\GroupView.xaml"
            this.TasksDocking.Close += new System.EventHandler<Telerik.Windows.Controls.Docking.StateChangeEventArgs>(this.TasksDocking_Close);
            
            #line default
            #line hidden
            return;
            case 2:
            this.SchedulerGroup = ((Telerik.Windows.Controls.RadPaneGroup)(target));
            
            #line 17 "..\..\..\Group\GroupView.xaml"
            this.SchedulerGroup.SelectionChanged += new System.Windows.RoutedEventHandler(this.RadPaneGroup_SelectionChanged);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

