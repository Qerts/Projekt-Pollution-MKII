﻿

#pragma checksum "D:\Git\Projekt-Pollution-MKII\Pollution\Pollution\Pollution.Windows\StationList.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "6F7D273D002D7C0740A7CEC2975D4328"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Pollution
{
    partial class StationList : global::Windows.UI.Xaml.Controls.Page, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 53 "..\..\..\StationList.xaml"
                ((global::Windows.UI.Xaml.Controls.ListViewBase)(target)).ItemClick += this.itemGridView_ItemClick;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 84 "..\..\..\StationList.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.homeButton_Tapped;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 85 "..\..\..\StationList.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.sortButton_Tapped;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


