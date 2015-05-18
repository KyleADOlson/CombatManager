
using System;
using CoreGraphics;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Foundation;
using UIKit;

namespace CombatManagerMono
{
    public class SideTab
    {
        public String Name {get; set;}
        public UIImage Icon {get; set;}
        public Object Tag {get; set;}
    }

    public class SideTabEventArgs : EventArgs
    {
        public SideTab Tab {get; set;}
    }

    public class SideTabBar : UIView
    {
        List<SideTab> _Tabs = new List<SideTab>();
        List<GradientButton> _Buttons = new List<GradientButton>();

        public event EventHandler<SideTabEventArgs> TabSelected;

        public float TabHeight {get; set;}

        GradientButton _SelectedTab;



        public SideTabBar ()
        {
            TabHeight = 130f;
        }

        public void AddTab(SideTab tab)
        {
            InsertTab(_Tabs.Count, tab);
        }

        public void InsertTab(int index, SideTab tab)
        {
            if (!_Tabs.Contains(tab))
            {
                GradientButton b = new GradientButton();
                b.SetText(tab.Name);
                b.SetImage(tab.Icon, UIControlState.Normal);
                b.Data = tab;
                b.CornerRadii = new float[] {16, 4, 0, 0};
                b.ImageEdgeInsets = new UIEdgeInsets(0, 0, 0, 5);               
                b.TitleLabel.AdjustsFontSizeToFitWidth = true;
                b.TouchUpInside += TabButtonClicked;
                Add (b);
                _Tabs.Insert(index, tab);
                _Buttons.Insert (index, b);
                if (_SelectedTab == null)
                {
                    _SelectedTab = b;
                }

                StyleTabs();
            }
        }

        void StyleTabs()
        {
            for (int i=0; i<_Tabs.Count; i++)
            {
                _Buttons[i].StyleSideTab(_SelectedTab == _Buttons[i]);
            }
        }


        void TabButtonClicked (object sender, EventArgs e)
        {
            
            if (_SelectedTab != sender)
            {
                _SelectedTab = (GradientButton)sender;

                StyleTabs();

                if (TabSelected != null)
                {

                    TabSelected(this, new SideTabEventArgs() {Tab = (SideTab)((GradientButton)sender).Data});
                }
            }
        }

        public void RemoveTab(SideTab tab)
        {
            int index = _Tabs.IndexOf(tab);
            if (index != -1)
            {
                GradientButton b = _Buttons[index];

                _Tabs.RemoveAt(index);
                _Buttons.RemoveAt(index);
            
                if (b == _SelectedTab)
                {
                    _SelectedTab = null;
                
                    if (_Tabs.Count > 0)
                    {
                        int newTab = Math.Max(0, index--);
                        _SelectedTab = _Buttons[newTab];
                    }
                
                }
            
            }


        }

        public IList<SideTab> Tabs
        {
            get
            {
                return _Tabs.AsReadOnly();
            }
        }


        public override void LayoutSubviews ()
        {
            base.LayoutSubviews ();

            float y = 0;
            float x = 0;
            float width = (float)Bounds.Width;
            foreach (GradientButton b in _Buttons)
            {
                b.Bounds = new CGSize(TabHeight, width).OriginRect();
                b.Center = b.Bounds.Center().Add (new CGPoint(x, y));
                b.Transform = CGAffineTransform.MakeRotation((float)-Math.PI/2.0f);
                y += TabHeight;
            }
        }
    }
}

