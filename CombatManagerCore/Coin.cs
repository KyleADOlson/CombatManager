using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Globalization;
using System.IO;


namespace CombatManager
{
    public class Coin : INotifyPropertyChanged, ICloneable
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private int _PP;
        private int _GP;
        private int _SP;
        private int _CP;

        public Coin()
        {

        }
        public Coin(Coin c)
        {
            this._PP = c._PP;
            this._GP = c._GP;
            this._SP = c._SP;
            this._CP = c._CP;
        }

        public Coin(String s)
        {
            string text = s.Replace(",", "");
            Regex regCoin = new Regex("(?<val>[0-9]+) +(?<type>(pp|gp|sp|cp))");

            foreach (Match m in regCoin.Matches(text))
            {

                int val = int.Parse(m.Groups["val"].Value);

                string type = m.Groups["type"].Value;

                if (type == "pp")
                {
                    _PP += val;
                }
                else if (type == "gp")
                {
                    _GP += val;
                }
                else if (type == "sp")
                {
                    _SP += val;
                }
                else if (type == "cp")
                {
                    _CP += val;
                }

            }
        }

        public int PP
        {
            get { return _PP; }
            set
            {
                if (_PP != value)
                {
                    _PP = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("PP")); }
                }
            }
        }
        public int GP
        {
            get { return _GP; }
            set
            {
                if (_GP != value)
                {
                    _GP = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("GP")); }
                }
            }
        }
        public int SP
        {
            get { return _SP; }
            set
            {
                if (_SP != value)
                {
                    _SP = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("SP")); }
                }
            }
        }
        public int CP
        {
            get { return _CP; }
            set
            {
                if (_CP != value)
                {
                    _CP = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("CP")); }
                }
            }
        }

        public decimal GPValue
        {
            get
            {
                decimal val = PP * 10;
                val += GP;

                val += ((decimal)SP) / 10.0m;
                val += ((decimal)CP) / 100.0m;

                return val;
            }
        }
        public override string ToString()
        {
            string text = "";

            bool first = true;

            if (PP != 0)
            {
                text += PP + " pp";
                first = false;
            }
            if (GP != 0)
            {
                if (!first)
                {
                    text += " ";
                }
                text += GP + " gp";
                first = false;
            }
            if (SP != 0)
            {
                if (!first)
                {
                    text += " ";
                }
                text += SP + " sp";
                first = false;
            }
            if (CP != 0)
            {
                if (!first)
                {
                    text += " ";
                }
                text += CP + " cp";
            }

            if (text.Length == 0)
            {
                text = "0 gp";
            }

            return text;

        }


        public object Clone()
        {
            return new Coin(this);
        }

        public static Coin operator +(Coin a, Coin b)
        {
            Coin c = new Coin();
            c.CP = a.CP + b.CP;
            c.SP = a.SP + b.SP;
            c.GP = a.GP + b.GP;
            c.PP = a.PP + b.PP;

            return c;
        }

        public static Coin operator -(Coin a, Coin b)
        {
            Coin c = new Coin();
            c.CP = a.CP - b.CP;
            c.SP = a.SP - b.SP;
            c.GP = a.GP - b.GP;
            c.PP = a.PP - b.PP;

            return c;
        }


    }
}
