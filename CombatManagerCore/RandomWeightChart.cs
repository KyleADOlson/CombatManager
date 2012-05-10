/*
 *  RandomWeightChart.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CombatManager
{
    class WeightChartRandom
    {
        public static Random Rand;
        static WeightChartRandom()
        {
            Rand = new Random();
        }

        
    }

    public class RandomWeightChart<T>
    {
        private int _Total;

        private List<KeyValuePair<int, T>> _Items;



        public RandomWeightChart()
        {
            _Items = new List<KeyValuePair<int, T>>();
        }

        public void AddItem(int weight, T item)
        {
            System.Diagnostics.Debug.Assert(weight > 0);
            _Items.Add(new KeyValuePair<int,T>(weight, item));
            _Total += weight;
        }

        public T GetRandomItem()
        {
            T item = default(T);

            int val = WeightChartRandom.Rand.Next(_Total);

            int count = 0;
            int i = 0;
            while (count < _Total)
            {
                count += _Items[i].Key;

                if (val < count)
                {
                    item = _Items[i].Value;
                    break;
                }
                i++;
            }

            return item;
        }

    }
}
