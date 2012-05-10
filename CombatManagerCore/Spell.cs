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

    public class Spell : INotifyPropertyChanged, ICloneable, IDBLoadable
    {
        public event PropertyChangedEventHandler PropertyChanged;
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
        private String _deity;
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

        private ConditionBonus _Bonus;

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




        private int _DBLoaderID;

        private SpellAdjuster _Adjuster;

        private static ObservableCollection<Spell> _Spells;
        private static SortedDictionary<string, string> _Schools;
        private static Dictionary<string, ObservableCollection<Spell>> _SpellDictionary;

        private static DBLoader<Spell> _SpellsDB;


        static Spell()
        {
            List<Spell> set = XmlListLoader<Spell>.Load("Spells.xml");

            List<Spell> remove = new List<Spell>();
            foreach (var cur in set)
            {
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
            _SpellDictionary = new Dictionary<string,ObservableCollection<Spell>>(new InsensitiveEqualityCompararer());

            _Schools = new SortedDictionary<string, string>();
            foreach (Spell s in _Spells)
            {
                string r = StringCapitalizer.Capitalize(s.school);
                _Schools[r] = r;
                AddSpellDictionaryItem(s);

            }


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
            else if (name.StartsWith("greater ", StringComparison.CurrentCultureIgnoreCase))
            {
                string subname = name.Substring("greater ".Length) + ", Greater";


                if (_SpellDictionary.ContainsKey(subname))
                {
                    return _SpellDictionary[subname][0];
                }

            }
            else if (name.StartsWith("lesser ", StringComparison.CurrentCultureIgnoreCase))
            {
                string subname = name.Substring("lesser ".Length) + ", Lesser";


                if (_SpellDictionary.ContainsKey(subname))
                {
                    return _SpellDictionary[subname][0];
                }

            }


            return null;
            
                
        }

        

        public static ObservableCollection<Spell> Spells
        {
            get
            {
                return _Spells;
            }
        }

        public static void AddCustomSpell(Spell s)
        {
            _SpellsDB.AddItem(s);
            Spells.Add(s);
        }

        public static void RemoveCustomSpell(Spell s)
        {

            _SpellsDB.DeleteItem(s);
            Spells.Remove(s);
        }

        public static void UpdateCustomSpell(Spell s)
        {
            _SpellsDB.UpdateItem(s);
        }



        public static ICollection<string> Schools
        {
            get
            {
                return _Schools.Values;
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
            _deity = s._deity;
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
				SortedList<int, int> levList = new SortedList<int, int>();
				
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
                    if (PropertyChanged != null) 
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("name"));
                        PropertyChanged(this, new PropertyChangedEventArgs("Name")); 
                    }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("school")); }
                }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("subschool")); }
                }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("descriptor")); }
                }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("spell_level")); }
                }
            }
        }
        public String casting_time
        {
            get { return _casting_time; }
            set
            {
                if (_casting_time != value)
                {
                    _casting_time = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("casting_time")); }
                }
            }
        }
        public String components
        {
            get { return _components; }
            set
            {
                if (_components != value)
                {
                    _components = value;
                    if (_Adjuster != null)
                    {
                        _Adjuster.UpdateComponents();
                    }
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("components")); }

                }
            }
        }
        public String costly_components
        {
            get { return _costly_components; }
            set
            {
                if (_costly_components != value)
                {
                    _costly_components = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("costly_components")); }
                }
            }
        }
        public String range
        {
            get { return _range; }
            set
            {
                if (_range != value)
                {
                    _range = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("range")); }
                }
            }
        }
        public String targets
        {
            get { return _targets; }
            set
            {
                if (_targets != value)
                {
                    _targets = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("targets")); }
                }
            }
        }
        public String duration
        {
            get { return _duration; }
            set
            {
                if (_duration != value)
                {
                    _duration = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("duration")); }
                }
            }
        }
        public String dismissible
        {
            get { return _dismissible; }
            set
            {
                if (_dismissible != value)
                {
                    _dismissible = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("dismissible")); }
                }
            }
        }
        public String shapeable
        {
            get { return _shapeable; }
            set
            {
                if (_shapeable != value)
                {
                    _shapeable = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("shapeable")); }
                }
            }
        }
        public String saving_throw
        {
            get { return _saving_throw; }
            set
            {
                if (_saving_throw != value)
                {
                    _saving_throw = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("saving_throw")); }
                }
            }
        }
        public String spell_resistence
        {
            get { return _spell_resistence; }
            set
            {
                if (_spell_resistence != value)
                {
                    _spell_resistence = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("spell_resistence")); }
                }
            }
        }
        public String description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("description")); }
                }
            }
        }
        public String description_formated
        {
            get { return _description_formated; }
            set
            {
                if (_description_formated != value)
                {
                    _description_formated = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("description_formated")); }
                }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("source")); }
                }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("full_text")); }
                }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("verbal")); }
                }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("somatic")); }
                }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("material")); }
                }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("focus")); }
                }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("divine_focus")); }
                }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("sor")); }
                }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("wiz")); }
                }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("cleric")); }
                }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("druid")); }
                }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("ranger")); }
                }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("bard")); }
                }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("paladin")); }
                }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("effect")); }
                }
            }
        }

        public String deity
        {
            get { return _deity; }
            set
            {
                if (_deity != value)
                {
                    _deity = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("deity")); }
                }
            }
        }
        public String area
        {
            get { return _area; }
            set
            {
                if (_area != value)
                {
                    _area = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("area")); }
                }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("alchemist")); }
                }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("summoner")); }
                }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("witch")); }
                }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("inquisitor")); }
                }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("oracle")); }
                }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("antipaladin")); }
                }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("assassin")); }
                }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("red_mantis_assassin")); }
                }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("adept")); }
                }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("URL")); }
                }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("magus")); }
                }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("SLA_Level")); }
                }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("preparation_time")); }
                }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("duplicated")); }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("PotionWeight")); }
                }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("DivineScrollWeight")); }
                }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("ArcaneScrollWeight")); }
                }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("WandWeight")); }
                }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("PotionLevel")); }
                }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("PotionCost")); }
                }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("ArcaneScrollLevel")); }
                }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("ArcaneScrollCost")); }
                }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("DivineScrollLevel")); }
                }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("DivineScrollCost")); }
                }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("WandLevel")); }
                }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("WandCost")); }
                }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Bonus")); }
                }
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

        [XmlIgnore]
        public bool IsCustom
        {
            get
            {
                return DBLoaderID != 0;
            }
        }
        

        public int DBLoaderID
        {
            get
            {
                return _DBLoaderID;
            }
            set
            {
                _DBLoaderID = value;
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
                    

        public class SpellAdjuster : INotifyPropertyChanged
        {

            public static Dictionary<string, string> classes = new Dictionary<string, string>();
            public event PropertyChangedEventHandler PropertyChanged;

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
                        if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Verbal")); }
                    }
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
                        if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Somatic")); }
                    }
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
                        if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Material")); }
                    }
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
                        if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Focus")); }
                    }
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
                        if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("DivineFocus")); }
                    }
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
                        if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("MaterialText")); }
                    }
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
                        if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("FocusText")); }
                    }
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
                        if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Duration")); }
                    }
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
                        if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Dismissible")); }
                    }
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
                                if (ul.FirstOrDefault(a => a.Property == info.Property) == null)
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

                    _Spell.spell_level = levelText;

                    _UpdatingLevels = false;
                }
            }

            private LevelAdjusterInfo LevelValue(IEnumerable<LevelAdjusterInfo> list, string property)
            {
                return list.FirstOrDefault(a => a.Property == property);
            }

            private string GetPairText(List<LevelAdjusterInfo> list, string class1, string class2)
            {
                LevelAdjusterInfo info1 = LevelValue(list, class1);
                LevelAdjusterInfo info2 = LevelValue(list, class2);

                if (info1 != null && info2 != null && info1.Level == info2.Level)
                {
                    list.RemoveAll(a => (a.Property == class1) || (a.Property == class2));

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

            public ObservableCollection<LevelAdjusterInfo> UnusedLevels
            {
                get
                {
                    return _UnusedLevels;
                }
            }


            public class LevelAdjusterInfo : INotifyPropertyChanged
            {

                public event PropertyChangedEventHandler PropertyChanged;

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
                            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Class")); }
                        }
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
                            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Level")); }
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
                            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Property")); }
                        }
                    }
                }

            }
        }

    }
}
