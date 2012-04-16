using System;
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
