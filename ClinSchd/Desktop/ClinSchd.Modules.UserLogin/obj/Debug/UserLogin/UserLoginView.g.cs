﻿#pragma checksum "..\..\..\UserLogin\UserLoginView.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "F84DB03CB79AF76B45333818546D8516"
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


namespace ClinSchd.Modules.UserLogin.UserLogin {
    
    
    /// <summary>
    /// UserLoginView
    /// </summary>
    public partial class UserLoginView : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 21 "..\..\..\UserLogin\UserLoginView.xaml"
        internal System.Windows.Controls.PasswordBox AccessCode;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\..\UserLogin\UserLoginView.xaml"
        internal System.Windows.Controls.PasswordBox VerifyCode;
        
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
            System.Uri resourceLocater = new System.Uri("/ClinSchd.Modules.UserLogin;component/userlogin/userloginview.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\UserLogin\UserLoginView.xaml"
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
            this.AccessCode = ((System.Windows.Controls.PasswordBox)(target));
            return;
            case 2:
            this.VerifyCode = ((System.Windows.Controls.PasswordBox)(target));
            return;
            case 3:
            
            #line 27 "..\..\..\UserLogin\UserLoginView.xaml"
            ((Telerik.Windows.Controls.RadButton)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_OK_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 28 "..\..\..\UserLogin\UserLoginView.xaml"
            ((Telerik.Windows.Controls.RadButton)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Cancel_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

