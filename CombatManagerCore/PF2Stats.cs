using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatManager
{
    class PF2Stats : SimpleNotifyClass
    {
        private string ancestry;
        private string background;
        private string age;
        private int totalLevel;
        private Stat keyStat;
        private int heroPoints;

        public string Ancestry
        {
            get => ancestry;
            set
            {
                if (ancestry != value)
                {
                    ancestry = value;
                    Notify("Ancestry");
                }
            }
        }
        public string Background
        {
            get => background;
            set
            {
                if (background != value)
                {
                    background = value;
                    Notify("Background");
                }
            }
        }

        public string Age
        {
            get => age;
            set
            {
                if (age != value)
                {
                    age = value;
                    Notify("Age");
                }
            }
        }
        public int TotalLevel
        {
            get => totalLevel;
            set
            {
                if (totalLevel != value)
                {
                    totalLevel = value;
                    Notify("TotalLevel");
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
        public int HeroPoints
        {
            get => heroPoints;
            set
            {
                if (heroPoints != value)
                {
                    heroPoints = value;
                    Notify("HeroPoints");
                }
            }
        }

    }
}
