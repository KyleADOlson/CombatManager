/*
 *  DieRoll.cs
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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Globalization;
using System.IO;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace CombatManager
{
    [DataContract]
    public class RollResult
    {
        public RollResult()
        {
            Rolls = new List<DieResult>();
        }

        [DataMember]
        public int Total { get; set; }

        [DataMember]
        public List<DieResult> Rolls { get; set; }

        [DataMember]
        public int Mod { get; set; }

        public static RollResult operator+ (RollResult a, RollResult r)
        {
            RollResult nr = new RollResult();
            nr.Total = a.Total + r.Total;
            nr.Rolls.AddRange(a.Rolls);
            nr.Rolls.AddRange(r.Rolls);
            nr.Mod = a.Mod + r.Mod;

            return nr;
        }

    }


    [DataContract]
    public class DieResult
    {
        [DataMember]
        public int Die { get; set; }

        [DataMember]
        public int Result { get; set; }
    }


    [DataContract]
    public class DieRoll
    {
        private int _count;
        private int _fraction;
        private int _die;
        private int _mod;
        private List<DieStep> _extraRolls;




        private static Random rand = new Random();
        public static Random Rand
        {
            get
            {
                return rand;
            }
        }

        private class DieStepComparer : IEqualityComparer<DieStep>
        {
            public bool Equals(DieStep a, DieStep b)
            {
                return (a.Count == b.Count && a.Die == b.Die);
            }

            public int GetHashCode(DieStep a)
            {
                return a.Count << 16 | a.Die;
            }
        }

        private static Dictionary<DieStep, DieStep> stepUpList;
        private static Dictionary<DieStep, DieStep> stepDownList;

        static DieRoll()
        {
            //create steps
            stepUpList = new Dictionary<DieStep, DieStep>(new DieStepComparer());
            stepDownList = new Dictionary<DieStep, DieStep>(new DieStepComparer());

            stepUpList.Add(new DieStep(0, 1), new DieStep(1, 1));
            stepUpList.Add(new DieStep(1, 1), new DieStep(1, 2));
            stepUpList.Add(new DieStep(1, 2), new DieStep(1, 3));
            stepUpList.Add(new DieStep(1, 3), new DieStep(1, 4));
            stepUpList.Add(new DieStep(1, 4), new DieStep(1, 6));
            stepUpList.Add(new DieStep(1, 6), new DieStep(1, 8));
            stepUpList.Add(new DieStep(1, 8), new DieStep(2, 6));
            stepUpList.Add(new DieStep(2, 6), new DieStep(2, 8));
            stepUpList.Add(new DieStep(2, 8), new DieStep(4, 6));
            stepUpList.Add(new DieStep(4, 6), new DieStep(4, 8));

            foreach (KeyValuePair<DieStep, DieStep> step in stepUpList)
            {
                stepDownList.Add(step.Value, step.Key);
            }


            //other
            stepUpList.Add(new DieStep(2, 4), new DieStep(2, 6));
            stepDownList.Add(new DieStep(2, 4), new DieStep(1, 6));

            stepUpList.Add(new DieStep(1, 10), new DieStep(2, 8));
            stepDownList.Add(new DieStep(1, 10), new DieStep(1, 8));

            stepUpList.Add(new DieStep(1, 12), new DieStep(3, 6));
            stepDownList.Add(new DieStep(1, 12), new DieStep(1, 10));

            stepUpList.Add(new DieStep(3, 6), new DieStep(5, 6));
            stepDownList.Add(new DieStep(3, 6), new DieStep(1, 12));
        }

        public DieRoll()
        {
            fraction = 1;
        }

        public DieRoll(int count, int die, int mod)
        {
            this.count = count;
            this.die = die;
            this.mod = mod;
            fraction = 1;
        }
        public DieRoll(int count, int fraction, int die, int mod)
        {
            this.count = count;
            this.die = die;
            this.mod = mod;
            this.fraction = fraction;
        }

        public DieRoll(DieRoll old)
        {
            CopyFrom(old);
        }

        private void CopyFrom(DieRoll old)
        {

            if (old == null)
            {
                count = 0;
                die = 0;
                mod = 0;
            }
            else
            {
                count = old.count;
                fraction = old.fraction;
                die = old.die;
                mod = old.mod;


                if (old.extraRolls != null)
                {
                    extraRolls = new List<DieStep>();

                    foreach (DieStep step in old.extraRolls)
                    {
                        extraRolls.Add(new DieStep(old.count, old.die));
                    }
                }
                else
                {
                    extraRolls = null;
                }
            }
        }
		
		public override string ToString ()
		{
			return Text;	
		}

        public override int GetHashCode()
        {
            int val = (die << 12) ^ (count << 8) ^ (fraction << 4) ^ mod;

            if (extraRolls != null && extraRolls.Count > 0)
            {
                int extra = 0;

                foreach (DieStep step in extraRolls)
                {
                    extra ^= step.GetHashCode();
                }

                val |= (extra << 16);
            }

            return val;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(DieRoll))
            {
                return false;
            }
            DieRoll roll = (DieRoll)obj;

            if (roll.extraRolls == null ^ extraRolls == null)
            {
                return false;
            }

            if (extraRolls != null)
            {
                if (roll.extraRolls.Count != extraRolls.Count)
                {
                    return false;
                }

                for (int i = 0; i < extraRolls.Count; i++)
                {
                    if (roll.extraRolls[i] != extraRolls[i])
                    {
                        return false;
                    }
                }
            }

            return (roll.count == count && roll.die == die && roll.fraction == fraction && roll.mod == mod);
        }



        public static bool operator ==(DieRoll s1, DieRoll s2)
        {
            if ((((object)s1) == null) ^ (((object)s2) == null))
            {
                return false;
            }
            if (((object)s1) == null)
            {
                return true;
            }

            return s1.Equals(s2);
        }
        
        public static bool operator !=(DieRoll s1, DieRoll s2)
        {


            return !(s1 == s2);
        }

        public object Clone()
        {
            return new DieRoll(this);
        }

        [XmlIgnore]
        public DieStep Step
        {
            get
            {
                return new DieStep(this);
            }
            set
            {
                this.count = value.Count;
                this.die = value.Die;
            }
        }

        [XmlIgnore]
        public int TotalCount
        {
            get
            {
                int total = count;

                if (extraRolls != null)
                {
                    foreach (DieStep step in extraRolls)
                    {
                        total += step.Count;
                    }
                }

                return total;
            }
        }

        [DataMember]
        public string Text
        {
            get
            {
                string text = "";


                text += count;

                if (fraction > 1)
                {
                    text += "/" + fraction;
                }

                text += "d" +die;

                if (extraRolls != null && extraRolls.Count > 0)
                {
                    foreach (DieStep step in extraRolls)
                    {
                        text += "+" + step.Count + "d" + step.Die;
                    }
                }


                if (mod != 0)
                {
                    text += mod.PlusFormat();
                }


                return text;
            }
            set
            {
                CopyFrom(DieRoll.FromString(value));
            }
        }

        public static DieRoll StepDie(DieRoll roll, int diff)
        {
            DieRoll outRoll = new DieRoll(roll);
            if (diff > 0)
            {
                for (int i = 0; i < diff; i++)
                {
                    outRoll = StepUpDieRoll(outRoll);
                }
            }
            if (diff < 0)
            {
                for (int i = 0; i > diff; i--)
                {
                    outRoll = StepDownDieRoll(outRoll);
                }
            }

            return outRoll;
        }

        public static DieStep StepDie(DieStep step, int diff)
        {
            DieRoll r = new DieRoll(step.Count, step.Die, 0);
            return StepDie(r, diff).Step;

        }


        private static DieRoll StepUpDieRoll(DieRoll roll)
        {
            DieRoll outRoll = roll;

            DieStep step = new DieStep(roll.count, roll.die);

            if (stepUpList.ContainsKey(step))
            {
                step = stepUpList[step];
            }
            else
            {
                if (step.Count < 2)
                {
                    step.Count += 1;
                }
                else
                {
                    step.Count += 2;
                }
            }

            outRoll.count = step.Count;
            outRoll.die = step.Die;

            return outRoll;

        }

        private static DieRoll StepDownDieRoll(DieRoll roll)
        {
            DieRoll outRoll = roll;

            DieStep step = new DieStep(roll.count, roll.die);

            if (stepDownList.ContainsKey(step))
            {
                step = stepDownList[step];
            }
            else
            {
                if (step.Count > 3)
                {
                    step.Count -= 3;
                }
                else if (step.Count > 1)
                {
                    step.Count -= 1;
                }
                else if (step.Die > 1)
                {
                    {
                        step.Die -= 1;
                    }
                }
                else
                {
                    step.Count = 0;
                    step.Die = 1;
                }
            }

            outRoll.count = step.Count;
            outRoll.die = step.Die;

            return outRoll;

        }

        public static DieRoll FromString(string text)
        {
            return FromString(text, 0);
        }

        private const string DieRollRegexString = "([0-9]+)(/[0-9]+)?d([0-9]+)(?<extra>(\\+([0-9]+)d([0-9]+))*)((\\+|-)[0-9]+)?";


        public static DieRoll FromString(string text, int start)
        {
            DieRoll roll = null;
            if (text != null)
            {
                try
                {

                    Regex regRoll = new Regex(DieRollRegexString);

                    Match match = regRoll.Match(text, start);

                    if (match.Success)
                    {
                        roll = new DieRoll();

                        roll.count = int.Parse(match.Groups[1].Value);


                        if (match.Groups[2].Success)
                        {
                            roll.fraction = int.Parse(match.Groups[2].Value.Substring(1));
                        }
                        else
                        {
                            roll.fraction = 1;
                        }

                        roll.die = int.Parse(match.Groups[3].Value);


                        if (roll.die == 0)
                        {
                            throw new FormatException("Invalid Die Roll");
                        }

                        if (match.Groups["extra"].Success)
                        {
                            roll.extraRolls = new List<DieStep>();

                            Regex extraReg = new Regex("([0-9]+)d([0-9]+)");

                            foreach (Match d in extraReg.Matches(match.Groups["extra"].Value))
                            {
                                DieStep step = new DieStep();
                                step.Count = int.Parse(d.Groups[1].Value);
                                step.Die = int.Parse(d.Groups[2].Value);


                                if (step.Die == 0)
                                {
                                    throw new FormatException("Invalid Die Roll");
                                }

                                roll.extraRolls.Add(step);
                            }
                        }

                        if (match.Groups[7].Success)
                        {
                            roll.mod = int.Parse(match.Groups[7].Value);
                        }
                    }
                }
                catch(FormatException)
                {
                    roll = null;
                }
                catch (OverflowException)
                {
                    roll = null;
                }
            }

            return roll;
        }

        public int AverageRoll()
        {
            //return ((die + 1) * count) / 2 + mod;
            return (int) (AllRolls.Sum(roll => (Math.Floor((decimal) ((roll.Die + 1)*roll.Count))/2))+mod);
            
        }


        [XmlIgnore]
        public int count
        {
            get { return _count; }
            set
            {
                if (_count != value)
                {
                    _count = value;
                }
            }
        }


        [XmlIgnore]
        public int fraction
        {
            get { return _fraction; }
            set
            {
                if (_fraction != value)
                {
                    _fraction = value;
                }
            }
        }


        [XmlIgnore]
        public int die
        {
            get { return _die; }
            set
            {
                if (_die != value)
                {
                    _die = value;
                }
            }
        }


        [XmlIgnore]
        public int mod
        {
            get { return _mod; }
            set
            {
                if (_mod != value)
                {
                    _mod = value;
                }
            }
        }


        [XmlIgnore]
        public List<DieStep> extraRolls
        {
            get { return _extraRolls; }
            set
            {
                if (_extraRolls != value)
                {
                    _extraRolls = value;
                }
            }
        }

        [XmlIgnore]
        public List<DieStep> AllRolls
        {
            get
            {
                List<DieStep> steps = new List<DieStep>() { Step };

                if (_extraRolls != null)
                {
                    foreach (DieStep step in _extraRolls)
                    {
                        steps.Add(step);
                    }

                }
                return steps;

            }
			set
			{
				System.Diagnostics.Debug.Assert(value.Count > 0);
				
				count = value[0].Count;
				die = value[0].Die;
				
				_extraRolls = new List<DieStep>();
				for (int i=1; i<value.Count; i++)
				{
					_extraRolls.Add(value[i]);
				}
			}
        }

        public RollResult Roll()
        {
            RollResult roll = new RollResult();
            roll.Mod = mod;
            roll.Total = mod;


            foreach (DieStep step in AllRolls)
            {
                for (int i = 0; i < step.Count; i++)
                {
                    DieResult res = new DieResult();
                    res.Die = step.Die;
                    res.Result += rand.Next(1, step.Die + 1);
                    roll.Total += res.Result;
                    roll.Rolls.Add(res);
                }
            }

            return roll;
           
        }

        public void AddDie(int newdie)
        {
            AddDie(new DieStep(1, newdie));
        }

        public void AddDie(DieStep step)
        {
            if (this.die == step.Die)
            {
                this.count += step.Count;
            }
            else
            {
                bool added = false;
                if (_extraRolls != null)
                {
                    foreach (DieStep ex in _extraRolls)
                    {
                        if (ex.Die == step.Die)
                        {
                            ex.Count += step.Count;
                            added = true;
                            break;
                        }
                    }
                }

                if (!added)
                {
                    if (_extraRolls == null)
                    {
                        _extraRolls = new List<DieStep>();
                    }
                    _extraRolls.Add(new DieStep(step.Count, step.Die));
                }
            }
        }

        public void RemoveDie(DieStep step)
        {
            if (this.die == step.Die)
            {
                this.count -= step.Count;
                if (this.count < 0)
                {
                    this.count = 0;
                }
            }
            else
            {
                if (_extraRolls != null)
                {
                    for (int i=0; i<_extraRolls.Count; i++)
                    {
                        DieStep ex = extraRolls[i];
                        if (ex.Die == step.Die)
                        {
                            ex.Count -= step.Count;
                            if (ex.Count <= 0)
                            {
                                _extraRolls.RemoveAt(i);
                            }
                            break;
                        }
                    }
                }
            }
        }
        
        public int DieCount(int die)
        {
            int count = 0;

            foreach (DieStep d in this.AllRolls)
            {
                if (d.Die == die)
                {
                    count += d.Count;
                }
            }
            return count;
        }

    }

    
    [DataContract]
    public class DieStep : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        public DieStep() {}
        public DieStep(int Count, int Die) { this.Count = Count; this.Die = Die; }
        public DieStep(DieRoll roll) { this.Count = roll.count; this.Die = roll.die; }

        private int _Count;
        private int _Die;



        [XmlIgnore]
        public string Text
        {
            get
            {
                return Count + "d" + Die;
            }
        }

        public override string ToString()
        {
            return Text;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(DieStep))
            {
                return false;
            }

            DieStep step = (DieStep)obj;

            return (Count == step.Count && Die == step.Die);
        }

        public static bool operator ==(DieStep s1, DieStep s2)
        {
            if (((object)s1) == null ^ ((object)s2) == null)
            {
                return false;
            }
            if (((object)s1) == null)
            {
                return true;
            }

            return s1.Equals(s2);
        }

        public static bool operator !=(DieStep s1, DieStep s2)
        {
            return !(s1 == s2);
        }

        public override int GetHashCode()
        {
            return (Count << 8) | (Die);
        }

        public int Roll()
        {

            int val = 0;

            if (Die == 1)
            {
                val = Die * Count;
            }
            else if (Die > 1)
            {
                for (int i = 0; i < Count; i++)
                {
                    val += DieRoll.Rand.Next(1, Die + 1);
                }
            }

            return val;

        }

        public Double RollDouble(bool min1)
        {
            Double val = 0;

            if (Die == 1 && min1)
            {
                val = Die * Count;
            }
            else if (Die > 0)
            {
                for (int i = 0; i < Count; i++)
                {
                    Double d = DieRoll.Rand.NextDouble();

                    if (min1)
                    {
                        val += 1.0 + ((double)(Count - 1)) * d;
                    }
                    else
                    {
                        val += ((double)Count) * d;
                    }

                }
            }

            return val;
        }


        [DataMember]
        public int Count
        {
            get { return _Count; }
            set
            {
                if (_Count != value)
                {
                    _Count = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Count")); }
                }
            }
        }

        [DataMember]
        public int Die
        {
            get { return _Die; }
            set
            {
                if (_Die != value)
                {
                    _Die = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Die")); }
                }
            }
        }


    }
}
