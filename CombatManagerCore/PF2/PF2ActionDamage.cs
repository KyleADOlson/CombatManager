using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatManager.PF2
{
    public class PF2ActionDamage : SimpleNotifyClass, ICloneable
    {
        public DieRoll _dieRoll;
        public string _type;
        public int? _feet;

        public PF2ActionDamage() { }

        public PF2ActionDamage(PF2ActionDamage r)
        {
            CopyFrom(r);
        }

        public object Clone()
        {
            return new PF2ActionDamage(this);
        }

        public void CopyFrom(PF2ActionDamage r)
        {
            DieRoll = r._dieRoll.CloneOrNull();
            Type = r._type;
            Feet = r._feet;
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

        public string Type
        {
            get
            {
                return _type;
            }
            set
            {
                if (_type != value)
                {
                    _type = value;
                    Notify("Type");
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
            list.AddIfNotNull(_dieRoll);
            list.AddIfNotNull(_type);
            list.AddIfNotNull(_feet, postfix: " feet");
            return list.WeaveString(" ");
        }

    }
}
