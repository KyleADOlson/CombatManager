
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using CombatManager;

namespace CombatManagerDroid
{
    class ActionDialog : Dialog
    {
        Character _Character;

        public ActionDialog(Context context, Character character) : base (context)
        {
            character = _Character;

            SetContentView(Resource.Layout.ActionDialog);
            SetTitle(character.Name);
        }
    }
}

