using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatManager.PF2
{
    public class PF2ActionTrait : SimpleNotifyClass, ICloneable
    {
        public string _name;
        public DieRoll _dieRoll;
        public int? _feet;

        public PF2ActionTrait() { }

        public PF2ActionTrait(PF2ActionTrait r)
        {
            CopyFrom(r);
        }

        public void CopyFrom(PF2ActionTrait r)
        {
            Name = r._name;
            DieRoll = r._dieRoll;
            Feet = r._feet;

        }

        public object Clone()
        {
            return new PF2ActionTrait(this);

        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    Notify("Name");
                }
            }
        }

        public DieRoll DieRoll
        {
            get
            {
                return _dieRoll;
            }
            set
            {
                if (_dieRoll != value)
                {
                    _dieRoll = value.CloneOrNull();
                    Notify("DieRoll");
                }
            }
        }

        public int? Feet
        {
            get
            {
                return _feet;
            }
            set
            {
                if (_feet != value)
                {
                    _feet = value;
                    Notify("Feet");
                }
            }
        }

        public override string ToString()
        {

            List<string> list = new List<string>();
            list.AddIfNotNull(_name);
            list.AddIfNotNull(_dieRoll);
            list.AddIfNotNull(_feet, postfix: " feet");
            return list.WeaveString(" ");

        }
    }
}
