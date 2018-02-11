using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using CombatManager;
namespace CombatManagerDroid
{
    [Activity(Label = "SpellEditorActivity", Theme = "@android:style/Theme.Light.NoTitleBar")]
    public class SpellEditorActivity : Activity
    {

        static Spell spell;
        
        public static Spell Spell
        {
            get
            {
                return spell;
            }
            set
            {
                spell = value.Clone() as Spell;
            }
        }

        public static EditorActionType EditorAction
        {
            get; set;
        }

        public enum EditorActionType
        {
            Add,
            Update
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            SetContentView(Resource.Layout.SpellEditor);

            OKButton.Click += OKButton_Click;
            CancelButton.Click += CancelButton_Click;

            NameText.AttachEditTextString(spell, "Name");
            MaterialText.AttachEditTextString(spell.Adjuster, "MaterialText");
            FocusText.AttachEditTextString(spell.Adjuster, "FocusText");
            DescriptionText.AttachEditTextString(spell, "description");

            DismissibleCheckbox.AttachBool(spell.Adjuster, "Dismissible");
            VerbalCheckbox.AttachBool(spell.Adjuster, "Verbal");
            SomaticCheckbox.AttachBool(spell.Adjuster, "Somatic");
            FocusCheckbox.AttachBool(spell.Adjuster, "Focus");
            DivineFocusCheckbox.AttachBool(spell.Adjuster, "DivineFocus");

            SchoolButton.AttachButtonStringList(spell, "school", Spell.Schools);
            SubschoolButton.AttachTextCombo(spell, "subschool", Spell.Subschools);
            DescriptorButton.AttachTextCombo(spell, "descriptor", Spell.Descriptors);

            CastingTimeButton.AttachTextCombo(spell, "casting_time", Spell.CastingTimeOptions);
            RangeButton.AttachTextCombo(spell, "range", Spell.RangeOptions);
            AreaButton.AttachTextCombo(spell, "area", Spell.AreaOptions);
            TargetsButton.AttachTextCombo(spell, "targets", Spell.TargetsOptions);
            DurationButton.AttachTextCombo(spell, "duration", Spell.DurationOptions);
            SavingThrowButton.AttachTextCombo(spell, "saving_throw", Spell.SavingThrowOptions);
            SpellResistanceButton.AttachTextCombo(spell, "spell_resistence", Spell.SpellResistanceOptions);


            SpellLevelListView.Adapter = new SpellLevelAdapter(this, SpellLevelListView.Context);

            AddSpellLevelButton.Click += (sender, e) =>
            {
                var avList = from className in Spell.SpellAdjuster.Classes.Values
                             where !spell.Adjuster.ContainsClass(className)
                             select className;

                List<string> availableLevels = new List<string>();
                availableLevels.AddRange(avList);
                availableLevels.Sort();

                UIUtils.ShowListPopover(AddSpellLevelButton, "Class", availableLevels, (item) => 
                {
                    String newClass = availableLevels[item];
                    var lai = new Spell.SpellAdjuster.LevelAdjusterInfo() { Class = newClass, Level = 1 };
                    spell.Adjuster.Levels.Add(lai);

                    SpellLevelListView.Adapter = new SpellLevelAdapter(this, SpellLevelListView.Context);
                });
            };

            spell.PropertyChanged += (sender, e) =>
            {
                UpdateOK();
            };
            spell.Adjuster.PropertyChanged += (sender, e) =>
            {
                UpdateOK();
            };

            UpdateOK();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            switch (EditorAction)
            {
                case EditorActionType.Add:
                    Spell.AddCustomSpell(spell);
                    break;
                case EditorActionType.Update:
                    Spell.UpdateCustomSpell(spell, true);
                    break;
            }
            Finish();
        }

        void UpdateOK()
        {
            OKButton.Enabled = Spell.Name.NotNullString();

        }

        Button OKButton
        {
            get
            {
                return FindViewById<Button>(Resource.Id.okButton);
            }
        }

        Button CancelButton
        {
            get
            {
                return FindViewById<Button>(Resource.Id.cancelButton);
            }
        }

        public EditText NameText
        {
            get => FindViewById<EditText>(Resource.Id.nameText);
        }
        public EditText MaterialText
        {
            get => FindViewById<EditText>(Resource.Id.materialText);
        }
        public EditText FocusText
        {
            get => FindViewById<EditText>(Resource.Id.focusText);
        }
        public EditText DescriptionText
        {
            get => FindViewById<EditText>(Resource.Id.descriptionText);
        }

        Button SchoolButton
        {
            get =>FindViewById<Button>(Resource.Id.schoolButton);
        }
        Button SubschoolButton
        {
            get => FindViewById<Button>(Resource.Id.subschoolButton);
        }
        Button DescriptorButton
        {
            get => FindViewById<Button>(Resource.Id.descriptorButton);
        }
        Button CastingTimeButton
        {
            get => FindViewById<Button>(Resource.Id.castingTimeButton);
        }
        Button RangeButton
        {
            get => FindViewById<Button>(Resource.Id.rangeButton);
        }
        Button AreaButton
        {
            get => FindViewById<Button>(Resource.Id.areaButton);
        }
        Button TargetsButton
        {
            get => FindViewById<Button>(Resource.Id.targetsButton);
        }
        Button DurationButton
        {
            get => FindViewById<Button>(Resource.Id.durationButton);
        }
        CheckBox DismissibleCheckbox
        {
            get => FindViewById<CheckBox>(Resource.Id.dismissibleCheckbox);
        }
        Button SavingThrowButton
        {
            get => FindViewById<Button>(Resource.Id.savingThrowButton);
        }
        Button SpellResistanceButton
        {
            get => FindViewById<Button>(Resource.Id.spellResistanceButton);
        }
        Button AddSpellLevelButton
        {
            get => FindViewById<Button>(Resource.Id.addSpellLevelButton);
        }
        ListView SpellLevelListView
        {
            get => FindViewById<ListView>(Resource.Id.spellLevelListView);
        }
        CheckBox VerbalCheckbox
        {
            get => FindViewById<CheckBox>(Resource.Id.verbalCheckbox);
        }
        CheckBox SomaticCheckbox
        {
            get => FindViewById<CheckBox>(Resource.Id.somaticCheckbox);
        }
        CheckBox MaterialCheckbox
        {
            get => FindViewById<CheckBox>(Resource.Id.materialCheckbox);
        }
        CheckBox FocusCheckbox
        {
            get => FindViewById<CheckBox>(Resource.Id.focusCheckbox);
        }
        CheckBox DivineFocusCheckbox
        {
            get => FindViewById<CheckBox>(Resource.Id.divineFocusCheckbox);
        }


        class SpellLevelAdapter : BaseAdapter
        {
            
            Context _Context;
            SpellEditorActivity _Parent;
            

            public SpellLevelAdapter(SpellEditorActivity parent, Context context)
            {
                _Context = context;
                _Parent = parent;
                

            }

            public override int Count
            {
                get
                {
                    return spell.Adjuster.Levels.Count();
                }
            }

            public override Java.Lang.Object GetItem(int position)
            {
                return position;
            }
            public override long GetItemId(int position)
            {
                return position;
            }

            public override View GetView(int position, View convertView, ViewGroup parent)
            {

                var level = spell.Adjuster.Levels[position];


                View v = convertView;
                if (v == null)
                {
                    LayoutInflater vi = (LayoutInflater)Application.Context.GetSystemService(LayoutInflaterService);
                    v = vi.Inflate(Resource.Layout.SpellLevelItem, parent, false);
                    
                }
                TextView t = v.FindViewById<TextView>(Resource.Id.classText);
                t.Text = level.Class;

                EditText ed = v.FindViewById<EditText>(Resource.Id.levelText);
                ed.Text = level.Level.ToString();
                ed.Tag = position;
                ed.TextChanged += (sender, e) =>
                {
                    EditText editText = (EditText)sender;
                    var lev = spell.Adjuster.Levels[(int)(editText).Tag];

                    int val;
                    if (int.TryParse(editText.Text, out val))
                    {
                        if (val >= 0 && val <= 9)
                        {
                            level.Level = val;
                            editText.SetTextColor(new Android.Graphics.Color(0, 0, 0));
                        }
                        else
                        {

                            editText.SetTextColor(new Android.Graphics.Color(225, 45, 45));
                        }
                    };
                };
                
                ImageButton b = v.FindViewById<ImageButton>(Resource.Id.deleteButton);
                b.Tag = position;
                b.Click += (sender, e) =>
                {
                    var lev = spell.Adjuster.Levels[(int)((ImageButton)sender).Tag];

                    spell.Adjuster.Levels.Remove(lev);

                    _Parent.SpellLevelListView.Adapter = new SpellLevelAdapter(_Parent, _Parent.SpellLevelListView.Context);
                };

                return v;
            }
        }


    }
}