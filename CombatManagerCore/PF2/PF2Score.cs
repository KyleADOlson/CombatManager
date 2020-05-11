using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CombatManager.PF2
{
    public class PF2Score : SimpleNotifyClass
    {
        string name;
        Stat keyStat;
        int stat;
        ProficiencyRank rank;
        int item;
        int armor;
        bool usesArmor;
        bool signature;

        public string Name
        {
            get => name;
            set
            {
                if (name != value)
                {
                    name = value;
                    Notify("Name");
                }
            }
        }

        public int Stat
        {
            get => stat;
            set
            {
                if (stat != value)
                {
                    stat = value;
                    Notify("Stat");
                }
            }
        }
        public Stat KeyStat
        {
            get => keyStat;
            set
            {
                if (keyStat != value)
                {
                    keyStat = value;
                    Notify("KeyStat");
                }
            }
        }

        public ProficiencyRank Rank
        {
            get => rank;
            set
            {
                if (rank != value)
                {
                    rank = value;
                    Notify("Rank");
                }
            }
        }

        public int Item
        {
            get => item;
            set
            {
                if (item != value)
                {
                    item = value;
                    Notify("Item");
                }
            }
        }

        public int Armor
        {
            get => armor;
            set
            {
                if (armor != value)
                {
                    armor = value;
                    Notify("Armor");
                }
            }
        }

        public bool UsesArmor
        {
            get => usesArmor;
            set
            {
                if (usesArmor != value)
                {
                    usesArmor = value;
                    Notify("UsesArmor");
                }
            }
        }

        public bool Signature
        {
            get => signature;
            set
            {
                if (signature != value)
                {
                    signature = value;
                    Notify("Signature");
                }
            }
        }

        [XmlIgnore]
        public int Total
        {
            get
            {
                return stat + rank.Modifier() + item + (usesArmor ? armor : 0);
            }
        }



    }
}
