#if ANDROID
using System;
using Android.Content;


namespace CombatManager
{

    public static class CoreContext
    {
        static Context _Context;

        public static Context Context
        {
            get
            {
                return _Context;
            }
            set
            {
                _Context = value;
            }
        }

    }
}

#endif