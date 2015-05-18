using System;
using CoreGraphics;
using System.Linq;
using System.Collections.Generic;

using UIKit;

using CombatManager;
using System.Text;
using Foundation;
using System.IO;
using System.Text.RegularExpressions;


namespace CombatManagerMono
{
    public class DieRollerView : UIView
    {
        UIWebView _OutputView;
        List<UIButton> _DieButtons;
        GradientButton _RollButton;
        GradientButton _ClearButton;
        GradientButton _ClearHtmlButton;
        GradientView _BottomView;
        GradientButton _DieTextButton;
        GradientButton _TitleButton;

        string _DieText;

        TextBoxDialog _TBDialog;

        List<Object> _Results = new List<Object>();

        Dictionary<UIGestureRecognizer, GradientButton> _Recs = new Dictionary<UIGestureRecognizer, GradientButton>();

        public static DieRollerView Roller {get; set;}

        private static bool _Collapsed;

        public event EventHandler _CollpasedChanged;

        public DieRollerView ()
        {
            Roller = this;

            BackgroundColor = CMUIColors.PrimaryColorDark;
            ClipsToBounds = true;


            _TitleButton = new GradientButton();
            _TitleButton.SetText("Die Roller");
            _TitleButton.Font = UIFont.BoldSystemFontOfSize(17);
            _TitleButton.CornerRadius = 0;
            _TitleButton.TouchUpInside += (object sender, EventArgs e) => 
            {
                Collapsed = !Collapsed;
            };


            _OutputView = new UIWebView();

            _RollButton = new GradientButton();
            _RollButton.TouchUpInside += RollButtonClicked;
            _RollButton.SetText("Roll");
            _RollButton.SetImage(UIExtensions.GetSmallIcon("dice"), UIControlState.Normal);
            _RollButton.ImageEdgeInsets = new UIEdgeInsets(0, 0, 0, 7);
            _RollButton.CornerRadius = 0;
            _RollButton.Gradient = new GradientHelper(CMUIColors.SecondaryColorADarker, CMUIColors.SecondaryColorADark);
            _RollButton.Font = UIFont.BoldSystemFontOfSize(17);

            _ClearButton = new GradientButton();
            _ClearButton.TouchUpInside += ClearButtonClicked;
            _ClearButton.SetText("Clear");
            _ClearButton.CornerRadius = 0;
            _ClearButton.Font = UIFont.BoldSystemFontOfSize(17);
            _ClearButton.Gradient = new GradientHelper(CMUIColors.SecondaryColorBDarker, CMUIColors.SecondaryColorBDark);

            _ClearHtmlButton = new GradientButton();
            _ClearHtmlButton.TouchUpInside += _ClearHtmlButtonClicked;
            _ClearHtmlButton.SetText("Reset");
            _ClearHtmlButton.SetImage(UIExtensions.GetSmallIcon("reset"), UIControlState.Normal);
            _ClearHtmlButton.ImageEdgeInsets = new UIEdgeInsets(0, 0, 0, 7);
            _ClearHtmlButton.Font = UIFont.BoldSystemFontOfSize(17);
            _ClearHtmlButton.CornerRadius = 0;
            _ClearHtmlButton.Gradient = new GradientHelper(CMUIColors.SecondaryColorADarker, CMUIColors.SecondaryColorADark);
             
            _BottomView = new GradientView();
            _BottomView.ClipsToBounds = true;
            _BottomView.Gradient = new GradientHelper(CMUIColors.PrimaryColorDarker);

            Add (_TitleButton);
            Add (_OutputView);
            Add (_BottomView);
            Add (_ClearHtmlButton);
            
            _BottomView.AddSubviews(_RollButton, _ClearButton);

            BringSubviewToFront(_BottomView);

            _DieButtons = new List<UIButton>();

            foreach (var v in new int []{4, 6, 8, 10, 12, 20, 100})
            {
                GradientButton b = new GradientButton();
                b.CornerRadius = 0;
                b.SetText(v.ToString());
                b.Tag = v;
                b.TouchUpInside += DieClicked;

                UIImage im = null;

                switch (v)
                {
                case 4:
                case 6:
                case 8:
                case 10:
                case 12: 
                case 100:
                    im = UIExtensions.GetSmallIcon("d" + v);
                    break;                
                case 20:
                    im = UIExtensions.GetSmallIcon("d20p");
                    break;
                }

                if (im != null)
                {
                    b.BonusImage = im;
                    b.ContentEdgeInsets = new UIEdgeInsets(25, 0, 0, 0);
                }


                _BottomView.AddSubview(b);
                _DieButtons.Add(b);

                UISwipeGestureRecognizer rec = new UISwipeGestureRecognizer();
                rec.Direction = UISwipeGestureRecognizerDirection.Up | UISwipeGestureRecognizerDirection.Down;
                rec.Delegate = new SwipeGestureDelegate();
                rec.AddTarget(this, new ObjCRuntime.Selector("HandleDieSwipe:"));
                _Recs[rec] = b;

                b.AddGestureRecognizer(rec);

            }

            _DieTextButton = new GradientButton();
            CMStyles.TextFieldStyle(_DieTextButton);
            _BottomView.Add (_DieTextButton);
            _DieTextButton.TouchUpInside += DieTextButtonClicked;
            _DieTextButton.TitleLabel.AdjustsFontSizeToFitWidth = true;
            BringSubviewToFront(_TitleButton);

        }

        void _ClearHtmlButtonClicked (object sender, EventArgs e)
        {
            _Results.Clear();
            RenderResults();
        }

        String DieImageName(int die)
        {
            switch (die)
            {
            case 4:
            case 6:
            case 8:
            case 10:
            case 12: 
            case 100:
                return "d" + die;    
            case 20:
                return "d20p";
            }
            return null;

        }


        class SwipeGestureDelegate : UIGestureRecognizerDelegate
        {
            public override bool ShouldReceiveTouch (UIGestureRecognizer recognizer, UITouch touch)
            {
                return true;
            }
        }

        [Export("HandleDieSwipe:")]
        void HandleDieSwipe(UISwipeGestureRecognizer rec)
        {
            GradientButton b =  _Recs[rec];
            _BottomView.BringSubviewToFront(b);

            int value = (int)b.Tag;

            DieRoll r = DieRoll.FromString("1d" + value);

            SlideView(b, true);

            AddRoll (r);
        }

        void SlideView (UIView b, bool up)
        {
            UIView.Animate(.1f, 0f,
                            0,
                           () => {b.Layer.AffineTransform = CGAffineTransform.MakeTranslation(0, up?-10:10);},
            () => {b.Layer.AffineTransform =  CGAffineTransform.MakeTranslation(0, 0);});

        }

        void AddRoll(String description, DieRoll r)
        {

            if (r != null)
            {
                RollResult res = r.Roll();

                _Results.Insert(0, new Tuple<string, RollResult>(description, res));

                TrimList();
            }
            RenderResults();
        }

        void AddRoll(DieRoll r)
        {
            if (r != null)
            {
                RollResult res = r.Roll();

                _Results.Insert(0, res);

                TrimList();
            }
            RenderResults();
        }

        void DieTextButtonClicked (object sender, EventArgs e)
        {
            _TBDialog = new TextBoxDialog();
            _TBDialog.SingleLine = true;
            _TBDialog.Value = _DieText;
            _TBDialog.HeaderText = "Die Roll";
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
                roll = new DieRoll(1, (int)((UIButton)sender).Tag, 0);
            }
            else
            {
                roll.AddDie(new DieStep(1, (int)((UIButton)sender).Tag)); 
            }
            
            SlideView((UIView)sender, false);
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


            AddRoll(r);
            RenderResults();
        }

        void RenderResults()
        {

            StringBuilder resHtml = new StringBuilder();
            resHtml.CreateHtmlHeader();
            resHtml.AppendOpenTag("p");
            foreach (var v in _Results)
            {
                if (v is RollResult)
                {
                    RenderRollResult(resHtml, v as RollResult);
                }
                if (v is Tuple<string, RollResult>)
                {
                    var x = v as Tuple<string, RollResult>;

                    RenderRollResult(resHtml, x.Item2, true, 20, x.Item1);
                }
                else if (v is AttackRollResult)
                {
                    RenderAttackRollResult(resHtml, v as AttackRollResult);
                }
                else if (v is AttackSetResult)
                {
                    RenderAttackSetResult(resHtml, v as AttackSetResult);
                }
            }
            
            resHtml.AppendCloseTag("p");


            resHtml.CreateHtmlFooter();

            NSUrl baseUrl = new NSUrl(NSBundle.MainBundle.BundlePath, true);

            _OutputView.LoadHtmlString(resHtml.ToString(), baseUrl);

        }

        private void AppendCharacterHeader(StringBuilder resHtml, Character ch)
        {
            resHtml.CreateHeader(ch.Name, null, "h2");
        }

        private void RenderAttackSetResult(StringBuilder resHtml, AttackSetResult res)
        {
            if (res.Character != null)
            {
                AppendCharacterHeader(resHtml, res.Character);
            }

            foreach (AttackRollResult ar in res.Results)
            {
                RenderAttackRollResult(resHtml, ar);
            }
        }

        private void RenderAttackRollResult(StringBuilder resHtml, AttackRollResult ar)
        {
            if (ar.Character != null)
            {
                AppendCharacterHeader(resHtml, ar.Character);
            }

            resHtml.AppendOpenTagWithClass("span", "weaponheader");
            if (ar.Attack.Weapon == null)
            {
                resHtml.AppendSmallIcon("sword");
            }
            else if (ar.Attack.Weapon.Ranged)
            {
                resHtml.AppendSmallIcon("bow");
            }
            else if (ar.Attack.Weapon.Natural)
            {
                resHtml.AppendSmallIcon("claw");
            }
            else
            {
                resHtml.AppendSmallIcon("sword");
            }

            resHtml.AppendHtml(ar.Name.Capitalize());
            resHtml.AppendLineBreak();
            resHtml.AppendCloseTag("span");

            foreach (SingleAttackRoll res in ar.Rolls)
            {
                RenderRollResult(resHtml, res.Result, false, ar.Attack.CritRange);

                resHtml.AppendHtml(" Dmg: ");
                RenderRollResult(resHtml, res.Damage);

                foreach (BonusDamage bd in res.BonusDamage)
                {
                    resHtml.Append(" + ");
                    RenderRollResult(resHtml, bd.Damage);
                    resHtml.Append(" " + bd.DamageType);
                }

                if (res.CritResult != null)
                {
                    resHtml.AppendHtml(" Crit: ");
                    RenderRollResult(resHtml, res.CritResult, res.CritResult.Rolls[0].Result == 1);

                    if (res.CritResult.Rolls[0].Result != 1)
                    {                    
                        resHtml.AppendHtml(" Dmg: ");
                    }
                    RenderRollResult(resHtml, res.CritDamage);
                }

            }
            
        }

        private void RenderRollResult (StringBuilder resHtml, RollResult r, bool breakLine = true, int critRange = 20, string description = null)
        {

            if (description != null)
            {
                resHtml.AppendHtml(description);
                resHtml.AppendSpace();
            }

            bool first = true;
            foreach (DieResult dr in r.Rolls)
            {
                if (!first)
                {
                    if (dr.Result < 0)
                    {
                        resHtml.AppendHtml(" - ");
                    }
                    else
                    {
                        resHtml.AppendHtml(" + ");
                    }

                }

                resHtml.Append (DieHtml(dr.Die));

                string text;
                text = dr.Result.ToString();
                resHtml.AppendSpace();
                if (dr.Die == 20 && dr.Result >= critRange)
                {
                    resHtml.AppendOpenTagWithClass("span", "critical");
                    resHtml.AppendSpace();
                    resHtml.AppendHtml(text);
                    resHtml.AppendSpace();
                    resHtml.AppendCloseTag("span");
                    resHtml.AppendSpace();
                }
                else if (dr.Die == 20 && dr.Result == 1)
                {
                    resHtml.AppendOpenTagWithClass("span", "critfail");
                    resHtml.AppendSpace();
                    resHtml.AppendHtml(text);
                    resHtml.AppendSpace();
                    resHtml.AppendCloseTag("span");
                    resHtml.AppendSpace();
                }
                else
                {                    
                    resHtml.AppendHtml(text);
                }
                first = false;
                
            }
            if (r.Mod != 0)
            {
                resHtml.AppendHtml(r.Mod.PlusFormat());
            }
            resHtml.AppendHtml(" = ");
            resHtml.AppendOpenTagWithClass("sp", "bolded");
            resHtml.AppendHtml(r.Total.ToString());
            resHtml.AppendCloseTag("sp");
            if ( breakLine)
            {
                resHtml.AppendLineBreak();
            }
        }

        private string DieHtml(int val)
        {
            string text = "";
            string dieImage = DieImageName(val);
            if (dieImage != null)
            {
                text = "<img src=\"Images/External/" + dieImage + "-16.png\"/>";
            }
            return text;
        }

        private const float _BottomSize = 150;
        private const float _SideButtonSize = 40;
        private const float _TextHeight = 30;
        private const float _TopButtonSize = 35;


        public override void LayoutSubviews ()
        {
            if (_Collapsed)
            {
                _TitleButton.Frame = new CGRect(0, 0, Bounds.Width, _TopButtonSize);
            }
            else
            {
                _TitleButton.Frame = new CGRect(0, 0, Bounds.Width * 2.0f / 3.0f, _TopButtonSize);

            }

            _ClearHtmlButton.Frame = new CGRect(Bounds.Width * 2.0f/3.0f, 0, Bounds.Width/3.0f, _TopButtonSize);
            _OutputView.Frame = new CGRect(0, _TopButtonSize, Bounds.Width, Bounds.Height - _BottomSize-1 - _TopButtonSize);

            _BottomView.Frame = new CGRect(0, Bounds.Height - _BottomSize, Bounds.Width, _BottomSize);


            CGSize sideSize = new CGSize(_BottomView.Bounds.Height, _SideButtonSize);

            _RollButton.Bounds = sideSize.OriginRect();
            _RollButton.Center = new CGPoint(_SideButtonSize/2.0f, _BottomView.Bounds.Height/2.0f);
            _RollButton.Transform = CGAffineTransform.MakeRotation((float)Math.PI/2.0f);

            
            _ClearButton.Bounds = sideSize.OriginRect();
            _ClearButton.Center = new CGPoint(_BottomView.Bounds.Width - _SideButtonSize/2.0f, _BottomView.Bounds.Height/2.0f);
            _ClearButton.Transform = CGAffineTransform.MakeRotation((float)Math.PI/2.0f);

            CGRect dieSpace = new CGRect(_SideButtonSize, 0, _BottomView.Bounds.Size.Width - _SideButtonSize * 2, 
                                                 _BottomView.Bounds.Size.Height - _TextHeight);

            CGSize dieSize = new CGSize(dieSpace.Width/4, dieSpace.Height/2);


            for (int i=0; i<_DieButtons.Count; i++)
            {
                GradientButton b = (GradientButton)_DieButtons[i];

                int column = i%4;
                int row = i/4;
                CGPoint p = new CGPoint(dieSpace.X + column * dieSize.Width, 
                                      dieSpace.Y + dieSize.Height * row);
                CGSize s = dieSize;

                s.Width -=2;
                p.X += 1;
                s.Height -=2;
                p.Y += 1;




                b.Frame = new CGRect(p, s);


                if (b.BonusImage != null)
                {
                    CGRect rect = b.BonusImage.Size.OriginRect();
                    rect.X = (b.Frame.Width - rect.Width)/2.0f;
                    rect.Y = 5;
                    b.BonusImageRect = rect;
                }


            }

            _DieTextButton.Frame = new CGRect(_SideButtonSize, _BottomView.Bounds.Height - _TextHeight, 
                                            dieSpace.Width, _TextHeight);


        }

        public class SingleAttackRoll
        {
            public SingleAttackRoll()
            {
                BonusDamage = new List<BonusDamage>();
            }
            public RollResult Result {get; set;}
            public RollResult Damage {get; set;}
            public RollResult CritResult {get; set;}
            public RollResult CritDamage {get; set;}
            public List<BonusDamage> BonusDamage {get; set;}
        }

        public class BonusDamage
        {
            public String DamageType { get; set; }
            public RollResult Damage { get; set; }
        }

        public class AttackSetResult
        {
            
            Character _Character;

            List<AttackRollResult> _Results = new List<AttackRollResult>();

            public Character Character
            {
                get
                {
                    return _Character;
                }
                set
                {
                    _Character = value;
                }
            }

            public List<AttackRollResult> Results
            {
                get
                {
                    return _Results;
                }
            }

        }

        public class AttackRollResult
        {
            Attack _Attack;

            String _Name;

            Character _Character;

            public String Name
            {
                get
                {
                    return _Name;
                }
            }

            public Character Character
            {
                get
                {
                    return _Character;
                }
                set
                {
                    _Character = value;
                }
            }

            
            public Attack Attack
            {
                get
                {
                    return _Attack;
                }
            }


            public List<SingleAttackRoll> Rolls {get; set;}



            public AttackRollResult(Attack atk)
            {
                _Attack = atk;

                _Name = atk.Name;

                Rolls = new List<SingleAttackRoll>();

                if (atk.Weapon != null)
                {
                    _Name = atk.Weapon.Name;
                }

                int totalAttacks = atk.Count * atk.Bonus.Count;

                for (int atkcount = 0; atkcount < atk.Count; atkcount++)
                {
                    foreach (int mod in atk.Bonus)
                    {
                        SingleAttackRoll sr = new SingleAttackRoll();

                        DieRoll roll = new DieRoll(1, 20, mod);
                    
                        sr.Result = roll.Roll();
                        sr.Damage = atk.Damage.Roll();

                        if (atk.Plus != null)
                        {
                            Regex plusRegex = new Regex("(?<die>[0-9]+d[0-9]+((\\+|-)[0-9]+)?) (?<type>[a-zA-Z]+)");
                            Match dm = plusRegex.Match(atk.Plus);
                            if (dm.Success)
                            {
                                DieRoll bonusRoll = DieRoll.FromString(dm.Groups["die"].Value);
                                BonusDamage bd = new BonusDamage();
                                bd.Damage = bonusRoll.Roll();
                                bd.DamageType = dm.Groups["type"].Value;
                                sr.BonusDamage.Add(bd);
                            }
                        }

                        if (sr.Result.Rolls[0].Result >= atk.CritRange)
                        {
                            sr.CritResult = roll.Roll();

                            sr.CritDamage = new RollResult();

                            for (int i = 1; i < atk.CritMultiplier; i++)
                            {
                                RollResult crit = atk.Damage.Roll();
                            
                                sr.CritDamage = crit + sr.CritDamage;
                            }
                        }


                        Rolls.Add (sr);
                    }
                }

            }
        }

        public void RollSave(Monster.SaveType type, Character ch)
        {
            int? save = ch.Monster.GetSave(type);
            int mod = (save == null)?0:save.Value;
            DieRoll roll = new DieRoll(1, 20, mod);

            AddRoll (Monster.GetSaveText(type) + ": ", roll);
        }

        public void RollSkill(String skill, Character ch)
        {
            RollSkill(skill, null, ch);
        }

        public void RollSkill(String skill, String subtype, Character ch)
        {
            int mod = ch.Monster.GetSkillMod(skill, subtype);

            string text = skill + (subtype != null?" "+subtype:"") + ": ";
            
            DieRoll roll = new DieRoll(1, 20, mod);


            AddRoll (text, roll);

        }


        public void RollAttack(Attack atk, Character ch)
        {
            AttackRollResult res = new AttackRollResult(atk);
       
            res.Character = ch;

            _Results.Insert(0, res);

            TrimList ();

            RenderResults();
        }

        public void RollAttackSet(AttackSet atkSet, Character ch)
        {
            AttackSetResult res = new AttackSetResult();
            res.Character = ch;

            foreach (Attack at in atkSet.WeaponAttacks)
            {
                AttackRollResult ares = new AttackRollResult(at);
                res.Results.Add (ares);
            }
            foreach (Attack at in atkSet.NaturalAttacks)
            {
                AttackRollResult ares = new AttackRollResult(at);
                res.Results.Add (ares);
            }

            
            _Results.Insert(0, res);

            TrimList ();

            RenderResults();
        }

        private void TrimList()
        {
            while (_Results.Count > 50)
            {
                _Results.RemoveAt(_Results.Count - 1);
            }
        }

        public bool Collapsed
        {
            get
            {
                return _Collapsed;
            }
            set
            {
                if (_Collapsed != value)
                {
                    _Collapsed = value;
                    if (_CollpasedChanged != null)
                    {
                        _CollpasedChanged(this, new EventArgs());
                    }
                }
            }
        }

        public float HiddenHeight
        {
            get
            {
                return _TopButtonSize;
            }
        }


    }
}

