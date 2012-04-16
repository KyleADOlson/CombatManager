using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;


namespace CombatManager
{
    public class ActiveCondition : INotifyPropertyChanged, ICloneable
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private Condition _Condition;
        private int? _Turns;
        private int? _EndTurn;
        private string _Details;
        private InitiativeCount _InitiativeCount;

        public ActiveCondition()
        {

        }


        public ActiveCondition(ActiveCondition a)
        {
            if (a._Condition != null)
            {
                _Condition = (Condition)a._Condition.Clone();
            }
            _Turns = a._Turns;
            _EndTurn = a._EndTurn;
            if (a._InitiativeCount != null)
            {
                _InitiativeCount = (InitiativeCount)a._InitiativeCount.Clone();
            }
            _Details = a._Details;
        }

        public object Clone()
        {
            return new ActiveCondition(this);
        }

        public Condition Condition
        {
            get { return _Condition; }
            set
            {
                if (_Condition != value)
                {
                    _Condition = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Condition")); }
                }
            }
        }

        public int? Turns
        {
            get { return _Turns; }
            set
            {
                if (_Turns != value)
                {
                    _Turns = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Turns")); }
                }
            }
        }

        [XmlIgnore]
        public int? EndTurn
        {
            get { return _EndTurn; }
            set
            {
                if (_EndTurn != value)
                {
                    _EndTurn = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Turns")); }
                }
            }
        }

        [XmlIgnore]
        public InitiativeCount InitiativeCount
        {
            get { return _InitiativeCount; }
            set
            {
                if (_InitiativeCount != value)
                {
                    _InitiativeCount = new InitiativeCount((InitiativeCount)value) ;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("InitiativeCount")); }
                }
            }
        }

        [XmlIgnore]
        public ConditionBonus Bonus
        {
            get
            {
                if (Condition != null)
                {
                    if (Condition.Spell != null)
                    {
                        return Condition.Spell.Bonus;
                    }

                    return Condition.Bonus;
                }
                return null;

            }
        }

        public string Details
        {
            get { return _Details; }
            set
            {
                if (_Details != value)
                {
                    _Details = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Details")); }
                }
            }
        }

    }
}
