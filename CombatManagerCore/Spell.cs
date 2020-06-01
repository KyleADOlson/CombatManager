/*
 *  Spell.cs
 *
 *  Copyright (C) 2010-2012 Kyle Olson, kyle@kyleolson.com
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU General Public License
 *  as published by the Free Software Foundation; either version 2
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 * 
 *  You should have received a copy of the GNU General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 *
 */

﻿using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using ScottsUtils;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CombatManager
{

    public class Spell : BaseDBClass
    {
        private String _name;
        private String _school;
        private String _subschool;
        private String _descriptor;
        private String _spell_level;
        private String _casting_time;
        private String _components;
        private String _costly_components;
        private String _range;
        private String _targets;
        private String _effect;
        private String _dismissible;
        private String _area;
        private String _duration;
        private String _shapeable;
        private String _saving_throw;
        private String _spell_resistence;
        private String _description;
        private String _description_formated;
        private String _source;
        private String _full_text;
        private String _verbal;
        private String _somatic;
        private String _material;
        private String _focus;
        private String _divine_focus;
        private String _sor;
        private String _wiz;
        private String _cleric;
        private String _druid;
        private String _ranger;
        private String _bard;
        private String _paladin;
        private String _alchemist;
        private String _summoner;
        private String _witch;
        private String _inquisitor;
        private String _oracle;
        private String _antipaladin;
        private String _assassin;
        private String _adept;
        private String _red_mantis_assassin;
        private String _magus;
        private String _URL;
        private String _SLA_Level;
        private String _preparation_time;
        private bool _duplicated;
        private String _acid;
        private String _air;
        private String _chaotic;
        private String _cold;
        private String _curse;
        private String _darkness;
        private String _death;
        private String _disease;
        private String _earth;
        private String _electricity;
        private String _emotion;
        private String _evil;
        private String _fear;
        private String _fire;
        private String _force;
        private String _good;
        private String _language;
        private String _lawful;
        private String _light;
        private String _mind_affecting;
        private String _pain;
        private String _poison;
        private String _shadow;
        private String _sonic;
        private String _water;

        //annotation fields

        //bonus annotation
        private ConditionBonus _Bonus;
        //treasure table annotations
        private string _PotionWeight;
        private string _DivineScrollWeight;
        private string _ArcaneScrollWeight;
        private string _WandWeight;
        private String _PotionLevel;
        private String _PotionCost;
        private String _ArcaneScrollLevel;
        private String _ArcaneScrollCost;
        private String _DivineScrollLevel;
        private String _DivineScrollCost;
        private String _WandLevel;
        private String _WandCost;




        private SpellAdjuster _Adjuster;

        private static ObservableCollection<Spell> _Spells;

        private static Dictionary<int, Spell> _SpellsByDetailsID;

        private static SortedDictionary<string, string> _Schools;
        private static SortedSet<string> _Subschools;
        private static SortedSet<string> _Descriptors;
        private static Dictionary<string, ObservableCollection<Spell>> _SpellDictionary;

        private static SortedSet<string> _CastingTimeOptions;
        private static SortedSet<string> _RangeOptions;
        private static SortedSet<string> _AreaOptions;
        private static SortedSet<string> _TargetsOptions;
        private static SortedSet<string> _DurationOptions;
        private static SortedSet<string> _SavingThrowOptions;
        private static SortedSet<string> _SpellResistanceOptions;

        private static DBLoader<Spell> _SpellsDB;

        public delegate void SpellsDBUpdatedDelegate(ICollection<Spell> added, ICollection<Spell> updated, ICollection<Spell> removed);
        public static SpellsDBUpdatedDelegate SpellsDBUpdated;


        static Spell()
        {


        }

        static void LoadSpells()
        {
            List<Spell> set = XmlListLoader<Spell>.Load("SpellsShort.xml");

            List<Spell> remove = new List<Spell>();
            foreach (var cur in set)
            {

                //This needs to be removed at some point.  We shouldn't change the duration/areas field to add tags to the ui
                if (cur.dismissible == "1")
                {
                    if (!cur.duration.Contains("(D)"))
                    {
                        cur.duration += " (D)";
                    }
                }

                if (cur.shapeable == "1")
                {
                    if (!cur.area.Contains("(S)"))
                    {
                        cur.area += " (S)";
                    }
                }

                if (cur._duplicated)
                {
                    remove.Add(cur);
                }


            }

            foreach (Spell s in remove)
            {
                set.Remove(s);
            }


            _SpellsByDetailsID = new Dictionary<int, Spell>();
            foreach (Spell s in set)
            {
                _SpellsByDetailsID[s.detailsid] = s;

            }

            _Spells = new ObservableCollection<Spell>(set);

            if (DBSettings.UseDB)
            {
                _SpellsDB = new DBLoader<Spell>("spells.db");

                foreach (Spell s in _SpellsDB.Items)
                {
                    _Spells.Add(s);
                }
            }

            _Spells.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(_Spells_CollectionChanged);
            _SpellDictionary = new Dictionary<string, ObservableCollection<Spell>>(new InsensitiveEqualityCompararer());

            _Schools = new SortedDictionary<string, string>();
            _Subschools = new SortedSet<string>();
            _Descriptors = new SortedSet<string>();
            _CastingTimeOptions = new SortedSet<string>();
            _RangeOptions = new SortedSet<string>();
            _AreaOptions = new SortedSet<string>();
            _TargetsOptions = new SortedSet<string>();
            _DurationOptions = new SortedSet<string>();
            _SavingThrowOptions = new SortedSet<string>();
            _SpellResistanceOptions = new SortedSet<string>();
            foreach (Spell s in _Spells)
            {
                string r = StringCapitalizer.Capitalize(s.school);
                _Schools[r] = r;
                AddSpellDictionaryItem(s);

                //add subschool
                if (s.subschool.NotNullString())
                {
                    foreach (String subschool in s.subschool.Split(new char[] { ',', ';' }))
                    {
                        _Subschools.Add(subschool.Trim());
                    }
                }

                if (s.descriptor.NotNullString())
                {
                    //add descriptions
                    foreach (String desc in s.descriptor.Split(new char[] {',',';'}))
                    {
                        _Descriptors.Add(desc.Trim().TrimEnd(new char[] { 'U', 'M' }));
                    }
                }

               
                
            }

            List<String> doubles = new List<String>(_Subschools.Where(a => Regex.Match(a, "[a-zA-Z] or [a-zA-Z]").Success));
            foreach (String dub in doubles)
            {
                _Subschools.Remove(dub);
            }

            _Descriptors.RemoveWhere(a => a.StartsWith("see text"));
            List<String> ors = new List<String>(_Descriptors.Where(a => a.StartsWith("or ")));
            foreach (String orstring in ors)
            {
                _Descriptors.Remove(orstring);
                _Descriptors.Add(orstring.Substring(3));
            }
            doubles = new List<String>(_Descriptors.Where(a => Regex.Match(a, "[a-zA-Z] or [a-zA-Z]").Success));
            foreach (String dub in doubles)
            {
                _Descriptors.Remove(dub);
            }

            MakeOptions();
            

        }

        private static void MakeOptions()
        {

            //Casting Time
			_CastingTimeOptions.Add("1 hour");
			_CastingTimeOptions.Add("1 immediate action");
			_CastingTimeOptions.Add("1 minute");
			_CastingTimeOptions.Add("1 minute per page");
			_CastingTimeOptions.Add("1 minute/lb. created");
			_CastingTimeOptions.Add("1 round");
			_CastingTimeOptions.Add("1 round; see text");
			_CastingTimeOptions.Add("1 standard action");
			_CastingTimeOptions.Add("1 standard action or see text");
			_CastingTimeOptions.Add("10 minutes");
			_CastingTimeOptions.Add("10 minutes; see text");
			_CastingTimeOptions.Add("12 hours");
			_CastingTimeOptions.Add("2 rounds");
			_CastingTimeOptions.Add("24 hours");
			_CastingTimeOptions.Add("3 full rounds");
			_CastingTimeOptions.Add("3 rounds");
			_CastingTimeOptions.Add("30 minutes");
			_CastingTimeOptions.Add("6 rounds");
			_CastingTimeOptions.Add("at least 10 minutes; see text");
			_CastingTimeOptions.Add("see text");
            
            //Range
            _RangeOptions.Add("0 ft.");
			_RangeOptions.Add("0 ft.; see text");
			_RangeOptions.Add("1 mile");
			_RangeOptions.Add("1 mile/level");
			_RangeOptions.Add("10 ft.");
			_RangeOptions.Add("120 ft.");
			_RangeOptions.Add("15 ft.");
			_RangeOptions.Add("2 miles");
			_RangeOptions.Add("20 ft.");
			_RangeOptions.Add("30 ft.");
			_RangeOptions.Add("40 ft.");
			_RangeOptions.Add("40 ft./level");
			_RangeOptions.Add("5 miles");
			_RangeOptions.Add("50 ft.");
			_RangeOptions.Add("60 ft.");
			_RangeOptions.Add("anywhere within the area to be warded");
			_RangeOptions.Add("Area 10-ft.-radius emanation around the creature");
			_RangeOptions.Add("close (25 ft. + 5 ft./2 levels)");
			_RangeOptions.Add("close (25 ft. + 5 ft./2 levels) or see text");
			_RangeOptions.Add("close (25 ft. + 5 ft./2 levels)/100 ft.; see text");
			_RangeOptions.Add("close (25 ft. + 5 ft./2 levels); see text");
			_RangeOptions.Add("long (400 ft. + 40 ft./level)");
			_RangeOptions.Add("medium (100 ft. + 10 ft. level)");
			_RangeOptions.Add("medium (100 ft. + 10 ft./level)");
			_RangeOptions.Add("personal");
			_RangeOptions.Add("personal and touch");
			_RangeOptions.Add("personal or close (25 ft. + 5 ft./2 levels)");
			_RangeOptions.Add("personal or touch");
			_RangeOptions.Add("see text");
			_RangeOptions.Add("touch");
			_RangeOptions.Add("touch; see text");
			_RangeOptions.Add("unlimited");
			_RangeOptions.Add("up to 10 ft./level");

            //Area
            _AreaOptions.Add("10-ft. square/level; see text");
			_AreaOptions.Add("10-ft.-radius emanation centered on you");
			_AreaOptions.Add("10-ft.-radius emanation from touched creature");
			_AreaOptions.Add("10-ft.-radius emanation, centered on you");
			_AreaOptions.Add("10-ft.-radius spherical emanation, centered on you");
			_AreaOptions.Add("10-ft.-radius spread");
			_AreaOptions.Add("120-ft. line");
			_AreaOptions.Add("20-ft.-radius burst");
			_AreaOptions.Add("20-ft.-radius emanation");
			_AreaOptions.Add("20-ft.-radius emanation centered on a creature, object, or point in space");
			_AreaOptions.Add("20-ft.-radius emanation centered on a point in space");
			_AreaOptions.Add("20-ft.-radius spread");
			_AreaOptions.Add("2-mile-radius circle, centered on you; see text");
			_AreaOptions.Add("30-ft. cube/level (S)");
			_AreaOptions.Add("40 ft./level radius cylinder 40 ft. high");
			_AreaOptions.Add("40-ft. radius emanating from the touched point");
			_AreaOptions.Add("40-ft.-radius emanation");
			_AreaOptions.Add("40-ft.-radius emanation centered on you");
			_AreaOptions.Add("50-ft.-radius burst, centered on you");
			_AreaOptions.Add("5-ft.-radius emanation centered on you");
			_AreaOptions.Add("5-ft.-radius spread; or one solid object or one crystalline creature");
			_AreaOptions.Add("60-ft. cube/level (S)");
			_AreaOptions.Add("60-ft. line from you");
			_AreaOptions.Add("60-ft. line-shaped emanation from you");
			_AreaOptions.Add("80-ft.-radius burst");
			_AreaOptions.Add("80-ft.-radius spread (S)");
			_AreaOptions.Add("all allies and foes within a 40-ft.-radius burst centered on you");
			_AreaOptions.Add("all magical effects and magic items within a 40-ft.-radius burst, or one magic item (see text)");
			_AreaOptions.Add("all metal objects within a 40-ft.-radius burst");
			_AreaOptions.Add("barred cage (20-ft. cube) or windowless cell (10-ft. cube)");
			_AreaOptions.Add("circle, centered on you, with a radius of 400 ft. + 40 ft./level");
			_AreaOptions.Add("cloud spreads in 20-ft. radius, 20 ft. high");
			_AreaOptions.Add("cone-shaped burst");
			_AreaOptions.Add("cone-shaped emanation");
			_AreaOptions.Add("creatures and objects within 10-ft.-radius spread");
			_AreaOptions.Add("creatures and objects within a 5-ft.-radius burst");
			_AreaOptions.Add("creatures in a 20-ft.-radius spread");
			_AreaOptions.Add("creatures within a 20-ft.-radius spread");
			_AreaOptions.Add("cylinder (10-ft. radius, 40-ft. high)");
			_AreaOptions.Add("cylinder (20-ft. radius, 40 ft. high)");
			_AreaOptions.Add("cylinder (40-ft. radius, 20 ft. high)");
			_AreaOptions.Add("dirt in an area up to 750 ft. square and up to 10 ft. deep (S)");
			_AreaOptions.Add("four 40-ft.-radius spreads, see text");
			_AreaOptions.Add("line from your hand");
			_AreaOptions.Add("living creatures within a 10-ft.-radius burst");
			_AreaOptions.Add("nonchaotic creatures in a 40-ft.-radius spread centered on you");
			_AreaOptions.Add("nonevil creatures in a 40-ft.-radius spread centered on you");
			_AreaOptions.Add("nongood creatures in a 40-ft.-radius spread centered on you");
			_AreaOptions.Add("nonlawful creatures in a 40-ft.-radius spread centered on you");
			_AreaOptions.Add("nonlawful creatures within a burst that fills a 30-ft. cube");
			_AreaOptions.Add("one 20-ft. cube/level (S)");
			_AreaOptions.Add("one 20-ft. square/level");
			_AreaOptions.Add("one 30-ft. cube/level (S)");
			_AreaOptions.Add("one or more living creatures within a 10-ft.-radius burst");
			_AreaOptions.Add("or Target one 20-ft. cube/level  or one fire-based magic item (S)");
			_AreaOptions.Add("plants in a 40-ft.-radius spread");
			_AreaOptions.Add("see text");
			_AreaOptions.Add("several living creatures within a 40-ft.-radius burst");
			_AreaOptions.Add("several living creatures, no two of which may be more than 30 ft. apart");
			_AreaOptions.Add("several undead creatures within a 40-ft.-radius burst");
			_AreaOptions.Add("Target object touched");
			_AreaOptions.Add("Target or object touched or up to 5 sq. ft./level");
			_AreaOptions.Add("Target or one creature, one object, or a 5-ft. cube");
			_AreaOptions.Add("Target or see text");
			_AreaOptions.Add("Target, Effect, or  see text");
			_AreaOptions.Add("The caster and all allies within a 50-ft. burst, centered on the caster");
			_AreaOptions.Add("two 10-ft. cubes per level (S)");
			_AreaOptions.Add("up to 10-ft.-radius/level emanation centered on you");
			_AreaOptions.Add("up to 200 sq. ft./level (S)");
			_AreaOptions.Add("up to one 10-ft. cube/level (S)");
			_AreaOptions.Add("up to two 10-ft. cubes/level (S)");
			_AreaOptions.Add("water in a volume of 10 ft./level by 10 ft./level by 2 ft./level (S)");

            //Targets
			_TargetsOptions.Add("creature or object touchedobject touched");
			_TargetsOptions.Add("creature touched");
			_TargetsOptions.Add("one animal");
			_TargetsOptions.Add("one creature");
			_TargetsOptions.Add("one creature or object");
			_TargetsOptions.Add("one creature or object/level, no two of which can be more than 30 ft. apart");
			_TargetsOptions.Add("one creature/level");
			_TargetsOptions.Add("one creature/level in a 20-ft.-radius burst centered on you");
			_TargetsOptions.Add("one creature/level touched");
			_TargetsOptions.Add("one creature/level, no two of which can be more than 30 ft. apart");
			_TargetsOptions.Add("one humanoid creature");
			_TargetsOptions.Add("one humanoid creature/level, no two of which can be more than 30 ft. apart");
			_TargetsOptions.Add("one living creature");
			_TargetsOptions.Add("one living creature/level, no two of which may be more than 30 ft. apart");
			_TargetsOptions.Add("one melee weapon");
			_TargetsOptions.Add("one undead creature");
			_TargetsOptions.Add("see text");
			_TargetsOptions.Add("up to one creature per level, all within 30 ft. of each other");
			_TargetsOptions.Add("up to one touched creature/level");
			_TargetsOptions.Add("weapon touched");
			_TargetsOptions.Add("you");
			_TargetsOptions.Add("you or creature touched");

            //Duration
            _DurationOptions.Add("1 round/level");
            _DurationOptions.Add("1 min/level");
            _DurationOptions.Add("1 hour/level");
            _DurationOptions.Add("1 day/level");
            _DurationOptions.Add("concentration");
            _DurationOptions.Add("instantaneous");
            _DurationOptions.Add("permanent");
            _DurationOptions.Add("see text");

            //Saving Throw
            _SavingThrowOptions.Add("Fortitude half");
			_SavingThrowOptions.Add("Fortitude half; see text");
			_SavingThrowOptions.Add("Fortitude negates");
			_SavingThrowOptions.Add("Fortitude negates (harmless)");
			_SavingThrowOptions.Add("Fortitude partial");
			_SavingThrowOptions.Add("Fortitude partial; see text");
			_SavingThrowOptions.Add("Reflex half");
			_SavingThrowOptions.Add("Reflex half; see text");
			_SavingThrowOptions.Add("Reflex negates");
			_SavingThrowOptions.Add("Reflex negates (harmless)");
			_SavingThrowOptions.Add("Reflex partial");
			_SavingThrowOptions.Add("Reflex partial; see text");
			_SavingThrowOptions.Add("Will disbelief");
			_SavingThrowOptions.Add("Will half");
			_SavingThrowOptions.Add("Will half; see text");
			_SavingThrowOptions.Add("Will negates");
			_SavingThrowOptions.Add("Will negates (harmless)");
			_SavingThrowOptions.Add("Will partial");
			_SavingThrowOptions.Add("Will partial; see text");
			_SavingThrowOptions.Add("none");
			_SavingThrowOptions.Add("none; see text");
			_SavingThrowOptions.Add("see text");

            //SpellResistance
			_SpellResistanceOptions.Add("no");
			_SpellResistanceOptions.Add("see text");
			_SpellResistanceOptions.Add("yes");
			_SpellResistanceOptions.Add("yes (harmless)");
        }


        static void Spell_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "name")
            {
                Spell s = (Spell)sender;
                RemoveSpellDictionaryItem(s, s.Oldname);
                AddSpellDictionaryItem(s);
            }
        }

        private static void RemoveSpellDictionaryItem(Spell s, string oldname)
        {
            string checkname = oldname==null?s.name:oldname;

            if (_SpellDictionary.ContainsKey(checkname))
            {
                ObservableCollection<Spell> list = _SpellDictionary[checkname];
                list.Remove(s);
                if (list.Count == 0)
                {
                    _SpellDictionary.Remove(checkname);
                }
                s.PropertyChanged -= new PropertyChangedEventHandler(Spell_PropertyChanged);

            }
        }

        private static void AddSpellDictionaryItem(Spell s)
        {
            s.PropertyChanged += new PropertyChangedEventHandler(Spell_PropertyChanged);

            if (_SpellDictionary.ContainsKey(s.name))
            {

                ObservableCollection<Spell> list = _SpellDictionary[s.Name];
                if (!list.Contains(s))
                {
                    list.Add(s);
                }
            }
            else
            {
                _SpellDictionary[s.Name] = new ObservableCollection<Spell>() { s };
            }
        }

        static void _Spells_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (Spell s in e.OldItems)
                {
                    RemoveSpellDictionaryItem(s, null);
                }
            }
            if (e.NewItems != null)
            {
                foreach (Spell s in e.NewItems)
                {
                    AddSpellDictionaryItem(s);
                }

            }

        }

        public static Spell ByName(string name)
        {
            if (_SpellDictionary.ContainsKey(name))
            {
                return _SpellDictionary[name][0];
            }
            else if (_SpellDictionary.ContainsKey(CMStringUtilities.DecommaText(name)))
            {
                return _SpellDictionary[CMStringUtilities.DecommaText(name)][0];
            }
            else
            {
                name = StandarizeSpellName(name);

                if (_SpellDictionary.ContainsKey(name))
                {
                    return _SpellDictionary[name][0];
                }
            }

            return null;
        }

        public static string StandarizeSpellName(string name)
        {
            name = stripMeta(name);
            
            if (name.StartsWith("greater ", StringComparison.CurrentCultureIgnoreCase))
            {
                name = name.Substring("greater ".Length) + ", Greater";
            }

            if (name.StartsWith("lesser ", StringComparison.CurrentCultureIgnoreCase))
            {
                name = name.Substring("lesser ".Length) + ", Lesser";
            }
            
            if (name.StartsWith("mass ", StringComparison.CurrentCultureIgnoreCase))
            {
                name = name.Substring("mass ".Length) + ", Mass";
            }

            return name;
        }

        private static List<string> metaAdj = new List<string> { "quickened", "selective", "maximized", "empowered", "enlarged", "extended", "heightened", "silenced", "stilled", "widened" };
        private static string stripMeta(string name)
        {
            foreach (string meta in metaAdj)
            {
                if (name.IndexOf(meta, StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    name = Regex.Replace(name, meta, string.Empty, RegexOptions.IgnoreCase);
                }
            }
            return name.Trim();
        }        

        public static ObservableCollection<Spell> Spells
        {
            get
            {
                if (_Spells == null)
                {
                    LoadSpells();
                }
                return _Spells;
            }
        }


        public static Spell ByDetailsID(int id)
        {
            if (_SpellsByDetailsID == null)
            {
                LoadSpells();
            }
            Spell s;
            _SpellsByDetailsID.TryGetValue(id, out s);
            return s;
        }

        public static Spell ByDBLoaderID(int id)
        {
            return DBSpells.FirstOrDefault(s => s.DBLoaderID == id);
        }

        public static Spell ByID(bool custom, int id)
        {
            if (custom)
            {
                return ByDBLoaderID(id);
            }
            else
            {
                return ByDetailsID(id);
            }
        }

        public static bool TryByID(bool custom, int id, out Spell s)
        {
            s = ByID(custom, id);
            return s != null;
        }


        public static void AddCustomSpell(Spell s)
        {
            if (_Spells == null)
            {
                LoadSpells();
            }
            _SpellsDB.AddItem(s);
            Spells.Add(s);
            SpellsDBUpdated?.Invoke(new List<Spell>() { s }, new List<Spell>(), new List<Spell>());
        }

        public static void RemoveCustomSpell(Spell s)
        {

            _SpellsDB.DeleteItem(s);
            Spells.Remove(s);
            SpellsDBUpdated?.Invoke(new List<Spell>(), new List<Spell>(), new List<Spell>() { s });
        }

        public static void UpdateCustomSpell(Spell s, bool updateData = false)
        {
            _SpellsDB.UpdateItem(s);
            if (updateData)
            {
                Spell old = Spells.FirstOrDefault((c) => c.DBLoaderID == s.DBLoaderID);
                old.CopyFrom(s);
            }
            SpellsDBUpdated?.Invoke(new List<Spell>(), new List<Spell>() { s }, new List<Spell>());
        }

        public static IEnumerable<Spell> DBSpells
        {
            get
            {
                if (_Spells == null)
                {
                    LoadSpells();
                }
                return _SpellsDB.Items;
            }
        }


        public static ICollection<string> Schools
        {
            get
            {
                if (_Schools == null)
                {
                    LoadSpells();
                }
                return _Schools.Values;
            }
        }
        public static ICollection<string> Subschools
        {
            get
            {
                if (_Subschools == null)
                {
                    LoadSpells();
                }
                return _Subschools;
            }
        }
        public static ICollection<string> Descriptors
        {
            get
            {
                if (_Descriptors == null)
                {
                    LoadSpells();
                }
                return _Descriptors;
            }
        }

        
        public static ICollection<string> CastingTimeOptions
        {
            get
            {
                return _CastingTimeOptions;
            }
        }

        public static ICollection<string> RangeOptions
        {
            get
            {
                return _RangeOptions;
            }
        }

        public static ICollection<string> AreaOptions
        {
            get
            {
                return _AreaOptions;
            }
        }

        public static ICollection<string> TargetsOptions
        {
            get
            {
                return _TargetsOptions;
            }
        }

        public static ICollection<string> DurationOptions
        {
            get
            {
                return _DurationOptions;
            }
        }

        public static ICollection<string> SavingThrowOptions
        {
            get
            {
                return _SavingThrowOptions;
            }
        }

        public static ICollection<string> SpellResistanceOptions
        {
            get
            {
                return _SavingThrowOptions;
            }
        }

        public Spell()
        {

        }

        public Spell(Spell s)
        {
            CopyFrom(s);
        }

        public void CopyFrom(Spell s)
        {
            _name = s._name;
            _school = s._school;
            _subschool = s._subschool;
            _descriptor = s._descriptor;
            _spell_level = s._spell_level;
            _casting_time = s._casting_time;
            _components = s._components;
            _costly_components = s._costly_components;
            _range = s._range;
            _targets = s._targets;
            _effect = s._effect;
            _dismissible = s._dismissible;
            _area = s._area;
            _duration = s._duration;
            _shapeable = s._shapeable;
            _saving_throw = s._saving_throw;
            _spell_resistence = s._spell_resistence;
            _description = s._description;
            _description_formated = s._description_formated;
            _source = s._source;
            _full_text = s._full_text;
            _verbal = s._verbal;
            _somatic = s._somatic;
            _material = s._material;
            _focus = s._focus;
            _divine_focus = s._divine_focus;
            _sor = s._sor;
            _wiz = s._wiz;
            _cleric = s._cleric;
            _druid = s._druid;
            _ranger = s._ranger;
            _bard = s._bard;
            _paladin = s._paladin;
            _alchemist = s._alchemist;
            _summoner = s._summoner;
            _witch = s._witch;
            _inquisitor = s._inquisitor;
            _oracle = s._oracle;
            _antipaladin = s._antipaladin;
            _assassin = s._assassin;
            _adept = s._adept;
            _red_mantis_assassin = s._red_mantis_assassin;
            _magus = s._magus;
            _URL = s._URL;
            _SLA_Level = s._SLA_Level;
            _preparation_time = s._preparation_time;
            _duplicated = s._duplicated;
            _acid = s._acid;
            _air = s._air;
            _chaotic = s._chaotic;
            _cold = s._cold;
            _curse = s._curse;
            _darkness = s._darkness;
            _death = s._death;
            _disease = s._disease;
            _earth = s._earth;
            _electricity = s._electricity;
            _emotion = s._emotion;
            _evil = s._evil;
            _fear = s._fear;
            _fire = s._fire;
            _force = s._force;
            _good = s._good;
            _language = s._language;
            _lawful = s._lawful;
            _light = s._light;
            _mind_affecting = s._mind_affecting;
            _pain = s._pain;
            _poison = s._poison;
            _shadow = s._shadow;
            _sonic = s._sonic;
            _water = s._water;
            _PotionWeight = s._PotionWeight;
            _DivineScrollWeight = s.DivineScrollWeight;
            _ArcaneScrollWeight = s.ArcaneScrollWeight;
            _WandWeight = s._WandWeight;
            _PotionLevel = s._PotionLevel;
            _PotionCost = s._PotionCost;
            _ArcaneScrollLevel = s._ArcaneScrollLevel;
            _ArcaneScrollCost = s._ArcaneScrollCost;
            _DivineScrollLevel = s._DivineScrollLevel;
            _DivineScrollCost = s._DivineScrollCost;
            _WandLevel = s._WandLevel;
            _WandCost = s._WandCost;

            if (s._Bonus != null)
            {
                _Bonus = (ConditionBonus)s._Bonus.Clone();
            }

            _DBLoaderID = s._DBLoaderID;

            _Adjuster = null;
        }

        public object Clone()
        {
            return new Spell(this);
        }

        protected override void SelfPropertyChanged(string name)
        {
            if (name == "DetailsID")
            {
                Notify("detailsid");
            }
        }

        public int? LevelForClass(CharacterClassEnum cl)
		{
			string levelStr = null;
			switch (cl)
			{
			case CharacterClassEnum.Alchemist:
				levelStr =  alchemist;
				break;
            
			case CharacterClassEnum.Antipaladin:
				if (antipaladin != null && antipaladin.Trim().Length > 0)
				{
					levelStr = antipaladin;
				}
				else
				{
					levelStr = paladin;
				}
           	break;
			case CharacterClassEnum.Bard:
				levelStr = bard;;
				break;
            case CharacterClassEnum.Cleric:
            {

                levelStr = cleric;
				break;
            }
            case CharacterClassEnum.Druid:
            {
                levelStr = druid;
				break;
            }
            case CharacterClassEnum.Inquisitor:
            {
                levelStr = inquisitor;
				break;
            }
            case CharacterClassEnum.Magus:
            {
                levelStr = magus;
				break;
            }
            case CharacterClassEnum.Oracle:
            {
				if (oracle != null && oracle.Trim().Length > 0)
				{
					levelStr = oracle;
				}
				else
				{
					levelStr = cleric;
				};
				break;
            }
            case CharacterClassEnum.Paladin:
            {
                levelStr = paladin;
				break;
            }
            case CharacterClassEnum.Ranger:
            {
                levelStr = ranger;
				break;
            }
            case CharacterClassEnum.Sorcerer:
            {

                levelStr = sor;
				break;
            }
            case CharacterClassEnum.Summoner:
            {
                levelStr = summoner;
				break;
            }
            case CharacterClassEnum.Witch:
            {
                levelStr = witch;
				break;
            }
            case CharacterClassEnum.Wizard:
            {
                levelStr = wiz;
				break;
            }
			}
				
			if (levelStr != null)
			{
				int val;
				if (int.TryParse(levelStr, out val))
				{
					return val;
				}
				
			}
		
			return null;
		}
		
		public IEnumerable<int> AllLevels
		{
			get
			{
                SortedDictionary<int, int> levList = new SortedDictionary<int, int>();
				
				foreach (CharacterClassEnum cl in Enum.GetValues(typeof(CharacterClassEnum)))
				{
					int? val = LevelForClass(cl);
					
					if (val != null)
					{
						levList[val.Value] = val.Value;
					}
				}
				
				return levList.Values;
				
			}
		}
		
		public bool IsLevel(int lev)
		{
			foreach (CharacterClassEnum cl in Enum.GetValues(typeof(CharacterClassEnum)))
			{
				int? val = LevelForClass(cl);
				
				if (val == lev)
				{
					return true;
				}
			}
			return false;
		}
        
        public String name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    Oldname = _name;
                    _name = value;
                    Notify("name");
                    Notify("Name");
                }
            }
        }

        [XmlIgnore]
        private string Oldname { get; set; }


        [XmlIgnore]
        public String Name
        {
            get { return name; }
            set { name = value; }
        }
        public String school
        {
            get { return _school; }
            set
            {
                if (_school != value)
                {
                    _school = value;
                    Notify("school");                }
            }
        }
        public String subschool
        {
            get { return _subschool; }
            set
            {
                if (_subschool != value)
                {
                    _subschool = value;
                    Notify("subschool");                }
            }
        }
        public String descriptor
        {
            get { return _descriptor; }
            set
            {
                if (_descriptor != value)
                {
                    _descriptor = value;
                    Notify("descriptor");                }
            }
        }
        public String spell_level
        {
            get { return _spell_level; }
            set
            {
                if (_spell_level != value)
                {
                    _spell_level = value;
                    Notify("spell_level");                }
            }
        }
        public String casting_time
        {
            get
            {
                UpdateFromDetailsDB();
                return _casting_time;
            }
            set
            {
                if (_casting_time != value)
                {
                    _casting_time = value;
                    Notify("casting_time");                }
            }
        }
        public String components
        {
            get
            {
                UpdateFromDetailsDB();
                return _components;
            }
            set
            {
                if (_components != value)
                {
                    _components = value;
                    if (_Adjuster != null)
                    {
                        _Adjuster.UpdateComponents();
                    }
                    Notify("components");
                }
            }
        }
        public String costly_components
        {
            get
            {
                UpdateFromDetailsDB();
                return _costly_components;
            }
            set
            {
                if (_costly_components != value)
                {
                    _costly_components = value;
                    Notify("costly_components");                }
            }
        }
        public String range
        {
            get
            {
                UpdateFromDetailsDB();
                return _range;
            }
            set
            {
                if (_range != value)
                {
                    _range = value;
                    Notify("range");                }
            }
        }
        public String targets
        {
            get
            {
                UpdateFromDetailsDB();
                return _targets;
            }
            set
            {
                if (_targets != value)
                {
                    _targets = value;
                    Notify("targets");                }
            }
        }
        public String duration
        {
            get
            {
                //UpdateFromDetailsDB();
                return _duration;
            }
            set
            {
                if (_duration != value)
                {
                    _duration = value;
                    Notify("duration");                }
            }
        }
        public String dismissible
        {
            get
            {
                //UpdateFromDetailsDB();
                return _dismissible;
            }
            set
            {
                if (_dismissible != value)
                {
                    _dismissible = value;
                    Notify("dismissible");                }
            }
        }
        public String shapeable
        {
            get
            {
                //UpdateFromDetailsDB();
                return _shapeable;
            }
            set
            {
                if (_shapeable != value)
                {
                    _shapeable = value;
                    Notify("shapeable");                }
            }
        }
        public String saving_throw
        {
            get
            {
                UpdateFromDetailsDB();
                return _saving_throw;
            }
            set
            {
                if (_saving_throw != value)
                {
                    _saving_throw = value;
                    Notify("saving_throw");                }
            }
        }
        public String spell_resistence
        {
            get
            {
                UpdateFromDetailsDB();
                return _spell_resistence;
            }
            set
            {
                if (_spell_resistence != value)
                {
                    _spell_resistence = value;
                    Notify("spell_resistence");                }
            }
        }
        public String description
        {
            get 
            {
                UpdateFromDetailsDB();
                return _description; 
            }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    Notify("description");                }
            }
        }
        public String description_formated
        {
            get
            {
                UpdateFromDetailsDB();
                return _description_formated;
            }
            set
            {
                if (_description_formated != value)
                {
                    _description_formated = value;
                    Notify("description_formated");                }
            }
        }
        public String source
        {
            get { return _source; }
            set
            {
                if (_source != value)
                {
                    _source = value;
                    Notify("source");                }
            }
        }
        public String full_text
        {
            get { return _full_text; }
            set
            {
                if (_full_text != value)
                {
                    _full_text = value;
                    Notify("full_text");                }
            }
        }
        public String verbal
        {
            get { return _verbal; }
            set
            {
                if (_verbal != value)
                {
                    _verbal = value;
                    Notify("verbal");                }
            }
        }
        public String somatic
        {
            get { return _somatic; }
            set
            {
                if (_somatic != value)
                {
                    _somatic = value;
                    Notify("somatic");                }
            }
        }
        public String material
        {
            get { return _material; }
            set
            {
                if (_material != value)
                {
                    _material = value;
                    Notify("material");                }
            }
        }
        public String focus
        {
            get { return _focus; }
            set
            {
                if (_focus != value)
                {
                    _focus = value;
                    Notify("focus");                }
            }
        }
        public String divine_focus
        {
            get { return _divine_focus; }
            set
            {
                if (_divine_focus != value)
                {
                    _divine_focus = value;
                    Notify("divine_focus");                }
            }
        }
        public String sor
        {
            get { return _sor; }
            set
            {
                if (_sor != value)
                {
                    _sor = value;
                    if (_sor == "NULL")
                    {
                        _sor = null;
                    }   
                    Notify("sor");                }
            }
        }
        public String wiz
        {
            get { return _wiz; }
            set
            {
                if (_wiz != value)
                {
                    _wiz = value;
                    if (_wiz == "NULL")
                    {
                        _wiz = null;
                    }   
                    Notify("wiz");                }
            }
        }
        public String cleric
        {
            get { return _cleric; }
            set
            {
                if (_cleric != value)
                {
                    _cleric = value;
                    if (_cleric == "NULL")
                    {
                        _cleric = null;
                    }   
                    Notify("cleric");                }
            }
        }
        public String druid
        {
            get { return _druid; }
            set
            {
                if (_druid != value)
                {
                    _druid = value;
                    if (_druid == "NULL")
                    {
                        _druid = null;
                    }   
                    Notify("druid");                }
            }
        }
        public String ranger
        {
            get { return _ranger; }
            set
            {
                if (_ranger != value)
                {
                    _ranger = value;
                    if (_ranger == "NULL")
                    {
                        _ranger = null;
                    }    
                    Notify("ranger");                }
            }
        }
        public String bard
        {
            get { return _bard; }
            set
            {
                if (_bard != value)
                {
                    _bard = value;
                    if (_bard == "NULL")
                    {
                        _bard = null;
                    }     
                    Notify("bard");                }
            }
        }
        public String paladin
        {
            get { return _paladin; }
            set
            {
                if (_paladin != value)
                {
                    _paladin = value;
                    if (_paladin == "NULL")
                    {
                        _paladin = null;
                    }     
                    Notify("paladin");                }
            }
        }
        public String effect
        {
            get { return _effect; }
            set
            {
                if (_effect != value)
                {
                    _effect = value;
                    Notify("effect");                }
            }
        }

        public String area
        {
            get
            {
                //UpdateFromDetailsDB();
                return _area;
            }
            set
            {
                if (_area != value)
                {
                    _area = value;
                    Notify("area");                }
            }
        }


        public String alchemist
        {
            get { return _alchemist; }
            set
            {
                if (_alchemist != value)
                {
                    _alchemist = value;
                    if (_alchemist == "NULL")
                    {
                        _alchemist = null;
                    }     
                    Notify("alchemist");                }
            }
        }
        public String summoner
        {
            get { return _summoner; }
            set
            {
                if (_summoner != value)
                {
                    _summoner = value;
                    if (_summoner == "NULL")
                    {
                        _summoner = null;
                    }     
                    Notify("summoner");                }
            }
        }
        public String witch
        {
            get { return _witch; }
            set
            {
                if (_witch != value)
                {
                    _witch = value;
                    if (_witch == "NULL")
                    {
                        _witch = null;
                    }     
                    Notify("witch");                }
            }
        }
        public String inquisitor
        {
            get { return _inquisitor; }
            set
            {
                if (_inquisitor != value)
                {
                    _inquisitor = value;
                    if (_inquisitor == "NULL")
                    {
                        _inquisitor = null;
                    }      
                    Notify("inquisitor");                }
            }
        }
        public String oracle
        {
            get { return _oracle; }
            set
            {
                if (_oracle != value)
                {
                    _oracle = value;
                    if (_oracle == "NULL")
                    {
                        _oracle = null;
                    }                  
                    Notify("oracle");                }
            }
        }
        public String antipaladin
        {
            get { return _antipaladin; }
            set
            {
                if (_antipaladin != value)
                {
                    _antipaladin = value;
                    if (_antipaladin == "NULL")
                    {
                        _antipaladin = null;
                    }
                    Notify("antipaladin");                }
            }
        }

        public String assassin
        {
            get { return _assassin; }
            set
            {
                if (_assassin != value)
                {
                    _assassin = value;
                    Notify("assassin");                }
            }
        }

        public String red_mantis_assassin
        {
            get { return _red_mantis_assassin; }
            set
            {
                if (_red_mantis_assassin != value)
                {
                    _red_mantis_assassin = value;
                    Notify("red_mantis_assassin");                }
            }
        }

        public String adept
        {
            get { return _adept; }
            set
            {
                if (_adept != value)
                {
                    _adept = value;
                    Notify("adept");                }
            }
        }
        public String URL
        {
            get { return _URL; }
            set
            {
                if (_URL != value)
                {
                    _URL = value;
                    Notify("URL");                }
            }
        }

        public String magus
        {
            get { return _magus; }
            set
            {
                if (_magus != value)
                {
                    _magus = value;
                    Notify("magus");                }
            }
        }
        public String SLA_Level
        {
            get { return _SLA_Level; }
            set
            {
                if (_SLA_Level != value)
                {
                    _SLA_Level = value;
                    Notify("SLA_Level");                }
            }
        }
        public String preparation_time
        {
            get { return _preparation_time; }
            set
            {
                if (_preparation_time != value)
                {
                    _preparation_time = value;
                    Notify("preparation_time");                }
            }
        }
        public bool duplicated
        {
            get { return _duplicated; }
            set
            {
                if (_duplicated != value)
                {
                    _duplicated = value;
                    Notify("duplicated");                }
            }
        }
        public String acid
        {
            get { return _acid; }
            set
            {
                if (_acid != value)
                {
                    _acid = value;
                    Notify("acid");                }
            }
        }
        public String air
        {
            get 
            { 
                UpdateFromDetailsDB(); 
                return _air; 
            }
            set
            {
                if (_air != value)
                {
                    _air = value;
                    Notify("air");                }
            }
        }
        public String chaotic
        {
            get
            {
                UpdateFromDetailsDB();
                return _chaotic;
            }
            set
            {
                if (_chaotic != value)
                {
                    _chaotic = value;
                    Notify("chaotic");                }
            }
        }
        public String cold
        {
            get
            {
                UpdateFromDetailsDB();
                return _cold;
            }
            set
            {
                if (_cold != value)
                {
                    _cold = value;
                    Notify("cold");                }
            }
        }
        public String curse
        {
            get
            {
                UpdateFromDetailsDB();
                return _curse;
            }
            set
            {
                if (_curse != value)
                {
                    _curse = value;
                    Notify("curse");                }
            }
        }
        public String darkness
        {
            get
            {
                UpdateFromDetailsDB();
                return _darkness;
            }
            set
            {
                if (_darkness != value)
                {
                    _darkness = value;
                    Notify("darkness");                }
            }
        }
        public String death
        {
            get
            {
                UpdateFromDetailsDB();
                return _death;
            }
            set
            {
                if (_death != value)
                {
                    _death = value;
                    Notify("death");                }
            }
        }
        public String disease
        {
            get
            {
                UpdateFromDetailsDB();
                return _disease;
            }
            set
            {
                if (_disease != value)
                {
                    _disease = value;
                    Notify("disease");                }
            }
        }
        public String earth
        {
            get
            {
                UpdateFromDetailsDB();
                return _earth;
            }
            set
            {
                if (_earth != value)
                {
                    _earth = value;
                    Notify("earth");                }
            }
        }
        public String electricity
        {
            get
            {
                UpdateFromDetailsDB();
                return _electricity;
            }
            set
            {
                if (_electricity != value)
                {
                    _electricity = value;
                    Notify("electricity");                }
            }
        }
        public String emotion
        {
            get
            {
                UpdateFromDetailsDB();
                return _emotion;
            }
            set
            {
                if (_emotion != value)
                {
                    _emotion = value;
                    Notify("emotion");                }
            }
        }
        public String evil
        {
            get
            {
                UpdateFromDetailsDB();
                return _evil;
            }
            set
            {
                if (_evil != value)
                {
                    _evil = value;
                    Notify("evil");                }
            }
        }
        public String fear
        {
            get
            {
                UpdateFromDetailsDB();
                return _fear;
            }
            set
            {
                if (_fear != value)
                {
                    _fear = value;
                    Notify("fear");                }
            }
        }
        public String fire
        {
            get
            {
                UpdateFromDetailsDB();
                return _fire;
            }
            set
            {
                if (_fire != value)
                {
                    _fire = value;
                    Notify("fire");                }
            }
        }
        public String force
        {
            get
            {
                UpdateFromDetailsDB();
                return _force;
            }
            set
            {
                if (_force != value)
                {
                    _force = value;
                    Notify("force");                }
            }
        }
        public String good
        {
            get
            {
                UpdateFromDetailsDB();
                return _good;
            }
            set
            {
                if (_good != value)
                {
                    _good = value;
                    Notify("good");                }
            }
        }
        public String language
        {
            get
            {
                UpdateFromDetailsDB();
                return _language;
            }
            set
            {
                if (_language != value)
                {
                    _language = value;
                    Notify("language");                }
            }
        }
        public String lawful
        {
            get
            {
                UpdateFromDetailsDB();
                return _lawful;
            }
            set
            {
                if (_lawful != value)
                {
                    _lawful = value;
                    Notify("lawful");                }
            }
        }
        public String light
        {
            get
            {
                UpdateFromDetailsDB();
                return _light;
            }
            set
            {
                if (_light != value)
                {
                    _light = value;
                    Notify("light");                }
            }
        }
        public String mind_affecting
        {
            get
            {
                UpdateFromDetailsDB();
                return _mind_affecting;
            }
            set
            {
                if (_mind_affecting != value)
                {
                    _mind_affecting = value;
                    Notify("mind_affecting");                }
            }
        }
        public String pain
        {
            get
            {
                UpdateFromDetailsDB();
                return _pain;
            }
            set
            {
                if (_pain != value)
                {
                    _pain = value;
                    Notify("pain");                }
            }
        }
        public String poison
        {
            get
            {
                UpdateFromDetailsDB();
                return _poison;
            }
            set
            {
                if (_poison != value)
                {
                    _poison = value;
                    Notify("poison");                }
            }
        }
        public String shadow
        {
            get
            {
                UpdateFromDetailsDB();
                return _shadow;
            }
            set
            {
                if (_shadow != value)
                {
                    _shadow = value;
                    Notify("shadow");                }
            }
        }
        public String sonic
        {
            get
            {
                UpdateFromDetailsDB();
                return _sonic;
            }
            set
            {
                if (_sonic != value)
                {
                    _sonic = value;
                    Notify("sonic");                }
            }
        }
        public String water
        {
            get
            {
                UpdateFromDetailsDB();
                return _water;
            }
            set
            {
                if (_water != value)
                {
                    _water = value;
                    Notify("water");                }
            }
        }

        [XmlElement("id")]
        public int detailsid
        {
            get { return _DBLoaderID; }
            set
            {
                if (_DBLoaderID != value)
                {
                    DBLoaderID = value;
                }
            }
        }


        public string PotionWeight
        {
            get { return _PotionWeight; }
            set
            {
                if (_PotionWeight != value)
                {
                    _PotionWeight = value;
                    Notify("PotionWeight");                }
            }
        }
        public string DivineScrollWeight
        {
            get { return _DivineScrollWeight; }
            set
            {
                if (_DivineScrollWeight != value)
                {
                    _DivineScrollWeight = value;
                    Notify("DivineScrollWeight");                }
            }
        }
        public string ArcaneScrollWeight
        {
            get { return _ArcaneScrollWeight; }
            set
            {
                if (_ArcaneScrollWeight != value)
                {
                    _ArcaneScrollWeight = value;
                    Notify("ArcaneScrollWeight");                }
            }
        }
        public string WandWeight
        {
            get { return _WandWeight; }
            set
            {
                if (_WandWeight != value)
                {
                    _WandWeight = value;
                    Notify("WandWeight");                }
            }
        }


        public String PotionLevel
        {
            get { return _PotionLevel; }
            set
            {
                if (_PotionLevel != value)
                {
                    _PotionLevel = value;
                    Notify("PotionLevel");                }
            }
        }
        public String PotionCost
        {
            get { return _PotionCost; }
            set
            {
                if (_PotionCost != value)
                {
                    _PotionCost = value;
                    Notify("PotionCost");                }
            }
        }
        public String ArcaneScrollLevel
        {
            get { return _ArcaneScrollLevel; }
            set
            {
                if (_ArcaneScrollLevel != value)
                {
                    _ArcaneScrollLevel = value;
                    Notify("ArcaneScrollLevel");                }
            }
        }
        public String ArcaneScrollCost
        {
            get { return _ArcaneScrollCost; }
            set
            {
                if (_ArcaneScrollCost != value)
                {
                    _ArcaneScrollCost = value;
                    Notify("ArcaneScrollCost");                }
            }
        }
        public String DivineScrollLevel
        {
            get { return _DivineScrollLevel; }
            set
            {
                if (_DivineScrollLevel != value)
                {
                    _DivineScrollLevel = value;
                    Notify("DivineScrollLevel");                }
            }
        }
        public String DivineScrollCost
        {
            get { return _DivineScrollCost; }
            set
            {
                if (_DivineScrollCost != value)
                {
                    _DivineScrollCost = value;
                    Notify("DivineScrollCost");                }
            }
        }
        public String WandLevel
        {
            get { return _WandLevel; }
            set
            {
                if (_WandLevel != value)
                {
                    _WandLevel = value;
                    Notify("WandLevel");                }
            }
        }
        public String WandCost
        {
            get { return _WandCost; }
            set
            {
                if (_WandCost != value)
                {
                    _WandCost = value;
                    Notify("WandCost");                }
            }
        }

        public ConditionBonus Bonus
        {
            get { return _Bonus; }
            set
            {
                if (_Bonus != value)
                {
                    _Bonus = value;
                    Notify("Bonus");                }
            }
        }

        public static List<string> DetailsFields
        {
            get
            {
                return new List<String>() 
                {
                    "casting_time",
                    "components",
                    "costly_components",
                    "range",
                    "targets",
                    "effect",
                    //"dismissible",
                    //"area",
                    //"duration",
                    //"shapeable",
                    "saving_throw",
                    "spell_resistence",
                    "description",
                    "description_formated",
                    "acid",
                    "air",
                    "chaotic",
                    "cold",
                    "curse",
                    "darkness",
                    "death",
                    "disease",
                    "earth",
                    "electricity",
                    "emotion",
                    "evil",
                    "fear",
                    "fire",
                    "force",
                    "good",
                    "language",
                    "lawful",
                    "light",
                    "mind_affecting",
                    "pain",
                    "poison",
                    "shadow",
                    "sonic",
                    "water",
                };

            }
        }

        void UpdateFromDetailsDB()
        {
            if (_DBLoaderID != 0)
            {
                //perform updating from DB
                var list = DetailsDB.LoadDetails(_DBLoaderID.ToString(), "Spells", DetailsFields);


                _casting_time = list["casting_time"];
                _components = list["components"];
                _costly_components = list["costly_components"];
                _range = list["range"];
                _targets = list["targets"];
                _effect = list["effect"];
                //_dismissible = list["dismissible"];
                //_area = list["area"];
                //_duration = list["duration"];
                //_shapeable = list["shapeable"];
                _saving_throw = list["saving_throw"];
                _spell_resistence = list["spell_resistence"];
                _description = list["description"];
                _description_formated = list["description_formated"];
                _acid = list["acid"];                    
                _air = list["air"];
                _chaotic = list["chaotic"];
                _cold= list["cold"];
                _curse= list["curse"];
                _darkness= list["darkness"];
                _death= list["death"];
                _disease= list["disease"];
                _earth= list["earth"];
                _electricity= list["electricity"];
                _emotion= list["emotion"];
                _evil= list["evil"];
                _fear= list["fear"];
                _fire= list["fire"];
                _force= list["force"];
                _good= list["good"];
                _language= list["language"];
                _lawful= list["lawful"];
                _light= list["light"];
                _mind_affecting= list["mind_affecting"];
                _pain= list["pain"];
                _poison= list["poison"];
                _shadow= list["shadow"];
                _sonic= list["sonic"];
                _water= list["water"];

                _DBLoaderID = 0;
            }
        }
        
        [XmlIgnore]
        public String nameforsort
        {
            get
            {
                return RomanNumbers.FindAndReplace(name);
            }
        }

        public override string ToString()
        {
            return name;
        }

        [XmlIgnore]
        public SpellAdjuster Adjuster
        {
            get
            {
                if (_Adjuster == null)
                {
                    _Adjuster = new SpellAdjuster(this);
                }

                return _Adjuster;
            }
        }
                    

        public class SpellAdjuster : SimpleNotifyClass
        {

            public static Dictionary<string, string> classes = new Dictionary<string, string>();

            private Spell _Spell;
            private ObservableCollection<LevelAdjusterInfo> _Levels;
            private ObservableCollection<LevelAdjusterInfo> _UnusedLevels;

            private bool _Verbal;
            private bool _Somatic;
            private bool _Material;
            private bool _Focus;
            private bool _DivineFocus;
            private String _MaterialText;
            private String _FocusText;
            private String _Duration;
            private bool _Dismissible;


            private bool _Parsing;
            private bool _UpdatingText;
            private bool _UpdatingLevels;
            private bool _LoadingLevels;


            static SpellAdjuster()
            {
                classes["sor"] = "Sorcerer";
                classes["wiz"] = "Wizard";
                classes["cleric"] = "Cleric";
                classes["druid"] = "Druid";
                classes["ranger"] = "Ranger";
                classes["bard"] = "Bard";
                classes["paladin"] = "Paladin";
                classes["alchemist"] = "Alchemist";
                classes["summoner"] = "Summoner";
                classes["witch"] = "Witch";
                classes["inquisitor"] = "Inquisitor";
                classes["oracle"] = "Oracle";
                classes["antipaladin"] = "Antipaladin";
                classes["assassin"] = "Assassin";
                classes["adept"] = "Adept";
                classes["red_mantis_assassin"] = "Red Mantis Assassin";
                classes["magus"] = "Magus";
            }

            public static Dictionary<string, string> Classes
            {
                get
                {
                    return classes;
                }
            }

            public SpellAdjuster(Spell spell)
            {
                _Spell = spell;
                _Spell.PropertyChanged += new PropertyChangedEventHandler(_spell_PropertyChanged);
                _Levels = new ObservableCollection<LevelAdjusterInfo>();
                _UnusedLevels = new ObservableCollection<LevelAdjusterInfo>();

                _Levels.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(_Levels_CollectionChanged);

                LoadInfo();
                
                ParseComponents();

                ParseDuration();
            }



            public void UpdateComponents()
            {
                ParseComponents();
            }

            private void ParseComponents()
            {
                if (!_UpdatingText)
                {
                    _Parsing = true;
                    try
                    {
                        if (_Spell.components != null)
                        {
                            Match m = Regex.Match(_Spell.components,
                                "^(?<v>V)?(, )?(?<s>S)?(, )?" +
                                "((?<m>M(?<mdf>///DF)?( \\((?<mtext>[^)]+)\\))?))?" +
                                "(,? ?" + "((?<f>F)|(?<df>DF)|(?<fdf>///FDF))" +
                                "( \\((?<ftext>[^)]+)\\))?" + ")?"
                                );

                            if (m.Success)
                            {
                                Verbal = m.GroupSuccess("v");
                                Somatic = m.GroupSuccess("s");
                                Material = m.GroupSuccess("m");
                                Focus = m.AnyGroupSuccess(new string[] { "f", "fdf" });
                                DivineFocus = m.AnyGroupSuccess(new string[] { "df", "mdf", "fdf" });


                                MaterialText = m.Value("mtext");
                                FocusText = m.Value("ftext");
                            }

                        }
                    }
                    finally
                    {
                        _Parsing = false;
                    }
                }

            }

            private void UpdateText()
            {
                if (!_Parsing)
                {
                    _UpdatingText = true;
                    _Spell.components = ToString();
                    _UpdatingText = false;
                }
            }

            public override string  ToString()
            {
                string text = "";

                if (Verbal)
                {
                    text = text.AppendListItem("V");
                }
                
                if (Somatic)
                {
                    text = text.AppendListItem("S");
                }

                if (Material)
                {
                    if (DivineFocus && !Focus)
                    {
                        text = text.AppendListItem("M/DF");
                    }
                    else
                    {
                        text = text.AppendListItem("M");
                    }

                    if (MaterialText != null && MaterialText.Trim().Length > 0)
                    {
                        text += " (" + MaterialText.Trim() + ")";
                    }
                }

                if (Focus || DivineFocus)
                {
                    if (!Material && !Focus)
                    {
                        text = text.AppendListItem("DF");
                    }
                    else if (Focus && DivineFocus)
                    {
                        text = text.AppendListItem("F/DF");
                    }
                    else if (Focus && !DivineFocus)
                    {
                        text = text.AppendListItem("F");
                    }

                    if (!(DivineFocus && Material &&  !Focus) && FocusText != null && FocusText.Length > 0)
                    {
                        text += " (" + FocusText + ")";
                    }
                }

                return text;

            }

            public bool Verbal
            {
                get { return _Verbal; }
                set
                {
                    if (_Verbal != value)
                    {
                        _Verbal = value;
                        UpdateText();
                        Notify("Verbal");                    }
                }
            }
            public bool Somatic
            {
                get { return _Somatic; }
                set
                {
                    if (_Somatic != value)
                    {
                        _Somatic = value;
                        UpdateText();
                        Notify("Somatic");                    }
                }
            }
            public bool Material
            {
                get { return _Material; }
                set
                {
                    if (_Material != value)
                    {
                        _Material = value;
                        UpdateText();
                        Notify("Material");                    }
                }
            }
            public bool Focus
            {
                get { return _Focus; }
                set
                {
                    if (_Focus != value)
                    {
                        _Focus = value;
                        UpdateText();
                        Notify("Focus");                    }
                }
            }
            public bool DivineFocus
            {
                get { return _DivineFocus; }
                set
                {
                    if (_DivineFocus != value)
                    {
                        _DivineFocus = value;
                        UpdateText();
                        Notify("DivineFocus");                    }
                }
            }
            public String MaterialText
            {
                get { return _MaterialText; }
                set
                {
                    if (_MaterialText != value)
                    {
                        _MaterialText = value;
                        UpdateText();
                        Notify("MaterialText");                    }
                }
            }
            public String FocusText
            {
                get { return _FocusText; }
                set
                {
                    if (_FocusText != value)
                    {
                        _FocusText = value;
                        UpdateText();
                        Notify("FocusText");                    }
                }
            }


            public String Duration
            {
                get 
                { 
                    return _Duration; 
                }
                set
                {
                    if (_Duration != value)
                    {
                        _Duration = value;
                        SetDuration();
                        Notify("Duration");                    }
                }
            }
            public bool Dismissible
            {
                get 
                { 
                    return _Dismissible; 
                }
                set
                {
                    if (_Dismissible != value)
                    {
                        _Dismissible = value;
                        SetDuration();
                        Notify("Dismissible");                    }
                }
            }

            private void ParseDuration()
            {
                if (_Spell.duration != null)
                {
                    Match m = Regex.Match(_Spell.duration, "(?<text>[^(]+) \\(D\\)");

                    if (m.Success)
                    {
                        _Dismissible = true;
                        _Duration = m.Value("text");
                    }
                    else
                    {
                        _Dismissible = false;
                        _Duration = _Spell.duration;
                    }
                   
                }
            }

            private void SetDuration()
            {
                if (_Duration != null)
                {
                    _Spell.duration = _Duration;
                    if (_Dismissible)
                    {
                        _Spell.duration += " (D)";
                    }
                }
            }



            void _Levels_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
            {
                if (e.NewItems != null)
                {
                    foreach (LevelAdjusterInfo info in e.NewItems)
                    {
                        if (info != null)
                        {
                            info.PropertyChanged += new PropertyChangedEventHandler(info_PropertyChanged);
                        }
                    }
                }

                if (!_LoadingLevels)
                {

                    List<LevelAdjusterInfo> ul = new List<LevelAdjusterInfo>(_UnusedLevels);

                    if (e.OldItems != null)
                    {
                        foreach (LevelAdjusterInfo info in e.OldItems)
                        {
                            if (info != null)
                            {
                                if (ul.FirstOrDefault(a => a != null && a.Property == info.Property) == null)
                                {
                                    ul.Add(info);
                                }
                            }

                        }
                    }


                    if (e.NewItems != null)
                    {
                        foreach (LevelAdjusterInfo info in e.NewItems)
                        {
                            if (info != null)
                            {
                                info.PropertyChanged += new PropertyChangedEventHandler(info_PropertyChanged);

                                LevelAdjusterInfo oldInfo = ul.FirstOrDefault(a => a.Property == info.Property);
                                if (oldInfo != null)
                                {
                                    ul.Remove(oldInfo);
                                }
                            }
                        }
                    }
                    ul.Sort((a, b) => a.Class.CompareTo(b.Class));

                    System.Diagnostics.Debug.Assert(_UnusedLevels != null);
                    if (_UnusedLevels != null)
                    {
                        _UnusedLevels.Clear();
                        foreach (LevelAdjusterInfo info in ul)
                        {
                            _UnusedLevels.Add(info);
                        }
                    }
                }


                SaveInfo();
            }

            void _spell_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (classes.ContainsKey(e.PropertyName))
                {
                    LoadInfo();
                }
            }

            public void LoadInfo()
            {
                
                if (!_UpdatingLevels)
                {
                    _LoadingLevels = true;
                    _Levels.Clear();
                    _UnusedLevels.Clear();
                    List<LevelAdjusterInfo> ul = new List<LevelAdjusterInfo>();
                    
                    Type t = typeof(Spell);

                    foreach (KeyValuePair<string, string> pair in classes)
                    {
                        PropertyInfo p = t.GetProperty(pair.Key);

                        string val = (string)p.GetGetMethod().Invoke(_Spell, new object[] { });
                        bool added = false;
                        
                        LevelAdjusterInfo info = new LevelAdjusterInfo();
                        info.Class = pair.Value;
                        info.Property = pair.Key;


                        if (val != null && val.Length > 0 && val != "NULL")
                        {
                            int level;
                            if (int.TryParse(val, out level))
                            {
                                info.Level = level;
                                _Levels.Add(info);
                                added = true;

                            }
                        }

                        if (!added)
                        {
                            ul.Add(info);

                        }


                    }

                    ul.Sort((a, b) => a.Class.CompareTo(b.Class));

                    foreach (var lev in ul)
                    {
                        _UnusedLevels.Add(lev);
                    }

                    _LoadingLevels = false;
                }
            }

            public void SaveInfo()
            {
                if (!_LoadingLevels)
                {
                    _UpdatingLevels = true;

                    Type t = typeof(Spell);
                    foreach (KeyValuePair<string, string> pair in classes)
                    {

                        PropertyInfo p = t.GetProperty(pair.Key);

                        LevelAdjusterInfo info = LevelValue(_Levels, pair.Key);

                        if (info == null)
                        {
                            p.GetSetMethod().Invoke(_Spell, new object[] { null });
                        }
                        else
                        {
                            p.GetSetMethod().Invoke(_Spell, new object[] { info.Level.ToString() });
                        }
                    }

                    List<LevelAdjusterInfo> list = new List<LevelAdjusterInfo>(_Levels);


                    string levelText = "";

                    while (list.Count > 0)
                    {
                        LevelAdjusterInfo info = list[0];

                        if (info == null)
                        {
                            System.Console.WriteLine("Info null");
                            list.RemoveAt(0);
                        }
                        else
                        {

                            string text = null;

                            if (info.Property == "sor" || info.Property == "wiz")
                            {
                                text = GetPairText(list, "sor", "wiz");
                            }
                            else if (info.Property == "cleric" || info.Property == "oracle")
                            {
                                text = GetPairText(list, "cleric", "oracle");
                            }

                            if (text == null)
                            {
                                text = info.Class + " " + info.Level;
                                list.RemoveAt(0);
                            }


                            if (levelText.Length > 0)
                            {
                                levelText += ", ";
                            }
                            levelText += text;
                        }
                    }

                    _Spell.spell_level = levelText;

                    _UpdatingLevels = false;
                }
            }

            private LevelAdjusterInfo LevelValue(IEnumerable<LevelAdjusterInfo> list, string property)
            {
                return list.FirstOrDefault(a => (a != null && a.Property == property));
            }

            private string GetPairText(List<LevelAdjusterInfo> list, string class1, string class2)
            {
                LevelAdjusterInfo info1 = LevelValue(list, class1);
                LevelAdjusterInfo info2 = LevelValue(list, class2);

                if (info1 != null && info2 != null && info1.Level == info2.Level)
                {
                    list.RemoveAll(a => a!= null && (a.Property == class1) || (a.Property == class2));

                    string text = info1.Class + "/" + info2.Class + " " + info1.Level;

                    return text;
                }
                else
                {
                    return null;
                }
            }



            void info_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                SaveInfo();
            }

            public ObservableCollection<LevelAdjusterInfo> Levels
            {
                get
                {
                    return _Levels;
                }             
            }

            public bool ContainsClass(String className)
            {
                foreach (LevelAdjusterInfo li in _Levels)
                {
                    if (li.Class == className)
                    {
                        return true;
                    }
                }
                return false;
            }

            public ObservableCollection<LevelAdjusterInfo> UnusedLevels
            {
                get
                {
                    return _UnusedLevels;
                }
            }


            public class LevelAdjusterInfo : SimpleNotifyClass
            {

                private string _Class;
                private int _Level;
                private string _Property;


                public string Class
                {
                    get { return _Class; }
                    set
                    {
                        if (_Class != value)
                        {
                            _Class = value;
                            Notify("Class");                        }
                    }
                }
                public int Level
                {
                    get { return _Level; }
                    set
                    {
                        if (_Level != value)
                        {
                            _Level = value;
                            Notify("Level");                        
                        }
                    }
                }
                public string Property
                {
                    get { return _Property; }
                    set
                    {
                        if (_Property != value)
                        {
                            _Property = value;
                            Notify("Property");                        
                        }
                    }
                }

            }
        }

    }
}
