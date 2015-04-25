using System;
using UIKit;

namespace CombatManagerMono
{
    public static  class CMStyles
    {


        public static void TextFieldStyle(GradientButton b, float fontsize = 15.0f)
        {

            b.SetTitleColor(UIColor.Black, UIControlState.Normal);
            b.BorderColor = 0xFF777777.UIColor();
            b.Gradient = new GradientHelper(0xFFFFFFFF.UIColor());
            b.TitleLabel.Font = UIFont.SystemFontOfSize(fontsize);
            b.TitleLabel.LineBreakMode = UILineBreakMode.TailTruncation;
            AlignButtonLeft(b);
            b.CornerRadius = 3.0f;

        }

        public static void AlignButtonLeft(this UIButton b, float gap = 7.0f)
        {
            
            b.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
            b.TitleEdgeInsets = new UIEdgeInsets(0, gap, 0, gap);
        }
        
        public static void AlignButtonRight(this UIButton b, float gap = 7.0f)
        {
            
            b.HorizontalAlignment = UIControlContentHorizontalAlignment.Right;
            b.TitleEdgeInsets = new UIEdgeInsets(0, gap, 0, gap);
        }

        public static void StyleBasicPanel(this GradientView panel)
        {
            panel.BackgroundColor = UIColor.Clear;
            panel.Gradient = new GradientHelper(CMUIColors.PrimaryColorDarker);
            panel.BorderColor = UIExtensions.RGBColor(0xFFFFFF);
            panel.Border = 2;
        }

        public static void StyleTab(this GradientButton button, bool selected)
        {
            button.Border = 2;
                
            button.CornerRadii = new float[] {4, 16, 0, 0};              
            if (selected)
            {
                button.Gradient = new GradientHelper(CMUIColors.PrimaryColorDarker, CMUIColors.PrimaryColorMedium);
            }
            else
            {
                
                button.Gradient = new GradientHelper(CMUIColors.PrimaryColorMedium, CMUIColors.PrimaryColorDarker);
            }
            button.BorderColor = 0xFFFFFFFF.UIColor();
                
        }

        public static void StyleSideTab(this GradientButton button, bool selected)
        {
            button.Border = 2;
                
            button.CornerRadii = new float[] {16, 4, 0, 0};              
            if (selected)
            {
                button.Gradient = new GradientHelper(CMUIColors.PrimaryColorDarker, CMUIColors.PrimaryColorMedium);
            }
            else
            {
                
                button.Gradient = new GradientHelper(CMUIColors.PrimaryColorMedium, CMUIColors.PrimaryColorDarker);
            }
            button.BorderColor = 0xFFFFFFFF.UIColor();                
        }

        public static void StyleStandardButton(this GradientButton b)
        {
            
            b.BorderColor = UIExtensions.RGBColor(0xFFFFFF);
            b.SetTitleColor(UIColor.White, UIControlState.Normal);
            b.SetTitleColor(UIColor.DarkGray, UIControlState.Highlighted);
            b.Border = 2;
            b.DisabledGradient = new GradientHelper(0xFF999999.UIColor(), 0xFF555555.UIColor());
        }

        public static void MakeCheckButtonStyle(this GradientButton button, bool isChecked)
        {

            button.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
            button.ImageEdgeInsets = new UIEdgeInsets(0, 6, 0, 12);
            UIEdgeInsets s = button.TitleEdgeInsets;
            s.Left = 12;
            button.TitleEdgeInsets = s;

            if (isChecked)
            {
                button.SetImage(UIImage.FromFile("Images/External/CheckBox.png"), UIControlState.Normal);

            }
            else
            {
                
                button.SetImage(UIImage.FromFile("Images/External/CheckBoxUnchecked.png"), UIControlState.Normal);

            }
        }

    }
}

