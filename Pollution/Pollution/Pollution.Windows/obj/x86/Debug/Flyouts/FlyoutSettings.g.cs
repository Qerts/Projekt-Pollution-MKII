﻿

#pragma checksum "D:\Git\Projekt-Pollution-MKII\Pollution\Pollution\Pollution.Windows\Flyouts\FlyoutSettings.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "E84898028064FCEFC21FC3873E3CDB42"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Pollution.Flyouts
{
    partial class FlyoutSettings : global::Windows.UI.Xaml.Controls.SettingsFlyout, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 29 "..\..\..\Flyouts\FlyoutSettings.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.Selector)(target)).SelectionChanged += this.listBox1_SelectionChanged;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 41 "..\..\..\Flyouts\FlyoutSettings.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ToggleButton)(target)).Checked += this.checkLiveTile_Checked;
                 #line default
                 #line hidden
                #line 41 "..\..\..\Flyouts\FlyoutSettings.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ToggleButton)(target)).Unchecked += this.checkLiveTile_Checked;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 44 "..\..\..\Flyouts\FlyoutSettings.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.Selector)(target)).SelectionChanged += this.listLang_SelectionChanged;
                 #line default
                 #line hidden
                break;
            case 4:
                #line 25 "..\..\..\Flyouts\FlyoutSettings.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ToggleButton)(target)).Checked += this.checkNearest_Checked;
                 #line default
                 #line hidden
                #line 25 "..\..\..\Flyouts\FlyoutSettings.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ToggleButton)(target)).Unchecked += this.checkNearest_Checked;
                 #line default
                 #line hidden
                break;
            case 5:
                #line 26 "..\..\..\Flyouts\FlyoutSettings.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ToggleButton)(target)).Checked += this.checkNearestWithoutQuality_Checked;
                 #line default
                 #line hidden
                #line 26 "..\..\..\Flyouts\FlyoutSettings.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ToggleButton)(target)).Unchecked += this.checkNearestWithoutQuality_Checked;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


