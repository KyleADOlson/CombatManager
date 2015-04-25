using System;
using CoreGraphics;
using Foundation;
using UIKit;

using CombatManager;
using System.Collections.Generic;


namespace CombatManagerMono
{
    public partial class SpellEditorDialog : StandardDialogView
    {
        Spell _spell;

        List<ButtonPropertyManager> _Managers = new List<ButtonPropertyManager>();

        static bool UserInterfaceIdiomIsPhone
        {
            get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
        }

        public SpellEditorDialog (Spell spell)
            : base(UserInterfaceIdiomIsPhone?"SpellEditorDialog_iPhone":"SpellEditorDialog_iPad", null)
        {
            _spell = spell;
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

            m = new ButtonPropertyManager(NameButton, MainUI.MainView, _spell, "Name") {Title = "Name", Multiline=false};
            _Managers.Add(m);
            m = new ButtonPropertyManager(SchoolButton, MainUI.MainView, _spell, "school") {Title = "School", Multiline=false};
            _Managers.Add(m);
            m = new ButtonPropertyManager(SubschoolButton, MainUI.MainView, _spell, "subschool") {Title = "Subschool", Multiline=false};
            _Managers.Add(m);
            m = new ButtonPropertyManager(DescriptorButton, MainUI.MainView, _spell, "descriptor") {Title = "Descriptor", Multiline=false};
            _Managers.Add(m);
            m = new ButtonPropertyManager(CastingTimeButton, MainUI.MainView, _spell, "casting_time") {Title = "Casting Time", Multiline=false};
            _Managers.Add(m);
            m = new ButtonPropertyManager(RangeButton, MainUI.MainView, _spell, "range") {Title = "Range", Multiline=false};
            _Managers.Add(m);
            m = new ButtonPropertyManager(AreaButton, MainUI.MainView, _spell, "area") {Title = "Area", Multiline=false};
            _Managers.Add(m);
            m = new ButtonPropertyManager(TargetsButton, MainUI.MainView, _spell, "targets") {Title = "Targets", Multiline=false};
            _Managers.Add(m);
            m = new ButtonPropertyManager(DurationButton, MainUI.MainView, _spell.Adjuster, "Duration") {Title = "Duration", Multiline=false};
            _Managers.Add(m);
            m = new ButtonPropertyManager(DismissableButton, MainUI.MainView, _spell.Adjuster, "Dismissible") {Title = "Dismissible", Multiline=false};
            _Managers.Add(m);
            m = new ButtonPropertyManager(SavingThrowButton, MainUI.MainView, _spell, "saving_throw") {Title = "Saving Throw", Multiline=false};
            _Managers.Add(m);
            m = new ButtonPropertyManager(SpellResistanceButton, MainUI.MainView, _spell, "spell_resistence") {Title = "Spell Resistance", Multiline=false};
            _Managers.Add(m);

            m = new ButtonPropertyManager(VerbalButton, MainUI.MainView, _spell.Adjuster, "Verbal") {Title = "Verbal", Multiline=false};
            _Managers.Add(m);

            m = new ButtonPropertyManager(SomaticButton, MainUI.MainView, _spell.Adjuster, "Somatic") {Title = "Somatic", Multiline=false};
            _Managers.Add(m);

            m = new ButtonPropertyManager(MaterialButton, MainUI.MainView, _spell.Adjuster, "Material") {Title = "Material", Multiline=false};
            _Managers.Add(m);

            m = new ButtonPropertyManager(MaterialTextButton, MainUI.MainView, _spell.Adjuster, "MaterialText") {Title = "Material Text", Multiline=false};
            _Managers.Add(m);

            m = new ButtonPropertyManager(FocusButton, MainUI.MainView, _spell.Adjuster, "Focus") {Title = "Focus", Multiline=false};
            _Managers.Add(m);
            m = new ButtonPropertyManager(DivineFocusButton, MainUI.MainView, _spell.Adjuster, "DivineFocus") {Title = "Divine Focus", Multiline=false};
            _Managers.Add(m);
            m = new ButtonPropertyManager(FocusTextButton, MainUI.MainView, _spell.Adjuster, "FocusText") {Title = "FocusText", Multiline=false};
            _Managers.Add(m);
            m = new ButtonPropertyManager(DescriptionButton, MainUI.MainView, _spell, "description") {Title = "Description", Multiline=true};
            _Managers.Add(m);

            foreach (ButtonPropertyManager bpm in _Managers)
            {
                bpm.Button.BackgroundColor = UIColor.Clear;
            }
            LevelsButton.TouchUpInside += (object sender, EventArgs e) => 
            {
                SpellLevelsDialog dlg = new SpellLevelsDialog(_spell);
                dlg.OKClicked += (object se, EventArgs ea) => 
                {

                };
                View.AddSubview(dlg.View);
            };
            LevelsButton.BackgroundColor = UIColor.Clear;
        }


        partial void OnOkButtonClicked(NSObject sender)
        {
            HandleOK();
        }

        partial void OnCancelButtonClicked(NSObject sender)
        {
            HandleCancel();

        }
    }
}

