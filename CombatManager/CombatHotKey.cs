using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Xml.Serialization;

namespace CombatManager
{
 
    public class CombatHotKey : SimpleNotifyClass
    {
        Key _Key = Key.A;
        ModifierKeys _Modifier;
        CharacterAction _Type;
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
            set {
                if (_Key != value)
                {
                    _Key = value;
                    Notify("Key");
                }
            }
        }

        public ModifierKeys Modifier
        {

            get { return _Modifier; }
            set
            {
                if (_Modifier != value)
                {
                    _Modifier = value;
                    Notify("Modifier");
                }
            }
        }

        [XmlIgnore]
        public CharacterAction Type
        {
            get { return _Type; }
            set
            {
                if (_Type != value)
                {
                    _Type = value;
                    Notify("Type");
                    Notify("IntType");
                }
            }
        }

        public String Subtype
        {
            get { return _Subtype; }
            set
            {
                if (_Subtype != value)
                {
                    _Subtype = value;
                    Notify("Subtype");
                }
            }
        }
		
		
        public int IntType
		{
			
            get { return (int)Type; }
            set {
                if (value != IntType)
                {
                    Type = (CharacterAction)value;
                }

            }
		}

        [XmlIgnore]
        public bool AltKey
        {
            get { return GetModifier(ModifierKeys.Alt); }
            set
            {
                SetModifier(ModifierKeys.Alt, value);
                Notify("AltKey");
            }
        }
        [XmlIgnore]
        public bool ShiftKey
        {
            get { return GetModifier(ModifierKeys.Shift); }
            set
            {
                SetModifier(ModifierKeys.Shift, value);
                Notify("ShiftKey");
            }
        }
        [XmlIgnore]
        public bool CtrlKey
        {
            get { return GetModifier(ModifierKeys.Control); }
            set
            {
                SetModifier(ModifierKeys.Control, value);
                Notify("CtrlKey");
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
            Notify("AltKey");
            Notify("ShiftKey");
            Notify("CtrlKey");
        }

    }
}
