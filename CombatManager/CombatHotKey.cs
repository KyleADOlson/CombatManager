using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Xml.Serialization;

namespace CombatManager
{
    public enum CombatHotKeyType
    {
        MeleeAttack,
        RangedAttack,
        Save,
        Skill
    }

    public class CombatHotKey
    {
        Key _Key = Key.A;
        ModifierKeys _Modifier;
        CombatHotKeyType _Type;
        String _Subtype;

        public CombatHotKey() { }
        public CombatHotKey(CombatHotKey hk)
        {
            _Key = hk._Key;
            _Modifier = hk._Modifier;
            _Type = hk._Type;
            _Subtype = hk._Subtype;
        }

        public Key Key
        {
            get { return _Key; }
            set { _Key = value; }
        }

        public ModifierKeys Modifier
        {
            get { return _Modifier; }
            set { _Modifier = value; }
        }

        public CombatHotKeyType Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        public String Subtype
        {
            get { return _Subtype; }
            set { _Subtype = value; }
        }
		
		
        [XmlIgnore]
        public int IntType
		{
			
            get { return (int)_Type; }
            set { _Type = (CombatHotKeyType)value; }
		}

        [XmlIgnore]
        public bool AltKey
        {
            get { return GetModifier(ModifierKeys.Alt); }
            set
            {
                SetModifier(ModifierKeys.Alt, value);
            }
        }
        [XmlIgnore]
        public bool ShiftKey
        {
            get { return GetModifier(ModifierKeys.Shift); }
            set
            {
                SetModifier(ModifierKeys.Shift, value);
            }
        }
        [XmlIgnore]
        public bool CtrlKey
        {
            get { return GetModifier(ModifierKeys.Control); }
            set
            {
                SetModifier(ModifierKeys.Control, value);
            }
        }

        private bool GetModifier(ModifierKeys key)
        {
            return (_Modifier & key) == key;
        }

        private void SetModifier(ModifierKeys key, bool value)
        {
            if (value)
            {
                _Modifier |= key;
            }
            else
            {
                _Modifier &= ~key;
            }
        }

    }
}
