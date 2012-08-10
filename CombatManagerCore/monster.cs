/*
 *  monster.cs
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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.XPath;
using System.Xml;
using System.Xml.Serialization;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;

namespace CombatManager
{

    public class MonsterParseException : Exception
    {
        public MonsterParseException(string message)
            : base(message)
        {

        }

        public MonsterParseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

    }

    [DataContract]
    public class Monster : INotifyPropertyChanged, ICloneable, IDBLoadable
    {
        static ObservableCollection<Monster> monsters;
        

        static void LoadBestiary()
        {
            List<Monster> set = XmlListLoader<Monster>.Load("BestiaryShort.xml");
			

            List<Monster> set2 = XmlListLoader<Monster>.Load("NPCShort.xml");
			
			if (set2 != null)
			{
	            foreach (Monster m in set2)
	            {
	                m.NPC = true;
	            }
	            set.AddRange(set2);
			}
		
#if!MONO
            if (DBSettings.UseDB)
            {
                List<Monster> dbMonsters = new List<Monster>(MonsterDB.DB.Monsters);
                set.AddRange(dbMonsters);
            }
#endif
			 
            monsters = new ObservableCollection<Monster>(set);


        }

        public static ObservableCollection<Monster> Monsters
        {
            get
            {
                return monsters;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        private String name;
        private String cr;
        private String xp;
        private String race;
        private String className;
        private String alignment;
        private String size;
        private String type;
        private String subType;
        private int init;
        private String senses;
        private String ac;
        private String ac_mods;
        private int hp;
        private String hd;
        private String saves;
        private int fort;
        private int reflex;
        private int will;
        private String save_mods;
        private String resist;
        private String dr;
        private String sr;
        private String speed;
        private String melee;
        private String ranged;
        private String space;
        private String reach;
        private String specialAttacks;
        private String spellLikeAbilities;
        private String abilitiyScores;
        private int baseAtk;
        private String cmb;
        private String cmd;
        private String feats;
        private String skills;
        private String racialMods;
        private String languages;
        private String sq;
        private String environment;
        private String organization;
        private String treasure;
        private String description_visual;
        private String group;
        private String source;
        private String isTemplate;
        private String specialAbilities;
        private String description;
        private String fullText;
        private String gender;
        private String bloodline;
        private String prohibitedSchools;
        private String beforeCombat;
        private String duringCombat;
        private String morale;
        private String gear;
        private String otherGear;
        private String vulnerability;
        private String note;
        private String characterFlag;
        private String companionFlag;
        private String fly;
        private String climb;
        private String burrow;
        private String swim;
        private String land;
        private String templatesApplied;
        private String offenseNote;
        private String baseStatistics;
        private String spellsPrepared;
        private String spellDomains;
        private String aura;
        private String defensiveAbilities;
        private String immune;
        private String hp_mods;
        private String spellsKnown;
        private String weaknesses;
        private String speed_mod;
        private String monsterSource;
        private String extractsPrepared;
        private String ageCategory;
        private bool dontUseRacialHD;
        private String variantParent;
        private bool npc;
        private String descHTML;

        private bool statsParsed;
        private int? strength;
        private int? dexterity;
        private int? constitution;
        private int? intelligence;
        private int? wisdom;
        private int? charisma;

        private bool specialAblitiesParsed;
        private ObservableCollection<SpecialAbility> specialAbilitiesList;

        private bool acParsed;
        private int fullAC;
        private int touchAC;
        private int flatFootedAC;
        private int naturalArmor;
        private int shield;
        private int armor;
        private int dodge;
        private int deflection;

        private bool skillsParsed;
        private bool skillValuesMayNeedUpdate;
        private Dictionary<String, SkillValue> skillValueDictionary;
        private List<SkillValue> skillValueList;

        private bool featsParsed;
        private List<string> featsList;

        private static Dictionary<string, Stat> _SkillsList;
        private static Dictionary<string, SkillInfo> _SkillsDetails;


        private ObservableCollection<ActiveCondition> _ActiveConditions;
        private ObservableCollection<Condition> _UsableConditions;
        
        private bool _LoseDexBonus;
        private bool _DexZero;
        private int? _PreLossDex;
        private bool _StrZero;
        private int? _PreLossStr;

        private int _DBLoaderID;

        private ObservableCollection<SpellBlockInfo> _SpellLikeAbilitiesBlock;
        private ObservableCollection<SpellBlockInfo> _SpellsKnownBlock;
        private ObservableCollection<SpellBlockInfo> _SpellsPreparedBlock;

        private struct DragonColorInfo
        {
            public string element;
            public string weaponType;
            public int distance;

            public DragonColorInfo(string element, string weaponType, int distance)
            {
                this.element = element;
                this.weaponType = weaponType;
                this.distance = distance;
            }
        }


        private static Dictionary<string, int> xpValues;
        private static Dictionary<string, bool> thrownAttacks;
        private static Dictionary<int, int> lowCRToIntChart;
        private static Dictionary<int, int> intToLowCRChart;
        private static Dictionary<string, int> flyQualityList;
        private static Dictionary<string, DragonColorInfo> dragonColorList;
        private static SortedDictionary<string, string> weaponNameList;
        private static Dictionary<CreatureType, string> creatureTypeNames;
        private static List<string> creatureTypeNamesList;
        private static Dictionary<string, CreatureType> creatureTypes;


        private MonsterAdjuster _Adjuster;



        public enum OrderAxis
        {
            Lawful = 0,
            Neutral = 1,
            Chaotic = 2
        }

        public enum MoralAxis
        {
            Good = 0,
            Neutral = 1,
            Evil = 2
        }


        public enum SaveType
        {
            Fort = 0,
            Ref,
            Will
        }
            

        public struct AlignmentType
        {
            public OrderAxis Order;
            public MoralAxis Moral;
        }

        public class SkillInfo
        {
            public string Name { get; set; }
            public Stat Stat { get; set; }
            public List<String> Subtypes { get; set; }
            public bool TrainedOnly { get; set; }
        }

        public enum FlyQuality
        {
            Clumsy = 0,
            Poor = 1,
            Average = 2,
            Good = 3,
            Perfect = 4
        }




        static Monster()
        {
            _SkillsList = new Dictionary<string, Stat>(new InsensitiveEqualityCompararer());




            _SkillsList["Acrobatics"] = Stat.Dexterity;
            _SkillsList["Appraise"] = Stat.Intelligence;
            _SkillsList["Bluff"] = Stat.Charisma;
            _SkillsList["Climb"] = Stat.Strength;
            _SkillsList["Craft"] = Stat.Intelligence;
            _SkillsList["Diplomacy"] = Stat.Charisma;
            _SkillsList["Disable Device"] = Stat.Dexterity;
            _SkillsList["Disguise"] = Stat.Charisma;
            _SkillsList["Escape Artist"] = Stat.Dexterity;
            _SkillsList["Fly"] = Stat.Dexterity;
            _SkillsList["Handle Animal"] = Stat.Charisma;
            _SkillsList["Heal"] = Stat.Wisdom;
            _SkillsList["Intimidate"] = Stat.Charisma;
            _SkillsList["Knowledge"] = Stat.Intelligence;
            _SkillsList["Linguistics"] = Stat.Intelligence;
            _SkillsList["Perception"] = Stat.Wisdom;
            _SkillsList["Perform"] = Stat.Charisma;
            _SkillsList["Profession"] = Stat.Wisdom;
            _SkillsList["Ride"] = Stat.Dexterity;
            _SkillsList["Sense Motive"] = Stat.Wisdom;
            _SkillsList["Sleight of Hand"] = Stat.Dexterity;
            _SkillsList["Spellcraft"] = Stat.Intelligence;
            _SkillsList["Stealth"] = Stat.Dexterity;
            _SkillsList["Survival"] = Stat.Wisdom;
            _SkillsList["Swim"] = Stat.Strength;
            _SkillsList["Use Magic Device"] = Stat.Charisma;


            _SkillsDetails = new Dictionary<string, SkillInfo>(new InsensitiveEqualityCompararer());
            foreach (KeyValuePair<string, Stat> item in _SkillsList)
            {
                SkillInfo info = new SkillInfo();
                info.Name = item.Key;
                info.Stat = item.Value;
                _SkillsDetails.Add(info.Name, info);
            }

            _SkillsDetails["Disable Device"].TrainedOnly = true;
            _SkillsDetails["Handle Animal"].TrainedOnly = true;
            _SkillsDetails["Profession"].TrainedOnly = true;
            _SkillsDetails["Sleight of Hand"].TrainedOnly = true;
            _SkillsDetails["Spellcraft"].TrainedOnly = true;
            _SkillsDetails["Use Magic Device"].TrainedOnly = true;

            SkillInfo know = _SkillsDetails["Knowledge"];
            know.TrainedOnly = true;
            know.Subtypes = new List<string>();
            know.Subtypes.Add("Arcana");
            know.Subtypes.Add("Dungeoneering");
            know.Subtypes.Add("Engineering");
            know.Subtypes.Add("Geography");
            know.Subtypes.Add("History"); 
            know.Subtypes.Add("Local"); 
            know.Subtypes.Add("Nature"); 
            know.Subtypes.Add("Nobility");  	 
            know.Subtypes.Add("Planes");
            know.Subtypes.Add("Religion");

            SkillInfo craft = _SkillsDetails["Craft"];
            craft.Subtypes = new List<string>();
            craft.Subtypes.Add("Alchemy");
            craft.Subtypes.Add("Armor");
            craft.Subtypes.Add("Baskets");
            craft.Subtypes.Add("Blacksmith");
            craft.Subtypes.Add("Books");
            craft.Subtypes.Add("Bows");
            craft.Subtypes.Add("Calligraphy");
            craft.Subtypes.Add("Carpentry");
            craft.Subtypes.Add("Cloth");
            craft.Subtypes.Add("Clothing");
            craft.Subtypes.Add("Gemcutting");
            craft.Subtypes.Add("Glass");
            craft.Subtypes.Add("Jewelry");
            craft.Subtypes.Add("Leather");
            craft.Subtypes.Add("Locks");
            craft.Subtypes.Add("Painting");
            craft.Subtypes.Add("Pottery");
            craft.Subtypes.Add("Rope");
            craft.Subtypes.Add("Sculpture");
            craft.Subtypes.Add("Ships");
            craft.Subtypes.Add("Shoes");
            craft.Subtypes.Add("Stonemasonry");
            craft.Subtypes.Add("Traps");
            craft.Subtypes.Add("Weapons");

            SkillInfo perform = _SkillsDetails["Perform"];

            perform.Subtypes = new List<string>();
            perform.Subtypes.Add("Act"); 
            perform.Subtypes.Add("Comedy"); 
            perform.Subtypes.Add("Dance"); 
            perform.Subtypes.Add("Keyboard Instruments"); 
            perform.Subtypes.Add("Oratory"); 
            perform.Subtypes.Add("Percussion Instruments"); 
            perform.Subtypes.Add("Sing"); 
            perform.Subtypes.Add("String Instruments"); 
            perform.Subtypes.Add("Wind Instruments");


            SkillInfo profession = _SkillsDetails["Profession"];
            profession.Subtypes = new List<string>();
            profession.Subtypes.Add("Architect"); 
            profession.Subtypes.Add("Baker"); 
            profession.Subtypes.Add("Barkeep"); 
            profession.Subtypes.Add("Barmaid"); 
            profession.Subtypes.Add("Barrister"); 
            profession.Subtypes.Add("Brewer"); 
            profession.Subtypes.Add("Butcher"); 
            profession.Subtypes.Add("Clerk"); 
            profession.Subtypes.Add("Cook"); 
            profession.Subtypes.Add("Courtesean"); 
            profession.Subtypes.Add("Driver"); 
            profession.Subtypes.Add("Engineer"); 
            profession.Subtypes.Add("Farmer"); 
            profession.Subtypes.Add("Fisherman"); 
            profession.Subtypes.Add("Fortune-Teller"); 
            profession.Subtypes.Add("Gambler"); 
            profession.Subtypes.Add("Gardener"); 
            profession.Subtypes.Add("Herbalist"); 
            profession.Subtypes.Add("Innkeeper"); 
            profession.Subtypes.Add("Librarian"); 
            profession.Subtypes.Add("Medium"); 
            profession.Subtypes.Add("Merchant"); 
            profession.Subtypes.Add("Midwife"); 
            profession.Subtypes.Add("Miller"); 
            profession.Subtypes.Add("Miner"); 
            profession.Subtypes.Add("Porter"); 
            profession.Subtypes.Add("Sailor"); 
            profession.Subtypes.Add("Scribe"); 
            profession.Subtypes.Add("Shepherd"); 
            profession.Subtypes.Add("Soldier"); 
            profession.Subtypes.Add("Soothsayer"); 
            profession.Subtypes.Add("Stable Master"); 
            profession.Subtypes.Add("Tanner"); 
            profession.Subtypes.Add("Torturer"); 
            profession.Subtypes.Add("Trapper");
            profession.Subtypes.Add("Woodcutter");



            try
            {
                xpValues = new Dictionary<string, int>();
                xpValues.Add("1/8", 50);
                xpValues.Add("1/6", 65);
                xpValues.Add("1/4", 100);
                xpValues.Add("1/3", 135);
                xpValues.Add("1/2", 200);

                lowCRToIntChart = new Dictionary<int, int>();
                lowCRToIntChart.Add(8, -4);
                lowCRToIntChart.Add(6, -3);
                lowCRToIntChart.Add(4, -2);
                lowCRToIntChart.Add(3, -1);
                lowCRToIntChart.Add(2, 0);

                intToLowCRChart = new Dictionary<int, int>();


                foreach (KeyValuePair<int, int> pair in lowCRToIntChart)
                {
                    intToLowCRChart.Add(pair.Value, pair.Key);
                }

                thrownAttacks = new Dictionary<string, bool>();
                thrownAttacks.Add("rock", true);
                thrownAttacks.Add("dagger", false);
                thrownAttacks.Add("club", false);
                thrownAttacks.Add("spear", false);
                thrownAttacks.Add("shortspear", false);
                thrownAttacks.Add("dart", false);
                thrownAttacks.Add("javelin", false);
                thrownAttacks.Add("throwing axe", false);
                thrownAttacks.Add("light hammer", false);
                thrownAttacks.Add("trident", false);
                thrownAttacks.Add("shuriken", false);
                thrownAttacks.Add("net", false);



                //fly quality
                flyQualityList = new Dictionary<string, int>();
                flyQualityList.Add("clumsy", 0);
                flyQualityList.Add("poor", 1);
                flyQualityList.Add("average", 2);
                flyQualityList.Add("good", 3);
                flyQualityList.Add("perfect", 4);

                //elements
                dragonColorList = new Dictionary<string, DragonColorInfo>();
                dragonColorList.Add("black", new DragonColorInfo("acid", "line", 60));
                dragonColorList.Add("blue", new DragonColorInfo("electricity", "line", 60));
                dragonColorList.Add("brass", new DragonColorInfo("fire", "line", 60));
                dragonColorList.Add("bronze", new DragonColorInfo("electricity", "line", 60));
                dragonColorList.Add("copper", new DragonColorInfo("acid", "line", 60));
                dragonColorList.Add("gold", new DragonColorInfo("fire", "cone", 30));
                dragonColorList.Add("green", new DragonColorInfo("acid", "cone", 30));
                dragonColorList.Add("red", new DragonColorInfo("fire", "cone", 30));
                dragonColorList.Add("silver", new DragonColorInfo("cold", "cone", 30));
                dragonColorList.Add("white", new DragonColorInfo("cold", "cone", 30));

                LoadWeaponNames();

                creatureTypeNames = new Dictionary<CreatureType, string>();

                creatureTypeNames[CreatureType.Aberration] = "aberration";
                creatureTypeNames[CreatureType.Animal] = "animal";
                creatureTypeNames[CreatureType.Construct] = "construct";
                creatureTypeNames[CreatureType.Dragon] = "dragon";
                creatureTypeNames[CreatureType.Fey] = "fey";
                creatureTypeNames[CreatureType.Humanoid] = "humanoid";
                creatureTypeNames[CreatureType.MagicalBeast] = "magical beast";
                creatureTypeNames[CreatureType.MonstrousHumanoid] = "monstrous humanoid";
                creatureTypeNames[CreatureType.Ooze] = "ooze";
                creatureTypeNames[CreatureType.Outsider] = "outsider";
                creatureTypeNames[CreatureType.Plant] = "plant";
                creatureTypeNames[CreatureType.Undead] = "undead";
                creatureTypeNames[CreatureType.Vermin] = "vermin";

                creatureTypes = new Dictionary<string, CreatureType>();
                foreach (KeyValuePair<CreatureType, string> name in creatureTypeNames)
                {
                    creatureTypes[name.Value] = name.Key;
                }
				
				
				creatureTypeNamesList = new List<string>(creatureTypeNames.Values);
				creatureTypeNamesList.Sort();

                LoadBestiary();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                throw;
            }
        }
		
		public static List<string> CreatureTypeNames
		{
			get
			{
				return creatureTypeNamesList;
			}
		}

        private static void LoadWeaponNames()
        {
            weaponNameList = new SortedDictionary<string, string>();

            foreach (Weapon wp in Weapon.Weapons.Values)
            {
                if (!wp.Natural)
                {
                    string wpName = wp.Name.ToLower();
                    weaponNameList[wpName] = wpName;
                }
            }


        }

        public Monster()
        {
            skillValueDictionary = new Dictionary<string, SkillValue>(new InsensitiveEqualityCompararer());
            skillValueList = new List<SkillValue>();
        }

    


        public static Monster BlankMonster()
        {
            Monster m = new Monster();

            m.abilitiyScores = "Str 10, Dex 10, Con 10, Int 10, Wis 10, Cha 10";
            m.ParseStats();

            m.AC = "10, touch 10, flat-footed 10";
            m.AC_Mods = "";

            m.CMB = "+0";
            m.CMD = "10";

            m.HP = 5;
            m.HD = "1d8";

            m.Ref = 0;
            m.Fort = 0;
            m.Will = 0;

            m.CR = "1";
            m.XP = GetCRValue(m.CR).ToString();

            m.Alignment = "N";
            m.Size = "Medium";
            m.Type = "humanoid";

            m.Init = 0;
            m.Senses = "Perception +0";

            m.Speed = "30 ft.";

            return m;
        }

        public void CopyFrom(Monster m)
        {
            ActiveConditions.Clear();
            foreach (ActiveCondition c in m.ActiveConditions)
            {
                ActiveConditions.Add(new ActiveCondition(c));
            }

            UsableConditions.Clear();
            foreach (Condition c in m.UsableConditions)
            {
                UsableConditions.Add(new Condition(c));
            }

            DexZero = m.DexZero;
			
		    Name=m.name;
            CR=m.cr;
            XP=m.xp;
            Race=m.race;
            className=m.className;
            Alignment=m.alignment;
            Size=m.size;
            Type=m.type;
            SubType=m.subType;
            Init=m.init;
            Senses=m.senses;
            AC=m.ac;
            AC_Mods=m.ac_mods;
            HP=m.hp;
            HD=m.hd;
            Saves=m.saves;
            Fort=m.fort;
            Ref=m.reflex;
            Will=m.will;
            Save_Mods=m.save_mods;
            Resist=m.resist;
            DR=m.dr;
            SR=m.sr;
            Speed=m.speed;
            Melee=m.melee;
            Ranged=m.ranged;
            Space=m.space;
            Reach=m.reach;
            SpecialAttacks=m.specialAttacks;
            SpellLikeAbilities=m.spellLikeAbilities;
            AbilitiyScores=m.abilitiyScores;
            BaseAtk=m.baseAtk;
            CMB=m.cmb;
            CMD=m.cmd;
            Feats=m.feats;
            Skills=m.skills;
            RacialMods=m.racialMods;
            Languages=m.languages;
            SQ=m.sq;
            Environment=m.environment;
            Organization=m.organization;
            Treasure=m.treasure;
            Description_Visual=m.description_visual;
            Group=m.Group;
            Source=m.Source;
            IsTemplate=m.isTemplate;
            SpecialAbilities=m.specialAbilities;
            Description=m.description;
            FullText=m.fullText;
            Gender=m.gender;
            Bloodline=m.bloodline;
            ProhibitedSchools=m.prohibitedSchools;
            BeforeCombat=m.beforeCombat;
            DuringCombat=m.duringCombat;
            Morale=m.morale;
            Gear=m.gear;
            OtherGear=m.otherGear;
            Vulnerability=m.vulnerability;
            Note=m.note;
            CharacterFlag=m.characterFlag;
            CompanionFlag=m.companionFlag;
            Fly=m.fly;
            Climb=m.climb;
            Burrow=m.burrow;
            Swim=m.swim;
            Land=m.land;
            TemplatesApplied=m.templatesApplied;
            OffenseNote=m.offenseNote;
            BaseStatistics=m.baseStatistics;
            SpellsPrepared=m.spellsPrepared;
            SpellDomains=m.spellDomains;
            Aura=m.aura;
            DefensiveAbilities=m.defensiveAbilities;
            Immune=m.immune;
            HP_Mods=m.hp_mods;
            SpellsKnown=m.spellsKnown;
            Weaknesses=m.weaknesses;
            Speed_Mod=m.speed_mod;
            MonsterSource=m.monsterSource;
            ExtractsPrepared = m.extractsPrepared;
            AgeCategory = m.ageCategory;
            DontUseRacialHD = m.dontUseRacialHD;
            VariantParent = m.variantParent;
            NPC = m.npc;
            DescHTML = m.descHTML;


            StatsParsed=m.statsParsed;
            Strength=m.strength;
            Dexterity=m.dexterity;
            Constitution=m.constitution;
            Intelligence=m.intelligence;
            Wisdom=m.wisdom;
            Charisma=m.charisma;

            SpecialAblitiesParsed = m.specialAblitiesParsed;
            if (m.specialAbilitiesList != null)
            {
                specialAbilitiesList = new ObservableCollection<SpecialAbility>();
                foreach (SpecialAbility ability in m.specialAbilitiesList)
                {
                    specialAbilitiesList.Add((SpecialAbility)ability.Clone());
                }
            }

            AcParsed = m.acParsed;
            FullAC = m.fullAC;
            TouchAC = m.touchAC;
            FlatFootedAC = m.flatFootedAC;
            NaturalArmor = m.naturalArmor;
            Armor = m.armor;
            Dodge = m.dodge;
            Shield = m.shield;
            Deflection = m.deflection;

            skillsParsed = m.skillsParsed;
            if (m.skillsParsed)
            {
                skillValueDictionary = new Dictionary<string, SkillValue>(new InsensitiveEqualityCompararer());
                foreach (SkillValue skillValue in m.skillValueDictionary.Values)
                {

                    skillValueDictionary[skillValue.FullName] = (SkillValue)skillValue.Clone();
                }
            }

            featsParsed = m.featsParsed;
            if (featsList != null)
            {
                featsList = new List<string>(m.featsList);
            }

            if (m._SpellsPreparedBlock != null)
            {
                _SpellsPreparedBlock = new ObservableCollection<SpellBlockInfo>();
                foreach (SpellBlockInfo info in m._SpellsPreparedBlock)
                {
                    _SpellsPreparedBlock.Add(new SpellBlockInfo(info));
                }
            }
            if (m._SpellsKnownBlock != null)
            {
                _SpellsKnownBlock = new ObservableCollection<SpellBlockInfo>();
                foreach (SpellBlockInfo info in m._SpellsKnownBlock)
                {
                    _SpellsKnownBlock.Add(new SpellBlockInfo(info));
                }
            }
            if (m._SpellLikeAbilitiesBlock != null)
            {
                _SpellLikeAbilitiesBlock = new ObservableCollection<SpellBlockInfo>();
                foreach (SpellBlockInfo info in m._SpellLikeAbilitiesBlock)
                {
                    _SpellLikeAbilitiesBlock.Add(new SpellBlockInfo(info));
                }
            }

            DBLoaderID = m.DBLoaderID;

        }


		public object Clone()
		{
			Monster m = new Monster();

            BaseClone(m);
			
		    m.name=name;
            m.cr=cr;
            m.xp=xp;
            m.race=race;
            m.className=className;
            m.alignment=alignment;
            m.size=size;
            m.type=type;
            m.subType=subType;
            m.init=init;
            m.senses=senses;
            m.ac=ac;
            m.ac_mods=ac_mods;
            m.hp=hp;
            m.hd=hd;
            m.saves=saves;
            m.fort=fort;
            m.reflex=reflex;
            m.will=will;
            m.save_mods=save_mods;
            m.resist=resist;
            m.dr=dr;
            m.sr=sr;
            m.speed=speed;
            m.melee=melee;
            m.ranged=ranged;
            m.space=space;
            m.reach=reach;
            m.specialAttacks=specialAttacks;
            m.spellLikeAbilities=spellLikeAbilities;
            m.abilitiyScores=abilitiyScores;
            m.baseAtk=baseAtk;
            m.cmb=cmb;
            m.cmd=cmd;
            m.feats=feats;
            m.skills=skills;
            m.racialMods=racialMods;
            m.languages=languages;
            m.sq=sq;
            m.environment=environment;
            m.organization=organization;
            m.treasure=treasure;
            m.description_visual=description_visual;
            m.group=group;
            m.source=source;
            m.isTemplate=isTemplate;
            m.specialAbilities=specialAbilities;
            m.description=description;
            m.fullText=fullText;
            m.gender=gender;
            m.bloodline=bloodline;
            m.prohibitedSchools=prohibitedSchools;
            m.beforeCombat=beforeCombat;
            m.duringCombat=duringCombat;
            m.morale=morale;
            m.gear=gear;
            m.otherGear=otherGear;
            m.vulnerability=vulnerability;
            m.note=note;
            m.characterFlag=characterFlag;
            m.companionFlag=companionFlag;
            m.fly=fly;
            m.climb=climb;
            m.burrow=burrow;
            m.swim=swim;
            m.land=land;
            m.templatesApplied=templatesApplied;
            m.offenseNote=offenseNote;
            m.baseStatistics=baseStatistics;
            m.spellsPrepared=spellsPrepared;
            m.spellDomains=spellDomains;
            m.aura=aura;
            m.defensiveAbilities=defensiveAbilities;
            m.immune=immune;
            m.hp_mods=hp_mods;
            m.spellsKnown=spellsKnown;
            m.weaknesses=weaknesses;
            m.speed_mod=speed_mod;
            m.monsterSource=monsterSource;
            m.extractsPrepared = extractsPrepared;
            m.ageCategory = ageCategory;
            m.dontUseRacialHD = dontUseRacialHD;
            m.variantParent = variantParent;
            m.npc = npc;
            m.descHTML = descHTML;


            m.statsParsed=statsParsed;
            m.strength=strength;
            m.dexterity=dexterity;
            m.constitution=constitution;
            m.intelligence=intelligence;
            m.wisdom=wisdom;
            m.charisma=charisma;

            m.specialAblitiesParsed = specialAblitiesParsed;
            if (specialAbilitiesList != null)
            {
                m.specialAbilitiesList = new ObservableCollection<SpecialAbility>();
                foreach (SpecialAbility ability in specialAbilitiesList)
                {
                    m.specialAbilitiesList.Add((SpecialAbility)ability.Clone());
                }
            }

            m.acParsed = acParsed;
            m.fullAC = fullAC;
            m.touchAC = touchAC;
            m.flatFootedAC = flatFootedAC;
            m.naturalArmor = naturalArmor;
            m.armor = armor;
            m.dodge = dodge;
            m.shield = shield;
            m.deflection = deflection;

            m.skillsParsed = skillsParsed;
            if (skillsParsed)
            {
                m.skillValueDictionary = new Dictionary<string, SkillValue>(new InsensitiveEqualityCompararer());
                foreach (SkillValue skillValue in skillValueDictionary.Values)
                {

                    m.skillValueDictionary[skillValue.FullName] = (SkillValue)skillValue.Clone();
                }
            }

            m.featsParsed = featsParsed;
            if (featsList != null)
            {
                m.featsList = new List<string>(featsList);
            }

            if (_SpellsPreparedBlock != null)
            {
                m._SpellsPreparedBlock = new ObservableCollection<SpellBlockInfo>();
                foreach (SpellBlockInfo info in _SpellsPreparedBlock)
                {
                    m._SpellsPreparedBlock.Add(new SpellBlockInfo(info));
                }
            }
            if (_SpellsKnownBlock != null)
            {
                m._SpellsKnownBlock = new ObservableCollection<SpellBlockInfo>();
                foreach (SpellBlockInfo info in _SpellsKnownBlock)
                {
                    m._SpellsKnownBlock.Add(new SpellBlockInfo(info));
                }
            }
            if (_SpellLikeAbilitiesBlock != null)
            {
                m._SpellLikeAbilitiesBlock = new ObservableCollection<SpellBlockInfo>();
                foreach (SpellBlockInfo info in _SpellLikeAbilitiesBlock)
                {
                    m._SpellLikeAbilitiesBlock.Add(new SpellBlockInfo(info));
                }
            }



            m.DBLoaderID = DBLoaderID;

            return m;
		}

        public static List<Monster> FromFile(string filename)
        {
            List<Monster> returnMonsters = null;

            try
            {
                using (FileStream stream = new FileStream(filename, FileMode.Open))
                {

                    XPathDocument doc = new XPathDocument(stream);

                    XPathNavigator n = doc.CreateNavigator();

                    //look for herolab file
                    XPathNodeIterator it = n.Select("/document");

                    if (it.MoveNext())
                    {
                        string sig = it.Current.GetAttribute("signature", "");

                        if (sig == "Hero Lab Portfolio")
                        {
                            it = n.Select("/document/product");

                            if (it.MoveNext())
                            {

                                int major = 0;
                                int minor = 0;
                                int patch = 0;

                                int.TryParse(it.Current.GetAttribute("major", ""), out major);
                                int.TryParse(it.Current.GetAttribute("minor", ""), out minor);
                                int.TryParse(it.Current.GetAttribute("patch", ""), out patch);

                                if (!CheckVersion(major, minor, patch, 3, 6, 7))
                                {
                                    throw new MonsterParseException("Combat Manager requires files from a newer version of HeroLab." +
                                        "\r\nUpgrade HeroLab to the newest version, reload the file and save the file.");
                                }

                                returnMonsters = FromHeroLabFile(doc);
                            }
                        }
                    }
                    else
                    {
                        //look for PCGen file
                        it = n.Select("/group-set/groups/group/combatants");
                        if (it.MoveNext())
                        {
                            returnMonsters = FromPCGenExportFile(doc);
                        }


                    }


                    if (returnMonsters == null)
                    {
                        throw new MonsterParseException("Unrecognized file format");
                    }
                }


            }
            catch (IOException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                throw new MonsterParseException("CombatManager was not able to read the file.", ex);
            }
            catch (ArgumentException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                throw new MonsterParseException("CombatManger was not able to understand the file.", ex);
            }
            catch (FormatException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                throw new MonsterParseException("CombatManger was not able to understand the file.", ex);
            }
            catch (XmlException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                throw new MonsterParseException("CombatManger was not able to understand the file.", ex);
            }



            return returnMonsters;
        }

        private static bool CheckVersion(int major, int minor, int patch, int checkMajor, int checkMinor, int checkPatch)
        {
            if (major > checkMajor)
            {
                return true;
            }
            else if (major == checkMajor)
            {
                if (minor > checkMinor)
                {
                    return true;
                }
                else if (minor == checkMinor)
                {
                    if (patch >= checkPatch)
                    {
                        return true;
                    }
                }
            }
            return false;

        }

        private static List<Monster> FromHeroLabFile(XPathDocument doc)
        {
            List<Monster> monsters = new List<Monster>();


            //attempt to get the stats block
            XPathNavigator n = doc.CreateNavigator();
           

            XPathNodeIterator it = n.Select("/document/portfolio/hero");

            while (it.MoveNext())
            {

                Monster monster = new Monster();

                monster.Name = it.Current.GetAttribute("heroname", "");


                XPathNodeIterator itStats = it.Current.SelectChildren("statblock", "");

                if (itStats.MoveNext())
                {
                    string statsblock = itStats.Current.Value;

                    ImportHeroLabBlock(statsblock, monster);

                    monsters.Add(monster);
                }

                XPathNodeIterator itMinion = it.Current.SelectDescendants("minion", "", false);
                while (itMinion.MoveNext())
                {
                    monster = new Monster();
                    monster.Name = itMinion.Current.GetAttribute("heroname", "");
                    itStats = itMinion.Current.SelectChildren("statblock", "");

                    if (itStats.MoveNext())
                    {
                        string statsblock = itStats.Current.Value;

                        ImportHeroLabBlock(statsblock, monster);

                        monsters.Add(monster);
                    }
                }
            }

            return monsters;

        }


        private static List<Monster> FromPCGenExportFile(XPathDocument doc)
        {
            List<Monster> monsters = new List<Monster>();

            
            //attempt to get the stats block
            XPathNavigator n = doc.CreateNavigator();

            XPathNodeIterator it = n.Select("/group-set/groups/group/combatants/combatant");
            while (it.MoveNext())
            {

                Monster monster = new Monster();

                //get name
                XPathNodeIterator itVal = it.Current.SelectChildren("name", "");
                if (itVal.MoveNext())
                {
                    monster.Name = itVal.Current.Value;
                }

                Match m = null;

                //get type
                itVal = it.Current.SelectChildren("fullType", "");
                if (itVal.MoveNext())
                {
                    Regex regFull = new Regex("(?<align>" + AlignString + ")( )+(?<size>" + SizesString + ")( )*\r\n?" +
                         "(?<type>" + TypesString + ")", RegexOptions.IgnoreCase);

                    m = regFull.Match(itVal.Current.Value);

                    if (m.Success)
                    {
                        monster.Alignment = m.Groups["align"].Value;
                        if (monster.Alignment == "NN")
                        {
                            monster.Alignment = "N";
                        }
                        monster.Size = m.Groups["size"].Value;
                        monster.Type = m.Groups["type"].Value;
                    }
                }

                //get hp
                monster.HP = GetIntValue(it, "hitPoints");
                string hd = GetStringValue(it, "hitDice");

                hd = Regex.Replace(hd, "\\([0-9]+ hp\\)", "");
                hd = Regex.Replace(hd, "\\(|\\)", "");
                hd = "(" + hd.Trim() + ")";
                monster.HD = hd;

                //get stats
                string abilityScores = GetStringValue(it, "fullAbilities");

                string cha = GetStringValue(it, "cha");
                Regex regInt = new Regex("(?<val>[0-9]+) ");
                m = regInt.Match(cha);
                int chaInt;
                if (int.TryParse(m.Groups["val"].Value, out chaInt))
                {
                    abilityScores = Regex.Replace(abilityScores, "Cha (?<val>[0-9]+)",
                        delegate(Match ma)
                        {
                            return "Cha " + chaInt;
                        }
                    );
                }

                monster.abilitiyScores = abilityScores;
               



                monster.CR = GetStringValue(it, "challengeRating");

                monster.XP = GetCRValue(monster.CR).ToString();

                monster.Init = GetIntValue(it, "init-modifier");

                string ac = GetStringValue(it, "fullArmorClass");
                if (ac != null)
                {
                    ac = ac.Replace(":", "");
                    monster.AC = ac;
                }

                monster.ParseAC();


                monster.Fort = GetIntValue(it, "fortSave");
                monster.Ref = GetIntValue(it, "reflexSave");
                monster.Will = GetIntValue(it, "willSave");

                //load skills
                string skills = GetStringValue(it, "skills");
                skills = Regex.Replace(skills, ";( )*\r\n", ", ").Trim().Trim(new char[] { ',' });
                monster.skills = skills;

                
                //BAB, CMB, CMD
                SizeMods mods = SizeMods.GetMods(SizeMods.GetSize(monster.Size));
                monster.BaseAtk = GetIntValue(it, "baseAttack");


                monster.CMB = CMStringUtilities.PlusFormatNumber(monster.BaseAtk + AbilityBonus(monster.Strength) +
                    mods.Combat);

                monster.CMD = (monster.BaseAtk + mods.Combat + AbilityBonus(monster.Strength) + AbilityBonus(monster.Dexterity) + 10).ToString();


                string feats = GetStringValue(it, "feats");
                if (feats != null)
                {
                    feats = FixPCGenFeatList(feats);
                }
                monster.feats = feats;

                //load and fix speed
                string speed = GetStringValue(it, "speed");
                speed = Regex.Replace(speed, "Walk ", "");
                monster.Speed = speed.ToLower();

                

                //load senses
                string senses = GetStringValue(it, "senses").ToLower();

                //fix low light vision
                senses = Regex.Replace(senses, "low-light,", "low-light vision,");
                senses = Regex.Replace(senses, "^ *, perception", "Perception");

                //remove unneeded brackets
                senses = Regex.Replace(senses, "\\((?<val>[0-9]+) ft.\\)", delegate (Match ma)
                    {
                        return ma.Groups["val"].Value + " ft.";
                    }
                );
                
                //add perception
                Regex regSense =  new Regex(", Listen (\\+|-)[0-9]+, Spot (\\+|-)[0-9]+", RegexOptions.IgnoreCase);
                int perception = 0;
                if (monster.SkillValueDictionary.ContainsKey("Perception"))
                {
                    perception = monster.SkillValueDictionary["Perception"].Mod;
                }
                senses = regSense.Replace(senses, "; Perception " + CMStringUtilities.PlusFormatNumber(perception));

                //set senses
                monster.Senses = senses;


                monster.SpecialAttacks = GetStringValue(it, "specialAttacks");

                string gear = GetStringValue(it, "possessions");
                if (gear != null)
                {
                    gear = Regex.Replace(gear, "\r\n", "");
                    gear = Regex.Replace(gear, "[\\];[]+ *", ", ");
                    monster.gear = gear;
                }


                string attacks = GetStringValue(it, "attack");
                attacks = Regex.Replace(attacks, "(\r?\n)|:|(\r)", "");

                List<String> meleeStrings = new List<string>();
                List<String> rangedStrings = new List<string>();

                Regex regAttack = new Regex("(?<weapon>[ ,a-zA-Z0-9+]+)( \\((?<sub>[-+ a-zA-Z0-9/]+)\\))? (?<bonus>(N/A|([-+/0-9]+))) (?<type>(melee|ranged)) (?<dmg>\\([0-9]+d[0-9]+(\\+[0-9]+)?(/[0-9]+-20)?(/x[0-9]+)?\\))");

                foreach (Match ma in regAttack.Matches(attacks))
                {
                    string bonus = ma.Groups["bonus"].Value;

                    if (bonus == "N/A")
                    {
                        bonus = "+0";
                    }

                    string weaponname = ma.Groups["weapon"].Value.Trim();

                    if (String.Compare(weaponname, "Longbow", true) == 0)
                    {
                        if (ma.Groups["sub"].Success)
                        {
                            string sub = ma.Groups["sub"].Value;

                            if (Regex.Match(sub, "composite", RegexOptions.IgnoreCase).Success)
                            {
                                weaponname = "composite longbow";
                            }
                        }
                    }

                    string attack = weaponname + " " + bonus +
                        " " + ma.Groups["dmg"].Value;

                    if (ma.Groups["type"].Value == "melee")
                    {
                        meleeStrings.Add(attack.Trim());
                    }
                    else
                    {
                        rangedStrings.Add(attack.Trim());
                    }                       
                }

                string melee = "";

                foreach (string attack in meleeStrings)
                {
                    if (melee.Length > 0)
                    {
                        melee += " or ";
                    }
                    melee += attack;
                }

                monster.Melee = melee;


                string ranged = "";

                foreach (string attack in rangedStrings)
                {
                    if (ranged.Length > 0)
                    {
                        ranged += " or ";
                    }
                    ranged += attack;
                }

                monster.Ranged = ranged;


                monsters.Add(monster);


            }
            return monsters;
        }

        private static int? GetNullableIntValue(XPathNodeIterator it, string name)
        {
            int? returnVal = null;

            string val = GetStringValue(it, name);

            if (val != null)
            {
                int intVal;
                if (int.TryParse(val, out intVal))
                {
                    returnVal = intVal;
                }
            }

            return returnVal;
        }
        private static int GetIntValue(XPathNodeIterator it, string name)
        {
            int returnVal = 0;

            string val = GetStringValue(it, name);

            if (val != null)
            {
                int intVal;
                if (int.TryParse(val, out intVal))
                {
                    returnVal = intVal;
                }
            }

            return returnVal;
        }

        private static string GetStringValue(XPathNodeIterator it, string name)
        {
            string val = null;


            XPathNodeIterator itVal = it.Current.SelectChildren(name, "");
            if (itVal.MoveNext())
            {
                val = itVal.Current.Value;
            }


            return val;
        }

        private static string HeroLabStatRegexString(string stat)
        {
            return StringCapitalizer.Capitalize(stat) + " ([0-9]+/)?(?<" + stat.ToLower() + ">([0-9]+|-))";
        }

        private static void ImportHeroLabBlock(string statsblock, Monster monster)
        {

            //System.Diagnostics.Debug.WriteLine(statsblock);


            String strStatSeparator = ",[ ]+";

            //get stats
            string statsRegStr = HeroLabStatRegexString("Str") + strStatSeparator +
                 HeroLabStatRegexString("Dex") + strStatSeparator +
                  HeroLabStatRegexString("Con") + strStatSeparator +
                   HeroLabStatRegexString("Int") + strStatSeparator +
                    HeroLabStatRegexString("Wis") + strStatSeparator +
                     HeroLabStatRegexString("Cha");


            Regex regStats = new Regex(statsRegStr);
            
            
            
            Match m = regStats.Match(statsblock);
            monster.AbilitiyScores = "Str " + m.Groups["str"].Value +
            ", Dex " + m.Groups["dex"].Value +
            ", Con " + m.Groups["con"].Value +
            ", Int " + m.Groups["int"].Value +
            ", Wis " + m.Groups["wis"].Value +
            ", Cha " + m.Groups["cha"].Value;

            Regex regCR = new Regex("CR (?<cr>[0-9]+(/[0-9]+)?)\r\n");
            m = regCR.Match(statsblock);
            if (m.Success)
            {
                monster.CR = m.Groups["cr"].Value;

                monster.XP = GetCRValue(monster.CR).ToString();
            }



            //CN Medium Humanoid (Orc)
            string sizesText = SizesString;

            string typesText = TypesString;



            Regex regeAlSizeType = new Regex("(?<align>" + AlignString + ") (?<size>" + sizesText +
                ") (?<type>" + typesText + ") *(\\(|\r\n)");
            m = regeAlSizeType.Match(statsblock);

            if (m.Success)
            {
                monster.Alignment = m.Groups["align"].Value;
                if (monster.Alignment == "NN")
                {
                    monster.Alignment = "N";
                }
                monster.Size = m.Groups["size"].Value;
                monster.Type = m.Groups["type"].Value.ToLower();
            }
            else
            {
                monster.Alignment = "N";
                monster.Size = "Medium";
                monster.Type = "humanoid";
            }

            string raceClass = GetLine("Male", statsblock, true);
            if (raceClass == null)
            {
                raceClass = GetLine("Female", statsblock, true);
                monster.Gender = "Female";
            }
            else
            {
                monster.Gender = "Male";
            }

            if (raceClass != null)
            {
                m = Regex.Match(raceClass, "(?<race>[-a-zA-Z]+) (?<class>.+)?");

                if (m.Success)
                {
                    monster.Race = m.Groups["race"].Value;
                    if (m.Groups["class"].Success)
                    {
                        monster.Class = m.Groups["class"].Value;
                    }
                }

            }



            //init, senses, perception

            //Init +7; Senses Darkvision (60 feet); Perception +2
            Regex regSense = new Regex("Init (?<init>(\\+|-)[0-9]+)(; Senses )((?<senses>.+)(;|,) )?Perception (?<perception>(\\+|-)[0-9]+)");
            m = regSense.Match(statsblock);

            if (m.Groups["init"].Success)
            {
                monster.Init = int.Parse(m.Groups["init"].Value);
            }
            else
            {
                monster.Init = 0;
            }
            monster.Senses = "";
            if (m.Groups["senses"].Success)
            {
                monster.Senses += m.Groups["senses"].Value + "; ";
            }
            int perception = 0;

            if (m.Groups["perception"].Success)
            {
                perception = int.Parse(m.Groups["perception"].Value);
            }

            monster.Senses += "Perception " + CMStringUtilities.PlusFormatNumber(perception);

            Regex regArmor = new Regex("(?<ac>AC -?[0-9]+, touch -?[0-9]+, flat-footed -?[0-9]+) +(?<mods>\\([-a-zA-Z0-9, +]+\\))?", RegexOptions.IgnoreCase);
            m = regArmor.Match(statsblock);
            monster.AC = m.Groups["ac"].Value;
            if (m.Groups["mods"].Success)
            {
                monster.AC_Mods = m.Groups["mods"].Value;
            }
            else
            {
                monster.AC_Mods = "";
            }


            Regex regHP = new Regex("hp (?<hp>[0-9]+) ((?<hd>\\([-+0-9d]+\\))|(\\(\\)))");
            m = regHP.Match(statsblock);
            if (m.Groups["hp"].Success)
            {
                monster.HP = int.Parse(m.Groups["hp"].Value);
            }
            else
            {
                monster.HP = 0;
            }
            if (m.Groups["hd"].Success)
            {
                monster.HD = m.Groups["hd"].Value;
            }
            else
            {
                monster.HD = "0d0";
            }

            Regex regSave = new Regex("Fort (?<fort>[-+0-9]+), Ref (?<ref>[-+0-9]+), Will (?<will>[-+0-9]+)");
            m = regSave.Match(statsblock);
            if (m.Success)
            {
                monster.Fort = int.Parse(m.Groups["fort"].Value);
                monster.Ref = int.Parse(m.Groups["ref"].Value);
                monster.Will = int.Parse(m.Groups["will"].Value);
            }
            else
            {
                monster.Fort = 0;
                monster.Ref = 0;
                monster.Will = 0;
            }

            int defStart = m.Index + m.Length;

            Regex endLine = new Regex("(?<line>.+)");
            m = endLine.Match(statsblock, defStart+1);
            String defLine = m.Value;

            //string da = FixHeroLabDefensiveAbilities(GetLine("Defensive Abilities", statsblock, true));
            monster.DefensiveAbilities = GetItem("Defensive Abilities", defLine, true);
            monster.Resist = GetItem("Resist", defLine, false);
            monster.Immune = GetItem("Immune", defLine, false);
            monster.SR = GetItem("SR", defLine, false);

            /*Regex regResist = new Regex("(?<da>.+); Resist (?<resist>.+)");

            if (da != null)
            {
                m = regResist.Match(da);

                if (m.Success)
                {
                    monster.DefensiveAbilities = m.Groups["da"].Value;
                    monster.Resist = m.Groups["resist"].Value;
                }
                else
                {
                    monster.DefensiveAbilities = da;
                }
            }*/

            monster.Weaknesses = GetLine("Weakness", statsblock, false);


            monster.Speed = GetLine("Spd", statsblock, false);


            monster.SpecialAttacks = GetLine("Special Attacks", statsblock, true);

            Regex regBase = new Regex("Base Atk (?<bab>[-+0-9]+); CMB (?<cmb>[^;]+); CMD (?<cmd>.+)");
            m = regBase.Match(statsblock);

            if (m.Success)
            {
                monster.BaseAtk = int.Parse(m.Groups["bab"].Value);
                monster.CMB = m.Groups["cmb"].Value;
                monster.CMD = m.Groups["cmd"].Value;
            }
            else
            {
                monster.BaseAtk = 0;
                monster.CMB = "+0";
                monster.CMD = "10";
            }

            monster.Feats = FixHeroLabFeats(GetLine("Feats", statsblock, true));

            monster.Skills = GetLine("Skills", statsblock, true);

            monster.Languages = GetLine("Languages", statsblock, false);

            monster.SQ = GetLine("SQ", statsblock, true);
            if (monster.SQ != null)
            {
                monster.SQ = monster.SQ.ToLower();
            }
            monster.Gear = GetLine("Gear", statsblock, true);
            if (monster.Gear == null)
            {
                monster.Gear = GetLine("Combat Gear", statsblock, true);
            }



            Regex regLine = new Regex("(?<text>.+)\r\n");

            Regex regSpecial = new Regex("SPECIAL ABILITIES\r\n(-+)\r\n(?<special>(.|\r|\n)+)((Created With Hero Lab)|(Hero Lab))");
            m = regSpecial.Match(statsblock);
            if (m.Success)
            {
                string special = m.Groups["special"].Value;

                //parse special string

                MatchCollection matches = regLine.Matches(special);

                Regex regWeaponTraining = new Regex("Weapon Training: (?<group>[a-zA-Z]+) \\+(?<value>[0-9]+)");
                Regex regSR = new Regex("Spell Resistance \\((?<SR>[0-9]+)\\)");
                Regex regSpecialAbility = new Regex("(?<name>[-\\.+, a-zA-Z0-9:]+)"
                    + "( \\((?<mod>[-+][0-9]+)\\))?"
                    + "( [0-9]+')?"
                    + "( \\(CMB (?<CMB>[0-9]+)\\))?"
                    + "( \\((?<AtWill>At will)\\))?"
                    + "( \\((?<daily>[0-9]+)/day\\))?"
                    + "( \\(DC (?<DC>[0-9]+)\\))?"
                    + "( \\((?<othertext>[0-9A-Za-z, /]+)\\))?"
                    + " \\((?<type>(Ex|Su|Sp)(, (?<cp>[0-9]+) CP)?)\\) (?<text>.+)");

                foreach (Match ma in matches)
                {
                    string text = ma.Groups["text"].Value;

                    //check for weapon training
                    Match lm = regWeaponTraining.Match(text);

                    if (lm.Success)
                    {
                        string group = lm.Groups["group"].Value;
                        int val = int.Parse(lm.Groups["value"].Value);

                        monster.AddWeaponTraining(group, val);
                    }
                    if (!lm.Success)
                    {
                        lm = regSR.Match(text);

                        if (lm.Success)
                        {
                            monster.SR = lm.Groups["SR"].Value;

                        }
                    }
                    if (!lm.Success)
                    {
                        lm = regSpecialAbility.Match(text);

                        if (lm.Success)
                        {
                            SpecialAbility sa = new SpecialAbility();
                            sa.Name = lm.Groups["name"].Value;
                            sa.Type = lm.Groups["type"].Value;
                            sa.Text = lm.Groups["text"].Value;

                            if (lm.Groups["AtWill"].Success)
                            {
                                sa.Text += " (at will)";
                            }
                            else if (lm.Groups["Daily"].Success)
                            {
                                sa.Text += " (" + lm.Groups["Daily"].Value + "/day)";
                            }

                            if (lm.Groups["DC"].Success)
                            {
                                sa.Text += " (DC " + lm.Groups["DC"].Value + ")";
                            }


                            monster.SpecialAbilitiesList.Add(sa);

                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine(text);
                        }

                    }


                }
            }

            string endAttacks = "[a-zA-Z]+ Spells Known|Special Attacks|Spell-Like Abilities|-------";

            Regex regMelee = new Regex("\r\nMelee (?<melee>(.|\r|\n)+?)\r\n(Ranged|" + endAttacks + ")");

            m = regMelee.Match(statsblock);

            if (m.Success)
            {
                string attacks = m.Groups["melee"].Value;

                monster.Melee = FixHeroLabAttackString(attacks);

            }

            Regex regRanged = new Regex(
                "\r\nRanged (?<ranged>(.|\r|\n)+?)\r\n(" + endAttacks + ")");

            m = regRanged.Match(statsblock);

            if (m.Success)
            {
                string attacks = m.Groups["ranged"].Value;

                monster.Ranged = FixHeroLabAttackString(attacks);
            }

            monster.SpellLikeAbilities = GetLine("Spell-Like Abilities", statsblock, false);

            Regex regSpells = new Regex(
                "\r\n[ a-zA-Z]+ (?<spells>Spells Known (.|\r|\n)+?)\r\n------");

            m = regSpells.Match(statsblock);
            if (m.Success)
            {
                string spells = m.Groups["spells"].Value;

                monster.SpellsKnown = spells;
            }
        }

        private static string TypesString
        {
            get
            {
                string typesText = "";
                bool firstType = true;
                foreach (string name in creatureTypeNames.Values)
                {
                    if (!firstType)
                    {
                        typesText += "|";
                    }
                    else
                    {
                        firstType = false;
                    }

                    typesText += "(" + StringCapitalizer.Capitalize(name)
                      + ")";
                }

                return typesText;
            }
        }

        private static string SizesString
        {
            get
            {
                string sizesText = "";
                bool firstSize = true;
                foreach (MonsterSize size in Enum.GetValues(typeof(MonsterSize)))
                {
                    if (!firstSize)
                    {
                        sizesText += "|";
                    }
                    else
                    {
                        firstSize = false;
                    }

                    sizesText += "(" + SizeMods.GetSizeText(size)
                      + ")";
                }
                return sizesText;
            }
        }

        private static string AlignString
        {
            get
            {
                return "(L|C|N)(G|E|N)?";
            }
        }

        private static string FixHeroLabAttackString(string text)
        {
            string attacks = text;


            attacks = Weapon.ReplaceOriginalWeaponNames(attacks, false);

            attacks = attacks.ToLower();

            attacks = Regex.Replace(attacks, "and\r\n  ", "or");
            attacks = Regex.Replace(attacks, "/20/", "/");
            attacks = Regex.Replace(attacks, "/20\\)", ")");
            attacks = Regex.Replace(attacks, " \\(from armor\\)", "");

            return attacks;
        }

        private static string FixHeroLabFeats(string text)
        {

            string returnText = text;

            if (returnText != null)
            {
                returnText = Regex.Replace(returnText, " ([-+][0-9]+)(/[-+][0-9]+)*", "");

                returnText = Regex.Replace(returnText, " \\([0-9]+d[0-9]+\\)", "");
                returnText = Regex.Replace(returnText, " \\([0-9]+/day\\)", "");
            }

            return returnText;
        }

        private static string FixHeroLabDefensiveAbilities(string text)
        {
            string returnText = text;

            if (returnText != null)
            {
                returnText = Regex.Replace(returnText, " \\(Lv >=[0-9]+\\)", "");
            }

            return returnText;
        }

        private static string GetLine(string start, string text, bool bFix)
        {
            string returnText = null;

            Regex regLine = new Regex(start + " *(?<line>.+)");
            Match m = regLine.Match(text);

            if (m.Success)
            {
                returnText = m.Groups["line"].Value;
                returnText = returnText.Trim();

                if (bFix)
                {
                    returnText = FixHeroLabList(returnText);
                }
            }

            return returnText;

        }

        public static string GetItem(string start, string text, bool bFix)
        {
            string returnText = null;

            Regex regLine = new Regex("(; |^)" + start + " *(?<line>.+?)(;|$)");
            Match m = regLine.Match(text);

            if (m.Success)
            {
                returnText = m.Groups["line"].Value;
                returnText = returnText.Trim();

                if (bFix)
                {
                    returnText = FixHeroLabList(returnText);
                }
            }

            return returnText;
        }

        private static string FixHeroLabList(string text)
        {

            string returnText = text;

            if (returnText != null)
            {
                returnText = ReplaceHeroLabSpecialChar(returnText);
                returnText = Weapon.ReplaceOriginalWeaponNames(returnText, false);
                returnText = ReplaceColonItems(returnText);
                
            }


            return returnText;
        }

        private static string ReplaceHeroLabSpecialChar(string text)
        {
            string returnText = text;

            if (returnText != null)
            {
                returnText = returnText.Replace("&#151;", "-");
            }
            
            return returnText;
        
        }

        private static string ReplaceColonItems(string text)
        {
            Regex reg = new Regex(": (?<val>[-+/. a-zA-Z0-9]+?)(?<mod> (\\+|-)[0-9]+)?((?<comma>,)|\r|\n|$)");

            return reg.Replace(text, delegate(Match m)
            {
                string val = " (" + m.Groups["val"] + ")";

                if (m.Groups["mod"].Success)
                {
                    val += m.Groups["mod"].Value;
                }


                if (m.Groups["comma"].Success)
                {
                    val += ",";
                }

                return val;
            });

        }


        private static string FixPCGenFeatList(string text)
        {
            string val = text;

            foreach (KeyValuePair<string, string> pair in Feat.AltFeatMap)
            {
                val = Regex.Replace(val, pair.Key, pair.Value);
            }

            return val;
        }

        protected

        void ParseSpecialAbilities()
        {
            if (specialAbilitiesList != null && specialAbilitiesList.Count > 0)
            {
                specialAblitiesParsed = true;
            }
            else
            {
                if (specialAbilitiesList == null)
                {
                    specialAbilitiesList = new ObservableCollection<SpecialAbility>();
                    specialAbilitiesList.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(SpecialAbilitiesList_CollectionChanged);
                
                }
                if (specialAbilities != null)
                {
                    string specText = specialAbilities;

                    //trim off extra dragon info at end
                    Regex dragonTableRegex = new Regex("Age Category S pecial Abilities");
                    Match dragonMatch = dragonTableRegex.Match(specText);
                    if (dragonMatch.Success)
                    {
                        specText = specText.Substring(0, dragonMatch.Index);
                    }


                    List<String> list = new List<string>();
					string specRegString = "\\((?<type>(Ex|Su|Sp))(, (?<cp>[0-9]+) CP)?\\):?";
                    Regex specFindRegex = new Regex("((?<startline>^)|\\.)[-a-zA-Z ',]+" + specRegString);
					Regex specRegex = new Regex(specRegString);
					Match specFindMatch = specFindRegex.Match(specText);
					List<int> locList = new List<int>();
					
					while (specFindMatch.Success)
					{
						int index = specFindMatch.Index	;
						if (!specFindMatch.GroupSuccess("startline"))
						{
							index++;
						}
						locList.Add(index);
						
						specFindMatch = specFindMatch.NextMatch();
					}
					
					for (int i=0; i<locList.Count; i++)
					{
						if (i + 1 == locList.Count)
						{
							list.Add(specText.Substring(locList[i], specText.Length - locList[i]).Trim());
						}
						else
						{
							
							list.Add(specText.Substring(locList[i], locList[i+1] - locList[i]).Trim());
						}
					}
					
                    foreach (string strSpec in list)
                    {
                        Match match = specRegex.Match(strSpec);

                        SpecialAbility spec = new SpecialAbility();

                        if (match.Success)
                        {
                            spec.Name = strSpec.Substring(0, match.Index).Trim();
                            spec.Type = match.Groups["type"].Value;
                            if (match.Groups["cp"].Success)
                            {
                                spec.ConstructionPoints = int.Parse(match.Groups["cp"].Value);
                            }
                            spec.Text = strSpec.Substring(match.Index + match.Length).Trim();
                        }
                        else
                        {
                            spec.Name = "";
                            spec.Type = "";
                            spec.Text = strSpec;
                        }


                        specialAbilitiesList.Add(spec);

                    }
                    SpecialAblitiesParsed = true;
                }
            }

        }

        void SpecialAbilitiesList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SpecialAblitiesParsed = true;
        }

        void ParseAC()
        {
            Regex regExAC = new Regex("AC [0-9]+\\,");

            //match AC
            Match match = regExAC.Match(ac);
            if (match.Success)
            {
                string acText = match.Value.Substring(3, match.Length - 4);

                int.TryParse(acText, out fullAC);

            }
            else
            {
                regExAC = new Regex("^[0-9]+\\,");

                match = regExAC.Match(ac);

                if (match.Success)
                {
                    string acText = match.Value.Substring(0, match.Length - 1);

                    int.TryParse(acText, out fullAC);

                }
            }

            //match flat footed
            Regex regTouch = new Regex("ouch [0-9]+\\,");

            match = regTouch.Match(ac);

            if (match.Success)
            {
                string text = match.Value.Substring(5, match.Length-6);
                int.TryParse(text, out touchAC);
            }


            //match flat footed
            Regex regFlatFooted = new Regex("ooted [0-9]+");

            match = regFlatFooted.Match(ac);

            if (match.Success)
            {
                string flatText = match.Value.Substring(6);
                int.TryParse(flatText, out flatFootedAC);
            }

            naturalArmor = FindModifierNumber(ac_mods, "natural");

            shield = FindModifierNumber(ac_mods, "shield");
            armor = FindModifierNumber(ac_mods, "armor");
            dodge = FindModifierNumber(ac_mods, "dodge");
            deflection = FindModifierNumber(ac_mods, "deflection");

            acParsed = true;

        }

        private void ParseSkills()
        {
            skillValueDictionary = new Dictionary<String, SkillValue>(new InsensitiveEqualityCompararer());
            if (skills != null)
            {
                Regex skillReg = new Regex("([ a-zA-Z]+)( )(\\(([- a-zA-Z]+)\\) )?((\\+|-)[0-9]+)");

                foreach (Match match in skillReg.Matches(skills))
                {
                    SkillValue value = new SkillValue();


                    value.Name = match.Groups[1].Value.Trim();
                    

                    if (match.Groups[3].Success)
                    {
                        value.Subtype = match.Groups[4].Value;

                    }

                    value.Mod = int.Parse(match.Groups[5].Value);

                    skillValueDictionary[value.FullName] = value;
                   
                    
                }
            }

            skillsParsed =true;
        }

        private void ParseFeats()
        {
            if (featsList != null && featsList.Count > 0)
            {
                FeatsParsed = true;
            }
            else
            {

                featsList = new List<string>();

                if (Feats != null)
                {

                    Regex regFeat = new Regex("(^| )([- a-zA-Z0-9]+( \\([- ,a-zA-Z0-9]+\\))?)(\\*+)?(\\Z|,)");

                    foreach (Match m in regFeat.Matches(Feats))
                    {
                        string val = m.Groups[2].Value;

                        //remove B end of feat error
                        //hopefully no feat ever ends in a capital B
                        if (val[val.Length - 1] == 'B')
                        {
                            val = val.Substring(0, val.Length - 1);
                        }

                        featsList.Add(val);

                        

                    }
                    featsParsed = true;
                }
                
                featsList.Sort();

            }


        }

        private static int FindModifierNumber(string text, string modifierName)
        {
            int value = 0;
            if (text != null)
            {
                Regex regName = new Regex("(\\+|-)[0-9]+ " + modifierName);

                Match match = regName.Match(text);

                if (match.Success)
                {
                    string valText = match.Value;

                    Regex regSpace = new Regex(" ");
                    Match spaceMatch = regSpace.Match(valText);

                    string numText = valText.Substring(0, spaceMatch.Index);

                    int.TryParse(numText, out value);

                }
            }

            return value;
        }
       

        private static string ReplaceModifierNumber(string text, string modifierName, int value, bool addIfZero)
        {
            string valText = CMStringUtilities.PlusFormatNumber(value) + " " + modifierName;

            if (value == 0 && !addIfZero)
            {
                valText = "";
            }


            Regex regName = new Regex("(, )?(\\+|-)[0-9]+ " + modifierName + "(, )?", RegexOptions.IgnoreCase);
            
            string returnText = text;
            if (text != null || value != 0)
            {
                if (text == null)
                {
                    text = "";
                }

                Match match = regName.Match(text);
                if (match.Success)
                {
                    if (value == 0 && !addIfZero)
                    {
                        if (match.Groups[1].Success && match.Groups[3].Success)
                        {
                            valText = ", ";
                        }
                    }
                    else
                    {
                        valText = match.Groups[1] + valText + match.Groups[3];
                    }

                    returnText = regName.Replace(text, valText);
                }
                else
                {
                    if (text.Length == 0)
                    {
                        returnText = valText;
                    }
                    else if (valText.Length > 0)
                    {
                        returnText = text.Insert(text.Length - 1, ", " + valText);
                    }
                }
            }
            
            return returnText;
        }


        private static string ChangeCMD(string text, int diff)
        {
            string returnText = ChangeStartingNumber(text, diff);


            if (returnText != null)
            {
                Regex regVs = new Regex("([-\\+0-9/]+)( vs\\.)");

                returnText = regVs.Replace(returnText, delegate(Match m)
                                {
                                    int val = int.Parse(m.Groups[1].Value) + diff;

                                    return val.ToString() + m.Groups[2];
                                }
                            );
            }


            return returnText;
        }

        private string ChangeThrownDamage(string text, int diff, int diffPlusHalf)
        {
            if (text == null)
            {
                return null;
            }

            string returnText = text;

            foreach (KeyValuePair<string, bool> thrownAttack in thrownAttacks)
            {


                Regex regAttack = new Regex(Attack.RegexString(thrownAttack.Key));

                returnText = regAttack.Replace(returnText, delegate(Match m)
                {
                    Attack info = Attack.ParseAttack(m);

                    if (!info.AltDamage)
                    {
                        if (thrownAttack.Value)
                        {
                            info.Damage.mod += diffPlusHalf;
                        }
                        else
                        {
                            info.Damage.mod += diff;
                        }
                    }

                    return info.Text;

                });



            }


            return returnText;
        }

        public int GetSkillMod(string skill, string subtype)
        {


            SkillValue val = new SkillValue(skill);
            val.Subtype = subtype;

            if (SkillValueDictionary.ContainsKey(val.FullName))
            {
                return SkillValueDictionary[val.FullName].Mod;
            }

            else
            {
                Stat stat;
                if (SkillsList.TryGetValue(val.Name, out stat))
                {
                    return AbilityBonus(GetStat(stat));
                }
                else
                {
                    return 0;
                }

            }
        }

        public bool AddOrChangeSkill(string skill,  int diff)
        {
            return AddOrChangeSkill(skill, null, diff);
        }


        public bool AddOrChangeSkill(string skill, string subtype, int diff)
        {
            bool added = false;


            SkillValue val = new SkillValue(skill);
            val.Subtype = subtype;

            if (SkillValueDictionary.ContainsKey(val.FullName))
            {
                ChangeSkill(val.FullName, diff);
            }
            else
            {

                val.Mod = diff;

                Stat stat;
                if (SkillsList.TryGetValue(val.Name, out stat))
                {
                    val.Mod += AbilityBonus(GetStat(stat));
                }

                SkillValueDictionary[val.FullName] = val;

                UpdateSkillFields(val);                

                added = true;

            }



            return added;
        }



        public bool ChangeSkill(string skill, int diff)
        {

            SkillValue value;

            if (SkillValueDictionary.TryGetValue(skill, out value))
            {
                value.Mod += diff;

                UpdateSkillFields(value);
                return true;
            }
            else
            {
                return false;
            }
            
        }

        private static string SetSkillStringMod(string text, string skill, int val)
        {

            string returnText = text;

            Regex regName = new Regex(skill + " (\\([-a-zA-Z ]+\\) )?(\\+|-)[0-9]+");

            Match match = regName.Match(text);
            if (match.Success)
            {
                Regex regMod = new Regex("(\\+|-)[0-9]+");

                Match modMatch = regMod.Match(match.Value);

                int newVal = val;

                String newText = CMStringUtilities.PlusFormatNumber(newVal);

                returnText = returnText.Remove(match.Index + modMatch.Index, modMatch.Length);
                returnText = returnText.Insert(match.Index + modMatch.Index, newText);
            }

            return returnText;
        }

        private static string ChangeSkillStringMod(string text, string skill, int diff)
        {
            return ChangeSkillStringMod(text, skill, diff, false);
        }

        private static string ChangeSkillStringMod(string text, string skill, int diff, bool add)
        {

            string returnText = text;

            Regex regName = new Regex(skill + " (\\([-a-zA-Z ]+\\) )?(\\+|-)[0-9]+");

            bool added = false;

            if (returnText != null)
            {
                Match match = regName.Match(returnText);
                if (match.Success)
                {
                    Regex regMod = new Regex("(\\+|-)[0-9]+");

                    Match modMatch = regMod.Match(match.Value);

                    int oldVal = int.Parse(modMatch.Value);
                    int newVal = oldVal + diff;

                    String newText = CMStringUtilities.PlusFormatNumber(newVal);

                    returnText = returnText.Remove(match.Index + modMatch.Index, modMatch.Length);
                    returnText = returnText.Insert(match.Index + modMatch.Index, newText);
                    added = true;
                }
            }
           
            if (add && ! added)
            {
                returnText = AddToStringList(returnText, skill + " " + CMStringUtilities.PlusFormatNumber(diff));
            }

            return returnText;
        }

        private void AddRacialSkillBonus(string type, int diff)
        {
            RacialMods = AddPlusModValue(RacialMods, type, diff);

            AddOrChangeSkill(type, diff);
        }

        private static string AddPlusModValue(string text, string type, int diff)
        {
            string returnText = text;
            if (returnText == null)
            {
                returnText = "";
            }
            
            Regex regVal = new Regex("((\\+|-)[0-9]+) " + type);

            bool replaced = false;

            returnText = regVal.Replace(returnText, delegate(Match m)
                {
                    int val = int.Parse(m.Groups[1].Value);

                    val += diff;
                    replaced = true;

                    return CMStringUtilities.PlusFormatNumber(val) + " " + type;
                });

            if (!replaced)
            {
                returnText = AddToStringList(returnText, 
                    CMStringUtilities.PlusFormatNumber(diff) + " " + type);
            }

            return returnText;

        }

        protected void UpdateSkillFields(SkillValue skill)
        {
            if (skill.Name.CompareTo("Perception") == 0)
            {
                //Adjust perception
                senses = SetSkillStringMod(Senses, skill.FullName, skill.Mod);
            }
        }

        private static string ChangeStartingNumber(string text, int diff)
        {
            string returnText = text;
            if (text != null)
            {

                Regex regName = new Regex("^-?[0-9]+");

                Match match = regName.Match(returnText);

                if (match.Success)
                {
                    int val = int.Parse(match.Value);

                    val += diff;

                    returnText = regName.Replace(returnText, val.ToString(), 1);
                }
            }

            return returnText;
        }

        private static int GetStartingNumber(string text)
        {
            int val = 0;

            if (text != null)
            {
                Regex regName = new Regex("^-?[0-9]+");

                Match match = regName.Match(text);

                if (match.Success)
                {
                    val = int.Parse(match.Value);
                }
            }

            return val;
        }

        private static string AddSR(string text, int value)
        {
            string returnText = text;

            if (returnText == null || returnText.Length == 0)
            {
                returnText = value.ToString();
            }
            else
            {
                returnText = ChangeStartingNumberMaxValue(text, value);
            }

            return returnText;
        }

        private static string ChangeStartingNumberMaxValue(string text, int value)
        {
            string returnText = text;

            Regex regName = new Regex("^-?[0-9]+");

            returnText = regName.Replace(returnText, delegate(Match m)
                {

                    int val = int.Parse(m.Value);

                    return Math.Max(val, value).ToString();
                }
                , 1);

            return returnText;
        }

        private static string ChangeStartingMod(string text, int diff)
        {
            string returnText = text;

            if (text != null)
            {

                Regex regName = new Regex("^(\\+|-)[0-9]+");

                Match match = regName.Match(returnText);

                if (match.Success)
                {
                    int val = int.Parse(match.Value);

                    val += diff;

                    returnText = regName.Replace(returnText, CMStringUtilities.PlusFormatNumber(val), 1);
                }
            }

            return returnText;
        }

        private static string ChangeStartingModOrVal(string text, int diff)
        {
            string returnText = text;

            if (text != null)
            {

                Regex regName = new Regex("^(\\+|-)?[0-9]+");

                Match match = regName.Match(returnText);

                if (match.Success)
                {
                    int val = int.Parse(match.Value);

                    val += diff;

                    returnText = regName.Replace(returnText, CMStringUtilities.PlusFormatNumber(val), 1);
                }
            }

            return returnText;
        }

        private static int GetStartingModOrVal(string text)
        {
            int val = 0;
            if (text != null)
            {
                Regex regName = new Regex("^(\\+|-)?[0-9]+");

                Match match = regName.Match(text);

                if (match.Success)
                {
                    val = int.Parse(match.Value);
                }
            }


            return val;
        }

        private static int GetStartingMod(string text)
        {
            int val = 0;
            if (text != null)
            {
                Regex regName = new Regex("^(\\+|-)[0-9]+");

                Match match = regName.Match(text);

                if (match.Success)
                {
                    val = int.Parse(match.Value);
                }
            }


            return val;
        }






        public bool MakeAdvanced()
        {
            AdjustNaturalArmor(2);
            AdjustStrength(4);
            AdjustDexterity(4);
            AdjustConstitution(4);
            AdjustIntelligence(4);
            AdjustWisdom(4);
            AdjustCharisma(4);
            AdjustCR(1);

            return true;
        }

        public bool MakeGiant()
        {
            if (SizeMods.GetSize(size) != MonsterSize.Colossal)
            {

                AdjustSize(1);
                AdjustNaturalArmor(3);
                AdjustStrength(4);
                AdjustConstitution(4);
                AdjustDexterity(-2);

                AdjustCR(1);

                return true;
            }

            return false;
        }

        public bool MakeCelestial()
        {
            return MakeSummoned(HalfOutsiderType.Celestial) ;
        }

        public bool MakeFiendish()
        {

            return MakeSummoned(HalfOutsiderType.Fiendish);
        }

        public bool MakeEntropic()
        {
            return MakeSummoned(HalfOutsiderType.Entropic);
        }

        public bool MakeResolute()
        {

            return MakeSummoned(HalfOutsiderType.Resolute);
        }

        public static string GetOutsiderOpposedType(HalfOutsiderType type)
        {
            string opType = "";

            switch (type)
            {
                case HalfOutsiderType.Celestial:
                    opType = "evil";
                    break;
                case HalfOutsiderType.Fiendish:
                    opType = "good";
                    break;
                case HalfOutsiderType.Entropic:
                    opType = "law";
                    break;
                case HalfOutsiderType.Resolute:
                    opType = "chaos";
                    break;
            }

            System.Diagnostics.Debug.Assert(opType != "");

            return opType;

            
        }
        public static string GetOutsiderDRType(HalfOutsiderType type)
        {
            string opType = "";

            switch (type)
            {
                case HalfOutsiderType.Celestial:
                    opType = "evil";
                    break;
                case HalfOutsiderType.Fiendish:
                    opType = "good";
                    break;
                case HalfOutsiderType.Entropic:
                    opType = "lawful";
                    break;
                case HalfOutsiderType.Resolute:
                    opType = "chaotic";
                    break;
            }

            System.Diagnostics.Debug.Assert(opType != "");

            return opType;


        }

        public static string GetOutsiderTypeName(HalfOutsiderType type)
        {
            string opType = "";

            switch (type)
            {
                case HalfOutsiderType.Celestial:
                    opType = "celestial";
                    break;
                case HalfOutsiderType.Fiendish:
                    opType = "fiendish";
                    break;
                case HalfOutsiderType.Entropic:
                    opType = "entropic";
                    break;
                case HalfOutsiderType.Resolute:
                    opType = "resolute";
                    break;
            }

            System.Diagnostics.Debug.Assert(opType != "");

            return opType;
        }




        private bool MakeSummoned(HalfOutsiderType outsiderType)
        {
            //add darkvision
            Senses = ChangeDarkvisionAtLeast(Senses, 60);

            //add smite evil as swift action

            AddSmite(outsiderType, false);
            

            int resistAmount = 5;
            if (HDRoll.count > 10)
            {
                resistAmount = 15;
            }
            else if (HDRoll.count > 4)
            {
                resistAmount = 10;
            }

            //add resist acid, cold, electricity
            Resist = AddResitance(Resist, "acid", resistAmount);
            Resist = AddResitance(Resist, "cold", resistAmount);
            Resist = AddResitance(Resist, "electricity", resistAmount);

            //add DR/evil as needed
            DR = AddSummonDR(DR, HD, GetOutsiderDRType(outsiderType));

            //add CR as needed
            DieRoll roll = FindNextDieRoll(HD, 0);
            if (roll.count >= 5)
            {
                AdjustCR(1);
            }

            //add SR = CR+5
            SR = AddSummonSR(SR, CR, 5);

            return true;
        }

        public bool MakeYoung()
        {
            if (SizeMods.GetSize(size) != MonsterSize.Fine)
            {
                AdjustNaturalArmor(-2);

                AdjustStrength(-4);
                AdjustConstitution(-4);
                AdjustDexterity(4);

                AdjustSize(-1);

                AdjustCR(-1);
                return true;
            }
            return false;
        }

        public bool AugmentSummoning()
        {

            AdjustStrength(4);
            AdjustConstitution(4);

            return true;
        }



        public int AddRacialHD(int dice, Stat stat, bool size)
        {
            CreatureTypeInfo typeInfo = CreatureTypeInfo.GetInfo(Type);

            int oldHP = HP;

            DieRoll oldHD = FindNextDieRoll(HD, 0);

            //add hd
            AdjustHD(dice);
            
            DieRoll newHD = FindNextDieRoll(HD, 0);

            int appliedDice = newHD.count - oldHD.count;

            //adjust skills
            int skillCount = Math.Max(typeInfo.Skills + AbilityBonus(Intelligence), 1);           
            AdjustSkills(skillCount * newHD.TotalCount - skillCount * oldHD.TotalCount);


            //add stats if needed
            int statCount = appliedDice / 4;
            if (statCount != 0)
            {
                switch (stat)
                {
                    case Stat.Strength:
                        AdjustStrength(statCount);
                        break;
                    case Stat.Dexterity:
                        AdjustDexterity(statCount);
                        break;
                    case Stat.Constitution:
                        AdjustConstitution(statCount);
                        break;
                    case Stat.Intelligence:
                        AdjustIntelligence(statCount);
                        break;
                    case Stat.Wisdom:
                        AdjustWisdom(statCount);
                        break;
                    case Stat.Charisma:
                        AdjustCharisma(statCount);
                        break;
                }
            }

            //change BAB
            int babChange = typeInfo.GetBAB(newHD.count) - typeInfo.GetBAB(oldHD.count);
            AdjustBaseAttack(babChange, false);

            bool fortGood = false;
            bool refGood = false;
            bool willGood = false;

            //adjust saves
            if (typeInfo.SaveVariesCount == 0)
            {
                fortGood = typeInfo.FortGood;
                refGood = typeInfo.RefGood;
                willGood = typeInfo.WillGood;

            }
            else
            {
                //calc saves
                int baseFort = Fort - AbilityBonus(Type=="undead"?Charisma: Constitution);
                int baseRef = Ref - AbilityBonus(Dexterity);
                int baseWill = Will - AbilityBonus(Wisdom);

                List<KeyValuePair<int, int>> list = new List<KeyValuePair<int, int>>();

                list.Add(new KeyValuePair<int, int>(baseFort, 0));
                list.Add(new KeyValuePair<int, int>(baseRef, 1));
                list.Add(new KeyValuePair<int, int>(baseWill, 2));


                list.Sort((a, b) => b.Key - a.Key);

                for (int i=0; i < typeInfo.SaveVariesCount; i++)
                {
                    switch (list[i].Value)
                    {
                        case 0:
                            fortGood = true;
                            break;
                        case 1:
                            refGood = true;
                            break;
                        case 2:
                            willGood = true;
                            break;
                    }
                }


            }



            Fort += CreatureTypeInfo.GetSave(fortGood, newHD.count)
                     - CreatureTypeInfo.GetSave(fortGood, oldHD.count);
            Will += CreatureTypeInfo.GetSave(willGood, newHD.count)
                     - CreatureTypeInfo.GetSave(willGood, oldHD.count);
            Ref += CreatureTypeInfo.GetSave(refGood, newHD.count)
                     - CreatureTypeInfo.GetSave(refGood, oldHD.count);




            //add size if needed
            if (size)
            {
                int sizeChanges = 0;

                if (newHD.count > oldHD.count)
                {
                    sizeChanges = (int)Math.Log(((double)newHD.count) / (double)oldHD.count, 2.0);
                }
                if (oldHD.count > newHD.count)
                {
                    sizeChanges = -(int)Math.Log(((double)oldHD.count) / (double)newHD.count, 2.0);
                }

                MonsterSize oldSize = SizeMods.GetSize(Size);

                AdjustSize(sizeChanges);

                MonsterSize newSize = SizeMods.GetSize(Size);

                //add chart size change bonuses
                if (sizeChanges > 0)
                {
                    while (((int)oldSize) < (int)newSize)
                    {

                        oldSize = (MonsterSize)1 + (int)oldSize;

                        SizeMods mods = SizeMods.GetMods(oldSize);
                        AdjustChartMods(mods, true);

                    }
                }
                else if (sizeChanges < 0)
                {

                    while (((int)oldSize) > (int)newSize)
                    {
                        SizeMods mods = SizeMods.GetMods(oldSize);
                        AdjustChartMods(mods, false);

                        oldSize = (MonsterSize)((int)oldSize) - 1;
                    }
                }
            }

            //calculate the new CR
            int crLevel = 0;
            if (!CR.Contains('/'))
            {
                crLevel = int.Parse(CR);
            }

            if (oldHP < HP)
            {
                oldHP += GetCRHPChange(crLevel + 1);

                while (oldHP < HP)
                {
                    crLevel++;
                    AdjustCR(1);

                    oldHP += GetCRHPChange(crLevel + 1);
                }
            }
            else if (oldHP > HP)
            {
                oldHP -= GetCRHPChange(crLevel);
                while (oldHP > HP && crLevel > 1)
                {
                    crLevel--;
                    AdjustCR(-1);

                    oldHP -= GetCRHPChange(crLevel);
                }
                
            }

            return appliedDice;

        }
        

        public bool MakeHalfDragon(string color)
        {
            //living creatures only
            if (Constitution == null)
            {
                return false;
            }


            //get element type
            DragonColorInfo colorInfo = dragonColorList[color.ToLower()];
            string element = colorInfo.element;

            //make type dragon
            Type = "dragon";

            //add natual armor
            AdjustNaturalArmor(4);

            //add darkvision
            Senses = ChangeDarkvisionAtLeast(Senses, 60);

            //add low light vision
            Senses = AddSense(Senses, "low-light vision");
           
            //add imunnity sleep, paralysis, breath weapon type
            Immune = AddImmunity(Immune, element);
            Immune = AddImmunity(Immune, "sleep");
            Immune = AddImmunity(Immune, "paralysis");

            //add fly average 2x speed
            Speed = AddFlyFromMove(Speed, 2, "average");

            //set bite & 2 claw attacks
            AddNaturalAttack("bite", 2, 1);


            AddNaturalAttack("claw", 1, 0);

            //add breath weapon
            SpecialAttacks = AddSpecialAttack(SpecialAttacks,
                "breath weapon (" + colorInfo.distance + "-foot " + colorInfo.weaponType + " of " +
                    colorInfo.element + ", " + HDRoll.count +
                    "d6 " + colorInfo.element + " damage, Reflex DC " + 
                    (10 + HDRoll.count/2 + AbilityBonus(Constitution)) + " half)", 1);
            
            
            //add stats
            AdjustStrength(8);
            AdjustConstitution(6);
            AdjustIntelligence(2);
            AdjustCharisma(2);

            //increase CR
            AdjustCR(2);
            if (CR == "2")
            {
                AdjustCR(1);
            }
                
            return true;
        }

        public bool MakeHalfCelestial(HashSet<Stat> bonusStats)
        {

            return MakeHalfOutsider(HalfOutsiderType.Celestial, bonusStats);
        }


        public bool MakeHalfFiend(HashSet<Stat> bonusStats)
        {
            return MakeHalfOutsider(HalfOutsiderType.Fiendish, bonusStats);

        }

        public enum HalfOutsiderType
        {
            Celestial,
            Fiendish,
            Entropic,
            Resolute
        }


        public bool MakeHalfOutsider(HalfOutsiderType outsiderType, HashSet<Stat> bonusStats)
        {
            //living creatures only
            if (Constitution == null || bonusStats.Count != 3 || 
                Intelligence == null || Intelligence < 4)
            {
                return false;
            }

           
            //increase CR
            if (HDRoll.count < 5)
            {
                AdjustCR(1);
            }
            else if (HDRoll.count < 11)
            {
                AdjustCR(2);
            }
            else
            {
                AdjustCR(3);
            }

            //make alignment good/evil
            AlignmentType align = ParseAlignment(Alignment);
            align.Moral = (outsiderType == HalfOutsiderType.Celestial)?MoralAxis.Good:MoralAxis.Evil;
            Alignment = AlignmentText(align);

            //change type
            Type = "outsider";
            SubType = "(native)";

            //adjust armor
            AdjustNaturalArmor(1);

            //add darkvision
            Senses = ChangeDarkvisionAtLeast(Senses, 60);
            
            //add immunity
            Immune = AddImmunity(Immune, (outsiderType == HalfOutsiderType.Celestial)?"disease":"poison");

            //add resistance
            Resist = AddResitance(Resist, "acid", 10);
            Resist = AddResitance(Resist, "cold", 10);
            Resist = AddResitance(Resist, "electricity", 10);

            if (outsiderType == HalfOutsiderType.Celestial)
            {
                Save_Mods = AddPlusModValue(Save_Mods, "vs. poison", 4);
            }

            //add DR
            DR = AddDR(DR, "magic", (HDRoll.count <= 11) ? 5 : 10);

            //add SR
            SR = AddSR(SR, int.Parse(CR) + 11);

            //add fly average 2x speed
            Speed = AddFlyFromMove(Speed, 2, "good");

            if (outsiderType == HalfOutsiderType.Fiendish)
            {
                //set bite & 2 claw attacks
                AddNaturalAttack("bite", 1, 1);

                AddNaturalAttack("claw", 2, 0);
            }



            AddSmite(outsiderType, true);


            //add spell like abilities
            if (Intelligence >= 8 || (Wisdom != null && Wisdom >= 8))
            {
                int hdCount = HDRoll.count;

                if (hdCount >= 1)
                {
                    if (outsiderType == HalfOutsiderType.Celestial)
                    {
                        SpellLikeAbilities = AddSpellLikeAbility(SpellLikeAbilities, "protection from evil", 3, hdCount);
                        SpellLikeAbilities = AddSpellLikeAbility(SpellLikeAbilities, "bless", 1, hdCount);
                    }
                    else
                    {
                        SpellLikeAbilities = AddSpellLikeAbility(SpellLikeAbilities, "darkness", 3, hdCount);
                    }

                }
                if (hdCount >= 3)
                {
                    if (outsiderType == HalfOutsiderType.Celestial)
                    {
                        
                        SpellLikeAbilities = AddSpellLikeAbility(SpellLikeAbilities, "aid", 1, hdCount);
                        SpellLikeAbilities = AddSpellLikeAbility(SpellLikeAbilities, "detect evil", 1, hdCount);
                    }
                    else
                    {
                        SpellLikeAbilities = AddSpellLikeAbility(SpellLikeAbilities, "desecrate", 1, hdCount);
                    }
                }
                if (hdCount >= 5)
                {
                    if (outsiderType == HalfOutsiderType.Celestial)
                    {
                        SpellLikeAbilities = AddSpellLikeAbility(SpellLikeAbilities, "cure serious wounds", 1, hdCount);
                        SpellLikeAbilities = AddSpellLikeAbility(SpellLikeAbilities, "neutralize poison", 1, hdCount);

                    }
                    else
                    {
                        SpellLikeAbilities = AddSpellLikeAbility(SpellLikeAbilities, "unholy blight", 1, hdCount);
                    }
                }
                if (hdCount >= 7)
                {
                    if (outsiderType == HalfOutsiderType.Celestial)
                    {
                        SpellLikeAbilities = AddSpellLikeAbility(SpellLikeAbilities, "holy smite", 1, hdCount);
                        SpellLikeAbilities = AddSpellLikeAbility(SpellLikeAbilities, "remove disease", 1, hdCount);

                    }
                    else
                    {
                        SpellLikeAbilities = AddSpellLikeAbility(SpellLikeAbilities, "poison", 3, hdCount);
                    }
                }
                if (hdCount >= 9)
                {
                    if (outsiderType == HalfOutsiderType.Celestial)
                    {
                        SpellLikeAbilities = AddSpellLikeAbility(SpellLikeAbilities, "dispel evil", 1, hdCount);

                    }
                    else
                    {
                        SpellLikeAbilities = AddSpellLikeAbility(SpellLikeAbilities, "contagion", 1, hdCount);
                    }
                }
                if (hdCount >= 11)
                {
                    if (outsiderType == HalfOutsiderType.Celestial)
                    {
                        SpellLikeAbilities = AddSpellLikeAbility(SpellLikeAbilities, "holy word", 1, hdCount);

                    }
                    else
                    {
                        SpellLikeAbilities = AddSpellLikeAbility(SpellLikeAbilities, "blasphemy", 1, hdCount);
                    }
                }
                if (hdCount >= 13)
                {
                    if (outsiderType == HalfOutsiderType.Celestial)
                    {
                        SpellLikeAbilities = AddSpellLikeAbility(SpellLikeAbilities, "holy aura", 3, hdCount);
                        SpellLikeAbilities = AddSpellLikeAbility(SpellLikeAbilities, "hallow", 1, hdCount);

                    }
                    else
                    {
                        SpellLikeAbilities = AddSpellLikeAbility(SpellLikeAbilities, "unholy aura", 3, hdCount);
                        SpellLikeAbilities = AddSpellLikeAbility(SpellLikeAbilities, "unhallow", 1, hdCount);
                    }
                }
                if (hdCount >= 15)
                {
                    if (outsiderType == HalfOutsiderType.Celestial)
                    {
                        SpellLikeAbilities = AddSpellLikeAbility(SpellLikeAbilities, "mass charm monster", 1, hdCount);

                    }
                    else
                    {
                        SpellLikeAbilities = AddSpellLikeAbility(SpellLikeAbilities, "horrid writing", 1, hdCount);
                    }
                }
                if (hdCount >= 17)
                {
                    if (outsiderType == HalfOutsiderType.Celestial)
                    {
                        SpellLikeAbilities = AddSpellLikeAbility(SpellLikeAbilities, "summon monster IX(celestials only)", 1, hdCount);
                    }
                    else
                    {
                        SpellLikeAbilities = AddSpellLikeAbility(SpellLikeAbilities, "summon monster IX(fiends only)", 1, hdCount);
                    }
                }
                if (hdCount >= 19)
                {
                    if (outsiderType == HalfOutsiderType.Celestial)
                    {
                        SpellLikeAbilities = AddSpellLikeAbility(SpellLikeAbilities, "resurrection", 1, hdCount);
                    }
                    else
                    {
                        SpellLikeAbilities = AddSpellLikeAbility(SpellLikeAbilities, "destruction", 1, hdCount);
                    }
                }
                
            }

            //add stats
            foreach (Stat stat in Enum.GetValues(typeof(Stat)))
            {
                if (bonusStats.Contains(stat))
                {
                    AdjustStat(stat, 4);
                }
                else
                {
                    AdjustStat(stat, 2);
                }
            }


            return true;
        }

        public void AddSmite(HalfOutsiderType outsiderType, bool half)
        {

            string smiteAlignment = GetOutsiderOpposedType(outsiderType);
            string halfType = (half ? "half-" : "") + GetOutsiderTypeName(outsiderType);
            
            //add smite good/evil
            SpecialAttacks = AddSpecialAttack(SpecialAttacks, "smite " + smiteAlignment, 1);

            SpecialAbility ab = new SpecialAbility();
            ab.Name = "Smite " + smiteAlignment;
            ab.Type = "Su";
            ab.Text = "Once per day, as a swift action, the " + halfType + " can smite " + smiteAlignment.ToLower() + " as the smite evil ability of a paladin of the same level as the " + halfType + "'s Hit Dice" + ((outsiderType == HalfOutsiderType.Celestial) ? "" : (", except affecting a " + smiteAlignment.ToLower() + " target")) + ". The smite persists until target is dead or the " + halfType + " rests.";
            SpecialAbilitiesList.Add(ab);
        }


        public bool MakeSkeleton(bool bloody, bool burning, bool champion)
        {
            if (String.Compare(Type, "undead", true) == 0 || 
                String.Compare(Type, "construct", true) == 0 ||
                Strength == null || Dexterity == null || 
                    (SubType != null && String.Compare(SubType, "swarm", true) == 0))
            {
                return false;
            }
            if (champion && (Intelligence == null || Intelligence.Value < 3))
            {
                return false;
            }

            //add dex
            AdjustDexterity(2);

            //add strength
            if (champion)
            {
                AdjustStrength(2);
            }

            //remove con
            Constitution = null;

            if (!champion)
            {
                //remove int
                int? old = Intelligence;
                intelligence = null;
                ApplyIntelligenceAdjustments(old);

                //make wisdom 10
                AdjustWisdom(10 - Wisdom.Value);

                //make charisma 10
                AdjustCharisma(10 - Charisma.Value);
            }



            //change type
            Type = "undead";

            //add natural armor
            switch ( SizeMods.GetSize(Size))
            {
                case MonsterSize.Small:
                    AdjustNaturalArmor(1);
                    break;
                case MonsterSize.Medium:
                case MonsterSize.Large:
                    AdjustNaturalArmor(2);
                    break;
                case MonsterSize.Huge:
                    AdjustNaturalArmor(3);
                    break;
                case MonsterSize.Gargantuan:
                    AdjustNaturalArmor(6);
                    break;
                case MonsterSize.Colossal:
                    AdjustNaturalArmor(10);
                    break;
            }

            //change cr
            int count = HDRoll.count;
            if (count == 1)
            {
                Adjuster.CR = "1/3";
            }
            else if (count <= 11)
            {
                Adjuster.CR = (count / 2).ToString();
            }
            else
            {
                Adjuster.CR = ((count / 3) + 5).ToString();
            }

            //change hd
            DieRoll roll = HDRoll;
            roll.die = 8;
            if (champion)
            {
                //adjust for new charisma
                roll.count += 2;
                roll.mod = roll.TotalCount * AbilityBonus(Charisma);

            }
            else
            {
                roll.mod = 0;
                if (roll.extraRolls != null)
                {
                    roll.extraRolls.Clear();
                }
            }
            HD = "(" + DieRollText(roll) + ")";
            HP = roll.AverageRoll();

            //change saves
            Fort = roll.count / 3 + AbilityBonus(Charisma);
            Ref = roll.count / 3 + AbilityBonus(Dexterity);
            Will = roll.count / 2 + AbilityBonus(Wisdom);



            //remove special attacks & special abilities
            if (!champion)
            {
                RemoveAllForUndead();
            }
            else
            {

                //Add Channel Resistance +4
                DefensiveAbilities = ChangeSkillStringMod(DefensiveAbilities, "channel resistance", 4, true);
            }


            //add darkvision
            Senses = ChangeDarkvisionAtLeast(Senses, 60);

            
            //add DR
            DR = AddDR(null, "bludgeoning", 5);

            //add immunities
            if (bloody)
            {
                Immune = AddImmunity(Immune, "cold");
            }
            
            if (burning)
            {
                //switch cold to fire immunity                
                Immune = AddImmunity(Immune, "fire");
            }

            Immune = AddImmunity(Immune, "undead traits");

            //remove fly
            Speed = RemoveFly(Speed);

            //adjust bab
            AdjustBaseAttack(roll.count * 3 / 4 - BaseAtk, true);
            
            //add claws
            AddNaturalAttack("claw", 2, 0);

            //remove feats
            //add improved initiative
            AddFeat("Improved Initiative");
            Init = AbilityBonus(Dexterity) + 4;




            if (champion)
            {
                AdjustCR(1);
            }
            
            
            DieRoll hdRoll = HDRoll;
            if (bloody)
            {
                AdjustCharisma(4);
            }
            else if (burning)
            {
                AdjustCharisma(2);
            }


            //adjust for bloody Skeleton
            if (bloody)
            {
                //adjust cr
                AdjustCR(1);

                //add fast healing
                if (hdRoll.count > 1)
                {
                    AddFastHealing(hdRoll.count / 2);
                }


                //Add Channel Resistance +4
                DefensiveAbilities = ChangeSkillStringMod(DefensiveAbilities, "channel resistance", 4, true);


                //add deathless
                SQ = AddToStringList(SQ, "deathless");
                SpecialAbility ab = new SpecialAbility();
                ab.Name = "Deathless";
                ab.Text = "A bloody skeleton is destroyed when reduced to 0 hit points, but it returns to unlife 1 hour later at 1 hit point, allowing its fast healing thereafter to resume healing it. A bloody skeleton can be permanently destroyed if it is destroyed by positive energy, if it is reduced to 0 hit points in the area of a bless or hallow spell, or if its remains are sprinkled with a vial of holy water.";
                ab.Type = "Su"; 
                SpecialAbilitiesList.Add(ab);


            }
            if (burning)
            {
                //adjust cr
                AdjustCR(1);


                //add fire to melee attacks
                Melee = SetPlusOnMeleeAttacks(Melee, "1d6 fire", false);

                
                //add aura
                Aura = AddToStringList(Aura, "fiery aura");
                SpecialAbility ab = new SpecialAbility();
                ab.Name = "Fiery Aura";
                ab.Text = "Creatures adjacent to a burning skeleton take 1d6 points of fire damage at the start of their turn. Anyone striking a burning skeleton with an unarmed strike or natural attack takes 1d6 points of fire damage.";
                ab.Type = "Ex"; 
                SpecialAbilitiesList.Add(ab);


                //add fiery death
                int deathDC = 10 + hdRoll.count /2 + AbilityBonus(Charisma);
                SQ = AddToStringList(Aura, "fiery death (DC " + deathDC + ")");
                ab = new SpecialAbility();
                ab.Name = "Fiery Death";
                ab.Text = "A burning skeleton explodes into a burst of flame when it dies. Anyone adjacent to the skeleton when it is destroyed takes 1d6 points of fire damage. A Reflex save (DC " + deathDC + ") halves this damage.";
                ab.Type = "Su"; 
                SpecialAbilitiesList.Add(ab);

            }


            return true;
        }
        public bool MakeLich()
        {
            if (String.Compare(Type, "undead", true) == 0 ||
                Strength == null || Dexterity == null)
            {
                return false;
            }


            //increase CR
            AdjustCR(2);

            //make alignment evil
            AlignmentType align = ParseAlignment(Alignment);
            align.Moral = MoralAxis.Evil;
            Alignment = AlignmentText(align);


            //make type undead
            Type = "undead";

            //Add darkvision
            Senses = ChangeDarkvisionAtLeast(Senses, 60);


            //set natural armor bonus to 5 or creature's natural armor, whichever better
            if (NaturalArmor < 5)
            {
                AdjustNaturalArmor(5 - NaturalArmor);
            }

            //int+2, wis+2, cha+2, con -
            AdjustIntelligence(2);
            AdjustWisdom(2);
            AdjustCharisma(2);
            Constitution = null;

            //add channel resistance 4

            // add DR 15/bludgeoning and magic

            //add immunity to cold and electricity

            //add rejuvenation (su)

            //add melee touch attack

            //add Fear Aura (su)

            //add Paralyzing Touch (su)

            //add +8 racial bonus on Perception, Sense Motive and Stealth.


            //Make HD D8
            DieRoll roll = HDRoll;
            roll.die = 8;
            //add charisma and toughness
            roll.mod = AbilityBonus(Charisma) * roll.count +
                (roll.count < 3 ? 3 : roll.count);
            HD = "(" + DieRollText(roll) + ")";
            HP = roll.AverageRoll();

            return true;
        }
        public bool MakeVampire()
        {
            if (String.Compare(Type, "undead", true) == 0 ||
                Strength == null || Dexterity == null)
            {
                return false;
            }

            //increase CR
            AdjustCR(2);

            //Make Evil
            AlignmentType align = ParseAlignment(Alignment);
            align.Moral = MoralAxis.Evil;
            Alignment = AlignmentText(align);

            //Make undead
            Type = "undead";

            //Add darkvision
            Senses = ChangeDarkvisionAtLeast(Senses, 60);

            //Add 6 to Natural armor
            AdjustNaturalArmor(6);

            //change ability scores
            AdjustStrength(6);
            AdjustDexterity(4);
            AdjustIntelligence(2);
            AdjustWisdom(2);
            AdjustCharisma(2);
            Constitution = null;

            //Add Channel Resistance +4
            DefensiveAbilities = ChangeSkillStringMod(DefensiveAbilities, "channel resistance", 4, true);

            //Add DR 10/Magic and silver
            DR = AddDR(DR, "magic and silver", 10);

            //Add resist cold and electricity 10
            Resist = AddResitance(Resist, "cold", 10);
            Resist = AddResitance(Resist, "electricity", 10);

            //add fast healing 5
            AddFastHealing(5);

            //add vampire weaknesses
            Weaknesses = AddToStringList(Weaknesses, "vampire weaknesses");

            //add slam
            AddNaturalAttack("slam", 1, 0);

            //**add special attacks
            
            //blood drain
            SpecialAttacks = AddToStringList(SpecialAttacks, "blood drain");
            SpecialAbility ab = new SpecialAbility();
            ab.Name = "Blood Drain";
            ab.Type = "Su";
            ab.Text = "A vampire can suck blood from a grappled opponent; if the vampire establishes or maintains a pin, it drains blood, dealing 1d4 points of Constitution damage. The vampire heals 5 hit points or gains 5 temporary hit points for 1 hour (up to a maximum number of temporary hit points equal to its full normal hit points) each round it drains blood.";
            SpecialAbilitiesList.Add(ab);

            //children of the night
            SpecialAttacks = AddToStringList(SpecialAttacks, "children of the night");
            ab = new SpecialAbility();
            ab.Name = "Children of the Night";
            ab.Type = "Su";
            ab.Text = "Once per day, a vampire can call forth 1d6+1 rat swarms, 1d4+1 bat swarms, or 2d6 wolves as a standard action. (If the base creature is not terrestrial, this power might summon other creatures of similar power.) These creatures arrive in 2d6 rounds and serve the vampire for up to 1 hour.";
            SpecialAbilitiesList.Add(ab);

            //create spawn
            SpecialAttacks = AddToStringList(SpecialAttacks, "create spawn");
            ab = new SpecialAbility();
            ab.Name = "Create Spawn";
            ab.Type = "Su";
            ab.Text = "A vampire can create spawn out of those it slays with blood drain or energy drain, provided that the slain creature is of the same creature type as the vampire's base creature type. The victim rises from death as a vampire in 1d4 days. This vampire is under the command of the vampire that created it, and remains enslaved until its master's destruction. A vampire may have enslaved spawn totaling no more than twice its own Hit Dice; any spawn it creates that would exceed this limit become free-willed undead. A vampire may free an enslaved spawn in order to enslave a new spawn, but once freed, a vampire or vampire spawn cannot be enslaved again.";
            SpecialAbilitiesList.Add(ab);

            //dominate
            SpecialAttacks = AddToStringList(SpecialAttacks, "dominate");
            ab = new SpecialAbility();
            ab.Name = "Dominate";
            ab.Type = "Su";
            ab.Text = "A vampire can crush a humanoid opponent's will as a standard action. Anyone the vampire targets must succeed on a Will save or fall instantly under the vampire's influence, as though by a dominate person spell (caster level 12th). The ability has a range of 30 feet. At the GM's discretion, some vampires might be able to affect different creature types with this power.";
            SpecialAbilitiesList.Add(ab);

            //energy drain
            SpecialAttacks = AddToStringList(SpecialAttacks, "energy drain");
            ab = new SpecialAbility();
            ab.Name = "EnergyDrain";
            ab.Type = "Su";
            ab.Text = "A creature hit by a vampire's slam (or other natural weapon) gains two negative levels. This ability only triggers once per round, regardless of the number of attacks a vampire makes.";
            SpecialAbilitiesList.Add(ab);

            //**add special qualities

            //change shape
            SQ = AddToStringList(SQ, "change shape (dire bat or wolf, beast shape II)");
            ab = new SpecialAbility();
            ab.Name = "Change Shape";
            ab.Text = "A vampire can use change shape to assume the form of a dire bat or wolf, as beast shape II.";            
            ab.Type = "Su"; SpecialAbilitiesList.Add(ab);

            //gaseous form
            SQ = AddToStringList(SQ, "gaseous form");
            ab = new SpecialAbility();
            ab.Name = "Gaseous Form";
            ab.Type = "Su";
            ab.Text = "As a standard action, a vampire can assume gaseous form at will (caster level 5th), but it can remain gaseous indefinitely and has a fly speed of 20 feet with perfect maneuverability.";
            SpecialAbilitiesList.Add(ab);

            //shadowless
            SQ = AddToStringList(SQ, "shadowless");
            ab = new SpecialAbility();
            ab.Name = "Shadowless";
            ab.Type = "Ex";
            ab.Text = "A vampire casts no shadows and shows no reflection in a mirror.";
            SpecialAbilitiesList.Add(ab);

            //spider climb
            SQ = AddToStringList(SQ, "spider climb");
            ab = new SpecialAbility();
            ab.Name = "Spider Climb";
            ab.Type = "Ex";
            ab.Text = "A vampire can climb sheer surfaces as though under the effects of a spider climb spell.";
            SpecialAbilitiesList.Add(ab);

            //add racial skill bonuses
            AddRacialSkillBonus("Bluff", 8);
            AddRacialSkillBonus("Perception", 8);
            AddRacialSkillBonus("Sense Motive", 8);
            AddRacialSkillBonus("Stealth", 8);

            //add feats
            AddFeat("Alertness");
            AddFeat("CombatReflexes");
            AddFeat("Dodge");
            AddFeat("Improved Initiative");
            AddFeat("Lightning Reflexes");
            AddFeat("Toughness");



            //Make HD D8
            DieRoll roll = HDRoll;
            roll.die = 8;
            //add charisma and toughness
            roll.mod = AbilityBonus(Charisma) * roll.count + 
                (roll.count<3? 3: roll.count);
            HD = "(" + DieRollText(roll) + ")";
            HP = roll.AverageRoll();
            



            return true;
        }

        public enum ZombieType
        {
            Normal = 0,
            Fast = 1,
            Plague = 2
        }


        public bool MakeZombie(ZombieType zombieType)
        {
            
            if (String.Compare(Type, "undead", true) == 0 ||
                Strength == null || Dexterity == null)
            {
                return false;
            }

            //Make Neutral Evil
            AlignmentType al = new AlignmentType() ;
            al.Moral = MoralAxis.Evil;
            al.Order = OrderAxis.Neutral;
            Alignment = AlignmentText(al);
            

            //change type
            Type = "undead";



            //adjust strength
            AdjustStrength(2);

            //Adjust dex
            AdjustDexterity(-2);

            //Adjust Charisma
            if (Charisma == null)
            {
                charisma = 10;
            }
            else
            {
                AdjustCharisma(10 - Charisma.Value);
            }

            //Adjust Wisdom
            if (Wisdom == null)
            {
                wisdom = 10;
            }
            else
            {
                AdjustWisdom(10 - Wisdom.Value);
            }

            //change hd
            DieRoll roll = HDRoll;
            roll.die = 8;

            switch (SizeMods.GetSize(Size))
            {
                case MonsterSize.Small:
                case MonsterSize.Medium:
                    roll.count++;
                    break;
                case MonsterSize.Large:
                    roll.count += 2;
                    break;
                case MonsterSize.Huge:
                    roll.count += 4;
                    break;
                case MonsterSize.Gargantuan:
                    roll.count += 6;
                    break;
                case MonsterSize.Colossal:
                    roll.count += 10;
                    break;
            }

            HD = "(" + DieRollText(roll) + ")";
            if (roll.extraRolls != null)
            {
                roll.extraRolls.Clear();
            }


            roll.mod = Math.Max(3, roll.count);
            HP = roll.AverageRoll();

            //change saves
            Fort = roll.count / 3 + AbilityBonus(Charisma);
            Ref = roll.count / 3 + AbilityBonus(Dexterity);
            Will = roll.count / 2 + AbilityBonus(Wisdom);

            //change cr
            int count = HDRoll.count;
            if (count == 1)
            {
                Adjuster.CR = "1/4";
            }
            else if (count == 2)
            {
                Adjuster.CR = "1/4";
            }
            else if (count <= 12)
            {
                Adjuster.CR = ((count - 1) / 2).ToString();
            }
            else
            {
                Adjuster.CR = ((count - 12) / 3 + 6).ToString();
            }



            //remove special abilities
            RemoveAllForUndead();


            //add darkvision
            Senses = ChangeDarkvisionAtLeast(Senses, 60);


            //add natural ac
            MonsterSize ms = SizeMods.GetSize(Size);
            if (ms >= MonsterSize.Small)
            {
                int bonus = 0;

                if (ms < MonsterSize.Gargantuan)
                {
                    bonus = ((int)ms) - (int)MonsterSize.Tiny;
                }
                else if (ms == MonsterSize.Gargantuan)
                {
                    bonus = +7;
                }
                else if (ms == MonsterSize.Colossal)
                {
                    bonus += 11;
                }

                AdjustNaturalArmor(bonus - NaturalArmor);
            }



            //add undead imuinites
            Immune = AddToStringList(Immune, "undead immunities");


            //add DR/5 slashing
            DR = AddDR(DR, "slashing", 5);

            //make fly clumsy
            Speed = SetFlyQuality(Speed, "clumsy");

            //adjust BAB
            AdjustBaseAttack(roll.count * 3 / 4 - BaseAtk, true);


            //add slam (+1 diestep)
            AddNaturalAttack("slam", 1, 1);

            //add toughness
            AddFeat("Toughness");

            //add staggered SQ
            SQ = AddToStringList(SQ, "staggered");
            SpecialAbility ab = new SpecialAbility();
            ab.Name = "Staggered";
            ab.Type = "Ex";
            ab.Text = "Zombies have poor reflexes and can only perform a single move action or standard action each round. A zombie can move up to its speed and attack in the same round as a charge action.";
            SpecialAbilitiesList.Add(ab);

            //set values for zombie subtypes
            if (zombieType == ZombieType.Fast)
            {
                //speed + 10
                Speed = ChangeStartingNumber(Speed, 10);

                //remove DR
                DR = null;

                //remove staggered
                SQ = RemoveFromStringList(SQ, "staggered");
                SpecialAbilitiesList.Clear();

                //Quick Strikes
                SpecialAttacks = AddToStringList(SpecialAttacks, "quick strikes");
                ab = new SpecialAbility();
                ab.Name = "Quick Strikes";
                ab.Type = "Ex";
                ab.Text = "Whenever a fast zombie takes a full-attack action, it can make one additional slam attack at its highest base attack bonus.";
                SpecialAbilitiesList.Add(ab);
                //switch to 2 slams for quick strikes
                AddNaturalAttack("slam", 2, 1);

                //Add dex to change from -2 to +2
                AdjustDexterity(4);
            }
            else if (zombieType == ZombieType.Plague)
            {
                //remove DR
                DR = null;

                //get save dc
                int dc = 10 + roll.count / 2 + AbilityBonus(Charisma);

                //add death burst
                SpecialAttacks = AddToStringList(SpecialAttacks, "death burst (DC " + dc + ")");
                ab = new SpecialAbility();
                ab.Name = "Death Burst";
                ab.Type = "Ex";
                ab.Text = "When a plague zombie dies, it explodes in a burst of decay. All creatures adjacent to the plague zombie are exposed to its plague as if struck by a slam attack and must make a Fortitude save or contract zombie rot.";
                SpecialAbilitiesList.Add(ab);

                //add disease
                Melee = SetPlusOnMeleeAttacks(Melee, "disease", true);
                ab = new SpecialAbility();
                ab.Name = "Zombie Rot";
                ab.Type = "Su";
                ab.Text = "The slam attack — as well as any other natural attacks — of a plague zombie carries the zombie rot disease.\r\n\r\n" + 
                            "Zombie rot: slam; save Fort DC " + dc + "; onset 1d4 days; frequency 1/day; effect 1d2 Con, this damage cannot be healed while the creature is infected; cure 2 consecutive saves. Anyone who dies while infected rises as a plague zombie in 2d6 hours.";
                SpecialAbilitiesList.Add(ab);

            }



            return true;
        }

        public void AdjustBaseAttack(int diff, bool fix)
        {
            //adjust bab
            BaseAtk += diff;

            SizeMods mods = SizeMods.GetMods(SizeMods.GetSize(Size));

            //adjust cmb
            CMB_Numeric += diff;

            //adjust cmd
            CMD_Numeric += diff;

            if (fix)
            {
                //fix attacks
                Melee = FixMeleeAttacks(Melee, BaseAtk + AbilityBonus(Strength) + mods.Attack, AbilityBonus(Strength), true);

                Ranged = FixRangedAttacks(Ranged, BaseAtk + AbilityBonus(Dexterity) + mods.Attack, AbilityBonus(Strength), true, this);
            }
            else
            {

                Melee = ChangeAttackMods(Melee, diff);
                Ranged = ChangeAttackMods(Ranged, diff);
            }
        }

        public void FixMeleeAttacks()
        {
            FixAttacks(true, false);
        }

        public void FixRangedAttacks()
        {
            FixAttacks(false, true);
        }

        public void FixAttacks()
        {
            FixAttacks(true, true);
        }

        public void FixAttacks(bool fixMelee, bool fixRanged)
        {

            ObservableCollection<AttackSet> sets = new ObservableCollection<AttackSet>(MeleeAttacks);
            ObservableCollection<Attack> ranged = new ObservableCollection<Attack>(RangedAttacks);
            CharacterAttacks attacks = new CharacterAttacks(sets, ranged);
            if (fixMelee)
            {
                Melee = MeleeString(attacks);
            }
            if (fixRanged)
            {
                Ranged = RangedString(attacks);
            }
        }

        public void AddNaturalAttack(string name, int count, int step)
        {
           
            if (Weapon.Weapons.ContainsKey(name))
            {
                ObservableCollection<AttackSet> sets = new ObservableCollection<AttackSet>(MeleeAttacks);
                ObservableCollection<Attack> ranged = new ObservableCollection<Attack>(RangedAttacks);
                CharacterAttacks attacks = new CharacterAttacks(sets, ranged);

                bool bAdded = false;
                foreach (WeaponItem wi in attacks.NaturalAttacks)
                {
                    if (String.Compare(wi.Name, Name, true) == 0)
                    {
                        if (wi.Count < count)
                        {
                            wi.Count = count;
                        }
                        bAdded = true;
                        break;
                    }

                }

                if (!bAdded)
                {
                    WeaponItem item = new WeaponItem(Weapon.Weapons[name]);
                    item.Count = count;
                    attacks.NaturalAttacks.Add(item);
                }

                Melee = MeleeString(attacks);
            }
            else
            {

                Attack attack = new Attack();
                attack.CritMultiplier = 2;
                attack.CritRange = 20;
                attack.Name = name;
                attack.Count = count;
                MonsterSize monsterSize = SizeMods.GetSize(Size);
                SizeMods mods = SizeMods.GetMods(monsterSize);

                DieRoll damageRoll = new DieRoll(0, 1, AbilityBonus(Strength));
                attack.Damage = DieRoll.StepDie(damageRoll, (int)monsterSize + step);


                attack.Bonus = new List<int>() { BaseAtk + AbilityBonus(Strength) + mods.Attack };
                Melee = AddAttack(Melee, attack);
            }

        }

        public void RemoveAllForUndead()
        {
            //remove defensive abilities
            DefensiveAbilities = null;
            SR = null;
            Weaknesses = null;
            Resist = null;
            DR = null;
            Aura = null;


            //remove Immunity
            Immune = null;

            //remove skills
            SkillValueDictionary.Clear();
            Languages = null;
            RacialMods = null;

            //remove feats
            FeatsList.Clear();

            //remove special attacks & special abilities
            SpecialAbilities = null;
            specialAbilitiesList.Clear();
            SpecialAttacks = null;
            SpellLikeAbilities = null;
            SpellsKnown = null;
            SpellDomains = null;
            SpellsPrepared = null;
            SQ = null;

        }

        public void ApplyFeatChanges(string feat, bool added)
        {

            if (feat == "Alertness")
            {
                AddOrChangeSkill("Perception", added ? 2 : -2);
                AddOrChangeSkill("Sense Motive", added ? 2 : -2);

            }
            else if (feat == "Dodge")
            {

                FullAC += added ? 1 : -1;
                TouchAC += added ? 1 : -1;
            }
            else if (feat == "Improved Initiative")
            {
                Init += added ? 4 : -4;
            }
            else if (feat == "Lightning Reflexes")
            {
                Ref += added ? 2 : -2;
            }
            else if (feat == "Toughness")
            {
                DieRoll roll = HDRoll;

                int bonus = roll.count < 3 ? 3 : roll.count;

                roll.mod += added ? bonus : -bonus;

                HDRoll = roll;

                HP += added ? bonus : -bonus;

            }
            else if (feat == "Weapon Finesse")
            {
                //Attacks
                FixMeleeAttacks();
            }
        }


        private static int GetCRHPChange(int crLevel)
        {
            int val = 0;

            if (crLevel <= 2)
            {
                val = 5;
            }
            else if (crLevel <= 4)
            {
                val = 10;
            }
            else if (crLevel <= 12)
            {
                val = 15;
            }
            else if (crLevel <= 16)
            {
                val = 20;
            }
            else if (crLevel <= 20)
            {
                val = 30;
            }
            else
            {
                val = 30;
            }

            return val;
        }

        private void AdjustChartMods(SizeMods mods, bool add)
        {
            if (mods.Strength != 0)
            {
                AdjustStrength(mods.Strength * (add ? 1 : -1));
            }
            if (mods.Dexterity != 0)
            {
                AdjustDexterity(mods.Dexterity * (add ? 1 : -1));
            }
            if (mods.Constitution != 0)
            {
                AdjustConstitution(mods.Constitution * (add ? 1 : -1));
            }
            if (mods.NaturalArmor != 0)
            {
                AdjustNaturalArmor(mods.NaturalArmor * (add ? 1 : -1));
            }
        }




        public void AdjustNaturalArmor(int value)
        {
            int mod = Math.Max(value, -NaturalArmor);

            NaturalArmor += mod;
            FullAC += mod;
            FlatFootedAC += mod;
            AC_Mods = ReplaceModifierNumber(ac_mods, "natural", NaturalArmor, false);
            
        }

        public void AdjustShield(int value)
        {

            Shield += value;
            FullAC += value;
            FlatFootedAC += value;
            AC_Mods = ReplaceModifierNumber(ac_mods, "shield", Shield, false);

        }

        public void AdjustSizeACModifier(int diff)
        {

            //adjust AC
            FullAC += diff;
            TouchAC += diff;
            FlatFootedAC += diff;

            int mod = FindModifierNumber(ac_mods, "size");

            AC_Mods = ReplaceModifierNumber(ac_mods, "size", mod + diff, false);
        }

        public void AdjustArmor(int value)
        {

            Armor += value;
            FullAC += value;
            FlatFootedAC += value;
            AC_Mods = ReplaceModifierNumber(ac_mods, "armor", Armor, false);

        }


        public void AdjustDodge(int value)
        {
            Dodge += value;
            FullAC += value;
            CMD += value;
            TouchAC += value;
            AC_Mods = ReplaceModifierNumber(ac_mods, "dodge", Dodge, false);

        }
        public void AdjustDeflection(int value)
        {
            Deflection += value;
            FullAC += value;
            FlatFootedAC += value;
            CMD += value;
            TouchAC += value;
            AC_Mods = ReplaceModifierNumber(ac_mods, "deflection", Deflection, false);

        }


        public void AdjustCR(int diff)
        {
            if (diff > 0)
            {
                if (CR.Contains('/'))
                {
                    CR = (diff).ToString();
                }
                else
                {
                    int crVal = int.Parse(CR);

                    crVal += diff;

                    CR = crVal.ToString();
                }
            }
            else
            {
                Regex crSlash = new Regex("([0-9]+)/([0-9]+)");

                Match match = crSlash.Match(CR);

                int crInt = 1;

                if (match.Success)
                {
                    int val = int.Parse(match.Groups[2].Value);

                    if (lowCRToIntChart.ContainsKey(val))
                    {
                        crInt = lowCRToIntChart[val];
                    }
                }
                else
                {
                    crInt = int.Parse(CR);
                }

                crInt += diff;

                if (crInt >= 1)
                {
                    CR = crInt.ToString();
                }
                else
                {
                    int crValOut;

                    if (intToLowCRChart.TryGetValue(crInt, out crValOut))
                    {
                        CR = "1/" + crValOut.ToString();
                    }
                    else
                    {
                        CR = "1/8";
                    }
                }

            }

            XP = GetXPString(CR);
        }

        public static int GetCRChartInt(string crText)
        {
            Regex crSlash = new Regex("([0-9]+)/([0-9]+)");

            Match match = crSlash.Match(crText);

            int crInt = 1;

            if (match.Success)
            {
                int val = 1;

                if (int.TryParse(match.Groups[2].Value, out val))
                {

                    if (lowCRToIntChart.ContainsKey(val))
                    {
                        crInt = lowCRToIntChart[val];
                    }
                }
            }
            else
            {
                crInt = int.Parse(crText);
            }

            return crInt;

        }

        private static string GetXPString(string crText)
        {
            return GetCRValue(crText).ToString("#,#", CultureInfo.InvariantCulture);
                 
        }

        public static int GetCRValue(string crText)
        {
            int xpVal = 0;

            if (crText.Contains('/'))
            {
                if (xpValues.ContainsKey(crText))
                {
                    xpVal = xpValues[crText];
                }
            }
            else
            {
                int x = int.Parse(crText);

                xpVal = GetIntCRValue(x);
            }

            return xpVal;
        }

        public static int? TryGetCRValue(string crText)
        {

            int? xpVal = null;

            if (crText.Contains('/'))
            {
                if (xpValues.ContainsKey(crText))
                {
                    xpVal = xpValues[crText];
                }
            }
            else
            {
                int x;

                if (int.TryParse(crText, out x))
                {
                    if (x > 0)
                    {
                        xpVal = GetIntCRValue(x);
                    }
                }
            }

            return xpVal;
        }

        private static int GetAllIntCRValue(int crInt)
        {
            if (crInt < -4)
            {
                return 0;
            }
            if (crInt < 1)
            {
                return xpValues["1/" + intToLowCRChart[crInt]];
            }
            else
            {
                return GetIntCRValue(crInt);
            }
        }

        private static int GetIntCRValue(int crInt)
        {
            int x = crInt;

            int powVal = ((x + 1) / 2) + 1;
            int xpVal = ((int)Math.Pow(2, powVal)) * 100;
            if ((x - 1) % 2 != 0)
            {
                xpVal += xpVal / 2;
            }

            return xpVal;
        }

        public static string EstimateCR(int xp)
        {
            string CR = null;

            //get CR
            if (xp < GetAllIntCRValue(-4))
            {

            }
            else
            {
                int i = -4;


                while (!(xp >= GetAllIntCRValue(i) && xp < GetAllIntCRValue(i + 1)))
                {
                    i++;
                }

                if (i < 1)
                {
                    int denominator = intToLowCRChart[i];

                    CR = "1/" + denominator;
                }
                else
                {
                    CR = i.ToString();
                }

            }

            return CR;
        }



        public void AdjustDexterity(int value)
        {
            if (DexZero)
            {
                if (PreLossDex != null)
                {
                    PreLossDex += value;
                }
            }
            else
            {
                if (Dexterity != null)
                {
                    int? oldDex = Dexterity;
                    Dexterity += value;

                    ApplyDexterityAdjustments(oldDex);
                    
                }
            }
        }

        private void ApplyDexterityAdjustments(int? oldDex)
        {
                           
            int oldDexBonus = AbilityBonus(oldDex);
            int newDexBonus = AbilityBonus(Dexterity);

            int diff = newDexBonus - oldDexBonus;

            //adjust armor
            FullAC += diff;
            TouchAC += diff;

            if (newDexBonus < 0)
            {
                FlatFootedAC = FullAC;
            }
            else
            {
                FlatFootedAC = FullAC - newDexBonus;
            }

            AC_Mods = ReplaceModifierNumber(ac_mods, "Dex", newDexBonus, false);

            if (AC_Mods != null)
            {
                if (!Regex.Match(AC_Mods, "\\(.*\\)").Success)
                {
                    AC_Mods = "(" + AC_Mods + ")";
                }

                if (AC_Mods == "()")
                {
                    AC_Mods = "";
                }
            }
                

            //adjust initiative
            Init += diff;

            //adjust save
            Ref += diff;
                
            //adjust CMD
            CMD = ChangeCMD(CMD, diff);

            //adjust attacks
            if (Ranged != null && ranged.Length > 0)
            {
                Ranged = ChangeAttackMods(Ranged, diff);
            }

            ChangeSkillsForStat(Stat.Dexterity, diff);
                
            
        }


        public void AdjustWisdom(int value)
        {
            if (Wisdom != null)
            {
                int? oldWis = Wisdom;
                Wisdom += value;

                ApplyWisdomAdjustments(oldWis);
                
            }

        }

        private void ApplyWisdomAdjustments(int? oldWis)
        {

            int oldBonus = AbilityBonus(oldWis);
            int newBonus = AbilityBonus(Wisdom);

            int diff = newBonus - oldBonus;

            //adjust perception
            if (!ChangeSkill("Perception", diff))
            {
                Senses = ChangeSkillStringMod(Senses, "Perception", diff);
            }

            //adjust save
            Will += diff;

            ChangeSkillsForStat(Stat.Wisdom, diff);
        }

        public void AdjustIntelligence(int value)
        {
            if (Intelligence != null)
            {
                int? oldInt = Intelligence;
                Intelligence += value;

                ApplyIntelligenceAdjustments(oldInt);
                
            }
        }

        private void ApplyIntelligenceAdjustments(int? oldInt)
        {
            int oldBonus = AbilityBonus(oldInt);
            int newBonus = AbilityBonus(Intelligence);

            int diff = newBonus - oldBonus;


            //adjust skills
            ChangeSkillsForStat(Stat.Intelligence, diff);


            CreatureTypeInfo info = CreatureTypeInfo.GetInfo(Type);
            
            //get skill count
            int oldSkillCount = Math.Max(info.Skills + oldBonus, 1);
            int newSkillCount = Math.Max(info.Skills + newBonus, 1);
            AdjustSkills(newSkillCount * HDRoll.TotalCount - oldSkillCount * HDRoll.TotalCount);

        }

        [XmlIgnore]
        public int Perception
        {
            get
            {

                int perception = 0;
                if (SkillValueDictionary.ContainsKey("Perception"))
                {
                    perception = SkillValueDictionary["Perception"].Mod;
                }
                else
                {
                    perception = AbilityBonus(Wisdom);
                }

                return perception;
            }
            set
            {

            }
        }

        public void AdjustStrength(int value)
        {
            if (StrZero)
            {
                if (PreLossStr != null)
                {
                    PreLossStr += value;
                }
            }
            else
            {
                if (Strength != null)
                {
                    int? old = Strength;
                    Strength += value;

                    ApplyStrengthAdjustments(old);
                    


                }
            }
        }

        private void ApplyStrengthAdjustments(int? old)
        {
            int oldBonus = AbilityBonus(old);
            int newBonus = AbilityBonus(Strength);

            int diff = newBonus - oldBonus;
            int diffPlusHalf = (newBonus + newBonus / 2) - (oldBonus + oldBonus / 2);
            int halfDiff = newBonus / 2 - oldBonus / 2;

            //adjust attacks
            if (Melee != null && Melee.Length > 0)
            {
                Melee = ChangeAttackMods(Melee, diff);
                Melee = ChangeAttackDamage(Melee, diff, diffPlusHalf, halfDiff);
            }

            if (Ranged != null && Ranged.Length > 0)
            {
                Ranged = ChangeThrownDamage(Ranged, diff, diffPlusHalf);
            }

            //adjust CMB
            CMB = ChangeStartingMod(CMB, diff);

            //adjust CMD
            CMD = ChangeCMD(CMD, diff);

            //adjust skills
            ChangeSkillsForStat(Stat.Strength, diff);
        }

        public void AdjustConstitution(int value)
        {
            if (Constitution != null)
            {
                int? old = Constitution;
                Constitution += value;

                ApplyConstitutionAdjustments(old);
                

            }
        }

        private void ApplyConstitutionAdjustments(int? old)
        {

            int oldBonus = AbilityBonus(old);
            int newBonus = AbilityBonus(Constitution);

            int diff = newBonus - oldBonus;

            //adjust hp/hd
            ChangeHDMod(diff);

            //adjust save
            Fort += diff;

            //adjust skills
            ChangeSkillsForStat(Stat.Constitution, diff);

        }

        public void AdjustCharisma(int value)
        {
            if (Charisma != null)
            {
                int? old = Charisma;
                Charisma += value;

                ApplyCharismaAdjustments(old);
                
            }
        }

        private void ApplyCharismaAdjustments(int? old)
        {
            int oldBonus = AbilityBonus(old);
            int newBonus = AbilityBonus(Charisma);

            int diff = newBonus - oldBonus;

            if (String.Compare(Type, "Undead", true) == 0)
            {

                //adjust save
                Fort += diff;

                //adjust hd
                ChangeHDMod(diff);
            }

            //adjust skills
            ChangeSkillsForStat(Stat.Charisma, diff);

        }




        public void AdjustHD(int diff)
        {
            DieRoll roll = FindNextDieRoll(HD, 0);
            
            //check for toughness
            bool toughness = FeatsList.Contains("Toughness");

            //get hp mod
            int hpMod = 0;

            if (String.Compare(Type, "undead", true) == 0)
            {
                hpMod = AbilityBonus(Charisma);
            }
            else if (String.Compare(Type, "construct", true) != 0)
            {
                hpMod = AbilityBonus(Constitution);
            }

            int applyCount = diff;

            if (roll.count + diff < 1)
            {
                applyCount = 1 - roll.count;
            }

            
            int oldCount = roll.count;
            
            
            roll.count += applyCount;
            roll.mod += hpMod * applyCount;
            int toughnessExtra = 0;
            if (toughness)
            {
                if (applyCount > 0)
                {
                    int diffCount = oldCount;

                    if (diffCount < 3)
                    {
                        diffCount = 3;
                    }

                    toughnessExtra = roll.count - diffCount;

                    if (toughnessExtra > 0)
                    {
                        roll.mod += toughnessExtra;
                    }
                    else
                    {
                        toughnessExtra = 0;
                    }
                }
                else if (applyCount < 0)
                {
                    int newCount = roll.count;

                    if (newCount < 3)
                    {
                        newCount = 3;
                    }

                    toughnessExtra = newCount - oldCount;

                    if (toughnessExtra > 0)
                    {
                        roll.mod += toughnessExtra;
                    }
                    else
                    {
                        toughnessExtra = 0;
                    }

                }
            }

            HD = "(" + DieRollText(roll) + ")";

            HP += hpMod * applyCount + toughnessExtra;

            HP += (applyCount * roll.die) / 2 + applyCount/2;

            SpellLikeAbilities = ChangeSpellLikeCL(SpellLikeAbilities, applyCount);
 
        }

        private static string ChangeSpellLikeCL(String text, int diff)
        {
            if (text == null)
            {
                return null;
            }

            string returnText = text;

            Regex regEx = new Regex("(CL )([0-9]+)((th)|(rd)|(nd)|(st))");

            returnText = regEx.Replace(returnText, delegate(Match m)
            {
                int cl = int.Parse(m.Groups[2].Value) + diff;


                string end = "th";

                switch (cl % 10)
                {
                    case 1:
                        end = "st";
                        break;
                    case 2:
                        end = "nd";
                        break;
                    case 3:
                        end = "rd";
                        break;
                }

                return "CL " + cl.ToString() + end;

            });


            return returnText;            
        }

        private static string ChangeDarkvisionAtLeast(string text, int dist)
        {
            
            Regex regDark = new Regex("(darkvision )([0-9]+)( ft\\.)", RegexOptions.IgnoreCase);

            string returnText = text;
            bool bFound = false;

            returnText =  regDark.Replace(text, delegate(Match m)
                {
                    bFound = true;
                    return m.Groups[1] +
                        (Math.Max(int.Parse(m.Groups[2].Value), dist)).ToString() +
                        m.Groups[3];
                        
                }, 1);

            if (!bFound)
            {
                Match match = new Regex(";").Match(text);

                string newText = String.Format("darkvision {0} ft.", dist);

                newText += match.Success ? ", " : "; ";

                returnText = newText + returnText;
            }


            return returnText;
        }

        private static string AddSense(string text, string sense)
        {

            string returnText = text;
            if (returnText == null)
            {
                returnText = "";
            }


            Regex regSense = new Regex(Regex.Escape(sense), RegexOptions.IgnoreCase);

            if (!regSense.Match(returnText).Success)
            {
                bool bAdded = false;

                Regex regColon = new Regex(";");

                returnText = regColon.Replace(returnText, delegate(Match m)
                    {
                        bAdded = true;
                        return ", " + sense + ";";
                    }, 1);

                if (!bAdded)
                {
                    returnText = sense + "; " + returnText;
                }
                
            }

            return returnText;
        }

        private static string AddImmunity(string text, string type)
        {
            return AddToStringList(text, type);
           
        }

        private const string FlyString = "(fly )([0-9]+)( ft\\. \\()([a-zA-Z]+)(\\))";

        private static string AddFlyFromMove(string text, int speedMult, string quality)
        {
            
            string returnText = text;

            //get speed
            Regex regName = new Regex("^[0-9]+");
            Match match = regName.Match(returnText);
            int move = 0;
            if (match.Success)
            {
                move = int.Parse(match.Value);
            }
            int flySpeed = move * speedMult;

            Regex regFly = new Regex(FlyString, RegexOptions.IgnoreCase);

            bool bAdded = false;
            returnText = regFly.Replace(returnText, delegate(Match m)
                {
                    flySpeed = Math.Max(int.Parse(m.Groups[2].Value), flySpeed);

                    bAdded = true;
                    return m.Groups[1].Value + flySpeed +
                        m.Groups[3].Value + GetMaxFlyQuality(m.Groups[4].Value, quality) + m.Groups[5].Value;
                }, 1);

            if (!bAdded)
            {
                returnText = returnText + ", fly " + flySpeed + " ft. (" + quality + ")";
            }

            return  returnText;
        }

        private static string SetFlyQuality(string text, string quality)
        {
            string returnText = text;

            if (returnText != null)
            {

                Regex regFly = new Regex(FlyString, RegexOptions.IgnoreCase);

                returnText = regFly.Replace(returnText, delegate(Match m)
                {

                    return m.Groups[1].Value + m.Groups[2].Value +
                        m.Groups[3].Value + quality + m.Groups[5].Value;
                }, 1);
            }
            

            return returnText;
        }

        private static string RemoveFly(string text)
        {
            Regex regFly = new Regex("(, )?(fly )([0-9]+)( ft\\. \\()([a-zA-Z]+)(\\))", RegexOptions.IgnoreCase);


            return regFly.Replace(text, "");
        }

        private static int GetFlyQuality(string quality)
        {
            int value;

            if (!flyQualityList.TryGetValue(quality, out value))
            {
                value = -1;
            }
            
            return value;
        }

        public static string GetFlyQualityString(int val)
        {
            return flyQualityList.First(a => a.Value == val).Key;
        }

        private static string GetMaxFlyQuality(string quality1, string quality2)
        {
            return (GetFlyQuality(quality1) > GetFlyQuality(quality2))?quality1 : quality2;
        }

        private static string AddToStringList(string text, string type)
        {
            bool added;

            return AddToStringList(text, type, out added);

        }

        private static string AddToStringList(string text, string type,  out bool added)
        {
            added = false;


            string returnText = text;
            if (returnText == null)
            {
                returnText = "";
            }

            Regex regType = new Regex(Regex.Escape(type), RegexOptions.IgnoreCase);

            if (!regType.Match(returnText).Success)
            {

                returnText = returnText + (returnText.Length > 0 ? ", " : "") + type;

                added = true;    

            }

            return returnText;
        }

        private static string RemoveFromStringList(string text, string type)
        {
            bool removed;
            return RemoveFromStringList(text, type, out removed);
        }

        private static string RemoveFromStringList(string text, string type, out bool removed)
        {
            removed = false;

            Regex regex = new Regex("(^| )(" + type + ")(\\Z|,)");

            return regex.Replace(text, "").Trim();

        }


        private static string AddDR(string text, string type, int val)
        {

            Regex regDr = new Regex("([0-9]+)(/ " + type + ")");

            string returnText = text;

            if (returnText == null)
            {
                returnText = "";
            }

            bool bFound = false;

            returnText = regDr.Replace(returnText, delegate(Match m)
                {
                    bFound = true;
                    return
                        (Math.Max(int.Parse(m.Groups[1].Value), val)).ToString() +
                        m.Groups[2];

                }, 1);

            if (!bFound)
            {
                if (returnText.Length > 0)
                {
                    returnText += ", ";
                }
                returnText += val.ToString() + "/" + type;
                    
            }

            return returnText;

        }

        private static string AddAttack(string text, Attack attack)
        {
            Attack addAttack = attack;


            string returnText = text;
            if (returnText == null)
            {
                returnText = "";
            }

            Regex regAttack = new Regex(Attack.RegexString(attack.Name));
            bool bAdded = false;

            returnText = regAttack.Replace(returnText, delegate(Match m)
                {
                    bAdded = true;
                    Attack foundAttack = Attack.ParseAttack(m);



                    addAttack.Damage.Step =
                        (AverageDamage(foundAttack.Damage.Step) > AverageDamage(addAttack.Damage.Step)) ?
                        foundAttack.Damage.Step : addAttack.Damage.Step;


                    return addAttack.Text;

                    
                });

            if (!bAdded)
            {
                returnText += ", " + addAttack.Text;
            }

            return returnText;
        }



        private string FixMeleeAttacks(string text, int bonus, int damageMod, bool removePlus)
        {
            if (text == null)
            {
                return null;
            }

            string returnText = text;

            CharacterAttacks attacks = new CharacterAttacks(this);

            if (attacks.MeleeWeaponSets.Count > 0 || attacks.NaturalAttacks.Count > 0)
            {
                returnText = MeleeString(attacks);
            }


            return returnText;
            

        }

        private string FixRangedAttacks(string text, int bonus, int damageMod, bool removePlus, Monster monster)
        {

            if (text == null)
            {
                return null;
            }

            string returnText = text;

            CharacterAttacks attacks = new CharacterAttacks(this);

            if (attacks.RangedAttacks.Count > 0)
            {
                returnText = RangedString(attacks);
            }


            return returnText;
        }

        private static string SetPlusOnMeleeAttacks(string text, string plus, bool natural)
        {
            if (text == null)
            {
                return null;
            }

            string returnText = text;

            Regex regAttack = new Regex(Attack.RegexString(null));

            returnText = regAttack.Replace(returnText, delegate(Match m)
            {

                Attack info = Attack.ParseAttack(m);
                
    
                if (!natural || !weaponNameList.ContainsKey(info.Name.ToLower()))
                {
                    info.Plus = plus;
                }


                return info.Text;

            });

            return returnText;
        }



        private static double AverageDamage(DieStep step)
        {
            return AverageDamage(step.Count, step.Die);
        }

        private static double AverageDamage(int count, int die)
        {
            double val = ((double)die) / 2.0 + 0.5;

            return val * count;
        }





        private static string AddSpecialAttack(string text, string attack, int count)
        {
            Regex regSp = new Regex("(" + attack + " \\(?)([0-9]+)(/ ?day\\)?)", RegexOptions.IgnoreCase);

            string returnText = text;

            if (returnText == null)
            {
                returnText = "";
            }

            bool bFound = false;

            returnText = regSp.Replace(returnText, delegate(Match m)
                {
                    bFound = true;
                    return m.Groups[1] + 
                        (int.Parse(m.Groups[2].Value) + count).ToString() +
                        m.Groups[3];

                }, 1);

            if (!bFound)
            {
                if (returnText.Length > 0)
                {
                    returnText += ", ";
                }
                returnText += attack + " (" + count + "/day)" ;
                    
            }

            return returnText;
        }

        private static string AddSpellLikeAbility(string text, string ability, int count, int cl)
        {
            string returnText = text;

            bool addCL = false;
            if (returnText == null || returnText.Length == 0)
            {
                addCL = true;
            }


            returnText = AddToStringList(returnText, count + "/day-" + ability);

            if (addCL)
            {
                returnText = "(CL " + cl + ") " + returnText;
            }

            return returnText;

        }

        private static string AddSummonDR(string text, string HD, string type)
        {
            string returnText = text;

            DieRoll roll = FindNextDieRoll(HD, 0);

            if (roll.count > 4 && roll.count <= 10)
            {
                returnText = AddDR(returnText, type, 5);
            }
            else if (roll.count >= 10)
            {
                returnText = AddDR(returnText, type, 10);
            }

            return returnText;
        }

        private static string AddSummonSR(string text, string CR, int extra)
        {
            string returnText = text;

            if (returnText == null)
            {
                returnText = "0";
            }           

   
            int intCR = 0;

            if (!CR.Contains('/'))
            {
                intCR = int.Parse(CR);
            }


            int newSR = intCR + extra;

            Regex regNum = new Regex("[0-9]+");

            returnText = regNum.Replace(returnText, delegate(Match m)
                {
                    return Math.Max(int.Parse(m.Value), newSR).ToString();
                }, 1);



            return returnText;
        }

        public void AddFastHealing(int amount)
        {
            HP_Mods = AddTypeValToStringList(HP_Mods, "fast healing", amount);
        }

        private static string AddResitance(string text, string type, int val)
        {
            return AddTypeValToStringList(text, type, val);
        }

        

        private static string AddTypeValToStringList(string text, string type, int val)
        {

            Regex regRes = new Regex("(" + type + " )([0-9]+)", RegexOptions.IgnoreCase);

            string returnText = text;

            if (returnText == null)
            {
                returnText = "";
            }

            bool bFound = false;

            returnText = regRes.Replace(returnText, delegate(Match m)
            {
                bFound = true;
                return
                    m.Groups[1] +
                    (Math.Max(int.Parse(m.Groups[2].Value), val)).ToString();
                    

            }, 1);

            if (!bFound)
            {
                if (returnText.Length > 0)
                {
                    returnText += ", ";
                }
                returnText += type + " " + val;

            }

            return returnText;

        }


        private static string AddPlusModtoList(string text, string type, int val)
        {
            Regex regRes = new Regex("(" + type + " )((\\+|-)[0-9]+)");

            string returnText = text;

            if (returnText == null)
            {
                returnText = "";
            }

            bool bFound = false;

            returnText = regRes.Replace(returnText, delegate(Match m)
            {
                bFound = true;
                return
                    m.Groups[1] +
                    (Math.Max(int.Parse(m.Groups[2].Value), val)).ToString();


            }, 1);

            if (!bFound)
            {
                if (returnText.Length > 0)
                {
                    returnText += ", ";
                }
                returnText += type + " " + CMStringUtilities.PlusFormatNumber(val);

            }

            return returnText;

        }



        private void ChangeHDMod(int diff)
        {

            //adjust hp
            DieRoll hdRoll = FindNextDieRoll(HD, 0);
            if (hdRoll != null)
            {
                hdRoll.mod += (diff * hdRoll.count) / hdRoll.fraction;
                HD = ReplaceDieRoll(HD, hdRoll, 0);

                HP += diff * hdRoll.count;
            }
               
        }

        public bool HasDefensiveAbility(string quality)
        {
            if (DefensiveAbilities == null)
            {
                return false;
            }

            return new Regex(Regex.Escape(quality), RegexOptions.IgnoreCase).Match(DefensiveAbilities).Success;
        }


        private void AdjustSkills(int diff)
        {
            if (Intelligence != null)
            {
                CreatureTypeInfo info = CreatureTypeInfo.GetInfo(Type);

                List<KeyValuePair<SkillValue, int>> list = GetSkillRanks();
               

                if (list.Count > 0 && diff != 0)
                {
                    int count = Math.Abs(diff);

                    int listNum = 0;

                    for (int i = 0; i < count; i++)
                    {
                        int extraTries = list.Count;
                        bool added = false;
                        while (!added && extraTries > 0)
                        {
                            SkillValue skillValue = list[listNum].Key;

                            if (diff > 0)
                            {
                                if (skillValue.Mod < SkillMax(skillValue.Name))
                                {
                                    skillValue.Mod++;
                                    added = true;
                                }
                            }
                            else if (diff < 0)
                            {
                                if (skillValue.Mod > SkillMin(skillValue.Name))
                                {
                                    skillValue.Mod--;
                                    added = true;

                                    if (skillValue.Mod == SkillMin(skillValue.Name) && GetRacialSkillMod(skillValue.FullName) == 0)
                                    {
                                        SkillValueDictionary.Remove(skillValue.FullName);
                                    }
                                }
                            }

                            if (!added)
                            {
                                extraTries--;
                            }

                            listNum = (listNum + 1) % list.Count;
                        }

                        if (extraTries == 0)
                        {
                            if (diff > 0)
                            {
                                foreach (string skill in info.ClassSkills)
                                {
                                    if (skill != "Knowledge")
                                    {
                                        if (!skillValueDictionary.ContainsKey(skill))
                                        {

                                            SkillValue val = new SkillValue(skill);
                                            try
                                            {
                                                val.Mod = AbilityBonus(GetStat(SkillsList[val.Name])) +
                                                    4;
                                                skillValueDictionary[skill] = val;
                                                added = true;
                                            }
                                            catch (Exception ex)
                                            {
                                                System.Diagnostics.Debug.WriteLine(val.Name);
                                                System.Diagnostics.Debug.WriteLine(ex.ToString());
                                                throw;
                                            }
                                        }
                                    }
                                }

                            }
                            
                            if (!added)
                            {
                                break;
                            }
                        }

                    }
                }
            }

        }

        private int SkillMax(string skill)
        {
            CreatureTypeInfo info = CreatureTypeInfo.GetInfo(Type);

            return HDRoll.TotalCount + AbilityBonus(GetStat(SkillsList[skill])) + (info.IsClassSkill(skill)?3:0) + GetRacialSkillMod(skill);
        }

        private int SkillMin(string skill)
        {
            CreatureTypeInfo info = CreatureTypeInfo.GetInfo(Type);

            return AbilityBonus(GetStat(SkillsList[skill])) + (info.IsClassSkill(skill) ? 3 : 0) + GetRacialSkillMod(skill);
        }
        
        private int GetRacialSkillMod(string skill)
        {
            int mod = 0;

            if (RacialMods != null)
            {
                Regex regVal = new Regex("((\\+|-)[0-9]+) " + skill);


                Match m = regVal.Match(RacialMods);

                if (m.Success)
                {
                    mod = int.Parse(m.Groups[1].Value);

                }
            }


            return mod;
        }

        private List<KeyValuePair<SkillValue, int>> GetSkillRanks()
        {

            List<KeyValuePair<SkillValue, int>> list = new List<KeyValuePair<SkillValue, int>>();


            foreach (SkillValue skillValue in SkillValueDictionary.Values)
            {
                try
                {
                    int val = skillValue.Mod - AbilityBonus(GetStat(SkillsList[skillValue.Name]));

                    list.Add(new KeyValuePair<SkillValue, int>(skillValue, val));
                }

                catch (KeyNotFoundException)
                {
                    System.Diagnostics.Debug.WriteLine(Name + " Skill not found: " +skillValue.Name);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                    throw;
                }
                
            }



            list.Sort((a, b) => b.Value - a.Value);

            return list;
        }



        public static AlignmentType ParseAlignment(string alignment)
        {
            AlignmentType type = new AlignmentType();

            if (alignment.Contains("G"))
            {
                type.Moral = MoralAxis.Good;
            }
            else if (alignment.Contains("E"))
            {
                type.Moral = MoralAxis.Evil;
            }
            else
            {
                type.Moral = MoralAxis.Neutral;
            }


            if (alignment.Contains("L"))
            {
                type.Order = OrderAxis.Lawful;
            }
            else if (alignment.Contains("C"))
            {
                type.Order = OrderAxis.Chaotic;
            }
            else
            {
                type.Order = OrderAxis.Neutral;
            }


            return type;
        }

        public static String AlignmentText(AlignmentType alignment)
        {
            if (alignment.Moral == MoralAxis.Neutral && alignment.Order == OrderAxis.Neutral)
            {
                return "N";
            }

            string text = "";

            switch (alignment.Order)
            {
                case OrderAxis.Lawful:
                    text += "L";
                    break;
                case OrderAxis.Neutral:
                    text += "N";
                    break;
                case OrderAxis.Chaotic:
                    text += "C";
                    break;
            }

            switch (alignment.Moral)
            {
                case MoralAxis.Good:
                    text += "G";
                    break;
                case MoralAxis.Neutral:
                    text += "N";
                    break;
                case MoralAxis.Evil:
                    text += "E";
                    break;
            }

            return text;

        }

        public static string CreatureTypeText(CreatureType type)
        {
            return creatureTypeNames[type];
        }

        public static CreatureType ParseCreatureType(string name)
        {
            if (!creatureTypes.ContainsKey(name.ToLower()))
            {
                System.Diagnostics.Debug.WriteLine("Unknow creature type.  Type: " + name);
                return creatureTypes[creatureTypeNames[CreatureType.Humanoid]];
            }
            return creatureTypes[name.ToLower()];
            
        }

        

  

        public void BaseClone(Monster s)
        {
            foreach (ActiveCondition c in ActiveConditions)
            {
                s.ActiveConditions.Add(new ActiveCondition(c));
            }

            foreach (Condition c in UsableConditions)
            {
                s.UsableConditions.Add(new Condition(c));
            }

            s.DexZero = DexZero;
        }

        public void AddCondition(ActiveCondition c)
        {
            if (_ActiveConditions == null)
            {
                _ActiveConditions = new ObservableCollection<ActiveCondition>();
            }
            _ActiveConditions.Add(c);

            if (c.Bonus != null)
            {
                ApplyBonus(c.Bonus, false);
            }

        }

        public void RemoveCondition(ActiveCondition c)
        {

            _ActiveConditions.Remove(c);

            if (c.Bonus != null)
            {
                ApplyBonus(c.Bonus, true);
            }
        }

        [DBLoaderIgnore]
        public ObservableCollection<ActiveCondition> ActiveConditions
        {
            get
            {
                if (_ActiveConditions == null)
                {
                    _ActiveConditions = new ObservableCollection<ActiveCondition>();
                    //_ActiveConditions.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(_ActiveConditions_CollectionChanged);
                }

                return _ActiveConditions;

            }
        }



        public void ApplyBonus(ConditionBonus bonus, bool remove)
        {
            if (bonus.Str != null && Strength != null)
            {
                AdjustStrength(remove ? -bonus.Str.Value : bonus.Str.Value);
            }
            if (bonus.Dex != null && Dexterity != null)
            {
                AdjustDexterity(remove ? -bonus.Dex.Value : bonus.Dex.Value);
            }
            if (bonus.Con != null && Constitution != null)
            {
                AdjustConstitution(remove ? -bonus.Con.Value : bonus.Con.Value);
            }
            if (bonus.Int != null && Intelligence != null)
            {
                AdjustIntelligence(remove ? -bonus.Int.Value : bonus.Int.Value);
            }
            if (bonus.Wis != null && Wisdom != null)
            {
                AdjustWisdom(remove ? -bonus.Wis.Value : bonus.Wis.Value);
            }
            if (bonus.Cha != null && Charisma != null)
            {
                AdjustCharisma(remove ? -bonus.Cha.Value : bonus.Cha.Value);
            }
            if (bonus.StrSkill != null)
            {
                ChangeSkillsForStat(Stat.Strength, remove ? -bonus.StrSkill.Value : bonus.StrSkill.Value);
            }
            if (bonus.DexSkill != null)
            {
                ChangeSkillsForStat(Stat.Dexterity, remove ? -bonus.DexSkill.Value : bonus.DexSkill.Value);
            }
            if (bonus.ConSkill != null)
            {
                ChangeSkillsForStat(Stat.Constitution, remove ? -bonus.ConSkill.Value : bonus.ConSkill.Value);
            }
            if (bonus.IntSkill != null)
            {
                ChangeSkillsForStat(Stat.Intelligence, remove ? -bonus.IntSkill.Value : bonus.IntSkill.Value);
            }
            if (bonus.WisSkill != null)
            {
                ChangeSkillsForStat(Stat.Wisdom, remove ? -bonus.WisSkill.Value : bonus.WisSkill.Value);
            }
            if (bonus.ChaSkill != null)
            {
                ChangeSkillsForStat(Stat.Charisma, remove ? -bonus.ChaSkill.Value : bonus.ChaSkill.Value);
            }
            if (bonus.LoseDex)
            {
                LoseDexBonus = !remove;
            }
            if (bonus.Armor != null)
            {
                AdjustArmor(remove? -bonus.Armor.Value : bonus.Armor.Value);
            }
            if (bonus.Shield != null)
            {
                AdjustShield(remove ? -bonus.Shield.Value : bonus.Shield.Value);
            }
            if (bonus.Dodge != null)
            {
                AdjustDodge(remove ? -bonus.Dodge.Value : bonus.Dodge.Value);
            }
            if (bonus.NaturalArmor != null)
            {
                AdjustNaturalArmor(remove ? -bonus.NaturalArmor.Value : bonus.NaturalArmor.Value);
            }
            if (bonus.Deflection != null)
            {
                AdjustDeflection(remove ? -bonus.Deflection.Value : bonus.Deflection.Value);
            }
            if (bonus.AC != null)
            {
                int val = (remove ? -bonus.AC.Value : bonus.AC.Value);
                FullAC += val;
                TouchAC += val;
                FlatFootedAC += val;
                CMD_Numeric += val;
            }
            if (bonus.Initiative != null)
            {
                Init += remove ? -bonus.Initiative.Value : bonus.Initiative.Value;
            }
            if (bonus.AllAttack != null)
            {
                Melee = ChangeAttackMods(Melee, remove ? -bonus.AllAttack.Value : bonus.AllAttack.Value);
                Ranged = ChangeAttackMods(Ranged, remove ? -bonus.AllAttack.Value : bonus.AllAttack.Value);
            }
            if (bonus.MeleeAttack != null)
            {
                Melee = ChangeAttackMods(Melee, remove ? -bonus.MeleeAttack.Value : bonus.MeleeAttack.Value);
            }
            if (bonus.RangedAttack != null)
            {
                Ranged = ChangeAttackMods(Ranged, remove ? -bonus.RangedAttack.Value : bonus.RangedAttack.Value);
            }
            if (bonus.AttackDamage != null)
            {
                int val = remove ? -bonus.AttackDamage.Value : bonus.AttackDamage.Value;
                Melee = ChangeAttackDamage(Melee, val, val, val);
                Ranged = ChangeAttackDamage(Ranged, val, val, val);
            }
            if (bonus.MeleeDamage != null)
            {
                int val = remove ? -bonus.AttackDamage.Value : bonus.AttackDamage.Value;
                Melee = ChangeAttackDamage(Melee, val, val, val);
            }
            if (bonus.RangedDamage != null)
            {
                int val = remove ? -bonus.AttackDamage.Value : bonus.AttackDamage.Value;
                Ranged = ChangeAttackDamage(Ranged, val, val, val);
            }
            if (bonus.Fort != null)
            {
                Fort += remove ? -bonus.Fort.Value : bonus.Fort.Value;
            }
            if (bonus.Ref != null)
            {
                Ref += remove ? -bonus.Ref.Value : bonus.Ref.Value;
            }
            if (bonus.Will != null)
            {
                Will += remove ? -bonus.Will.Value : bonus.Will.Value;
            }
            if (bonus.Perception != null)
            {
                AddOrChangeSkill("Perception", remove ? -bonus.Perception.Value : bonus.Perception.Value);
            }
            if (bonus.AllSaves != null)
            {
                Fort += remove ? -bonus.AllSaves.Value : bonus.AllSaves.Value;
                Ref += remove ? -bonus.AllSaves.Value : bonus.AllSaves.Value;
                Will += remove ? -bonus.AllSaves.Value : bonus.AllSaves.Value;
            }
            if (bonus.AllSkills != null)
            {
                foreach (Stat stat in Enum.GetValues(typeof(Stat)))
                {
                    ChangeSkillsForStat(stat, remove ? -bonus.AllSkills.Value : bonus.AllSkills.Value);
                }
            }
            if (bonus.Size != null)
            {
                AdjustSize(remove ? -bonus.Size.Value : bonus.Size.Value);
            }
            if (bonus.CMB != null)
            {
                CMB_Numeric += remove ? -bonus.CMB.Value : bonus.CMB.Value;
            }
            if (bonus.CMD != null)
            {
                CMD_Numeric += remove ? -bonus.CMD.Value : bonus.CMD.Value;
            }
            if (bonus.DexZero)
            {
                DexZero = !remove;
            }
            if (bonus.StrZero)
            {
                StrZero = !remove;
            }
        }

        [DBLoaderIgnore]
        public ObservableCollection<Condition> UsableConditions
        {
            get
            {
                if (_UsableConditions == null)
                {
                    _UsableConditions = new ObservableCollection<Condition>();
                }

                return _UsableConditions;

            }
            set
            {
                if (_UsableConditions != value)
                {
                    _UsableConditions = value;
                    NotifyPropertyChanged("UsableConditions");
                    

                }
            }
        }



        [DataMember]
        public bool LoseDexBonus
        {
            get
            {
                return _LoseDexBonus;
            }
            set
            {
                if (_LoseDexBonus != value)
                {

                    _LoseDexBonus = value;

                    NotifyPropertyChanged("LoseDexBonus");
                }
            }
        }


        [DataMember]
        public bool DexZero
        {
            get
            {
                return _DexZero;
            }
            set
            {
                if (_DexZero != value)
                {

                    if (value)
                    {
                        _PreLossDex = Dexterity;
                        if (Dexterity != null)
                        {
                            AdjustDexterity(-Dexterity.Value);
                        }
                        _DexZero = true;
                    }
                    else
                    {
                        _DexZero = false;
                        if (_PreLossDex != null)
                        {
                            AdjustDexterity(_PreLossDex.Value);
                        }
                    }

                    NotifyPropertyChanged("DexZero");
                }
            }
        }


        [DataMember]
        public bool StrZero
        {
            get
            {
                return _StrZero;
            }
            set
            {
                if (_StrZero != value)
                {

                    if (value)
                    {
                        _PreLossStr = Strength;
                        if (Strength != null)
                        {
                            AdjustStrength(-Strength.Value);
                        }
                        _StrZero = true;
                    }
                    else
                    {
                        _StrZero = false;
                        if (_PreLossStr != null)
                        {
                            AdjustStrength(_PreLossStr.Value);
                        }
                    }

                    NotifyPropertyChanged("StrZero");
                }
            }
        }


        [DataMember]
        public int? PreLossDex
        {
            get { return _PreLossDex; }
            set
            {
                if (_PreLossDex != value)
                {
                    _PreLossDex = value;
                }
            }
        }



        [DataMember]
        public int? PreLossStr
        {
            get { return _PreLossStr; }
            set
            {
                if (_PreLossStr != value)
                {
                    _PreLossStr = value;
                }
            }
        }



        [XmlIgnore]
        protected DieRoll HDRoll
        {
            get
            {
                return FindNextDieRoll(HD, 0);
            }
            set
            {
                HD = ReplaceDieRoll(HD, HDRoll, 0);
            }
        }

        public static Stat StatFromName(string name)
        {
            Stat stat = Stat.Strength;

            if (String.Compare("Strength", name, true) == 0)
            {
                stat =  Stat.Strength;
            }
            else if (String.Compare("Dexterity", name, true) == 0)
            {
                stat =  Stat.Dexterity;
            }
            else if (String.Compare("Constitution", name, true) == 0)
            {
                stat =  Stat.Constitution;
            }
            else if (String.Compare("Intelligence", name, true) == 0)
            {
                stat =  Stat.Intelligence;
            }
            else if (String.Compare("Wisdom", name, true) == 0)
            {
                stat =  Stat.Wisdom;
            }
            else if (String.Compare("Charisma", name, true) == 0)
            {
                stat =  Stat.Charisma;
            }
        
            return stat;
        }

        public static string StatText(Stat stat)
        {
            string text = null;

            switch (stat)
            {
                case Stat.Strength:
                    text = "Strength";
                    break;
                case Stat.Dexterity:
                    text = "Dexterity";
                    break;
                case Stat.Constitution:
                    text = "Constitution";
                    break;
                case Stat.Intelligence:
                    text = "Intelligence";
                    break;
                case Stat.Wisdom:
                    text = "Wisdom";
                    break;
                case Stat.Charisma:
                    text = "Charisma";
                    break;
            }

            return text;
        }
		
		public static string ShortStatText(Stat stat)
        {
            string text = null;

            switch (stat)
            {
                case Stat.Strength:
                    text = "Str";
                    break;
                case Stat.Dexterity:
                    text = "Dex";
                    break;
                case Stat.Constitution:
                    text = "Con";
                    break;
                case Stat.Intelligence:
                    text = "Int";
                    break;
                case Stat.Wisdom:
                    text = "Wis";
                    break;
                case Stat.Charisma:
                    text = "Cha";
                    break;
            }

            return text;
        }


        public int? GetStat(Stat stat)
        {
            switch (stat)
            {
                case Stat.Strength:
                    return Strength;
                case Stat.Dexterity:
                    return Dexterity;
                case Stat.Constitution:
                    return Constitution;
                case Stat.Intelligence:
                    return Intelligence;
                case Stat.Wisdom:
                    return Wisdom;
                case Stat.Charisma:
                    return Charisma;
            }

            return null;

        }


        public static int AbilityBonus(int? score)
        {
            if (score == null)
            {
                return 0;
            }

            return (score.Value/2) - 5;
        }

        public void AdjustStat(Stat stat, int value)
        {
            switch (stat)
            {
                case Stat.Strength:
                    AdjustStrength(value);
                    break;
                case Stat.Dexterity:
                    AdjustDexterity(value);
                    break;
                case Stat.Constitution:
                    AdjustConstitution(value);
                    break;
                case Stat.Intelligence:
                    AdjustIntelligence(value);
                    break;
                case Stat.Wisdom:
                    AdjustWisdom(value);
                    break;
                case Stat.Charisma:
                    AdjustCharisma(value);
                    break;
            }

        }

        
        public static Dictionary<string, Stat> SkillsList 
        { 
            get
            {
                return _SkillsList;
            }
        }


        public static Dictionary<string, SkillInfo> SkillsDetails
        {
            get
            {
                return _SkillsDetails;
            }
        }

        private const string DieRollRegexString = "([0-9]+)(/[0-9]+)?d([0-9]+)(?<extra>(\\+([0-9]+)d([0-9]+))*)((\\+|-)[0-9]+)?";


        public static DieRoll FindNextDieRoll(string text)
        {
            return FindNextDieRoll(text, 0);
        }
        

        public static DieRoll FindNextDieRoll(string text, int start)
        {
            return DieRoll.FromString(text, start);
        }

        public static string ReplaceDieRoll(string text, DieRoll roll, int start)
        {
            int end;

            return ReplaceDieRoll(text, roll, start, out end);
        }

        public static string ReplaceDieRoll(string text, DieRoll roll, int start, out int end)
        {
            string returnText = text;

            end = -1;

            Regex regRoll = new Regex(DieRollRegexString);

            Match match = regRoll.Match(text, start);

            if (match.Success)
            {
                String dieText = DieRollText(roll);

                returnText = regRoll.Replace(returnText, dieText, 1, start);

                end = match.Index + dieText.Length;
            }

            return returnText;
        }

        public static string DieRollText(DieRoll roll)
        {
            if (roll == null)
            {
                return "0d0";
            }
            return roll.Text;
        }

        public bool AddFeat(string feat)
        {

            bool added = false;
            if (!FeatsList.Contains(feat))
            {
                FeatsList.Add(feat);
                added = true;
            }

            //add feats

            if (added)
            {
                ApplyFeatChanges(feat, true);

                FeatsList.Sort();

            }

            return added;
        }



        public void AdjustSize(int diff)
        {
            MonsterSize sizeOld = SizeMods.GetSize(Size);
            MonsterSize sizeNew = SizeMods.ChangeSize(sizeOld, diff);

            SizeMods oldMods = SizeMods.GetMods(sizeOld);
            SizeMods newMods = SizeMods.GetMods(sizeNew);

            //change size text
            Size = newMods.Name;

            //change skills
            ChangeSkill("Fly", newMods.Fly - oldMods.Fly);
            ChangeSkill("Stealth", newMods.Stealth - oldMods.Stealth);


            //adjust CMB
            CMB_Numeric += newMods.Combat - oldMods.Combat;

            //adjust CMD
            CMD_Numeric += newMods.Combat - oldMods.Combat;

            //adjust attacks
            int attackDiff = newMods.Attack - oldMods.Attack;
            if (Melee != null && Melee.Length > 0)
            {
                Melee = ChangeAttackMods(Melee, attackDiff);
                Melee = ChangeAttackDieStep(Melee, diff);
            }

            if (Ranged != null && Ranged.Length > 0)
            {
                Ranged = ChangeAttackMods(Ranged, attackDiff);
                Ranged = ChangeAttackDieStep(Ranged, diff);
            }

            //adjust size AC modfiier
            AdjustSizeACModifier(attackDiff);



            //adjust reach
            if (Reach != null && Reach.Length > 0)
            {
                Reach = ChangeReachForSize(Reach, sizeOld, sizeNew, diff);
            }


            //adjust space
            Regex regSwarm = new Regex("swarm");
            if (SubType == null || !regSwarm.Match(SubType).Success)
            {
                Space = newMods.Space;
            }
        }


        protected string ChangeAttackMods(string text, int diff)
        {
            if (text == null)
            {
                return null;
            }
            string returnText = text;

            //find mods 
            Regex regAttack = new Regex(Attack.RegexString(null));

            returnText = regAttack.Replace(returnText, delegate(Match m)
            {
                Attack info = Attack.ParseAttack(m);
                //get mod
                for (int i = 0; i < info.Bonus.Count; i++)
                {
                    info.Bonus[i] += diff;
                }

                return info.Text;

            });


            return returnText;
        }

        private static string ChangeAttackDieStep(string text, int diff)
        {
            if (text == null)
            {
                return null;
            }

            string returnText = text;

            Regex regAttack = new Regex(Attack.RegexString(null));

            returnText = regAttack.Replace(returnText, delegate(Match m)
            {
                Attack info = Attack.ParseAttack(m);


                info.Damage = DieRoll.StepDie(info.Damage, diff);

                return info.Text;

            });

            return returnText;

        }

        public string ChangeAttackDamage(string text, int diff, int diffPlusHalf, int halfDiff)
        {

            if (text == null)
            {
                return null;
            }

            string returnText = text;

            Regex regAttack = new Regex(Attack.RegexString(null));

            returnText = regAttack.Replace(returnText, delegate(Match m)
            {
                Attack info = Attack.ParseAttack(m);

                if (!info.AltDamage)
                {
                    int applyDiff = diff;

                    if (info.Weapon != null)
                    {
                        if (info.Weapon.Class == "Natual" && info.Weapon.Light)
                        {
                            applyDiff = halfDiff;
                        }

                        else if (info.Weapon.Hands == "Two-Handed")
                        {
                            applyDiff = diffPlusHalf;
                        }
                    }


                    info.Damage.mod += applyDiff;

                    if (info.OffHandDamage != null)
                    {
                        if (HasFeat("Double Slice") || HasSpecialAbility("Superior Two-Weapon Fighting"))
                        {
                            info.OffHandDamage.mod += applyDiff;
                        }
                        else
                        {
                            info.OffHandDamage.mod += halfDiff;
                        }

                    }

                }

                return info.Text;

            });

            return returnText;
        }


        private static string ChangeReachForSize(string reach, MonsterSize sizeOld, MonsterSize sizeNew, int diff)
        {

            int units = 0;


            if (sizeNew == MonsterSize.Tiny ||
               sizeNew == MonsterSize.Diminutive ||
               sizeNew == MonsterSize.Fine)
            {
                units = 0;
            }
            else if ((int)sizeOld < (int)MonsterSize.Small)
            {

                if (sizeNew == MonsterSize.Small)
                {
                    units = 1;
                }
                else
                {
                    units = ((int)sizeNew) - (int)MonsterSize.Small;
                }
            }
            else
            {


                Regex numReg = new Regex("[0-9]+");

                Match match = numReg.Match(reach);

                units = int.Parse(match.Value) / 5 + diff;


                if (sizeNew == MonsterSize.Small)
                {
                    units++;
                }
                else if (sizeOld == MonsterSize.Small)
                {
                    units--;
                }
            }


            int reachInt = units * 5;
            return String.Format("{0} ft.", reachInt);

        }

        public bool RemoveFeat(string feat)
        {
            bool removed = false;
            if (FeatsList.Contains(feat))
            {
                FeatsList.Remove(feat);
                removed = true;
            }

            //add feats

            if (removed)
            {
                ApplyFeatChanges(feat, false);

                FeatsList.Sort();

            }

            return removed;
        }



        public string MeleeString(CharacterAttacks attacks)
        {
            string text = "";

            
            //find combat feats
            CombatFeats cf = GetCombatFeats();

            int offHandAttacks = 1 + (cf.improvedTwoWeaponFighting ? 1 : 0) + (cf.greaterTwoWeaponFighting ? 1 : 0);


            bool firstSet = true;

            List<List<WeaponItem>> setList = new List<List<WeaponItem>>();

            foreach (List<WeaponItem> list in attacks.MeleeWeaponSets)
            {
                if (list.Count > 0)
                {
                    setList.Add(list);
                }
            }

            


            if (attacks.NaturalAttacks.Count > 0)
            {
                setList.Add(new List<WeaponItem>());
            }



            foreach (List<WeaponItem> set in setList)
            {
                List<Attack> attackSet = new List<Attack>();

                //determine if we are have multiple attacks
                bool hasOff = false;
                bool hasHeavyOff = false;
                foreach (WeaponItem item in set)
                {
                    if (item.Weapon.Hands == "Double" || !item.MainHand)
                    {
                        hasOff = true;
                    }
                    if (!item.MainHand && !item.Weapon.Light)
                    {
                        //ignore special cases
                        if (!(string.Compare(item.Name, "Whip") == 0 && cf.whipMastery))
                        {
                            hasHeavyOff = true;
                            break;
                        }
                    }
                }

                int handsUsed = 0;
                foreach (WeaponItem item in set)
                {

                    //find feats for atttack
                    AttackFeats af = GetAttackFeats(item);

                    //create attack
                    Attack attack = StartAttackFromItem(item, af, cf);

                    //get hand related bonus
                    int handMod = 0;
                    int offHandMod = 0;
                    GetHandMods(hasOff, hasHeavyOff, item, cf, out handMod, out offHandMod);                   

                    //add bonuses
                    attack.Bonus = new List<int>();
                    int baseBonus = BaseAtk;
                    bool firstBonus = true;
                    int count = 0;
                    while ((baseBonus > 0 || firstBonus) && (item.MainHand || count < offHandAttacks || cf.superiorTwoWeaponFighting))
                    {
                        attack.Bonus.Add(AttackBonus(baseBonus, handMod, item, cf, af));

                        if (item.Weapon.Double)
                        {
                            if (cf.superiorTwoWeaponFighting || count < offHandAttacks)
                            {
                                attack.Bonus.Add(AttackBonus(baseBonus, offHandMod, item, cf, af));
                            }
                            
                        }

                        if (firstBonus && item.HasSpecialAbility("speed"))
                        {
                            attack.Bonus.Add(AttackBonus(baseBonus, 0, item, cf, af));
                        }
                        
                        baseBonus -= 5;
                        firstBonus = false;
                        count++;

                    }

                    //set damage
                    SetAttackDamageMod(attack, item, af, cf, false, false);

                    //add to set
                    attackSet.Add(attack);

                    //add hands
                    handsUsed += item.Weapon.HandsUsed;
                }

                bool hasWeaponAttack = (handsUsed > 0);

                int handsToGive = (attacks.Hands - handsUsed);

                foreach (WeaponItem item in attacks.NaturalAttacks)
                {
                    bool useAttack = true;
                    int useCount = item.Count;

                    //skip attack if hand full
                    if (item.Weapon.IsHand && handsUsed > 0)
                    {

                        useAttack = false;
                        handsUsed -= item.Count;

                        if (handsToGive > 0)
                        {
                            handsToGive -= item.Count;
                            useAttack = true;

                            if (handsToGive < 0)
                            {
                                useCount = item.Count + handsToGive;
                            }
                        }

                    }

                    if (useAttack)
                    {
                        //find feats for atttack
                        AttackFeats af = GetAttackFeats(item);

                        //create attack
                        Attack attack = StartAttackFromItem(item, af, cf);
                        
                        //set count
                        attack.Count = useCount;

                        //get hand related bonus
                        int handMod = 0;
                        if (hasWeaponAttack)
                        {
                            if (cf.multiAttack)
                            {
                                handMod = -2;
                            }
                            else
                            {
                                handMod = -5;
                            }
                        }
                        else
                        {
                            if (item.Weapon.Light && (attacks.NaturalAttacks.Count > 1))
                            {
                                if (cf.multiAttack)
                                {
                                    handMod = -2;
                                }
                                else
                                {
                                    handMod = -5;
                                }
                            }
                        }


                        attack.Bonus = new List<int>();
                        int baseBonus = BaseAtk;

                        attack.Bonus.Add(AttackBonus(baseBonus, handMod, item, cf, af));

                        //set damage
                        SetAttackDamageMod(attack, item, af, cf, !((attacks.NaturalAttacks.Count > 1)||(item.Count > 1)) && set.Count == 0, set.Count > 0);

                        //add to set
                        attackSet.Add(attack);
                    }
                }

                if (attackSet.Count > 0)
                {
                    //add text to string
                    if (!firstSet)
                    {
                        text += " or ";
                    }

                    bool firstAttack = true;
                    foreach (Attack atk in attackSet)
                    {
                        if (firstAttack)
                        {
                            firstAttack = false;
                        }
                        else
                        {
                            text += ", ";
                        }

                        text += atk.Text;
                    }

                    firstSet = false;
                }
            }

            return text;
        }

        public string RangedString(CharacterAttacks attacks)
        {
            string text = null;


            if (attacks.RangedWeapons != null && attacks.RangedWeapons.Count > 0)
            {
                
                //find combat feats
                CombatFeats cf = GetCombatFeats();

                List<Attack> list = new List<Attack>();



                foreach (WeaponItem item in attacks.RangedWeapons)
                {
                    AttackFeats af = GetAttackFeats(item);


                    Attack attack = StartAttackFromItem(item, af, cf);


                    attack.Bonus = new List<int>();
                    int baseBonus = BaseAtk;
                    bool firstBonus = true;
                    int count = 0;
                    while ((!item.Weapon.Throw && baseBonus > 0) || firstBonus)
                    {
                        attack.Bonus.Add(AttackBonus(baseBonus, 0, item, cf, af));

                        if (firstBonus && item.HasSpecialAbility("speed"))
                        {                            
                            attack.Bonus.Add(AttackBonus(baseBonus, 0, item, cf, af));
                        }

                        baseBonus -= 5;
                        firstBonus = false;
                        count++;

                    }

                    SetRangedAttackDamageMod(attack, item, af, cf);


                    list.Add(attack);
                }


                text = "";
                bool firstAttack = true;
                foreach (Attack atk in list)
                {
                    if (firstAttack)
                    {
                        firstAttack = false;
                    }
                    else
                    {
                        text += " or ";
                    }

                    text += atk.Text;
                }

                
            }

            return text;
        }




        private struct AttackFeats
        {
            public bool weaponFocus;
            public bool weaponSpecialization;
            public bool greaterWeaponFocus;
            public bool greaterWeaponSpecialization;
            public bool improvedCritical;
            public bool improvedNaturalAttack;
            public int weaponTraining;
        }


        private AttackFeats GetAttackFeats(WeaponItem item)
        {
            AttackFeats af = new AttackFeats();

            string plural = item.Name + "s";

            if (item.Weapon.Plural != null && item.Weapon.Plural.Length > 0)
            {
                plural = item.Weapon.Plural;
            }

            af.weaponFocus = HasFeat("Weapon Focus", item.Weapon.Name) || HasFeat("Weapon Focus", plural);
            af.weaponSpecialization = HasFeat("Weapon Specialization", item.Weapon.Name) || HasFeat("Weapon Specialization", plural);
            af.greaterWeaponFocus = HasFeat("Greater Weapon Focus", item.Weapon.Name) || HasFeat("Greater Weapon Focus", plural);
            af.greaterWeaponSpecialization = HasFeat("Greater Weapon Specialization", item.Weapon.Name) || HasFeat("Greater Weapon Specialization", plural);
            af.improvedCritical = HasFeat("Improved Critical", item.Weapon.Name) || HasFeat("Improved Critical", plural);
            af.improvedNaturalAttack = HasFeat("Improved Natural Attack", item.Weapon.Name) || HasFeat("Improved Natural Attack", plural);
            if (item.Weapon.Natural)
            {
                af.weaponTraining = HasWeaponTraining("natural");
            }
            else
            {
                if (item.Weapon.Groups != null)
                {
                    foreach (string group in item.Weapon.Groups)
                    {
                        af.weaponTraining = Math.Max(af.weaponTraining, HasWeaponTraining(group));
                    }
                }
            }

            return af;
        }
        private struct CombatFeats
        {
            public bool multiweaponFighting;
            public bool twoWeaponFighting;
            public bool improvedTwoWeaponFighting;
            public bool greaterTwoWeaponFighting;
            public bool superiorTwoWeaponFighting;
            public bool doubleSlice;
            public bool multiAttack;
            public bool weaponFinesse;
            public bool whipMastery;
            public bool savageBite;
            public bool powerfulBite;
            public bool rockThrowing;
            public bool isDragon;
        }

        private CombatFeats GetCombatFeats()
        {
            CombatFeats cf = new CombatFeats();

            cf.multiweaponFighting = HasFeat("Multiweapon Fighting");
            cf.twoWeaponFighting = HasFeat("Two-Weapon Fighting");
            cf.improvedTwoWeaponFighting = HasFeat("Improved Two-Weapon Fighting");
            cf.greaterTwoWeaponFighting = HasFeat("Greater Two-Weapon Fighting");
            cf.superiorTwoWeaponFighting = HasSpecialAbility("Superior Two-Weapon Fighting");
            cf.doubleSlice = HasFeat("Double Slice");
            cf.multiAttack = HasFeat("Multiattack");
            cf.weaponFinesse = HasFeat("Weapon Finesse");
            cf.whipMastery = HasSQ("Whip Mastery");
            cf.savageBite = HasSpecialAbility("Savage Bite");
            cf.powerfulBite = HasSQ("powerful bite");
            cf.rockThrowing = HasSpecialAttack("Rock Throwing");
            if (Name != null)
            {
                cf.isDragon = new Regex(DragonRegexString, RegexOptions.IgnoreCase).Match(Name).Success;
            }
            return cf;
        }

        private static string DragonRegexString
        {
            get
            {
                return "((Blue)|(Black)|(Green)|(Red)|(White)|(Brass)|(Bronze)|(Copper)|(Gold)|(Silver)) Dragon";
            }
        }



        private Attack StartAttackFromItem(WeaponItem item, AttackFeats af, CombatFeats cf)
        {
            Attack attack = new Attack();
            attack.Weapon = item.Weapon;

            attack.CritMultiplier = item.Broken?2:item.Weapon.CritMultiplier;
            attack.CritRange = item.Broken?20:item.Weapon.CritRange;


            if (String.Compare(attack.Name, "Bite", true) == 0 && cf.savageBite)
            {
                attack.CritRange -=1;
            }

            if (af.improvedCritical || item.SpecialAbilitySet.ContainsKey("keen"))
            {
                attack.CritRange = 20 - (20 - attack.CritRange) * 2 - 1;
            }

            attack.Name = item.Name.ToLower();

            attack.MagicBonus = item.MagicBonus;
            attack.Masterwork = item.Masterwork;
            attack.Broken = item.Broken;
            attack.SpecialAbilities = item.SpecialAbilities;
            attack.Plus = item.PlusText;

            attack.RangedTouch = item.Weapon.RangedTouch;
            attack.AltDamage = item.Weapon.AltDamage;
            attack.AltDamageStat = item.Weapon.AltDamageStat;
            attack.AltDamageDrain = item.Weapon.AltDamageDrain;


            SetAttackDamageDie(item, attack, af, cf);

            return attack;
        }

        private void SetAttackDamageDie(WeaponItem item, Attack attack, AttackFeats af, CombatFeats cf)
        {
            MonsterSize size = SizeMods.GetSize(Size);

            attack.Damage = FindNextDieRoll(attack.Weapon.DmgM, 0);
            attack.Count = item.Count;

            if (item.Step == null)
            {

                if (attack.Damage != null)
                {
                    attack.Damage = DieRoll.StepDie(attack.Damage, ((int)size) - (int)MonsterSize.Medium);
                }
                else
                {
                    attack.Damage = new DieRoll(0, 0, 0);
                }

                if (af.improvedNaturalAttack)
                {
                    attack.Damage = DieRoll.StepDie(attack.Damage, 1);
                }

            }
            else
            {
                attack.Damage.Step = item.Step;
            }
        }

        private int WeaponStrengthDamageBonus(Attack attack, WeaponItem item, AttackFeats af, CombatFeats cf, bool onlyNatural, bool makeSecondary)
        {

            int strDamageBonus = AbilityBonus(Strength);

            if (cf.powerfulBite && String.Compare(attack.Name, "Bite", true) == 0 && !makeSecondary)
            {
                strDamageBonus = AbilityBonus(Strength) * 2;
            }
            else if (((cf.savageBite || cf.isDragon) && String.Compare(attack.Name, "Bite", true) == 0) ||
                (cf.isDragon && ((String.Compare(attack.Name, "Tail", true) == 0) || (String.Compare(attack.Name, "Tail Slap", true) == 0))) ||
                (attack.Weapon.Hands == "Two-Handed") || (onlyNatural && AbilityBonus(Strength) > 0) && !makeSecondary)
            {

                strDamageBonus += AbilityBonus(Strength) / 2;
            }
            else if (attack.Weapon.Light && !onlyNatural && !item.MainHand || makeSecondary)
            {
                strDamageBonus = AbilityBonus(Strength) / 2;
            }

            return strDamageBonus;
        }

        private int MeleeAbilityAttackBonus(WeaponItem item, CombatFeats cf)
        {
            int abilityAttackBonus = AbilityBonus(Strength);

            if (cf.weaponFinesse && item.Weapon.WeaponFinesse)
            {
                abilityAttackBonus = Math.Max(abilityAttackBonus, AbilityBonus(Dexterity));
            }

            return abilityAttackBonus;

        }

        private int RangedAbilityAttackBonus(WeaponItem item, CombatFeats cf)
        {
            int abilityAttackBonus = AbilityBonus(Dexterity);

            return abilityAttackBonus;

        }

        private void SetAttackDamageMod(Attack attack, WeaponItem item, AttackFeats af, CombatFeats cf, bool onlyNatural, bool makeSecondary)
        {

            int strDamageBonus;


            
            strDamageBonus = WeaponStrengthDamageBonus(attack, item, af, cf, onlyNatural, makeSecondary);
            

            attack.Damage.mod = strDamageBonus + WeaponSpecialBonus(attack, af);

            if (item.Weapon.Double)
            {
                attack.OffHandDamage = (DieRoll)attack.Damage.Clone();

                if (!cf.doubleSlice && !cf.superiorTwoWeaponFighting)
                {
                    attack.OffHandDamage.mod = WeaponStrengthDamageBonus(attack, item, af, cf, false, true);
                }
            }
        }

        
        private void SetRangedAttackDamageMod(Attack attack, WeaponItem item, AttackFeats af, CombatFeats cf)
        {
            int strDamageBonus = 0;

            if ((String.Compare(attack.Name, "Rock", true) == 0 && cf.rockThrowing))
            {
                strDamageBonus = AbilityBonus(Strength) + AbilityBonus(Strength) / 2;
            }
            else if (attack.Weapon.Throw || new Regex("composite", RegexOptions.IgnoreCase).Match(attack.Name).Success)
            {
                strDamageBonus = WeaponStrengthDamageBonus(attack, item, af, cf, false, false);
            }


            attack.Damage.mod = strDamageBonus + WeaponSpecialBonus(attack, af);
        }

        private int WeaponSpecialBonus(Attack attack, AttackFeats af)
        {
            return attack.MagicBonus + (attack.Broken ? -2 : 0) +
                (af.weaponSpecialization ? 2 : 0) + (af.greaterWeaponSpecialization ? 2 : 0) + af.weaponTraining;
        }


        private int AttackBonus(int baseBonus, int handMod, WeaponItem item, CombatFeats cf, AttackFeats af)
        {

            MonsterSize size = SizeMods.GetSize(Size);
            SizeMods mods = SizeMods.GetMods(size);

            int abilityAttackBonus = 0;

            if (item.Weapon.Ranged)
            {
                abilityAttackBonus = RangedAbilityAttackBonus(item, cf);
            }
            else
            {
                abilityAttackBonus = MeleeAbilityAttackBonus(item, cf);
            }

            return baseBonus + abilityAttackBonus + handMod + mods.Attack +
                            (item.Masterwork ? 1 : 0) + item.MagicBonus + (item.Broken ? -2 : 0) + 
                            (af.weaponFocus ? 1 : 0) + (af.greaterWeaponFocus ? 1 : 0) + af.weaponTraining;
                            
        }


        void GetHandMods(bool hasOff, bool hasHeavyOff, WeaponItem item, CombatFeats cf, out int handMod, out int offHandMod)
        {
            handMod = 0;
            offHandMod = 0;

            if (hasOff && !cf.superiorTwoWeaponFighting)
            {

                offHandMod = -8;
                if (hasHeavyOff)
                {
                    offHandMod -= 2;
                }
                if (cf.twoWeaponFighting || cf.multiweaponFighting)
                {
                    offHandMod += 6;
                }

                if (item.MainHand)
                {
                    handMod = -4;
                    if (hasHeavyOff)
                    {
                        handMod -= 2;
                    }
                    if (cf.twoWeaponFighting || cf.multiweaponFighting)
                    {
                        handMod += 2;
                    }
                }
                else
                {
                    handMod = offHandMod;
                }
            }
        }




        public bool HasFeat(string feat)
        {
            return HasFeat(feat, null);
        }



        public bool HasFeat(string feat, string subtype)
        {
            string text = feat;
            if (subtype != null)
            {
                text = text + " (" + subtype + ")";
            }

            return FeatsList.Contains(text, new InsensitiveEqualityCompararer());
        }


        public bool HasSQ(string quality)
        {
            if (SQ == null)
            {
                return false;
            }

            return new Regex(Regex.Escape(quality), RegexOptions.IgnoreCase).Match(SQ).Success;
        }

        public void AddWeaponTraining(string group, int val)
        {
            if (SQ == null || SQ.Length == 0)
            {
                SQ = "weapon training (" + group.ToLower() + " +" + val + ")";
            }
            else
            {
                Regex regWT = new Regex("(?<start>, )?Weapon Training \\((?<values>([ a-zA-Z]+ \\+[0-9]+,?)+)\\)");

                bool foundWeaponTraining = false;

                SQ = regWT.Replace(SQ, delegate(Match m)
                {
                    foundWeaponTraining = true;

                    string retString = "";
                    
                    if (m.Groups["start"].Success)
                    {
                        retString += m.Groups["start"].Value;
                    }

                    Regex regValues = new Regex("(?<name>[ a-zA-Z]+ \\+(?<val>[0-9]+)");

                    bool weaponFound = false;
                    retString += "weapon training (" + regValues.Replace(m.Groups["values"].Value, delegate(Match ma)
                    {
                        string valueString = null;

                        if (ma.Groups["name"].Value.Trim().ToLower() == group.ToLower())
                        {
                            weaponFound = true;

                            valueString = ma.Groups["name"] + " +" + Math.Max(int.Parse(ma.Groups["val"].Value), val);
                        }
                        else
                        {
                            valueString = ma.Value;
                        }

                        return valueString;

                    });

                    if (!weaponFound)
                    {
                        retString += ", " + group.ToLower();
                    }

                    retString += ")";
                        


                    return retString;
                }, 1);

                

                if (!foundWeaponTraining)
                {
                    SQ += ", weapon training (" + group.ToLower() + " +" + val + ")";
                }


            }


        }

        public int HasWeaponTraining(string group)
        {
            int val = 0;

            if (SQ != null && SQ.Length > 0)
            {
                Regex regWT = new Regex("(?<start>, )?weapon training \\((?<values>([ a-zA-Z]+ \\+[0-9]+,?)+)\\)", RegexOptions.IgnoreCase);

                Match m = regWT.Match(SQ);

                if (m.Success)
                {
                    Regex regValues = new Regex(Regex.Escape(group) + " \\+(?<val>[0-9]+)", RegexOptions.IgnoreCase);


                    m = regValues.Match(m.Groups["values"].Value);

                    if (m.Success)
                    {
                        val = int.Parse(m.Groups["val"].Value);
                    }
                    
                }
            }

            return val;
        }

        public bool HasSpecialAttack(string name)
        {
            if (SpecialAttacks == null)
            {
                return false;
            }

            return new Regex(Regex.Escape(name), RegexOptions.IgnoreCase).Match(SpecialAttacks).Success;
        }

        public bool HasSpecialAbility(string name)
        {
            if (SpecialAbilitiesList == null)
            {
                return false;
            }

            foreach (SpecialAbility a in SpecialAbilitiesList)
            {
                if (String.Compare(a.Name, name, true) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        public override string ToString()
        {
            return Name;
        }



        [XmlIgnore]
        public CreatureType CreatureType
        {
            get
            {
                return Monster.ParseCreatureType(Type);
            }
            set
            {
                Type = Monster.CreatureTypeText(value);
            }
        }

        [XmlIgnore]
        public List<AttackSet> MeleeAttacks
        {
            get
            {
                List<AttackSet> sets = new List<AttackSet>();

                if (Melee != null)
                {
                    Regex regOr = new Regex("\\) or ");

                    Regex regAttack = new Regex(Attack.RegexString(null));
                    int lastLoc = 0;

                    foreach (Match m in regOr.Matches(Melee))
                    {
                        AttackSet set = new AttackSet();
                        string text = Melee.Substring(lastLoc, m.Index - lastLoc + 1);

                        lastLoc = m.Index + m.Length;

                        foreach (Match a in regAttack.Matches(text))
                        {
                            Attack attack = Attack.ParseAttack(a);

                            if (attack.Weapon != null && attack.Weapon.Class != "Natural")
                            {
                                set.WeaponAttacks.Add(attack);
                            }
                            else
                            {
                                if (attack.Weapon == null)
                                {
                                    attack.Weapon = new Weapon(attack, false, SizeMods.GetSize(Size));

                                    if (attack.Weapon.Natural)
                                    {
                                        set.NaturalAttacks.Add(attack);
                                    }
                                    else
                                    {
                                        set.WeaponAttacks.Add(attack);
                                    }
                                }
                                else
                                {

                                    set.NaturalAttacks.Add(attack);
                                }
                            }

                        }

                        sets.Add(set);
                        
                    }

                    string lastText = Melee.Substring(lastLoc);


                    AttackSet newSet = new AttackSet();

                    foreach (Match a in regAttack.Matches(lastText))
                    {
                        Attack attack = Attack.ParseAttack(a);

                        if (attack.Weapon != null && attack.Weapon.Class != "Natural")
                        {
                            newSet.WeaponAttacks.Add(attack);
                        }
                        else
                        {
                            if (attack.Weapon == null)
                            {
                                attack.Weapon = new Weapon(attack, false, SizeMods.GetSize(Size));
                            }

                            newSet.NaturalAttacks.Add(attack);
                        }
                    }

                    sets.Add(newSet);


                }

                return sets;
            }
        }

        [XmlIgnore]
        public List<Attack> RangedAttacks
        {
            get
            {
                List<Attack> attacks = new List<Attack>();

                Regex regAttack = new Regex(Attack.RegexString(null));

                if (Ranged != null)
                {
                    foreach (Match m in regAttack.Matches(Ranged))
                    {
                        Attack attack = Attack.ParseAttack(m);

                        if (attack.Weapon == null)
                        {
                            attack.Weapon = new Weapon(attack, true, SizeMods.GetSize(Size));
                        }

                        attacks.Add(attack);

                    }
                }

                return attacks;
            }
        }

        public void ChangeSkillsForStat(Stat stat, int diff)
        {

            foreach (SkillValue skill in SkillValueDictionary.Values)
            {
                Stat skillStat;
                if (SkillsList.TryGetValue(skill.Name, out skillStat))
                {
                    if (skillStat == stat)
                    {
                        skill.Mod += diff;

                        UpdateSkillFields(skill);

                    }
                }
            }

        }





        [DataMember]
        public String Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Name"));
                }
            }
        }

        [DataMember]
        public String CR
        {
            get
            {
                return cr;
            }
            set
            {
                cr = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("CR"));
                }
            }
        }

        [DataMember]
        public String XP
        {
            get
            {
                return xp;
            }
            set
            {
                xp = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("XP"));
                }
            }
        }
        public String Race
        {
            get
            {
                return race;
            }
            set
            {
                race = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Race"));
                }
            }
        }

        [DataMember]
        public String Class
        {
            get
            {
                return className;
            }
            set
            {
                className = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Class"));
                }
            }
        }

        [DataMember]
        public String Alignment
        {
            get
            {
                return alignment;
            }
            set
            {
                alignment = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Alignment"));
                }
            }
        }

        [DataMember]
        public String Size
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Size"));
                }
            }
        }

        [DataMember]
        public String Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Type"));
                }
            }
        }

        [DataMember]
        public String SubType
        {
            get
            {
                return subType;
            }
            set
            {
                subType = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SubType"));
                }
            }
        }

        [DataMember]
        public int Init
        {
            get
            {
                return init;
            }
            set
            {
                init = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Init"));
                }
            }
        }

        [DataMember]
        public String Senses
        {
            get
            {
                return senses;
            }
            set
            {
                senses = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Senses"));
                }
            }
        }

        [DataMember]
        public String AC
        {
            get
            {
                return ac;
            }
            set
            {
                ac = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("AC"));
                }
            }
        }

        [DataMember]
        public String AC_Mods
        {
            get
            {
                return ac_mods;
            }
            set
            {
                ac_mods = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("AC_Mods"));
                }
            }
        }

        [DataMember]
        public int HP
        {
            get
            {
                return hp;
            }
            set
            {
                hp = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("HP"));
                }
            }
        }

        [DataMember]
        public String HD
        {
            get
            {
                return hd;
            }
            set
            {
                hd = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("HD"));
                }
            }
        }

        [DataMember]
        public String Saves
        {
            get
            {
                return saves;
            }
            set
            {
                saves = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Saves"));
                }
            }
        }

        [DataMember]
        public int Fort
        {
            get
            {
                return fort;
            }
            set
            {
                fort = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Fort"));
                }
            }
        }

        [DataMember]
        public int Ref
        {
            get
            {
                return reflex;
            }
            set
            {
                reflex = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Ref"));
                }
            }
        }

        [DataMember]
        public int Will
        {
            get
            {
                return will;
            }
            set
            {
                will = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Will"));
                }
            }
        }

        [DataMember]
        public String Save_Mods
        {
            get
            {
                return save_mods;
            }
            set
            {
                save_mods = value;
                if (save_mods == "NULL")
                {
                    save_mods = null;
                }
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Save_Mods"));
                }
            }
        }

        [DataMember]
        public String Resist
        {
            get
            {
                return resist;
            }
            set
            {
                resist = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Resist"));
                }
            }
        }

        [DataMember]
        public String DR
        {
            get
            {
                return dr;
            }
            set
            {
                dr = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("DR"));
                }
            }
        }

        [DataMember]
        public String SR
        {
            get
            {
                return sr;
            }
            set
            {
                sr = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SR"));
                }
            }
        }

        [DataMember]
        public String Speed
        {
            get
            {
                return speed;
            }
            set
            {
                speed = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Speed"));
                }
            }
        }

        [DataMember]
        public String Melee
        {
            get
            {
                return melee;
            }
            set
            {
                melee = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Melee"));
                }
            }
        }

        [DataMember]
        public String Ranged
        {
            get
            {
                return ranged;
            }
            set
            {
                ranged = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Ranged"));
                }
            }
        }

        [DataMember]
        public String Space
        {
            get
            {
                return space;
            }
            set
            {
                space = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Space"));
                }
            }
        }

        [DataMember]
        public String Reach
        {
            get
            {
                return reach;
            }
            set
            {
                reach = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Reach"));
                }
            }
        }

        [DataMember]
        public String SpecialAttacks
        {
            get
            {
                return specialAttacks;
            }
            set
            {
                specialAttacks = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SpecialAttacks"));
                }
            }
        }

        [DataMember]
        public String SpellLikeAbilities
        {
            get
            {
                return spellLikeAbilities;
            }
            set
            {
                if (spellLikeAbilities != value)
                {
                    spellLikeAbilities = value;
                    _SpellLikeAbilitiesBlock = null;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("SpellLikeAbilities"));
                    }
                }
            }
        }

        [XmlIgnore]
        public ObservableCollection<SpellBlockInfo> SpellLikeAbilitiesBlock
        {
            get
            {
                ParseSpellLikeAbilities();
                return _SpellLikeAbilitiesBlock;
            }
        }

        public void ParseSpellLikeAbilities()
        {
            if (_SpellLikeAbilitiesBlock == null && spellLikeAbilities != null && spellLikeAbilities.Length > 0)
            {
                _SpellLikeAbilitiesBlock = SpellBlockInfo.ParseInfo(spellLikeAbilities);
            }

        }


        [DataMember]
        public String AbilitiyScores
        {
            get
            {
                return abilitiyScores;
            }
            set
            {
                abilitiyScores = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("AbilitiyScores"));
                }
            }
        }

        [DataMember]
        public int BaseAtk
        {
            get
            {
                return baseAtk;
            }
            set
            {
                baseAtk = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("BaseAtk"));
                }
            }
        }

        [DataMember]
        public String CMB
        {
            get
            {
                return cmb;
            }
            set
            {
                cmb = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("CMB"));
                    PropertyChanged(this, new PropertyChangedEventArgs("CMB_Numeric"));
                }
            }
        }

        [DataMember]
        public String CMD
        {
            get
            {
                return cmd;
            }
            set
            {
                cmd = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("CMD"));
                    PropertyChanged(this, new PropertyChangedEventArgs("CMD_Numeric"));
                }
            }
        }

        [DataMember]
        public String Feats
        {
            get
            {
                return feats;
            }
            set
            {
                feats = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Feats"));
                }
            }
        }

        [DataMember]
        public String Skills
        {
            get
            {
                return skills;
            }
            set
            {
                skills = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Skills"));
                }
            }
        }

        [DataMember]
        public String RacialMods
        {
            get
            {
                return racialMods;
            }
            set
            {
                racialMods = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("RacialMods"));
                }
            }
        }

        [DataMember]
        public String Languages
        {
            get
            {
                return languages;
            }
            set
            {
                languages = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Languages"));
                }
            }
        }

        [DataMember]
        public String SQ
        {
            get
            {
                return sq;
            }
            set
            {
                sq = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SQ"));
                }
            }
        }

        [DataMember]
        public String Environment
        {
            get
            {
                return environment;
            }
            set
            {
                environment = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Environment"));
                }
            }
        }

        [DataMember]
        public String Organization
        {
            get
            {
                return organization;
            }
            set
            {
                organization = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Organization"));
                }
            }
        }

        [DataMember]
        public String Treasure
        {
            get
            {
                if (treasure == "NULL")
                {
                    treasure = null;
                }
                return treasure;
            }
            set
            {
                treasure = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Treasure"));
                }
            }
        }


        [DataMember]
        public String Description_Visual
        {
            get
            {
                if (description_visual == "NULL")
                {
                    description_visual = null;
                }
                return description_visual;
            }
            set
            {
                description_visual = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Description_Visual"));
                }
            }
        }

        [DataMember]   
        public String Group
        {
            get
            {
                return group;
            }
            set
            {
                group = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Group"));
                }
            }
        }

        [DataMember]
        public String Source
        {
            get
            {
                return source;
            }
            set
            {
                source = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Source"));
                }
            }
        }

        [DataMember]
        public String IsTemplate
        {
            get
            {
                return isTemplate;
            }
            set
            {
                isTemplate = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("IsTemplate"));
                }
            }
        }

        [DataMember]
        public String SpecialAbilities
        {
            get
            {
                return specialAbilities;
            }
            set
            {
                specialAbilities = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SpecialAbilities"));
                }
            }
        }

        [DataMember]
        public String Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Description"));
                }
            }
        }

        [DataMember]
        public String FullText
        {
            get
            {
                return fullText;
            }
            set
            {
                fullText = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("FullText"));
                }
            }
        }

        [DataMember]
        public String Gender
        {
            get
            {
                return gender;
            }
            set
            {
                gender = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Gender"));
                }
            }
        }

        [DataMember]
        public String Bloodline
        {
            get
            {
                return bloodline;
            }
            set
            {
                bloodline = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Bloodline"));
                }
            }
        }

        [DataMember]
        public String ProhibitedSchools
        {
            get
            {
                return prohibitedSchools;
            }
            set
            {
                prohibitedSchools = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ProhibitedSchools"));
                }
            }
        }


        [DataMember]
        public String BeforeCombat
        {
            get
            {
                if (beforeCombat == "NULL")
                {
                    beforeCombat = null;
                }
                return beforeCombat;
            }
            set
            {
                beforeCombat = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("BeforeCombat"));
                }
            }
        }

        [DataMember]
        public String DuringCombat
        {
            get
            {
                if (duringCombat == "NULL")
                {
                    duringCombat = null;
                }
                return duringCombat;
            }
            set
            {
                duringCombat = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("DuringCombat"));
                }
            }
        }

        [DataMember]
        public String Morale
        {
            get
            {
                if (morale == "NULL")
                {
                    morale = null;
                }
                return morale;
            }
            set
            {
                morale = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Morale"));
                }
            }
        }

        [DataMember]
        public String Gear
        {
            get
            {
                if (gear == "NULL")
                {
                    gear = null;
                }
                return gear;
            }
            set
            {
                gear = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Gear"));
                }
            }
        }

        [DataMember]
        public String OtherGear
        {
            get
            {
                return otherGear;
            }
            set
            {
                otherGear = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("OtherGear"));
                }
            }
        }

        [DataMember]
        public String Vulnerability
        {
            get
            {
                return vulnerability;
            }
            set
            {
                vulnerability = value;
                if (vulnerability == "NULL")
                {
                    vulnerability = null;
                }
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Vulnerability"));
                }
            }
        }

        [DataMember]
        public String Note
        {
            get
            {
                return note;
            }
            set
            {
                note = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Note"));
                }
            }
        }

        [DataMember]
        public String CharacterFlag
        {
            get
            {
                return characterFlag;
            }
            set
            {
                characterFlag = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("CharacterFlag"));
                }
            }
        }

        [DataMember]
        public String CompanionFlag
        {
            get
            {
                return companionFlag;
            }
            set
            {
                companionFlag = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("CompanionFlag"));
                }
            }
        }

        [DataMember]
        public String Fly
        {
            get
            {
                return fly;
            }
            set
            {
                fly = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Fly"));
                }
            }
        }

        [DataMember]
        public String Climb
        {
            get
            {
                return climb;
            }
            set
            {
                climb = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Climb"));
                }
            }
        }

        [DataMember]
        public String Burrow
        {
            get
            {
                return burrow;
            }
            set
            {
                burrow = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Burrow"));
                }
            }
        }

        [DataMember]
        public String Swim
        {
            get
            {
                return swim;
            }
            set
            {
                swim = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Swim"));
                }
            }
        }

        [DataMember]
        public String Land
        {
            get
            {
                return land;
            }
            set
            {
                land = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Land"));
                }
            }
        }

        [DataMember]
        public String TemplatesApplied
        {
            get
            {
                return templatesApplied;
            }
            set
            {
                templatesApplied = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("TemplatesApplied"));
                }
            }
        }

        [DataMember]
        public String OffenseNote
        {
            get
            {
                return offenseNote;
            }
            set
            {
                offenseNote = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("OffenseNote"));
                }
            }
        }

        [DataMember]
        public String BaseStatistics
        {
            get
            {
                return baseStatistics;
            }
            set
            {
                baseStatistics = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("BaseStatistics"));
                }
            }
        }

        [DataMember]
        public String SpellsPrepared
        {
            get
            {
                return spellsPrepared;
            }
            set
            {
                if (spellsPrepared != value)
                {
                    spellsPrepared = value;
                    _SpellsPreparedBlock = null;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("SpellsPrepared"));
                    }
                }
            }
        }
        [XmlIgnore]
        public ObservableCollection<SpellBlockInfo> SpellsPreparedBlock
        {
            get
            {
                ParseSpellsPrepared();
                return _SpellsPreparedBlock;
            }
        }

        public void ParseSpellsPrepared()
        {

            if (_SpellsPreparedBlock == null && spellsPrepared != null && spellsPrepared.Length > 0)
            {
                _SpellsPreparedBlock = SpellBlockInfo.ParseInfo(spellsPrepared);
            }
        }


        [DataMember]
        public String SpellDomains
        {
            get
            {
                return spellDomains;
            }
            set
            {
                spellDomains = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SpellDomains"));
                }
            }
        }

        [DataMember]
        public String Aura
        {
            get
            {
                return aura;
            }
            set
            {
                aura = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Aura"));
                }
            }
        }

        [DataMember]
        public String DefensiveAbilities
        {
            get
            {
                return defensiveAbilities;
            }
            set
            {
                defensiveAbilities = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("DefensiveAbilities"));
                }
            }
        }

        [DataMember]
        public String Immune
        {
            get
            {
                return immune;
            }
            set
            {
                immune = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Immune"));
                }
            }
        }


        [DataMember]
        public String HP_Mods
        {
            get
            {
                return hp_mods;
            }
            set
            {
                hp_mods = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("HP_Mods"));
                }
            }
        }


        [DataMember]
        public String SpellsKnown
        {
            get
            {
                return spellsKnown;
            }
            set
            {
                if (spellsKnown != value)
                {
                    spellsKnown = value;
                    _SpellsKnownBlock = null;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("SpellsKnown"));
                    }
                }
            }
        }

        [XmlIgnore]
        public ObservableCollection<SpellBlockInfo> SpellsKnownBlock
        {
            get
            {
                ParseSpellsKnown();
                return _SpellsKnownBlock;
            }
        }

        public void ParseSpellsKnown()
        {

            if (_SpellsKnownBlock == null && spellsKnown != null && spellsKnown.Length > 0)
            {
                _SpellsKnownBlock = SpellBlockInfo.ParseInfo(spellsKnown);
            }
        }



        [DataMember]
        public String Weaknesses
        {
            get
            {
                return weaknesses;
            }
            set
            {
                weaknesses = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Weaknesses"));
                }
            }
        }


        [DataMember]
        public String Speed_Mod
        {
            get
            {
                return speed_mod;
            }
            set
            {
                speed_mod = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Speed_Mod"));
                }
            }
        }


        [DataMember]
        public String MonsterSource
        {
            get
            {
                return monsterSource;
            }
            set
            {
                monsterSource = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("MonsterSource"));
                }
            }
        }



        [DataMember]
        public String ExtractsPrepared
        {
            get { return extractsPrepared; }
            set
            {
                if (extractsPrepared != value)
                {
                    extractsPrepared = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("ExtractsPrepared")); }
                }
            }
        }

        [DataMember]
        public String AgeCategory
        {
            get { return ageCategory; }
            set
            {
                if (ageCategory != value)
                {
                    ageCategory = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("AgeCategory")); }
                }
            }
        }

        [DataMember]
        public bool DontUseRacialHD
        {
            get { return dontUseRacialHD; }
            set
            {
                if (dontUseRacialHD != value)
                {
                    dontUseRacialHD = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("DontUseRacialHD")); }
                }
            }
        }

        [DataMember]
        public String VariantParent
        {
            get { return variantParent; }
            set
            {
                if (variantParent != value)
                {
                    variantParent = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("VariantParent")); }
                }
            }
        }


        [DataMember]
        public String DescHTML
        {
            get { return descHTML; }
            set
            {
                if (descHTML != value)
                {
                    descHTML = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("DescHTML")); }
                }
            }
        }



        private void ParseStats()
        {
            strength = ParseStat("Str", abilitiyScores);
            dexterity = ParseStat("Dex", abilitiyScores);
            constitution = ParseStat("Con", abilitiyScores);
            intelligence = ParseStat("Int", abilitiyScores);
            wisdom = ParseStat("Wis", abilitiyScores);
            charisma = ParseStat("Cha", abilitiyScores);
            statsParsed = true;
        }

        private int? ParseStat(string stat, string text)
        {
            int? res = null;

            if (text != null)
            {

                Regex regEnd = new Regex(",|$");
                Regex regStat = new Regex(stat);

                Match start = regStat.Match(text);
                if (start.Success)
                {
                    int matchEnd = start.Index + start.Length;
                    Match end = regEnd.Match(text, matchEnd);

                    if (end.Success)
                    {
                        int val = 0;
                        if (int.TryParse(text.Substring(matchEnd, end.Index - matchEnd).Trim(), out val))
                        {
                            res = val;
                        }

                    }
                }
            }

            return res;
            
        }


        [DataMember]
        public bool StatsParsed
        {
            get
            {
                return statsParsed;
            }
            set
            {
                statsParsed = value;
            }
        }


        [DataMember]
        public int? Strength
        {
            get
            {
                if (!statsParsed)
                {
                    ParseStats();
                }
                return strength;
            }
            set
            {
                if (strength != value)
                {

                    strength = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Strength"));
                    }
                }
            }
        }




        [DataMember]
        public int? Dexterity
        {
            get
            {
                if (!statsParsed)
                {
                    ParseStats();
                }
                return dexterity;
            }
            set
            {
                if (dexterity != value)
                {

                    dexterity = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Dexterity"));
                    }
                }
            }
        }


        [DataMember]
        public int? Constitution
        {
            get
            {
                if (!statsParsed)
                {
                    ParseStats();
                }
                return constitution;
            }
            set
            {
                if (constitution != value)
                {

                    constitution = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Constitution"));
                    }
                }
            }
        }


        [DataMember]
        public int? Intelligence
        {
            get
            {
                if (!statsParsed)
                {
                    ParseStats();
                }
                return intelligence;
            }
            set
            {
                if (intelligence != value)
                {
                    intelligence = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Intelligence"));
                    }
                }
            }
        }



        [DataMember]
        public int? Wisdom
        {
            get
            {
                if (!statsParsed)
                {
                    ParseStats();
                }
                return wisdom;
            }
            set
            {
                if (wisdom != value)
                {
                    wisdom = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Wisdom"));
                    }
                }
            }
        }



        [DataMember]
        public int? Charisma
        {
            get
            {
                if (!statsParsed)
                {
                    ParseStats();
                }
                return charisma;
            }
            set
            {
                if (charisma != value)
                {
                    charisma = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Charisma"));
                    }
                }

            }
        }


        [DataMember]
        public bool SpecialAblitiesParsed
        {
            get
            {
                return specialAblitiesParsed;
            }
            set
            {
                specialAblitiesParsed = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SpecialAblitiesParsed"));
                }
            }

        }


        [DataMember]
        public ObservableCollection<SpecialAbility> SpecialAbilitiesList
        {
            get
            {
                if (!specialAblitiesParsed)
                {
                    ParseSpecialAbilities();
                }
                return specialAbilitiesList;
            }
            set
            {
                specialAbilitiesList = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SpecialAbilitiesList"));
                }
            }
        }


        [DataMember]
        public bool SkillsParsed
        {
            get
            {
                return skillsParsed;
            }
            set
            {
                skillsParsed = value;
            }
        }

        [XmlIgnore]
        public Dictionary<String, SkillValue> SkillValueDictionary
        {
            get
            {
                if (!skillsParsed)
                {
                    ParseSkills();
                }
                else if (skillValuesMayNeedUpdate)
                {
                    skillValuesMayNeedUpdate = false;

                    foreach (SkillValue skillValue in skillValueList)
                    {
                        skillValueDictionary[skillValue.FullName] = skillValue;
                    }
                        
                }
                return skillValueDictionary;
            }
            set
            {
                skillValueDictionary = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SkillValueDictionary"));
                }
            }
        }


        [DataMember]
        public List<SkillValue> SkillValueList
        {
            get               
            {
                if (!skillValuesMayNeedUpdate)
                {
                    UpdateSkillValueList();
                }
                
                skillValuesMayNeedUpdate = true;
                return skillValueList;
            }
            set
            {
                skillValueDictionary = new Dictionary<string, SkillValue>(new InsensitiveEqualityCompararer());

                foreach (SkillValue val in value)
                {
                    skillValueDictionary[val.FullName] = val;
                }

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SkillValueDictionary"));
                }
            }
        }

        public void UpdateSkillValueList()
        {

            skillValueList.Clear();
            skillValueList.AddRange(skillValueDictionary.Values);
        }


        [DataMember]
        public bool FeatsParsed
        {
            get
            {
                return featsParsed;
            }
            set
            {
                featsParsed = value;
            }
        }


        [DataMember]
        public List<string> FeatsList
        {
            get
            {
                if (featsList == null)
                {
                    featsList = new List<string>();
                }
                if (!featsParsed)
                {
                    ParseFeats();
                }
                return featsList;
            }
            set
            {
                featsList = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("FeatsList"));
                }
            }
        }





        [DataMember]
        public bool AcParsed
        {
            get { return acParsed; }
            set
            {
                if (acParsed != value)
                {
                    acParsed = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("acParsed")); }
                }
            }
        }

        [DataMember]
        public int FullAC
        {
            get 
            {
                if (!acParsed)
                {
                    ParseAC();
                }
            
                return fullAC; 
            }
            set
            {
                if (fullAC != value)
                {
                    fullAC = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("FullAC")); }
                }
            }
        }

        [DataMember]
        public int TouchAC
        {
            get
            {
                if (!acParsed)
                {
                    ParseAC();
                }

                return touchAC;
            }
            set
            {
                if (touchAC != value)
                {
                    touchAC = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("TouchAC")); }
                }
            }
        }

        [DataMember]
        public int FlatFootedAC
        {
            get
            {
                if (!acParsed)
                {
                    ParseAC();
                }

                return flatFootedAC;
            }
            set
            {
                if (flatFootedAC != value)
                {
                    flatFootedAC = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("FlatFootedAC")); }
                }
            }
        }

        [DataMember]
        public int NaturalArmor
        {
            get
            {
                if (!acParsed)
                {
                    ParseAC();
                }

                return naturalArmor;
            }
            set
            {
                if (naturalArmor != value)
                {
                    naturalArmor = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("NaturalArmor")); }
                }
            }
        }

        [DataMember]
        public int Deflection
        {
            get
            {
                if (!acParsed)
                {
                    ParseAC();
                }

                return deflection;
            }
            set
            {
                if (deflection != value)
                {
                    deflection = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Deflection")); }
                }
            }
        }

        [DataMember]
        public int Shield
        {
            get
            {
                if (!acParsed)
                {
                    ParseAC();
                }

                return shield;
            }
            set
            {
                if (shield != value)
                {
                    shield = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Shield")); }
                }
            }
        }

        [DataMember]
        public int Armor
        {
            get
            {
                if (!acParsed)
                {
                    ParseAC();
                }

                return armor;
            }
            set
            {
                if (armor != value)
                {
                    armor = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Armor")); }
                }
            }
        }

        [DataMember]
        public int Dodge
        {
            get
            {
                if (!acParsed)
                {
                    ParseAC();
                }

                return dodge;
            }
            set
            {
                if (dodge != value)
                {
                    dodge = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Dodge")); }
                }
            }
        }


        [DataMember]
        public int CMB_Numeric
        {
            get 
            { 
                return GetStartingModOrVal(CMB); 
            }
            set
            {
                int num = CMB_Numeric;
                if (num != value)
                {
                    CMB = ChangeStartingModOrVal(CMB, value - num);
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("CMB_Numeric")); }
                }
            }
        }


        [DataMember]
        public int CMD_Numeric
        {
            get 
            { 
                return GetStartingNumber(CMD); 
            }
            set
            {
                int num = CMD_Numeric;
                if (CMD_Numeric != value)
                {
                    CMD = ChangeCMD(CMD, value - num);

                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("CMD_Numeric")); }
                }
            }
        }

        [XmlIgnore]
        public int? XPValue
        {
            get
            {
                if (xp != null)
                {

                    string xpstr = Regex.Replace(xp, ",", "");
                    int val = 0;
                    if (int.TryParse(xpstr, out val))
                    {
                        return val;
                    }
                }
                return null;
            }
        }

        [XmlIgnore]
        public bool NPC
        {
            get
            {
                return npc;
            }
            set
            {
                npc = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("NPC"));
                }
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

        public int GetSave(SaveType type)
        {
            if (type == SaveType.Fort)
            {
                return Fort;
            }
            else if (type == SaveType.Ref)
            {
                return Ref;
            }
            else if (type == SaveType.Will)
            {
                return Will;
            }

            return 0;
        }

        private void SetStatDirect(Stat stat, int? value)
        {
            switch (stat)
            {
                case Stat.Strength:
                    strength = value;
                    break;
                case Stat.Dexterity:
                    dexterity = value;
                    break;
                case Stat.Constitution:
                    constitution = value;
                    break;
                case Stat.Intelligence:
                    intelligence = value;
                    break;
                case Stat.Wisdom:
                    wisdom = value;
                    break;
                case Stat.Charisma:
                    charisma = value;
                    break;
            }


        }

        public static List<String> DragonColors
        {
            get
            {
                return new List<String> (dragonColorList.Keys);
            }
        }


        public static FlyQuality FlyQualityFromString(String strQuality)
        {
            FlyQuality qual = FlyQuality.Average;

            if (String.Compare(strQuality, "clumsy", true) == 0)
            {
                qual = FlyQuality.Clumsy;
            }
            if (String.Compare(strQuality, "poor", true) == 0)
            {
                qual = FlyQuality.Poor;
            }

            if (String.Compare(strQuality, "average", true) == 0)
            {
                qual = FlyQuality.Average;
            }

            if (String.Compare(strQuality, "good", true) == 0)
            {
                qual = FlyQuality.Good;
            }

            if (String.Compare(strQuality, "perfect", true) == 0)
            {
                qual = FlyQuality.Perfect;
            }


            return qual;
        }

        public static string StringFromFlyQuality(FlyQuality qual)
        {
            string text = "Average";

            switch (qual)
            {
                case FlyQuality.Clumsy:
                    text = "clumsy";
                    break;
                case FlyQuality.Poor:
                    text = "poor";
                    break;
                case FlyQuality.Average:
                    text = "average";
                    break;
                case FlyQuality.Good:
                    text = "good";
                    break;
                case FlyQuality.Perfect:
                    text = "perfect";
                    break;
            }

            return text;
        }

        [XmlIgnore]
        public MonsterAdjuster Adjuster
        {
            get
            {
                if (_Adjuster == null)
                {
                    _Adjuster = new MonsterAdjuster(this);
                }

                return _Adjuster;
            }
        }


        public class MonsterAdjuster : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            
            private Monster _Monster;

            private String _FlyQuality;
            
            public void NotifyPropertyChanged(string property)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(property));
                }
            }

            
            public MonsterAdjuster(Monster m)
            {
                _Monster = m;

                _Monster.PropertyChanged += new PropertyChangedEventHandler(Monster_PropertyChanged);
            }

            void  Monster_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                NotifyPropertyChanged(e.PropertyName);

                if (e.PropertyName == "Size")
                {
                    NotifyPropertyChanged("MonsterSize");
                }
				if (e.PropertyName == "SubType")
				{
					NotifyPropertyChanged("Subtype");	
				}
				
            }

            public int? Strength
            {
                get
                {
                    return _Monster.strength;
                }
                set
                {
                    if (_Monster.strength != value)
                    {
                        SetStat(Stat.Strength, value);
                        NotifyPropertyChanged("Strength");
                    }
                }
            }

            public int? Dexterity
            {
                get
                {
                    return _Monster.dexterity;
                }
                set
                {
                    if (_Monster.dexterity != value)
                    {
                        SetStat(Stat.Dexterity, value);
                        NotifyPropertyChanged("Dexterity");
                    }
                }
            }

            public int? Constitution
            {
                get
                {
                    return _Monster.constitution;
                }
                set
                {
                    if (_Monster.constitution != value)
                    {
                        SetStat(Stat.Constitution, value);
                        NotifyPropertyChanged("Constitution");
                    }
                }
            }

            public int? Intelligence
            {
                get
                {
                    return _Monster.intelligence;
                }
                set
                {
                    if (_Monster.intelligence != value)
                    {
                        SetStat(Stat.Intelligence, value);
                        NotifyPropertyChanged("Intelligence");
                    }
                }
            }

            public int? Wisdom
            {
                get
                {
                    return _Monster.wisdom;
                }
                set
                {
                    if (_Monster.wisdom != value)
                    {
                        SetStat(Stat.Wisdom, value);
                        NotifyPropertyChanged("Wisdom");
                    }
                }
            }

            public int? Charisma
            {
                get
                {
                    return _Monster.charisma;
                }
                set
                {
                    if (_Monster.charisma != value)
                    {
                        SetStat(Stat.Charisma, value);
                        NotifyPropertyChanged("Charisma");
                    }
                }
            }

            private void SetStat(Stat stat, int? newValue)
            {
                int? statVal = _Monster.GetStat(stat);

                if (statVal == null)
                {
                    //adjust from null strength
                    _Monster.SetStatDirect(stat, 10);
                    _Monster.AdjustStat(stat, newValue.Value - 10);
                }
                else if (newValue == null)
                {
                    //adjust to null strength
                    _Monster.AdjustStat(stat, 10 - statVal.Value);
                    _Monster.SetStatDirect(stat, null);
                }
                else
                {
                    _Monster.AdjustStat(stat, newValue.Value - statVal.Value);
                }
            }

            private int? GetSpeed(string speedType)
            {
                Regex speedReg = new Regex(speedType + " +(?<speed>[0-9]+) +ft\\.", RegexOptions.IgnoreCase);

                int? foundSpeed = null;

                Match m = speedReg.Match(_Monster.speed);

                if (m.Success)
                {
                    foundSpeed = int.Parse(m.Groups["speed"].Value);
                }

                return foundSpeed;
            }

            private void SetSpeed(string speedType, int? value)
            {
                if (value == null)
                {
                    Regex speedReg = new Regex(speedType + " +(?<speed>[0-9]+) +ft\\.(, *)?", RegexOptions.IgnoreCase);
                    _Monster.Speed = speedReg.Replace(_Monster.Speed, "");

                }
                else
                {
                    
                    bool bFound = false;
                    Regex speedReg = new Regex(speedType + " +(?<speed>[0-9]+) +ft\\.", RegexOptions.IgnoreCase);
                     _Monster.Speed = speedReg.Replace(_Monster.speed,
                                delegate(Match m)
                                {
                                    bFound = true;
                                    return value + " ft.";
                                }
                                );

                     if (!bFound)
                     {
                         _Monster.Speed += ", " + speedType + " " + value + " ft.";
                     }
                    
                }
            }


            public int LandSpeed
            {
                get
                {
                    int speed = 0;

                    Regex speedReg = new Regex("^ *(?<speed>[0-9]+) +ft\\.", RegexOptions.IgnoreCase);

                    Match m = speedReg.Match(_Monster.Speed);

                    if (m.Success)
                    {
                        speed = int.Parse(m.Groups["speed"].Value);
                    }

                    return speed;
                }
                set
                {
                    
                    Regex speedReg = new Regex("^ *(?<speed>[0-9]+) +ft\\.", RegexOptions.IgnoreCase);
                    _Monster.Speed = speedReg.Replace(_Monster.speed,
                                    delegate(Match m)
                                    {
                                        return value + " ft.";
                                    }
                                    );

                    NotifyPropertyChanged("LandSpeed");
                }
            }

            private bool ParseFly(out int speed, out string quality)
            {
                speed = 0;
                quality = null;

                Regex speedReg = new Regex("fly +(?<speed>[0-9]+) +ft\\. +\\((?<quality>[a-zA-Z]+)\\)", RegexOptions.IgnoreCase);

                Match m = speedReg.Match(_Monster.Speed);

                if (m.Success)
                {
                    speed = int.Parse(m.Groups["speed"].Value);
                    quality = StringCapitalizer.Capitalize(m.Groups["quality"].Value);
                }

                return m.Success;
            }

            private void SetFly(int speed, string quality)
            {
                Regex speedReg = new Regex("fly +(?<speed>[0-9]+) +ft\\. +\\((?<quality>[a-zA-Z]+)\\)", RegexOptions.IgnoreCase);

                string flyString = "fly " + speed + " ft. (" + quality.ToLower() + ")";
                
                bool found = false;
                _Monster.Speed = speedReg.Replace(_Monster.Speed, delegate(Match m)
                    {
                        found = true;
                        return flyString;
                    });
                

                if (!found)
                {
                    _Monster.Speed += ", " + flyString;
                }

            }

            private void RemoveFly()
            {
                Regex speedReg = new Regex("(, +)?fly +(?<speed>[0-9]+) +ft\\. +\\((?<quality>[a-zA-Z]+)\\)", RegexOptions.IgnoreCase);

                speedReg.Replace(_Monster.Speed, "");
                
            }

            public int? FlySpeed
            {
                get
                {
                    return GetSpeed("fly");
                }
                set
                {
                    if (value == null)
                    {
                        RemoveFly();
                    }
                    else
                    {
                        SetFly(value.Value, StringFromFlyQuality((FlyQuality)FlyQuality));
                    }
                }
            }


            public int FlyQuality
            {
                get
                {
                    int speed;
                    string quality;
                    if (ParseFly(out speed, out quality))
                    {
                        _FlyQuality = quality;
                    }
                    else if (_FlyQuality == null)
                    {
                        _FlyQuality = "Average";
                    }

                    return (int)FlyQualityFromString(_FlyQuality);
                }
                set
                {
                    _FlyQuality = StringFromFlyQuality((FlyQuality)value);

                    int speed;
                    string quality;
                    if (ParseFly(out speed, out quality))
                    {
                        SetFly(speed, _FlyQuality);
                    }

                }
            }


            public int? ClimbSpeed
            {
                get
                {
                    return GetSpeed("climb");
                }
                set
                {
                    SetSpeed("climb", value);
                    NotifyPropertyChanged("ClimbSpeed");
                }
            }
            public int? BurrowSpeed
            {
                get
                {
                    return GetSpeed("burrow");
                }
                set
                {
                    SetSpeed("burrow", value);
                    NotifyPropertyChanged("BurrowSpeed");
                }
            }
            public int? SwimSpeed
            {
                get
                {
                    return GetSpeed("swim");
                }
                set
                {
                    SetSpeed("swim", value);
                    NotifyPropertyChanged("SwimSpeed");
                }
            }

            public int MonsterSize
            {
                get
                {
                    return (int)SizeMods.GetSize(_Monster.Size);
                }
                set
                {
                    MonsterSize old = SizeMods.GetSize(_Monster.Size);

                    int diff = ((int)value) - (int)old;

                    _Monster.AdjustSize(diff);
                    NotifyPropertyChanged("MonsterSize");
                }
            }

            public int Armor
            {
                get
                {
                    return _Monster.Armor;
                }
                set
                {
                    if (value != _Monster.Armor)
                    {
                        _Monster.AdjustArmor(value - _Monster.Armor);
                        NotifyPropertyChanged("Armor");
                    }
                }
            }

            public int Deflection
            {
                get
                {
                    return _Monster.Deflection;
                }
                set
                {
                    if (value != _Monster.Deflection)
                    {
                        _Monster.AdjustDeflection(value - _Monster.Deflection);
                        NotifyPropertyChanged("Deflection");
                    }
                }
            }

            public int Dodge
            {
                get
                {
                    return _Monster.Dodge;
                }
                set
                {
                    if (value != _Monster.Dodge)
                    {
                        _Monster.AdjustDodge(value - _Monster.Dodge);
                        NotifyPropertyChanged("Dodge");
                    }
                }
            }

            public int NaturalArmor
            {
                get
                {
                    return _Monster.NaturalArmor;
                }
                set
                {
                    if (value != _Monster.NaturalArmor)
                    {
                        _Monster.AdjustNaturalArmor(value - _Monster.NaturalArmor);
                        NotifyPropertyChanged("NaturalArmor");
                    }
                }
            }

            public int Shield
            {
                get
                {
                    return _Monster.Shield;
                }
                set
                {
                    if (value != _Monster.Shield)
                    {
                        _Monster.AdjustShield(value - _Monster.Shield);
                        NotifyPropertyChanged("Shield");
                    }
                }
            }

            public int BaseAtk
            {
                get
                {
                    return _Monster.BaseAtk;
                }
                set
                {
                    if (value != _Monster.BaseAtk)
                    {
                        _Monster.AdjustBaseAttack(value - _Monster.BaseAtk, true);
                        NotifyPropertyChanged("BaseAtk");
                    }
                }
            }

            public string SpellLikeAbilities
            {
                get
                {
                    string SLA = _Monster.spellLikeAbilities;

                    if (SLA != null)
                    {
                        Regex regSLA = new Regex("Spell-Like Abilities +(?<SLA>.*)", RegexOptions.IgnoreCase);
                        Match m = regSLA.Match(SLA);

                        if (m.Success)
                        {
                            SLA = m.Groups["SLA"].Value;
                        }
                    }

                    return SLA;
                }
                set
                {
                    if (value == null || value.Trim().Length == 0)
                    {
                        _Monster.SpellLikeAbilities = null;
                    }

                    _Monster.SpellLikeAbilities = "Spell-Like Abilities " + value.Trim();
                    NotifyPropertyChanged("SpellLikeAbilities");
                }

            }

            public string SpellsPrepared
            {
                get
                {
                    string spells = _Monster.SpellsPrepared;

                    if (spells != null)
                    {
                        Regex regSpells = new Regex("^ *Spells Prepared +(?<spells>.*)", RegexOptions.IgnoreCase);
                        Match m = regSpells.Match(spells);

                        if (m.Success)
                        {
                            spells = m.Groups["spells"].Value;
                        }
                    }

                    return spells;
                }
                set
                {
                    if (value == null || value.Trim().Length == 0)
                    {
                        _Monster.SpellsPrepared = null;
                    }
                    _Monster.SpellsPrepared = "Spells Prepared " + value;
                    NotifyPropertyChanged("SpellsPrepared");
                }

            }

            public string SpellsKnown
            {
                get
                {
                    string spells = _Monster.SpellsKnown;

                    if (spells != null)
                    {

                        Regex regSpells = new Regex("^ *Spells Known +(?<spells>.*)", RegexOptions.IgnoreCase);
                        Match m = regSpells.Match(spells);

                        if (m.Success)
                        {
                            spells = m.Groups["spells"].Value;
                        }
                    }

                    return spells;
                }
                set
                {
                    if (value == null || value.Trim().Length == 0)
                    {
                        _Monster.SpellsKnown = null;
                    }
                    _Monster.SpellsKnown = "Spells Known " + value;
                    NotifyPropertyChanged("SpellsKnown");
                }

            }


            public String CR
            {
                get
                {
                    return _Monster.CR;
                }
                set
                {
                    
                    _Monster.CR = value;
                    _Monster.XP = GetXPString(value);

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("CR"));
                    }

                }
            }
			
			public string Subtype
			{
				get
				{
					if (_Monster.SubType == null)
					{
						return null;
					}
					return _Monster.SubType.Trim(new char[]{'(',')'});
				}
				set
				{
					if (value == null)
					{
						_Monster.SubType = null;
					}
					else 
					{
						string val = value.Trim();
						if (val.Length == 0)
						{
							_Monster.SubType = null;
						}
						else
						{
							if (!Regex.Match(val, "\\(.+\\)").Success)
							{
								val = "(" + val + ")";
							}
							_Monster.SubType = val;
						}
					}
					
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Subtype"));
                    }
					
				}
			}
			public DieRoll HD
			{
				get	
				{
					return DieRoll.FromString(_Monster.HD);
				}
				set
				{
					_Monster.HD = "(" + value.ToString() + ")";
					_Monster.HP = value.AverageRoll();
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("HD"));
                    }
				}
			}

            public int Space
            {
                get
                {
                    int ? space = FootConverter.Convert(_Monster.Space);
                    return (space == null)?0:(space.Value);
                }
                set
                {
                    _Monster.Space = FootConverter.ConvertBack(value);
                }
            }

            public int Reach
            {
                get
                {
                    int ? reach = FootConverter.Convert(_Monster.Reach);
                    return (reach == null)?0:(reach.Value);
                }
                set
                {
                    _Monster.Reach = FootConverter.ConvertBack(value);
                }
            }
        }

        [XmlIgnore]
        public int DBLoaderID
        {
            get
            {
                return _DBLoaderID;
            }
            set
            {
                if (_DBLoaderID != value)
                {
                    _DBLoaderID = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("DBLoaderID"));
                    }
                }

            }
        }
    }

}
