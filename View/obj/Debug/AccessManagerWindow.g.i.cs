﻿#pragma checksum "..\..\AccessManagerWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "48A8840B4D7863C42FD0F6BFAD1B63D0AFB5DC3342A0E5F4B3B5D13DDFFE950C"
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
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
using View;


namespace View {
    
    
    /// <summary>
    /// AccessManagerWindow
    /// </summary>
    public partial class AccessManagerWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 10 "..\..\AccessManagerWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox NonAdminTextComboBox;
        
        #line default
        #line hidden
        
        
        #line 11 "..\..\AccessManagerWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox TextsComboBox;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\AccessManagerWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox NonAdminsComboBox;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\AccessManagerWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button GrantTextAccess;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\AccessManagerWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button GranrAdminStatus;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\AccessManagerWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button RemoveTextAccessButton;
        
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
            System.Uri resourceLocater = new System.Uri("/View;component/accessmanagerwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\AccessManagerWindow.xaml"
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
            this.NonAdminTextComboBox = ((System.Windows.Controls.ComboBox)(target));
            
            #line 10 "..\..\AccessManagerWindow.xaml"
            this.NonAdminTextComboBox.DropDownOpened += new System.EventHandler(this.NonAdminTextComboBox_DropDownOpened);
            
            #line default
            #line hidden
            return;
            case 2:
            this.TextsComboBox = ((System.Windows.Controls.ComboBox)(target));
            
            #line 11 "..\..\AccessManagerWindow.xaml"
            this.TextsComboBox.DropDownOpened += new System.EventHandler(this.TextsComboBox_DropDownOpened);
            
            #line default
            #line hidden
            return;
            case 3:
            this.NonAdminsComboBox = ((System.Windows.Controls.ComboBox)(target));
            
            #line 14 "..\..\AccessManagerWindow.xaml"
            this.NonAdminsComboBox.DropDownOpened += new System.EventHandler(this.NonAdminsComboBox_DropDownOpened);
            
            #line default
            #line hidden
            return;
            case 4:
            this.GrantTextAccess = ((System.Windows.Controls.Button)(target));
            
            #line 15 "..\..\AccessManagerWindow.xaml"
            this.GrantTextAccess.Click += new System.Windows.RoutedEventHandler(this.GrantTextAccess_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.GranrAdminStatus = ((System.Windows.Controls.Button)(target));
            
            #line 16 "..\..\AccessManagerWindow.xaml"
            this.GranrAdminStatus.Click += new System.Windows.RoutedEventHandler(this.GranrAdminStatus_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.RemoveTextAccessButton = ((System.Windows.Controls.Button)(target));
            
            #line 17 "..\..\AccessManagerWindow.xaml"
            this.RemoveTextAccessButton.Click += new System.Windows.RoutedEventHandler(this.RemoveTextAccessButton_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
