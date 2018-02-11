using System;
using CombatManager;
using System.Collections.Generic;
using Android.Widget;
using Android.Content;
using Android.OS;

namespace CombatManagerDroid
{
    public class SpellFragment: LookupFragment<Spell>
    {
        string _Class = "All Classes";
        int _Level = -1;
        string _School = "All Schools";

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Spell.SpellsDBUpdated += (added, updated, removed) =>
            {
                RefreshPage();
            };
        }

        protected override List<Spell> GetItems ()
        {
            return new List<Spell>(Spell.Spells);
        }

        protected override string ItemHtml (Spell item)
        {
            return SpellHtmlCreator.CreateHtml(item);
        }

        protected override string ItemName (Spell item)
        {
            return item.Name;
        }

        protected override bool IsCustom(Spell item)
        {
            return item != null && item.IsCustom;
        }

        protected override bool CustomFilterItem(Spell item)
        {
            return ClassFilter(item) && LevelFilter(item) && SchoolFilter(item);
        }

        bool ClassFilter(Spell item)
        {
            if (_Class == "All Classes")
            {
                return true;
            }
            else
            {
                var en = CharacterClass.GetEnum(_Class);
                return item.LevelForClass(en) != null;
            }
        }

        bool LevelFilter(Spell item)
        {
            if (_Level == -1)
            {
                return true;
            }
            if (_Class == "All Classes")
            {
                return item.IsLevel(_Level);
            }
            else
            {
                var en = CharacterClass.GetEnum(_Class);
                return item.LevelForClass(en) == _Level;
            }
        }

        bool SchoolFilter(Spell item)
        {
            return _School == "All Schools" || String.Compare(item.school, _School, true) == 0;
        }

        protected override void BuildFilters()
        {
            Button b;

            b = BuildFilterButton("Spells", 180);

            List<String> classes = new List<string>( Spell.SpellAdjuster.Classes.Values );
            classes.Insert(0, "All Classes");
            PopupUtils.AttachButtonStringPopover("Classes", b, 
                                                 classes, 
                                                 0, (r1, index, val)=>
                                                 {
                _Class = val;
                UpdateFilter();

            });

            b = BuildFilterButton("Levels", 80);

            List<String> levels = new List<string>();
            for (int i=0; i<10; i++)
            {
                levels.Add(i.PastTense());
            }
            levels.Insert(0, "All Levels");
            PopupUtils.AttachButtonStringPopover("Levels", b, 
                                                 levels, 
                                                 0, (r1, index, val)=>
                                                 {
                _Level = index-1;
                UpdateFilter();

            });

            b = BuildFilterButton("Schools", 180);

            List<String> schools = new List<string>(Spell.Schools);
            schools.Insert(0, "All Schools");
            PopupUtils.AttachButtonStringPopover("Schools", b, 
                                                 schools, 
                                                 0, (r1, index, val)=>
                                                 {
                _School = val;
                UpdateFilter();

            });

            b = new Button(_v.Context);
            b.Text = "New";
            b.Click += (sender, e) =>
            {
                NewItem();
            };
            FilterLayout.AddView(b);
            NewButton = b;

            b = new Button(_v.Context);
            b.Text = "Customize";
            b.Click += (sender, e) =>
            {
                CustomizeItem();
            };
            FilterLayout.AddView(b);
            CustomizeButton = b;

            b = new Button(_v.Context);
            b.Text = "Edit";
            b.Click += (sender, e) =>
            {
                EditItem();
            };
            FilterLayout.AddView(b);
            EditButton = b;
        }

        void NewItem()
        {
            SpellEditorActivity.Spell = new Spell();
            SpellEditorActivity.EditorAction = SpellEditorActivity.EditorActionType.Add;
            Intent intent = new Intent(_v.Context, Java.Lang.Class.FromType(typeof(SpellEditorActivity)));

            _v.Context.StartActivity(intent);
        }

        void CustomizeItem()
        {
            Spell newSpell = SelectedItem.Clone() as Spell;
            newSpell.DBLoaderID = 0;
            SpellEditorActivity.Spell = SelectedItem;
            SpellEditorActivity.EditorAction = SpellEditorActivity.EditorActionType.Add;
            Intent intent = new Intent(_v.Context, Java.Lang.Class.FromType(typeof(SpellEditorActivity)));

            _v.Context.StartActivity(intent);
        }

        void EditItem()
        {
            SpellEditorActivity.Spell = SelectedItem;
            SpellEditorActivity.EditorAction = SpellEditorActivity.EditorActionType.Update;
            Intent intent = new Intent(_v.Context, Java.Lang.Class.FromType(typeof(SpellEditorActivity)));

            _v.Context.StartActivity(intent);
        }

        protected override void DeleteItem(Spell item)
        {
            Spell.RemoveCustomSpell(item);
        }
    }
}

