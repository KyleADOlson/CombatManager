using System;
using CoreGraphics;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using System.Collections.ObjectModel;
using UIKit;
using CombatManager;

namespace CombatManagerMono
{
    public partial class FeatEditorDialog : StandardDialogView
    {
        Feat _feat;

        List<ButtonPropertyManager> _Managers = new List<ButtonPropertyManager>();

        static bool UserInterfaceIdiomIsPhone
        {
            get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
        }

        public FeatEditorDialog (Feat feat)
            : base(UserInterfaceIdiomIsPhone?"FeatEditorDialog_iPhone":"FeatEditorDialog_iPad", null)
        {
            _feat = feat;
        }


        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();
            
            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            
            // Perform any additional setup after loading the view, typically from a nib.
            BackgroundView.BackgroundColor = UIColor.Clear;
            BackgroundView.BorderColor = UIExtensions.RGBColor(0xFFFFFF);
            BackgroundView.Border = 2.0f;
            BackgroundView.Gradient = new GradientHelper(CMUIColors.SecondaryColorADark);


            ButtonPropertyManager m;

            m = new ButtonPropertyManager(NameButton, MainUI.MainView, _feat, "Name") {Title = "Name", Multiline=false};
            _Managers.Add(m);
            m = new ButtonPropertyManager(PrerequisitesButton, MainUI.MainView, _feat, "Prerequistites") {Title = "Prerequisites", Multiline=false};
            _Managers.Add(m);
            m = new ButtonPropertyManager(BenefitButton, MainUI.MainView, _feat, "Benefit") {Multiline=true};
            _Managers.Add(m);
            m = new ButtonPropertyManager(NormalButton, MainUI.MainView, _feat, "Normal") {Multiline=true};
            _Managers.Add(m);
            m = new ButtonPropertyManager(SpecialButton, MainUI.MainView, _feat, "Special") {Multiline=true};
            _Managers.Add(m);
            m = new ButtonPropertyManager(TypesButton, MainUI.MainView, _feat, "Type") {StringList=true};
            _Managers.Add(m);
            List<KeyValuePair<object, string>> types = new List<KeyValuePair<object, string>>();
            foreach (String s in Feat.FeatTypes)
            {
                types.Add(new KeyValuePair<object, string>(s, s));
            }
            m.ValueList = types;
           

            foreach (GradientButton b in from x in _Managers select x.Button)
            {
                CMStyles.TextFieldStyle(b, 15f);
            }



        }

        partial void OKButtonClicked(NSObject sender)
        {
            HandleOK();
        }

        partial void CancelButtonClicked(NSObject sender)
        {
            HandleCancel();

        }
    }
}

