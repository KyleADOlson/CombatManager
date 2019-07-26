using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CombatManager
{
    public enum DD5Skill
    {
        Acrobatics = 0,
        AnimalHandling = 1,
        Arcana = 2,
        Athletics = 3,
        Deception = 4,
        History = 5,
        Insight = 6,
        Initimidation = 7,
        Investigation = 8,
        Medicine = 9,
        Nature = 10,
        Perception = 11,
        Performance = 12,
        Persuasion = 13,
        Religion = 14,
        SleightOfHand = 15,
        Stealth = 16,
        Survival = 17
    }


    public class DD5Stats : SimpleNotifyClass
    {

        private String age;
        private String eyes;
        private String skin;
        private String hair;


        private int proficiencyBonus;
        private int deathSaveSuccesses;
        private int deathSaveFailures;

        private ObservableCollection<String> personalityTrails;
        private ObservableCollection<String> bonds;
        private ObservableCollection<String> ideals;
        private ObservableCollection<String> flaws;

        private int? spellSaveDC;
        private int? spellAttack;


        private ObservableCollection<bool> skillsList;

        public String Age
        {
            get { return age; }
            set
            {
                if (age != value)
                {
                    age = value;
                    Notify("Age");
                }
            }
        }
        public String Eyes
        {
            get { return eyes; }
            set
            {
                if (eyes != value)
                {
                    eyes = value;
                    Notify("Eyes");
                }
            }
        }
        public String Skin
        {
            get { return skin; }
            set
            {
                if (skin != value)
                {
                    skin = value;
                    Notify("Skin");
                }
            }
        }
        public String Hair
        {
            get { return hair; }
            set
            {
                if (hair != value)
                {
                    hair = value;
                    Notify("Hair");
                }
            }
        }

        public int ProficiencyBonus
        {
            get
            {
                return proficiencyBonus;
            }
            set
            {
                if (proficiencyBonus != value)
                {
                    proficiencyBonus = value;
                    Notify("ProficiencyBonus");
                }
            }
        }

        public int? SpellSaveDC
        {
            get { return spellSaveDC; }
            set
            {
                if (spellSaveDC != value)
                {
                    spellSaveDC = value;
                    Notify("SpellSaveDC");
                }
            }
        }
        public int? SpellAttack
        {
            get { return spellAttack; }
            set
            {
                if (spellAttack != value)
                {
                    spellAttack = value;
                    Notify("SpellAttack");
                }
            }
        }

        public ObservableCollection<bool> SkillsList
        {
            get
            {
                if (skillsList == null)
                {
                    skillsList = new ObservableCollection<bool>();
                    skillsList.CollectionChanged += SkillsList_CollectionChanged;
                    int count = Enum.GetValues(typeof(DD5Skill)).Length;
                    for (int i = 0; i < count; i++)
                    {
                        skillsList.Add(false);
                    }
                }
                return skillsList;
            }
            set
            {
                if (skillsList != value)
                {
                    skillsList = value;
                    Notify("SkillsList");
                    Notify("Skills");
                }
            }
        }

        private void SkillsList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Notify("SkillsList");
            Notify("Skills");
        }

        private DD5Skills skills;

        [XmlIgnore]
        public DD5Skills Skills
        {
            get
            {
                if (skills == null)
                {
                    skills = new DD5Skills(this);
                }
                return skills;
            }
        }

        public int DeathSaveSuccesses
        {
            get => deathSaveSuccesses;
            set
            {
                if (deathSaveSuccesses != value)
                {
                    deathSaveSuccesses = value;
                    Notify("DeathSaveSuccesses");
                }
            }
            
        }
        public int DeathSaveFailures
        {
            get => deathSaveFailures;
            set
            {
                if (deathSaveFailures != value)
                {
                    deathSaveFailures = value;
                    Notify("DeathSaveFailures");
                }
            }
        }

        public ObservableCollection<String> PersonalityTrails
        {
            get { return personalityTrails; }
            set
            {
                if (personalityTrails != value)
                {
                    personalityTrails = value;
                    Notify("PersonalityTrails");
                }
            }
        }
        public ObservableCollection<String> Bonds
        {
            get { return bonds; }
            set
            {
                if (bonds != value)
                {
                    bonds = value;
                    Notify("Bonds");
                }
            }
        }
        public ObservableCollection<String> Ideals
        {
            get { return ideals; }
            set
            {
                if (ideals != value)
                {
                    ideals = value;
                    Notify("Ideals");
                }
            }
        }
        public ObservableCollection<String> Flaws
        {
            get { return flaws; }
            set
            {
                if (flaws != value)
                {
                    flaws = value;
                    Notify("Flaws");
                }
            }
        }


        public class DD5Skills : SimpleNotifyInternalClass<DD5Stats>
        {
            public DD5Skills(DD5Stats stats) : base(stats)
            {
            }
            
            public bool this[DD5Skill skill]
            {
                get
                {
                    return Parent.SkillsList[(int)skill];
                }
                set
                {
                    if (Parent.SkillsList[(int)skill] != value)
                    {
                        Parent.SkillsList[(int)skill] = value;
                    }
                }

            }
        }





    }
}
