using System;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;

using MonoTouch.UIKit;

using CombatManager;
using MonoTouch.CoreGraphics;
using System.Text;
using MonoTouch.Foundation;


namespace CombatManagerMono
{
    public class DieRollerView : UIView
    {
        UIWebView _OutputView;
        List<UIButton> _DieButtons;
        GradientButton _RollButton;
        GradientButton _ClearButton;
        GradientView _BottomView;
        GradientButton _DieTextButton;

        string _DieText;

        TextBoxDialog _TBDialog;

        List<RollResult> _Results = new List<RollResult>();


        public DieRollerView ()
        {
            _OutputView = new UIWebView();

            _RollButton = new GradientButton();
            _RollButton.TouchUpInside += RollButtonClicked;
            _RollButton.SetText("Roll");
            _RollButton.SetImage(UIExtensions.GetSmallIcon("dice"), UIControlState.Normal);
            _RollButton.ImageEdgeInsets = new UIEdgeInsets(0, 0, 0, 7);
            _RollButton.CornerRadius = 0;
            _RollButton.Gradient = new GradientHelper(CMUIColors.SecondaryColorADarker, CMUIColors.SecondaryColorADark);

            _ClearButton = new GradientButton();
            _ClearButton.TouchUpInside += ClearButtonClicked;
            _ClearButton.SetText("Clear");
            _ClearButton.CornerRadius = 0;
            _ClearButton.Gradient = new GradientHelper(CMUIColors.SecondaryColorBDarker, CMUIColors.SecondaryColorBDark);

            _BottomView = new GradientView();
            _BottomView.ClipsToBounds = true;
            _BottomView.Gradient = new GradientHelper(CMUIColors.PrimaryColorDarker);

            Add (_OutputView);
            Add (_BottomView);
            
            _BottomView.AddSubviews(_RollButton, _ClearButton);

            _DieButtons = new List<UIButton>();

            foreach (var v in new int []{4, 6, 8, 10, 12, 20, 100})
            {
                GradientButton b = new GradientButton();
                b.CornerRadius = 0;
                b.SetText(v.ToString());
                b.Tag = v;
                b.TouchUpInside += DieClicked;

                _BottomView.AddSubview(b);
                _DieButtons.Add(b);
            }

            _DieTextButton = new GradientButton();
            CMStyles.TextFieldStyle(_DieTextButton);
            _BottomView.Add (_DieTextButton);
            _DieTextButton.TouchUpInside += DieTextButtonClicked;
            _DieTextButton.TitleLabel.AdjustsFontSizeToFitWidth = true;

        }

        void DieTextButtonClicked (object sender, EventArgs e)
        {
            _TBDialog = new TextBoxDialog();
            _TBDialog.SingleLine = true;
            _TBDialog.Value = _DieText;
            _TBDialog.OKClicked += (x, ex) => 
            {
                _DieText = _TBDialog.Value;
                _DieTextButton.SetText(_DieText);
            };
            MainUI.MainView.Add(_TBDialog.View);


        }

        void DieClicked (object sender, EventArgs e)
        {
            DieRoll roll = DieRoll.FromString(_DieText);
            if (roll == null)
            {
                roll = new DieRoll(1, ((UIButton)sender).Tag, 0);
            }
            else
            {
                roll.AddDie(new DieStep(1, ((UIButton)sender).Tag)); 
            }
            _DieText = roll.Text;
            _DieTextButton.SetText(_DieText);
        }


        void ClearButtonClicked (object sender, EventArgs e)
        {
            _DieText = "";
            _DieTextButton.SetText("");   
        }

        void RollButtonClicked (object sender, EventArgs e)
        {
            DieRoll r = DieRoll.FromString(_DieText);

            if (r != null)
            {
                RollResult res = r.Roll();

                _Results.Insert(0, res);

                while (_Results.Count > 50)
                {
                    _Results.RemoveAt(_Results.Count - 1);
                }
            }
            RenderResults();
        }

        void RenderResults()
        {

            StringBuilder resHtml = new StringBuilder();
            resHtml.CreateHtmlHeader();
            resHtml.AppendOpenTag("p");
            foreach (RollResult r in _Results)
            {
                bool first = true;
                foreach (DieResult dr in r.Rolls)
                {
                    string text;
                    if (first)
                    {
                        text = dr.Result.ToString();
                        first = false;
                    }
                    else
                    {
                        text = " " + CMStringUtilities.PlusFormatSpaceNumber(dr.Result);
                    }
                    resHtml.AppendHtml(text);
                    
                }
                if (r.Mod != 0)
                {
                    resHtml.AppendHtml(r.Mod.PlusFormat());
                }
                resHtml.AppendHtml(" = ");
                resHtml.AppendOpenTagWithClass("sp", "bolded");
                resHtml.AppendHtml(r.Total.ToString());
                resHtml.AppendCloseTag("sp");
                resHtml.AppendLineBreak();
            }
            
            resHtml.AppendCloseTag("p");


            resHtml.CreateHtmlFooter();
            _OutputView.LoadHtmlString(resHtml.ToString(), new NSUrl("http://localhost/"));

        }

        private const float _BottomSize = 150;
        private const float _SideButtonSize = 40;
        private const float _TextHeight = 30;

        public override void LayoutSubviews ()
        {
            _OutputView.Frame = new RectangleF(0, 1, Bounds.Width, Bounds.Height - _BottomSize-2);

            _BottomView.Frame = new RectangleF(0, Bounds.Height - _BottomSize, Bounds.Width, _BottomSize);


            SizeF sideSize = new SizeF(_BottomView.Bounds.Height, _SideButtonSize + 2);

            _RollButton.Bounds = sideSize.OriginRect();
            _RollButton.Center = new PointF(_SideButtonSize/2.0f - 1, _BottomView.Bounds.Height/2.0f);
            _RollButton.Transform = CGAffineTransform.MakeRotation((float)Math.PI/2.0f);

            
            _ClearButton.Bounds = sideSize.OriginRect();
            _ClearButton.Center = new PointF(_BottomView.Bounds.Width - _SideButtonSize/2.0f + 1, _BottomView.Bounds.Height/2.0f);
            _ClearButton.Transform = CGAffineTransform.MakeRotation((float)Math.PI/2.0f);

            RectangleF dieSpace = new RectangleF(_SideButtonSize, 0, _BottomView.Bounds.Size.Width - _SideButtonSize * 2, 
                                                 _BottomView.Bounds.Size.Height - _TextHeight);

            SizeF dieSize = new SizeF(dieSpace.Width/4, dieSpace.Height/2);


            for (int i=0; i<_DieButtons.Count; i++)
            {
                UIButton b = _DieButtons[i];

                int column = i%4;
                int row = i/4;
                PointF p = new PointF(dieSpace.X + column * dieSize.Width, 
                                      dieSpace.Y + dieSize.Height * row);
                SizeF s = dieSize;

                s.Width -=2;
                p.X += 1;
                s.Height -=2;
                p.Y += 1;




                b.Frame = new RectangleF(p, s);



            }

            _DieTextButton.Frame = new RectangleF(_SideButtonSize, _BottomView.Bounds.Height - _TextHeight, 
                                            dieSpace.Width, _TextHeight);


        }

    }
}

