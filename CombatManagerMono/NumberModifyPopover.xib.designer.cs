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
    [Register ("NumberModifyPopover")]
    partial class NumberModifyPopover
    {
        [Outlet]
        CombatManagerMono.GradientButton subtractButton { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton setButton { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton addButton { get; set; }


        [Outlet]
        UIKit.UILabel numberLabel { get; set; }


        [Outlet]
        UIKit.UIPickerView pickerView { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton NullButton { get; set; }


        [Outlet]
        UIKit.UILabel NumPadLabel { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton NumberButton1 { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton NumberButton2 { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton NumberButton3 { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton NumberButton4 { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton NumberButton5 { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton NumberButton6 { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton NumberButton7 { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton NumberButton8 { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton NumberButton9 { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton NumberButton0 { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton ClearButton { get; set; }


        [Outlet]
        UIKit.UIView NumPadView { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton SwapButton { get; set; }


        [Action ("SwapButtonTouchUpInside:")]
        partial void SwapButtonTouchUpInside (Foundation.NSObject sender);


        [Action ("NumberTouchUpInside:")]
        partial void NumberTouchUpInside (Foundation.NSObject sender);


        [Action ("ClearButtonTouchUpInside:")]
        partial void ClearButtonTouchUpInside (Foundation.NSObject sender);


        [Action ("SubtractClicked:")]
        partial void SubtractClicked (Foundation.NSObject sender);


        [Action ("SetClicked:")]
        partial void SetClicked (Foundation.NSObject sender);


        [Action ("AddClicked:")]
        partial void AddClicked (Foundation.NSObject sender);


        [Action ("NullButtonTouchUpInside:")]
        partial void NullButtonTouchUpInside (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
            if (addButton != null) {
                addButton.Dispose ();
                addButton = null;
            }

            if (ClearButton != null) {
                ClearButton.Dispose ();
                ClearButton = null;
            }

            if (NullButton != null) {
                NullButton.Dispose ();
                NullButton = null;
            }

            if (NumberButton0 != null) {
                NumberButton0.Dispose ();
                NumberButton0 = null;
            }

            if (NumberButton1 != null) {
                NumberButton1.Dispose ();
                NumberButton1 = null;
            }

            if (NumberButton2 != null) {
                NumberButton2.Dispose ();
                NumberButton2 = null;
            }

            if (NumberButton3 != null) {
                NumberButton3.Dispose ();
                NumberButton3 = null;
            }

            if (NumberButton4 != null) {
                NumberButton4.Dispose ();
                NumberButton4 = null;
            }

            if (NumberButton5 != null) {
                NumberButton5.Dispose ();
                NumberButton5 = null;
            }

            if (NumberButton6 != null) {
                NumberButton6.Dispose ();
                NumberButton6 = null;
            }

            if (NumberButton7 != null) {
                NumberButton7.Dispose ();
                NumberButton7 = null;
            }

            if (NumberButton8 != null) {
                NumberButton8.Dispose ();
                NumberButton8 = null;
            }

            if (NumberButton9 != null) {
                NumberButton9.Dispose ();
                NumberButton9 = null;
            }

            if (numberLabel != null) {
                numberLabel.Dispose ();
                numberLabel = null;
            }

            if (NumPadLabel != null) {
                NumPadLabel.Dispose ();
                NumPadLabel = null;
            }

            if (NumPadView != null) {
                NumPadView.Dispose ();
                NumPadView = null;
            }

            if (pickerView != null) {
                pickerView.Dispose ();
                pickerView = null;
            }

            if (setButton != null) {
                setButton.Dispose ();
                setButton = null;
            }

            if (subtractButton != null) {
                subtractButton.Dispose ();
                subtractButton = null;
            }

            if (SwapButton != null) {
                SwapButton.Dispose ();
                SwapButton = null;
            }
        }
    }
}