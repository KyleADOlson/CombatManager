using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static CombatManager.Character;

namespace CombatManager
{
    public abstract class BaseMonster : BaseDBClass
    {

        protected string name;
        private string cr;
        private string alignment;
        private string size;
        private int hp;
        private string speed;
        private int init;
        private string languages;
        private string hp_mods;
        private int? fort;
        private int? reflex;
        private int? will;
        private string resist;
        private string immune;
        private string weaknesses;
        private string source;


        protected ObservableCollection<ActiveCondition> _ActiveConditions;
        protected ObservableCollection<ActiveResource> _TrackedResources;

        public void BaseMonsterCopy(BaseMonster m)
        {
            CR = m.cr;
            Alignment = m.alignment;
            Size = m.size;
            HP = m.hp;
            Speed = m.speed;
            Init = m.init;
            Languages = m.languages;
            HP_Mods = m.hp_mods;
            Fort = m.fort;
            Ref = m.reflex;
            Will = m.will;
            Resist = m.resist;
            Weaknesses = m.weaknesses;
            Immune = m.immune;
            Source = m.source;

        }

        public void BaseMonsterClone(BaseMonster m)
        {
            m.name = name;
            m.cr = cr;
            m.alignment = alignment;
            m.size = size;
            m.hp = hp;
            m.speed = speed;
            m.init = init;
            m.languages = languages;
            m.hp_mods = hp_mods;
            m.fort = fort;
            m.reflex = reflex;
            m.will = will;
            m.resist = resist;
            m.weaknesses = weaknesses;
            m.immune = immune;
            m.source = source;
        }

        [DataMember]
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                Notify("Name");
            }
        }

        [DataMember]
        public string CR
        {
            get
            {
                return cr;
            }
            set
            {
                cr = value;
                Notify("CR");
            }
        }

        [DataMember]
        public string Alignment
        {
            get
            {
                return alignment;
            }
            set
            {
                alignment = value;
                Notify("Alignment");
            }
        }

        [DataMember]
        public string Size
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
                Notify("Size");
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
                Notify("HP");
            }
        }

        [DataMember]
        public string HP_Mods
        {
            get
            {
                return hp_mods;
            }
            set
            {
                hp_mods = value;
                Notify("HP_Mods");
            }
        }

        [DataMember]
        public string Speed
        {
            get
            {
                return speed;
            }
            set
            {
                speed = value;
                Notify("Speed");
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
                Notify("Init");
            }
        }

        [DataMember]
        public string Languages
        {
            get
            {
                return languages;
            }
            set
            {
                languages = value;
                Notify("Languages");
            }
        }

        [DataMember]
        public int? Fort
        {
            get
            {
                return fort;
            }
            set
            {
                fort = value;
                Notify("Fort");
            }
        }

        [DataMember]
        public int? Ref
        {
            get
            {
                return reflex;
            }
            set
            {
                reflex = value;
                Notify("Ref");
            }
        }

        [DataMember]
        public int? Will
        {
            get
            {
                return will;
            }
            set
            {
                will = value;
                Notify("Will");
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
                 }

                return _ActiveConditions;

            }
        }

        [DataMember]
        public ObservableCollection<ActiveResource> TResources
        {
            get
            {
                return _TrackedResources;
            }
            set
            {
                if (_TrackedResources != value)
                {
                    _TrackedResources = value;

                    Notify("TResources");
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
                Notify("Resist");
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
                Notify("Immune");
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
                Notify("Weaknesses");
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
                Notify("Source");
            }
        }

        [DataMember]
        public virtual int? Strength
        {
            get; set;
        }

        [DataMember]
        public virtual int? Dexterity
        {
            get; set;
        }

        [DataMember]
        public virtual int? Constitution
        {
            get; set;
        }

        [DataMember]
        public virtual int? Intelligence
        {
            get; set;
        }

        [DataMember]
        public virtual int? Wisdom
        {
            get; set;
        }

        [DataMember]
        public virtual int? Charisma
        {
            get; set;
        }


        public int StrengthBonus
        {
            get
            {
                return AbilityBonus(Strength);
            }
            set
            {
                Strength = AbilityFromBonus(value);
            }
        }

        public int DexterityBonus
        {
            get
            {
                return AbilityBonus(Dexterity);
            }
            set
            {
                Dexterity = AbilityFromBonus(value);
            }
        }

        public int ConstitutionBonus
        {
            get
            {
                return AbilityBonus(Constitution);
            }
            set
            {
                Constitution = AbilityFromBonus(value);
            }
        }

        public int IntelligenceBonus
        {
            get
            {
                return AbilityBonus(Intelligence);
            }
            set
            {
                Intelligence = AbilityFromBonus(value);
            }
        }

        public int WisdomBonus
        {
            get
            {
                return AbilityBonus(Wisdom);
            }
            set
            {
                Wisdom = AbilityFromBonus(value);
            }
        }

        public int CharismaBonus
        {
            get
            {
                return AbilityBonus(Charisma);
            }
            set
            {
                Charisma = AbilityFromBonus(value);
            }
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
                default:
                    return Charisma;
            }
        }

        public int GetStatBonus(Stat stat)
        {
            return AbilityBonus(GetStat(stat));
        }

        public void SetStatDirect(Stat stat, int ? value)
        {

            switch (stat)
            {
                case Stat.Strength:
                    Strength = value;
                    break;
                case Stat.Dexterity:
                    Dexterity = value;
                    break;
                case Stat.Constitution:
                    Constitution = value;
                    break;
                case Stat.Intelligence:
                    Intelligence = value;
                    break;
                case Stat.Wisdom:
                    Wisdom = value;
                    break;
                case Stat.Charisma:
                default:
                    Charisma = value;
                    break;
            }
        }

        public void SetAbilityBonus(Stat stat, int bonus)
        {
            AdjustStat(stat, AbilityFromBonus(bonus).Value);
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


        public virtual void AdjustStrength(int value)
        {
            Strength = value;
        }
        public virtual void AdjustDexterity(int value)
        {
            Dexterity = value;
        }
        public virtual void AdjustConstitution(int value)
        {
            Constitution = value;
        }
        public virtual void AdjustIntelligence(int value)
        {
            Intelligence = value;
        }
        public virtual void AdjustWisdom(int value)
        {
            Wisdom = value;
        }
        public virtual void AdjustCharisma(int value)
        {
            Charisma = value;
        }

        public virtual void ApplyDefaultConditions()
        {
        }


        public virtual int GetStartingHP(HPMode mode)
        {
            return hp;
        }

        public virtual IEnumerable<ActiveResource> LoadResources()
        {
            return new List<ActiveResource>();
        }
    

        public ActiveCondition FindCondition(string name)
        {
            return ActiveConditions.FirstOrDefault
                (a => String.Compare(a.Condition.Name, name, true) == 0);
        }

        public void AddCondition(ActiveCondition c)
        {
            ActiveConditions.Add(c);

            if (c.Bonus != null)
            {
                ApplyBonus(c.Bonus, false);
            }

        }

        public void RemoveCondition(ActiveCondition c)
        {
            ActiveConditions.Remove(c);

            if (c.Bonus != null)
            {
                ApplyBonus(c.Bonus, true);
            }
        }

        public virtual void ApplyBonus(ConditionBonus bonus, bool remove)
        {

        }

        public static int AbilityBonus(int? score)
        {
            if (score == null)
            {
                return 0;
            }

            return (score.Value / 2) - 5;
        }

        public static int? AbilityFromBonus(int bonus)
        {
            return (bonus * 2) + 10;
        }

        public static string GetSaveText(SaveType type)
        {
            if (type == SaveType.Fort)
            {
                return "Fort";
            }
            else if (type == SaveType.Ref)
            {
                return "Ref";
            }
            else if (type == SaveType.Will)
            {
                return "Will";
            }

            return "";
        }

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

        protected static string AddToStringList(string text, string type)
        {
            return AddToStringList(text, type, out _);

        }

        protected static string AddToStringList(string text, string type, out bool added)
        {
            added = false;


            string returnText = text;
            if (returnText == null)
            {
                returnText = "";
            }


            if (!StringListHasItem(returnText, type))
            {

                returnText = returnText + (returnText.Length > 0 ? ", " : "") + type;

                added = true;

            }

            return returnText;
        }

        protected static bool StringListHasItem(string list, string item)
        {
            Regex regType = new Regex(Regex.Escape(item) + "(\\Z|$|,)", RegexOptions.IgnoreCase);

            return regType.Match(list).Success;
        }


        protected static string RemoveFromStringList(string text, string type)
        {
            bool removed;
            return RemoveFromStringList(text, type, out removed);
        }

        protected static string RemoveFromStringList(string text, string type, out bool removed)
        {
            removed = false;

            Regex regex = new Regex("(^| )(" + type + ")(\\Z|,)");

            return regex.Replace(text, "").Trim();

        }
    }
}
