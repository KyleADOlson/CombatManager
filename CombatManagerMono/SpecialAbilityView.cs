using System;
using CoreGraphics;
using System.Linq;
using System.Collections.Generic;


using UIKit;

using CombatManager;

namespace CombatManagerMono
{
    public class SpecialAbilityView : GradientView
    {
        GradientButton _SpecialTextButton;
        UIButton _DeleteButton;
        GradientButton _NameButton;
        GradientButton _TypeButton;

        UIImage _DeleteImage;

        SpecialAbility _Ability;
        Monster _Monster;

        List<ButtonPropertyManager> _Manager = new List<ButtonPropertyManager>();
        ButtonPropertyManager _SpecialTextManager;

        public SpecialAbilityView (Monster monster, SpecialAbility ability) 
        {
            _Monster = monster;
            _Ability = ability;


            BackgroundColor = UIColor.Clear;
            BorderColor = 0xFFFFFFFF.UIColor();
            Border = 2.0f;
            Gradient = new GradientHelper(CMUIColors.PrimaryColorDark);


            _DeleteButton = new UIButton();
            _DeleteImage = UIImage.FromFile("Images/External/redx.png");
            _DeleteButton.SetImage(_DeleteImage, UIControlState.Normal);
            _DeleteButton.TouchUpInside += delegate
            {
                _Monster.SpecialAbilitiesList.Remove(_Ability);
            };

            _SpecialTextButton = new GradientButton(); 
            _SpecialTextButton.SetTitleColor(0x00000000.UIColor(), UIControlState.Normal);
            _SpecialTextManager = new ButtonPropertyManager(_SpecialTextButton, MainUI.MainView, _Ability, "Text") {Multiline = true};
            _Manager.Add(_SpecialTextManager);

            _NameButton = new GradientButton(); 
            CMStyles.TextFieldStyle(_NameButton);           
            _Manager.Add(new ButtonPropertyManager(_NameButton, MainUI.MainView, _Ability, "Name"));

            _TypeButton = new GradientButton();
            ButtonPropertyManager typeMan = new ButtonPropertyManager(_TypeButton, MainUI.MainView, _Ability, "Type");
            typeMan.ValueList = new List<KeyValuePair<object, string>> {
                    new KeyValuePair<object, string>("Ex", "Ex"),
                    new KeyValuePair<object, string>("Sp", "Sp"),
                    new KeyValuePair<object, string>("Su", "Su"),
                    new KeyValuePair<object, string>("", "-"),};
            _Manager.Add(typeMan);

            Add (_DeleteButton);
            Add (_NameButton);
            Add (_TypeButton);
            Add (_SpecialTextButton);

            LayoutSubviews();
        }

        private static float borderMargin = 10.0f;
        private static float topHeight = 28.0f;


        public override void LayoutSubviews ()
        {
            CGSize s = _DeleteImage.Size;
            _DeleteButton.Frame = new CGRect(new CGPoint(Bounds.Width - s.Width - borderMargin, borderMargin), s);

            _TypeButton.Frame = new CGRect(_DeleteButton.Frame.X - 60, borderMargin, 60, topHeight);

            _NameButton.Frame = new CGRect(borderMargin,borderMargin, _TypeButton.Frame.X - borderMargin, topHeight);

            _SpecialTextButton.Frame = new CGRect(borderMargin, _NameButton.Frame.Bottom, Frame.Width-borderMargin*2.0f, Frame.Height - _NameButton.Frame.Bottom - borderMargin);
            _SpecialTextManager.TextView.Frame = _SpecialTextButton.Bounds;



        }


        public SpecialAbility Ability
        {
            get
            {
                return _Ability;
            }
        }
    }
}

