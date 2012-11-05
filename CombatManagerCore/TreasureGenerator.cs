/*
 *  TreasureGenerator.cs
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
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Globalization;
using System.IO;


namespace CombatManager
{
    public class TreasureGenerator : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;


        private int _Level;
        private int _Coin;
        private int _Goods;
        private int _Items;




        private static SortedDictionary<int, SortedDictionary<int, string>> coinChart;
        private static SortedDictionary<int, SortedDictionary<int, string>> goodsChart;
        private static SortedDictionary<int, SortedDictionary<int, string>> itemChart;


        private static SortedDictionary<int, List<Spell>> potionChart;
        private static SortedDictionary<int, List<Spell>> arcaneScrollChart;
        private static SortedDictionary<int, List<Spell>> divineScrollChart;
        private static SortedDictionary<int, List<Spell>> wandChart;
        private static List<int> potionLevelTotals;
        private static List<int> arcaneScrollLevelTotals;
        private static List<int> divineScrollLevelTotals;
        private static List<int> wandLevelTotals;

        private static SortedDictionary<int, GemChart> gemTypeChart;
        private static SortedDictionary<int, ArtChart> artChart;

        private static RandomWeightChart<RandomItemType> minorChart;
        private static RandomWeightChart<RandomItemType> mediumChart;
        private static RandomWeightChart<RandomItemType> majorChart;

        private static List<Equipment> equipmentByVal;

        private static Random rand = new Random();


        static TreasureGenerator()
        {
            LoadEquipmentChart();
            LoadTreasureChart();
            LoadGemChart();
            LoadArtChart();
            LoadRandomItemChart();
            LoadSpellItemCharts();


        }

        private static void LoadSpellItemCharts()
        {
            potionChart = new SortedDictionary<int, List<Spell>>();
            arcaneScrollChart = new SortedDictionary<int, List<Spell>>();
            divineScrollChart = new SortedDictionary<int, List<Spell>>();
            wandChart = new SortedDictionary<int, List<Spell>>();

            potionLevelTotals = new List<int>();
            arcaneScrollLevelTotals = new List<int>();
            divineScrollLevelTotals = new List<int>();
            wandLevelTotals = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                potionChart[i] = new List<Spell>();
                potionLevelTotals.Add(0);
            }
            for (int i = 0; i < 10; i++)
            {
                arcaneScrollChart[i] = new List<Spell>();
                arcaneScrollLevelTotals.Add(0);
            }
            for (int i = 0; i < 10; i++)
            {
                divineScrollChart[i] = new List<Spell>();
                divineScrollLevelTotals.Add(0);
            }
            for (int i = 0; i < 5; i++)
            {
                wandChart[i] =  new List<Spell>();
                wandLevelTotals.Add(0);
            }

            foreach (Spell spell in Spell.Spells)
            {
                if (spell.PotionWeight != null && spell.PotionLevel != null)
                {
                    int iLevel;
                    int weight;
                    if (int.TryParse(spell.PotionLevel, out iLevel) && int.TryParse(spell.PotionWeight, out weight))
                    {
                        potionChart[iLevel].Add(spell);
                        potionLevelTotals[iLevel] = potionLevelTotals[iLevel] + weight;
                    }

                }
                if (spell.ArcaneScrollWeight != null && spell.ArcaneScrollLevel != null)
                {
                    try
                    {
                        int iLevel;
                        if (int.TryParse(spell.ArcaneScrollLevel, out iLevel))
                        {
                            arcaneScrollChart[iLevel].Add(spell);
                            arcaneScrollLevelTotals[iLevel] = arcaneScrollLevelTotals[iLevel] + int.Parse(spell.ArcaneScrollWeight);
                        }
                    }
                    catch (Exception)
                    {
                        System.Diagnostics.Debug.WriteLine(spell.name + " " + spell.ArcaneScrollLevel + " " + spell.ArcaneScrollCost + " " + spell.ArcaneScrollWeight);
                        throw;
                    }


                }
                if (spell.DivineScrollWeight != null && spell.DivineScrollLevel != null)
                {

                    int iLevel;
                    int weight;
                    if (int.TryParse(spell.DivineScrollLevel, out iLevel) &&  int.TryParse(spell.DivineScrollWeight, out weight))
                    {
                        divineScrollChart[iLevel].Add(spell);
                        divineScrollLevelTotals[iLevel] = divineScrollLevelTotals[iLevel] + weight;
                    }

                }
                if (spell.WandWeight != null && spell.WandLevel != null)
                {
                    int iLevel;
                    int weight;
                    if (int.TryParse(spell.WandLevel, out iLevel) && int.TryParse(spell.WandWeight, out weight))
                    {
                        wandChart[iLevel].Add(spell);
                        wandLevelTotals[iLevel] = wandLevelTotals[iLevel] + weight;
                    }
                }
            }
        }

        private static void LoadTreasureChart()
        {

            List<TreasureChart> list = XmlListLoader<TreasureChart>.Load("TreasureChart.xml");


            coinChart = new SortedDictionary<int, SortedDictionary<int, string>>();
            goodsChart = new SortedDictionary<int, SortedDictionary<int, string>>();
            itemChart = new SortedDictionary<int, SortedDictionary<int, string>>();
            foreach (TreasureChart item in list)
            {
                if (item.CoinRoll != null)
                {
                    if (!coinChart.ContainsKey(item.Level))
                    {
                        coinChart[item.Level] = new SortedDictionary<int, string>();
                    }
                    coinChart[item.Level][item.CoinRoll.Value] = item.CoinValue;
                }
                if (item.GoodsRoll != null)
                {
                    if (!goodsChart.ContainsKey(item.Level))
                    {
                        goodsChart[item.Level] = new SortedDictionary<int, string>();
                    }
                    goodsChart[item.Level][item.GoodsRoll.Value] = item.GoodsValue;
                }
                if (item.ItemsRoll != null)
                {
                    if (!itemChart.ContainsKey(item.Level))
                    {
                        itemChart[item.Level] = new SortedDictionary<int, string>();
                    }
                    itemChart[item.Level][item.ItemsRoll.Value] = item.ItemsValue;
                }

            }
        }

        private static void LoadGemChart()
        {
            List<GemChart> list = XmlListLoader<GemChart>.Load("GemChart.xml");
            gemTypeChart = new SortedDictionary<int, GemChart>();

            foreach (GemChart item in list)
            {
                gemTypeChart.Add(item.Roll, item);
            }

        }

        private static void LoadArtChart()
        {
            List<ArtChart> list = XmlListLoader<ArtChart>.Load("ArtChart.xml");
            artChart = new SortedDictionary<int, ArtChart>();

            foreach (ArtChart item in list)
            {
                artChart.Add(item.Roll, item);
            }

        }

        private static void LoadEquipmentChart()
        {
            equipmentByVal = new List<Equipment>();

            string[] validTypes = new string[] {
                "Adventuring Gear",
                "Tools and Skill Kits",
                "Special Substances",
                "Clothing",
                "Black Market"};

            foreach (Equipment eq in Equipment.AllItems)
            {
                if (validTypes.Contains(eq.Type))
                {
                    if (!eq.Name.StartsWith("Slave") && !eq.Name.StartsWith("Tatoo"))
                    {
                        equipmentByVal.Add(eq);
                    }
                }
            }

            equipmentByVal.Sort((a, b) => a.CoinCost.GPValue.CompareTo(b.CoinCost.GPValue));
        }

        [Flags]
        public enum RandomItemType
        {
            None = 0x0,
            Mundane10 = 0x1,
            Mundane11t50 = 0x2,
            Mundane51t100 = 0x4,
            Mundane100 = 0x8,
            Armor = 0x10,
            Weapon = 0x20,
            Potion = 0x40,
            Scroll = 0x80,
            MinorWondrous = 0x100,
            MagicalArmor = 0x200,
            MagicalWeapon = 0x400,
            Wand = 0x800,
            Ring = 0x1000,
            MediumWondrous = 0x2000,
            Rod = 0x4000,
            Staff = 0x8000,
            MajorWondrous = 0x10000
        }

        public static String RandomItemString(RandomItemType type)
        {
            switch (type)
            {
            case RandomItemType.Potion:
                return "Potion";
            case RandomItemType.Scroll:
                return "Scroll";
            case RandomItemType.MinorWondrous:
                return "Wondrous Item";
            case RandomItemType.MagicalArmor:
                return "Armor";
            case RandomItemType.MagicalWeapon:
                return "Weapon";
            case RandomItemType.Wand:
                return "Wand";
            case RandomItemType.Ring:
                return "Ring";
            case RandomItemType.MediumWondrous:
                return "Wondrous Item";
            case RandomItemType.Rod:
                return "Rod";
            case RandomItemType.Staff:
                return "Staff";
            case RandomItemType.MajorWondrous:
                return "Wondrous Item";

            }
            return "";
        }


        private static void LoadRandomItemChart()
        {
            minorChart = new RandomWeightChart<RandomItemType>();
            mediumChart = new RandomWeightChart<RandomItemType>();
            majorChart = new RandomWeightChart<RandomItemType>();

            /*minorChart.AddItem(16, RandomItemType.Mundane10);
            minorChart.AddItem(14, RandomItemType.Mundane11t50);
            minorChart.AddItem(10, RandomItemType.Mundane51t100);
            minorChart.AddItem(4, RandomItemType.Mundane100);*/
            minorChart.AddItem(11, RandomItemType.Armor);
            minorChart.AddItem(14, RandomItemType.Weapon);
            minorChart.AddItem(8, RandomItemType.Potion);
            minorChart.AddItem(6, RandomItemType.Scroll);
            minorChart.AddItem(5, RandomItemType.MinorWondrous);
            minorChart.AddItem(3, RandomItemType.MagicalArmor);
            minorChart.AddItem(5, RandomItemType.MagicalWeapon);
            minorChart.AddItem(2, RandomItemType.Wand);
            minorChart.AddItem(2, RandomItemType.Ring);

            /*mediumChart.AddItem(4, RandomItemType.Mundane51t100);
            mediumChart.AddItem(10, RandomItemType.Mundane100);*/
            mediumChart.AddItem(2, RandomItemType.Armor);
            mediumChart.AddItem(3, RandomItemType.Weapon);
            mediumChart.AddItem(2, RandomItemType.Rod);
            mediumChart.AddItem(2, RandomItemType.Staff);
            mediumChart.AddItem(12, RandomItemType.Potion);
            mediumChart.AddItem(10, RandomItemType.Scroll);
            mediumChart.AddItem(8, RandomItemType.MinorWondrous);
            mediumChart.AddItem(15, RandomItemType.MagicalArmor);
            mediumChart.AddItem(15, RandomItemType.MagicalWeapon);
            mediumChart.AddItem(8, RandomItemType.Wand);
            mediumChart.AddItem(4, RandomItemType.Ring);
            mediumChart.AddItem(5, RandomItemType.MediumWondrous);



            majorChart.AddItem(10, RandomItemType.Potion);
            majorChart.AddItem(12, RandomItemType.Scroll);
            majorChart.AddItem(4, RandomItemType.MinorWondrous);
            majorChart.AddItem(12, RandomItemType.MagicalArmor);
            majorChart.AddItem(18, RandomItemType.MagicalWeapon);
            majorChart.AddItem(10, RandomItemType.Wand);
            majorChart.AddItem(8, RandomItemType.Ring);
            majorChart.AddItem(10, RandomItemType.MediumWondrous);
            majorChart.AddItem(6, RandomItemType.Rod);
            majorChart.AddItem(4, RandomItemType.Staff);
            majorChart.AddItem(6, RandomItemType.MajorWondrous);
                       
        }             


        public TreasureGenerator()
        {            
            _Level = 1;
            _Coin = 1;
            _Goods = 1;
            _Items = 1;
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

        public Treasure Generate()
        {
            Treasure treasure = new Treasure();
            treasure.Coin = new Coin();

            if (_Coin > 0)
            {
                for (int i = 0; i < _Coin; i++)
                {
                    treasure.Coin = treasure.Coin + GenerateCoin();
                }
                
            }

            if (_Goods > 0)
            {
                for (int i = 0; i < _Goods; i++)
                {
                    treasure.Goods.AddRange(GenerateGoods());
                }
            }
            treasure.Goods.Sort((a, b) => a.Name.CompareTo(b.Name));

            if (_Items > 0)
            {
                for (int i = 0; i < _Items; i++)
                {
                    treasure.Items.AddRange(GenerateItems());
                }
            }

            treasure.Items.Sort((a, b) => a.Name.CompareTo(b.Name));

            return treasure;
        }

        private Coin GenerateCoin()
        {
            int roll = rand.Next(1, 101);

            int actualLevel = Math.Min(Level, 20);

            string coinString = null;

            foreach (KeyValuePair<int, string> pair in coinChart[actualLevel])
            {
                if (pair.Key >= roll)
                {
                    coinString = pair.Value;
                    break;
                }
            }


            return RandomCoinFromString(coinString);
        }

        private List<Good> GenerateGoods()
        {
            int roll = rand.Next(1, 101);

            int actualLevel = Math.Min(Level, 20);


            string goodsString = null;

            foreach (KeyValuePair<int, string> pair in goodsChart[actualLevel])
            {
                if (pair.Key >= roll)
                {
                    goodsString = pair.Value;
                    break;
                }
            }

            return RandomGoodsFromString(goodsString);

        }

        private List<TreasureItem> GenerateItems()
        {


            int roll = rand.Next(1, 101);

            int actualLevel = Math.Min(Level, 20);

            string itemString = null;

            foreach (KeyValuePair<int, string> pair in itemChart[actualLevel])
            {
                if (pair.Key >= roll)
                {
                    itemString = pair.Value;
                    break;
                }
            }


            return RandomItemsFromString(itemString);
        }



        public static Coin RandomCoinFromString(string coinString)
        {
            Coin coin = new Coin();
            Regex coinVal = new Regex("(?<die>[0-9]+d[0-9]+)(?<hasbase> (x|×) (?<base>[0-9]+))? (?<type>(pp|gp|sp|cp))");

            if (coinString != null)
            {
                Match m = coinVal.Match(coinString);

                if (m.Success)
                {
                    string die = m.Groups["die"].Value;

                    int baseVal = 1;

                    if (m.Groups["hasbase"].Success)
                    {
                        baseVal = int.Parse(m.Groups["base"].Value);
                    }
                    string type = m.Groups["type"].Value;

                    DieRoll dieroll = DieRoll.FromString(die);

                    double mult = dieroll.Step.RollDouble(false);
                    int val = (int)(mult * (double)baseVal);

                    if (type == "pp")
                    {
                        coin.PP = val;
                    }
                    else if (type == "gp")
                    {
                        coin.GP = val;
                    }
                    else if (type == "sp")
                    {
                        coin.SP = val;
                    }
                    else if (type == "cp")
                    {
                        coin.CP = val;
                    }

                }
            }
            return coin;
        }

        public List<Good> RandomGoodsFromString(string goodsString)
        {
            List<Good> goods = new List<Good>();

            if (goodsString != null)
            {
                Regex regGoods = new Regex("((?<single>1)|(?<die>[0-9]+d[0-9]+)) ((?<type>art|gem))");


                Match m = regGoods.Match(goodsString);

                if (m.Success)
                {
                    int count = 0;

                    if (m.Groups["single"].Success)
                    {
                        count = 1;
                    }
                    else
                    {
                        DieRoll roll = DieRoll.FromString(m.Groups["die"].Value);

                        count = roll.Step.Roll();
                    }

                    bool gems = m.Groups["type"].Value == "gem";

                    for (int i = 0; i < count; i++)
                    {
                        goods.Add(gems ? GenerateRandomGem() : GenerateRandomArt());
                    }

                }
            }

            return goods;
        }

        public List<TreasureItem> RandomItemsFromString(string itemString)
        {
            List<TreasureItem> items = new List<TreasureItem>();

            if (itemString != null)
            {
                Regex regGoods = new Regex("((?<single>1)|(?<die>[0-9]+d[0-9]+)) ((?<type>mundane|minor|medium|major))");

                Match m = regGoods.Match(itemString);

                if (m.Success)
                {
                    int count = 0;

                    if (m.Groups["single"].Success)
                    {
                        count = 1;
                    }
                    else
                    {
                        DieRoll roll = DieRoll.FromString(m.Groups["die"].Value);

                        count = roll.Step.Roll();
                    }

                    string type = m.Groups["type"].Value;


                    for (int i = 0; i < count; i++)
                    {
                        if (type == "mundane")
                        {

                        }
                        else if (type == "minor")
                        {
                            TreasureItem item = GenerateRandomItem(ItemLevel.Minor);
                            if (item != null)
                            {
                                items.Add(item);
                            }
                        }
                        else if (type == "medium")
                        {

                            TreasureItem item = GenerateRandomItem(ItemLevel.Medium);
                            if (item != null)
                            {
                                items.Add(item);
                            }
                        }
                        else if (type == "major")
                        {
                            TreasureItem item = GenerateRandomItem(ItemLevel.Major);
                            if (item != null)
                            {
                                items.Add(item);
                            }

                        }
                    }

                }
            }

            return items;
        }

        public Good GenerateRandomGem()
        {
            Good g = new Good();

            int roll = rand.Next(1, 101);

            foreach (KeyValuePair<int, GemChart> pair in gemTypeChart)
            {
                if (pair.Key >= roll)
                {
                    GemChart chart = pair.Value;

                    g.Value = RandomCoinFromString(chart.Value).GP;
                    g.Name = RandomItemFromList(chart.Gem);

                    break;
                }
            }

            return g;
        }

        public Good GenerateRandomArt()
        {
            Good g = new Good();

            int roll = rand.Next(1, 101);

            foreach (KeyValuePair<int, ArtChart> pair in artChart)
            {
                if (pair.Key >= roll)
                {
                    ArtChart chart = pair.Value;

                    g.Value = RandomCoinFromString(chart.Value).GP; ;
                    g.Name = RandomItemFromList(chart.Art);

                    break;
                }
            }

            return g;

        }

        
        public TreasureItem GenerateRandomItem(ItemLevel level)
        {
            RandomItemType types = 0;
            foreach (RandomItemType type in Enum.GetValues(typeof(RandomItemType)))
            {
                types |= type;
            }
            return GenerateRandomItem(level, types);
        }

        public TreasureItem GenerateRandomItem(ItemLevel level, RandomItemType types)
        {
            TreasureItem item = null;


            RandomWeightChart<RandomItemType> list;

            switch (level)
            {
                case ItemLevel.Minor:
                    list = minorChart;
                    break;
                case ItemLevel.Medium:
                    list = mediumChart;
                    break;
                case ItemLevel.Major:
                default:
                    list = majorChart;
                    break;
            }

            while (item == null)
            {

                RandomItemType type = list.GetRandomItem();

                if (types.HasFlag(type))
                {

                    switch (type)
                    {
                        case RandomItemType.Mundane10:
                            item = GenerateMundaneEquipment(0, 10);
                            break;
                        case RandomItemType.Mundane11t50:
                            item = GenerateMundaneEquipment(11, 50);
                            break;
                        case RandomItemType.Mundane51t100:
                            item = GenerateMundaneEquipment(51, 100);
                            break;
                        case RandomItemType.Mundane100:
                            item = GenerateMundaneEquipment(100, int.MaxValue);
                            break;
                        case RandomItemType.Armor:
                            item = GenerateArmor(true);
                            break;
                        case RandomItemType.Weapon:
                            item = GenerateWeapon(true);
                            break;
                        case RandomItemType.Scroll:
                            item = GenerateScroll(level);
                            break;
                        case RandomItemType.Potion:
                            item = GeneratePotion(level);
                            break;
                        case RandomItemType.MagicalArmor:
                            item = GenerateMagicalArmor(level);
                            break;
                        case RandomItemType.MagicalWeapon:
                            item = GenerateMagicalWeapon(level);
                            break;
                        case RandomItemType.MajorWondrous:
                            item = GenerateWondrousItem(ItemLevel.Major);
                            break;
                        case RandomItemType.MediumWondrous:
                            item = GenerateWondrousItem(ItemLevel.Medium);
                            break;
                        case RandomItemType.MinorWondrous:
                            item = GenerateWondrousItem(ItemLevel.Minor);
                            break;
                        case RandomItemType.Rod:
                            item = GenerateRod(level);
                            break;
                        case RandomItemType.Staff:
                            item = GenerateStaff(level);
                            break;
                        case RandomItemType.Wand:
                            item = GenerateWand(level);
                            break;
                        case RandomItemType.Ring:
                            item = GenerateRing(level);
                            break;
                    }
                }
            }

            return item;

        }

        public TreasureItem GenerateMundaneEquipment(int minval, int maxval)
        {
            TreasureItem item = null;

            int minItem = -1;
            int maxItem = -1;

            for (int i = 0; i < equipmentByVal.Count; i++)
            {
                if (minItem == -1 && equipmentByVal[i].CoinCost.GPValue > minval)
                {
                    minItem = i;
                }
                if (maxItem == -1 && equipmentByVal[i].CoinCost.GPValue > maxval)
                {
                    maxItem = i;
                }
            }

            if (minItem == -1)
            {
                minItem = 0;
            }
            if (maxItem == -1)
            {
                maxItem = equipmentByVal.Count;
            }

            if (minItem < maxItem)
            {

                int roll = rand.Next(minItem, maxItem);
                Equipment eq = equipmentByVal[roll];

                item = new TreasureItem();
                item.Equipment = eq;
                item.Name = eq.Name;
                item.Value = eq.CoinCost.GPValue;
                item.Type = "Equipment";
            }

            return item;
        }

        public TreasureItem GenerateArmor(bool masterwork)
        {
            int roll = rand.Next(1, 101);

            string type = "";

            if (roll <= 55)
            {
                type = "Armor";
            }
            else if (roll <= 100)
            {
                type = "Shield";
            }



            return GetArmorWeaponChartItem("Armor", type, masterwork);
        }


        public TreasureItem GenerateWeapon(bool masterwork)
        {
            int roll = rand.Next(1, 101);

            string type = "";

            if (roll <= 45)
            {
                type = "Simple";
            }
            else if (roll <= 80)
            {
                type = "Martial";
            }
            else if (roll <= 100)
            {
                type = "Exotic";
            }



            return GetArmorWeaponChartItem("Weapon", type, masterwork);
        }

        private TreasureItem GetArmorWeaponChartItem(string type, string subtype, bool masterwork)
        {
            TreasureItem item = null;

            int val = ArmorWeaponChart.TotalWeights[subtype];

            string withName = null;
            int withValue = 0;
            while (item == null)
            {


                int totalWeight = 0;
                int cRoll = rand.Next(1, val);
                foreach (ArmorWeaponChart chart in
                        ArmorWeaponChart.Chart.Where(a => string.Compare(a.Type, subtype) == 0))
                {
                    totalWeight += int.Parse(chart.Weight);

                    if (totalWeight >= cRoll)
                    {

                        if (chart.Name.StartsWith("with"))
                        {
                            if (withName == null)
                            {
                                withName = chart.Name;
                                withValue = GPToInt(chart.Cost);
                            }

                        }
                        else
                        {
                            item = new TreasureItem();
                            item.Name = (masterwork ? "Masterwork " : "") + chart.Name + ((withName != null) ? (" " + withName) : "");

                            item.Type = type;
                            item.Value = GPToInt(chart.Cost) + withValue;

                        }
                        break;

                    }
                }
            }

            return item;

        }

        public TreasureItem GeneratePotion(ItemLevel level)
        {
            int roll = rand.Next(1, 101);

            int potionLevel = 0;

            switch (level)
            {
                case ItemLevel.Minor:
                    if (roll <= 20)
                    {
                        potionLevel = 0;
                    }
                    else if (roll <= 60)
                    {
                        potionLevel = 1;
                    }
                    else if (roll <= 100)
                    {
                        potionLevel = 2;
                    }

                    break;
                case ItemLevel.Medium:
                    if (roll <= 20)
                    {
                        potionLevel = 1;
                    }
                    else if (roll <= 60)
                    {
                        potionLevel = 2;
                    }
                    else if (roll <= 100)
                    {
                        potionLevel = 3;
                    }

                    break;
                case ItemLevel.Major:
                    if (roll <= 20)
                    {
                        potionLevel = 2;
                    }
                    else if (roll <= 100)
                    {
                        potionLevel = 3;
                    }

                    break;

            }


            return GeneratePotionOfLevel(potionLevel);
        }
        public TreasureItem GeneratePotionOfLevel(int level)
        {
            TreasureItem item = null;
            Spell spell = null;

            int roll = rand.Next(0, potionLevelTotals[level]);
            int current = 0;
            foreach (Spell val in potionChart[level])
            {
                current += int.Parse(val.PotionWeight);

                if (current > roll)
                {
                    spell = val;
                    break;
                }
            }

            if (spell != null)
            {
                item = new TreasureItem();
                item.Spell = spell;
                item.Type = "Potion";
                item.Name = "Potion of " + spell.name;
                item.Value = GPToInt(spell.PotionCost);
            }

            return item;
        }
        public TreasureItem GenerateArcaneScrollOfLevel(int level)
        {
            TreasureItem item = null;
            Spell spell = null;

            int roll = rand.Next(0, arcaneScrollLevelTotals[level]);
            int current = 0;
            foreach (Spell val in arcaneScrollChart[level])
            {
                current += int.Parse(val.ArcaneScrollWeight);

                if (current > roll)
                {
                    spell = val;
                    break;
                }
            }

            if (spell != null)
            {
                item = new TreasureItem();
                item.Spell = spell;
                item.Type = "Scroll";
                item.Name = "Scroll of " + spell.name;
                item.Value = GPToInt(spell.ArcaneScrollCost);
            }

            return item;
        }
        public TreasureItem GenerateDivineScrollOfLevel(int level)
        {
            TreasureItem item = null;
            Spell spell = null;

            int roll = rand.Next(0, divineScrollLevelTotals[level]);
            int current = 0;
            foreach (Spell val in divineScrollChart[level])
            {
                current += int.Parse(val.DivineScrollWeight);

                if (current > roll)
                {
                    spell = val;
                    break;
                }
            }

            if (spell != null)
            {
                item = new TreasureItem();
                item.Spell = spell;
                item.Type = "Scroll";
                item.Name = "Scroll of " + spell.name;
                item.Value = GPToInt(spell.DivineScrollCost);
            }

            return item;
        }

        public TreasureItem GenerateWandOfLevel(int level, bool fullCharge)
        {
            TreasureItem item = null;
            Spell spell = null;

            int roll = rand.Next(0, wandLevelTotals[level]);
            int current = 0;
            foreach (Spell val in wandChart[level])
            {
                current += int.Parse(val.WandWeight);

                if (current > roll)
                {
                    spell = val;
                    break;
                }
            }

            if (spell != null)
            {
                int charges = 50;

                if (!fullCharge)
                {
                    rand.Next(1, 51);
                }


                item = new TreasureItem(); 
                item.Spell = spell;
                item.Type = "Wand";
                item.Name = "Wand of " + spell.name + ", " + charges + " charges";
                item.Value = GPToInt(spell.WandCost);

                if (charges < 50)
                {
                    item.Value = item.Value * charges / 50;
                }
            }

            return item;
        }

        public static int GPToInt(string gp)
        {
            int val = 0;

            if (gp != null)
            {

                string parseGP = Regex.Replace(gp, "gp|,|\\+", "");

                int.TryParse(parseGP, out val);
            }

            return val;
        }

        public TreasureItem GenerateScroll(ItemLevel level)
        {

            int roll = rand.Next(1, 101);

            if (roll <= 70)
            {
                return GenerateScroll(level, true);
            }
            else
            {
                return GenerateScroll(level, false);
            }

        }

        public TreasureItem GenerateScroll(ItemLevel level, bool arcane)
        {

            int roll = rand.Next(1, 101);

            int scrollLevel = 0;

            switch (level)
            {
                case ItemLevel.Minor:
                    if (roll <= 5)
                    {
                        scrollLevel = 0;
                    }
                    else if (roll <= 50)
                    {
                        scrollLevel = 1;
                    }
                    else if (roll <= 95)
                    {
                        scrollLevel = 2;
                    }
                    else if (roll <= 100)
                    {
                        scrollLevel = 3;
                    }

                    break;
                case ItemLevel.Medium:
                    if (roll <= 5)
                    {
                        scrollLevel = 2;
                    }
                    else if (roll <= 65)
                    {
                        scrollLevel = 3;
                    }
                    else if (roll <= 95)
                    {
                        scrollLevel = 4;
                    }
                    else if (roll <= 100)
                    {
                        scrollLevel = 5;
                    }

                    break;
                case ItemLevel.Major:
                    if (roll <= 5)
                    {
                        scrollLevel = 4;
                    }
                    else if (roll <= 50)
                    {
                        scrollLevel = 5;
                    }
                    else if (roll <= 70)
                    {
                        scrollLevel = 6;
                    }
                    else if (roll <= 86)
                    {
                        scrollLevel = 7;
                    }
                    else if (roll <= 95)
                    {
                        scrollLevel = 8;
                    }
                    else if (roll <= 100)
                    {
                        scrollLevel = 9;
                    }

                    break;
            }

            if (arcane)
            {
                return GenerateArcaneScrollOfLevel(scrollLevel);
            }
            else
            {
                return GenerateDivineScrollOfLevel(scrollLevel);
            }
        }
        public TreasureItem GenerateWondrousItem(ItemLevel level)
        {
            TreasureItem item = new TreasureItem();

            SpecificItemChart chart = GenerateSpecificItem(level, "Wondrous Item");

            item.Name = chart.Name;

            if (MagicItem.Items.ContainsKey(chart.Name))
            {
                item.MagicItem = MagicItem.Items[chart.Name];
            }

            item.Value = GPToInt(chart.Cost);
            item.Type = "Wondrous Item";

            return item;
        }

        public TreasureItem GenerateMagicalWeapon(ItemLevel level)
        {
            TreasureItem item = null;


            item = GenerateWeapon(false);

            int bonus = 0;
            int specialBonus = 0;
            int specialCost = 0;
            bool specificWeapon = false;
            string weaponSpecial = "";

            RunSpecialType addSpecial = delegate()
            {
                int bonusIncrease = 0;
                int costIncrease = 0;

                Weapon wp = Weapon.Find(item.Name);

                string type = "Melee";

                if (wp != null && wp.Ranged)
                {
                    type = "Ranged";
                }

                

                string newSpecial = GenerateSpecial(type, level, weaponSpecial, out bonusIncrease, out costIncrease);

                if (bonusIncrease + specialBonus <= 9)
                {
                    if (weaponSpecial.Length > 0)
                    {
                        weaponSpecial += " ";
                    }
                    weaponSpecial += newSpecial;
                    specialBonus += bonusIncrease;
                    specialCost += costIncrease;
                }
            };



            while (bonus == 0 && !specificWeapon)
            {
                int roll = rand.Next(1, 101);
                switch (level)
                {
                    case ItemLevel.Minor:
                        if (roll <= 70)
                        {
                            bonus = 1;
                        }
                        else if (roll <= 85)
                        {
                            bonus = 2;
                        }
                        else if (roll <= 90)
                        {
                            specificWeapon = true;
                        }
                        else if (roll <= 100)
                        {
                            addSpecial();
                        }
                        break;
                    case ItemLevel.Medium:
                        if (roll <= 10)
                        {
                            bonus = 1;
                        }
                        else if (roll <= 29)
                        {
                            bonus = 2;
                        }
                        else if (roll <= 58)
                        {
                            bonus = 3;
                        }
                        else if (roll <= 62)
                        {
                            bonus = 4;
                        }
                        else if (roll <= 68)
                        {
                            specificWeapon = true;
                        }
                        else if (roll <= 100)
                        {
                            addSpecial();
                        }
                        break;
                    case ItemLevel.Major:
                        if (roll <= 20)
                        {
                            bonus = 3;
                        }
                        else if (roll <= 38)
                        {
                            bonus = 4;
                        }
                        else if (roll <= 49)
                        {
                            bonus = 5;
                        }
                        else if (roll <= 63)
                        {
                            specificWeapon = true;
                        }
                        else if (roll <= 100)
                        {
                            addSpecial();
                        }
                        break;
                }
            }

            if (bonus > 0)
            {
                if (bonus + specialBonus > 10)
                {
                    bonus = 10 - specialBonus;
                }


                item.Name = "+" + bonus + ((weaponSpecial.Length > 0) ? (" " + weaponSpecial + " ") : " ") + item.Name;
                item.Type = "Magical Weapon";
                item.Value = item.Value + BonusGPValue(bonus + specialBonus, true) + specialCost;
            }
            else if (specificWeapon)
            {
                SpecificItemChart chart = GenerateSpecificItem(level, "Weapon");

                item.Name = chart.Name;
                item.Value = GPToInt(chart.Cost);
                item.Type = "Magical Weapon";
                if (MagicItem.Items.ContainsKey(chart.Name))
                {
                    item.MagicItem = MagicItem.Items[chart.Name];
                }

            }


            return item;
        }

        delegate void RunSpecialType();

        public TreasureItem GenerateMagicalArmor(ItemLevel level)
        {
            TreasureItem item = null;

            //genrate item used (if not specific armor or sheild)
            item = GenerateArmor(false);


            int bonus = 0;
            int specialBonus = 0;
            int specialCost = 0;
            bool specificShield = false;
            bool specificArmor = false;
            string weaponSpecial = "";

            RunSpecialType addSpecial = delegate()
                {
                    int bonusIncrease = 0;
                    int costIncrease = 0;
                    string newSpecial = GenerateSpecial(item.Type, level, weaponSpecial, out bonusIncrease, out costIncrease);


                    if (bonusIncrease + specialBonus <= 9)
                    {
                        if (weaponSpecial.Length > 0)
                        {
                            weaponSpecial += " ";
                        }
                        weaponSpecial += newSpecial;
                        specialBonus += bonusIncrease;
                        specialCost += costIncrease;
                    }
                };



            while (bonus == 0 && !specificArmor && !specificShield)
            {

                int roll = rand.Next(1, 101);
                switch (level)
                {
                    case ItemLevel.Minor:
                        if (roll <= 80)
                        {
                            bonus = 1;
                        }
                        else if (roll <= 87)
                        {
                            bonus = 2;
                        }
                        else if (roll <= 89)
                        {
                            specificArmor = true;
                        }
                        else if (roll <= 91)
                        {
                            specificShield = true;
                        }
                        else if (roll <= 100)
                        {
                            addSpecial();
                        }
                        break;
                    case ItemLevel.Medium:
                        if (roll <= 10)
                        {
                            bonus = 1;
                        }
                        else if (roll <= 30)
                        {
                            bonus = 2;
                        }
                        else if (roll <= 50)
                        {
                            bonus = 3;
                        }
                        else if (roll <= 57)
                        {
                            bonus = 4;
                        }
                        else if (roll <= 60)
                        {
                            specificArmor = true;
                        }
                        else if (roll <= 63)
                        {
                            specificShield = true;
                        }
                        else if (roll <= 100)
                        {
                            addSpecial();
                        }
                        break;
                    case ItemLevel.Major:
                        if (roll <= 16)
                        {
                            bonus = 3;
                        }
                        else if (roll <= 38)
                        {
                            bonus = 4;
                        }
                        else if (roll <= 57)
                        {
                            bonus = 5;
                        }
                        else if (roll <= 60)
                        {
                            specificArmor = true;
                        }
                        else if (roll <= 63)
                        {
                            specificShield = true;
                        }
                        else if (roll <= 100)
                        {
                            addSpecial();
                        }
                        break;
                }
            }

            if (bonus > 0)
            {
                if (bonus + specialBonus > 10)
                {
                    bonus = 10 - specialBonus;
                }

                item.Name = "+" + bonus + ((weaponSpecial.Length > 0) ? (" " + weaponSpecial + " ") : " ") + item.Name;
                item.Type = "Magical " + item.Type;
                item.Value = item.Value + BonusGPValue(bonus + specialBonus, false) + specialCost;
            }
            else if (specificArmor)
            {

                SpecificItemChart chart = GenerateSpecificItem(level, "Armor");
                if (MagicItem.Items.ContainsKey(chart.Name))
                {
                    item.MagicItem = MagicItem.Items[chart.Name];
                }

                item.Name = chart.Name;
                item.Value = GPToInt(chart.Cost);
                item.Type = "Magical Weapon";
            }
            else if (specificShield)
            {

                SpecificItemChart chart = GenerateSpecificItem(level, "Shield");

                item.Name = chart.Name;
                item.Value = GPToInt(chart.Cost);
                item.Type = "Magical Weapon";
            }


            return item;
        }


        public string GenerateSpecial(String type, ItemLevel level, string currentSpecial, out int bonusIncrease, out int costIncrease)
        {
            bonusIncrease = 0;
            costIncrease = 0;

            List<ArmorWeaponSpecialChart> subchart = new List<ArmorWeaponSpecialChart>(ArmorWeaponSpecialChart.Chart.Where(
                delegate(ArmorWeaponSpecialChart chart)
                {
                    if (chart.Type != type)
                    {
                        return false;
                    }

                    return chart.LevelWeight(level) != null;

                }));

            int roll = rand.Next(1, 101);

            ArmorWeaponSpecialChart special = null;

            int val = 0;

            foreach (ArmorWeaponSpecialChart chart in subchart)
            {
                val += int.Parse(chart.LevelWeight(level));

                if (val >= roll)
                {
                    special = chart;
                    break;
                }
            }

            string specOut = "";

            if (special == null)
            {
                //run twice
                int bonus = 0;
                int cost = 0;

                specOut = GenerateSpecial(type, level, currentSpecial, out bonus, out cost);                                                
                bonusIncrease += bonus;
                costIncrease += cost;

                currentSpecial = specOut + " " + currentSpecial;

                specOut += " " + GenerateSpecial(type, level, currentSpecial, out bonus, out cost);
                bonusIncrease += bonus;
                costIncrease += cost;

            }
            else
            {
                if (!currentSpecial.Contains(special.Name))
                {

                    specOut = special.Name;
                    if (special.Bonus != null && special.Bonus.Length > 0)
                    {
                        bonusIncrease = int.Parse(special.Bonus);
                    }
                    if (special.Cost != null && special.Cost.Length > 0)
                    {
                        costIncrease = int.Parse(special.Cost);
                    }
                }
            }

            return specOut;
        }


        public int BonusGPValue(int bonus, bool weapon)
        {
            return bonus*bonus*(weapon?2000:1000);
        }

        public TreasureItem GenerateWand(ItemLevel level)
        {
           int roll = rand.Next(1, 101);

            int wandLevel = 0;

            switch (level)
            {
                case ItemLevel.Minor:
                    if (roll <= 5)
                    {
                        wandLevel = 0;
                    }
                    else if (roll <= 60)
                    {
                        wandLevel = 1;
                    }
                    else if (roll <= 100)
                    {
                        wandLevel = 2;
                    }
                    break;
                case ItemLevel.Medium:
                    if (roll <= 60)
                    {
                        wandLevel = 2;
                    }
                    else if (roll <= 100)
                    {
                        wandLevel = 3;
                    }
                    break;
                case ItemLevel.Major:
                    if (roll <= 60)
                    {
                        wandLevel = 3;
                    }
                    else if (roll <= 100)
                    {
                        wandLevel = 4;
                    }
                    break;
            }
            return GenerateWandOfLevel(wandLevel, false);
        }
        public TreasureItem GenerateRing(ItemLevel level)
        {
            TreasureItem item = new TreasureItem();

            SpecificItemChart chart = GenerateSpecificItem(level, "Ring");
            item.Name = "Ring of " + chart.Name;
            if (MagicItem.Items.ContainsKey(item.Name))
            {
                item.MagicItem = MagicItem.Items[item.Name];
            }

            item.Value = GPToInt(chart.Cost);
            item.Type = "Ring";

            return item;
        }
        public TreasureItem GenerateRod(ItemLevel level)
        {
            TreasureItem item = new TreasureItem();

            SpecificItemChart chart = GenerateSpecificItem(level, "Rod");
            item.Name = "Rod of " + chart.Name;
            item.MagicItem = MagicItem.ByName(item.Name);
            item.Value = GPToInt(chart.Cost);
            item.Type = "Rod";

            return item;
        }
        public TreasureItem GenerateStaff(ItemLevel level)
        {
            TreasureItem item = new TreasureItem();

            SpecificItemChart chart = GenerateSpecificItem(level, "Staff");
            item.Name = "Staff of " + chart.Name;
            if (MagicItem.Items.ContainsKey(chart.Name))
            {
                item.MagicItem = MagicItem.Items[item.Name];
            }

            item.Value = GPToInt(chart.Cost);
            item.Type = "Staff";

            return item;
        }

        public SpecificItemChart GenerateSpecificItem(ItemLevel level, string type)
        {
            int roll = rand.Next(1, SpecificItemChart.ChartTotal(level, type) + 1);
            int val = 0;

            foreach (SpecificItemChart chart in SpecificItemChart.Subchart(level, type))
            {
                val += int.Parse(chart.LevelWeight(level));

                if (val >= roll)
                {
                    return chart;
                }
            }

            return null;


        }



        private static string RandomItemFromList(string items)
        {
            List<String> list = TreasureStringList(items);

            if (list.Count > 0)
            {
                return list[rand.Next(0, list.Count)];
            }

            return "";
        }

        public static List<String> TreasureStringList(string items)
        {
            List<String> list = new List<string>();

            Regex regList = new Regex("(?<value>.+?)(; |$)");

            foreach (Match m in regList.Matches(items))
            {
                list.Add(m.Groups["value"].Value);
            }

            return list;
        }

        public int Coin
        {
            get { return _Coin; }
            set
            {
                if (_Coin != value)
                {
                    _Coin = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Coin")); }
                }
            }
        }
        public int Goods
        {
            get { return _Goods; }
            set
            {
                if (_Goods != value)
                {
                    _Goods = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Goods")); }
                }
            }
        }
        public int Items
        {
            get { return _Items; }
            set
            {
                if (_Items != value)
                {
                    _Items = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Items")); }
                }
            }
        }

    }
    public class GemChart : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int _Roll;
        private String _Value;
        private int _AverageValue;
        private String _Gem;

        public int Roll
        {
            get { return _Roll; }
            set
            {
                if (_Roll != value)
                {
                    _Roll = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Roll")); }
                }
            }
        }
        public String Value
        {
            get { return _Value; }
            set
            {
                if (_Value != value)
                {
                    _Value = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Value")); }
                }
            }
        }
        public int AverageValue
        {
            get { return _AverageValue; }
            set
            {
                if (_AverageValue != value)
                {
                    _AverageValue = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("AverageValue")); }
                }
            }
        }
        public String Gem
        {
            get { return _Gem; }
            set
            {
                if (_Gem != value)
                {
                    _Gem = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Gem")); }
                }
            }
        }


    }

    public class ArtChart : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int _Roll;
        private String _Value;
        private int _AverageValue;
        private String _Art;

        public int Roll
        {
            get { return _Roll; }
            set
            {
                if (_Roll != value)
                {
                    _Roll = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Roll")); }
                }
            }
        }
        public String Value
        {
            get { return _Value; }
            set
            {
                if (_Value != value)
                {
                    _Value = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Value")); }
                }
            }
        }
        public int AverageValue
        {
            get { return _AverageValue; }
            set
            {
                if (_AverageValue != value)
                {
                    _AverageValue = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("AverageValue")); }
                }
            }
        }
        public String Art
        {
            get { return _Art; }
            set
            {
                if (_Art != value)
                {
                    _Art = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Art")); }
                }
            }
        }


    }

    public class TreasureChart : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private int _Level;
        private int? _CoinRoll;
        private String _CoinValue;
        private int? _GoodsRoll;
        private String _GoodsValue;
        private int? _ItemsRoll;
        private String _ItemsValue;

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
        public int? CoinRoll
        {
            get { return _CoinRoll; }
            set
            {
                if (_CoinRoll != value)
                {
                    _CoinRoll = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("CoinRoll")); }
                }
            }
        }
        public String CoinValue
        {
            get { return _CoinValue; }
            set
            {
                if (_CoinValue != value)
                {
                    _CoinValue = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("CoinValue")); }
                }
            }
        }
        public int? GoodsRoll
        {
            get { return _GoodsRoll; }
            set
            {
                if (_GoodsRoll != value)
                {
                    _GoodsRoll = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("GoodsRoll")); }
                }
            }
        }
        public String GoodsValue
        {
            get { return _GoodsValue; }
            set
            {
                if (_GoodsValue != value)
                {
                    _GoodsValue = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("GoodsValue")); }
                }
            }
        }
        public int? ItemsRoll
        {
            get { return _ItemsRoll; }
            set
            {
                if (_ItemsRoll != value)
                {
                    _ItemsRoll = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("ItemsRoll")); }
                }
            }
        }
        public String ItemsValue
        {
            get { return _ItemsValue; }
            set
            {
                if (_ItemsValue != value)
                {
                    _ItemsValue = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("ItemsValue")); }
                }
            }
        }


    }
}
