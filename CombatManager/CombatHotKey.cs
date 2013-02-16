using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

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

        public bool AltKey
        {
            get { return GetKey(ModifierKeys.Alt); }
            set
            {
                SetKey(ModifierKeys.Alt, value);
            }
        }
        public bool ShiftKey
        {
            get { return GetKey(ModifierKeys.Shift); }
            set
            {
                SetKey(ModifierKeys.Shift, value);
            }
        }
        public bool CtrlKey
        {
            get { return GetKey(ModifierKeys.Control); }
            set
            {
                SetKey(ModifierKeys.Control, value);
            }
        }

        private bool GetKey(ModifierKeys key)
        {
            return (_Modifier & key) == key;
        }

        private void SetKey(ModifierKeys key, bool value)
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
