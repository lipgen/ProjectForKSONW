﻿#pragma checksum "..\..\..\Pages\StatusPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "68A2B861C2BB87EEC90B6181B2BDE65F"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18408
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

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
using System.Windows.Shell;


namespace ModernUINavigationApp1.Pages {
    
    
    /// <summary>
    /// StatusPage
    /// </summary>
    public partial class StatusPage : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 27 "..\..\..\Pages\StatusPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label Label1;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\..\Pages\StatusPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label Label1Date;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\..\Pages\StatusPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label Label2;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\..\Pages\StatusPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label Label2Date;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\..\Pages\StatusPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ButtonUpdate;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\..\Pages\StatusPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox textBox;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\..\Pages\StatusPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\..\Pages\StatusPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ProgressBar pb;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\..\Pages\StatusPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock label1;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/ModernUINavigationApp1;component/pages/statuspage.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Pages\StatusPage.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.Label1 = ((System.Windows.Controls.Label)(target));
            return;
            case 2:
            this.Label1Date = ((System.Windows.Controls.Label)(target));
            return;
            case 3:
            this.Label2 = ((System.Windows.Controls.Label)(target));
            return;
            case 4:
            this.Label2Date = ((System.Windows.Controls.Label)(target));
            return;
            case 5:
            this.ButtonUpdate = ((System.Windows.Controls.Button)(target));
            
            #line 31 "..\..\..\Pages\StatusPage.xaml"
            this.ButtonUpdate.Click += new System.Windows.RoutedEventHandler(this.ButtonUpdate_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.textBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 7:
            this.button = ((System.Windows.Controls.Button)(target));
            
            #line 34 "..\..\..\Pages\StatusPage.xaml"
            this.button.Click += new System.Windows.RoutedEventHandler(this.button_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.pb = ((System.Windows.Controls.ProgressBar)(target));
            return;
            case 9:
            this.label1 = ((System.Windows.Controls.TextBlock)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
