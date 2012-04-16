using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CombatManager
{

    public class DBSettings
    {
        private static bool _useDB = true;

        public static bool UseDB
        {
            get
            {
                return _useDB;
            }
            set
            {
                _useDB = value;
            }
        }
    }


}
