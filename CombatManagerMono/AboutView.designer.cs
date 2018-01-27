// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace CombatManagerMono
{
    [Register ("AboutView")]
    partial class AboutView
    {
        [Outlet]
        UIKit.UIButton OKButton { get; set; }


        [Outlet]
        UIKit.UIWebView WebView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (OKButton != null) {
                OKButton.Dispose ();
                OKButton = null;
            }

            if (WebView != null) {
                WebView.Dispose ();
                WebView = null;
            }
        }
    }
}