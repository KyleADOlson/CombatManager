
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
    [Activity (Label = "Monster Editor", Theme = "@android:style/Theme.Light.NoTitleBar")]   
    class MonsterEditorSpecialActivity : MonsterEditorActivity
    {
        protected override int LayoutID
        {
            get
            {
                return Resource.Layout.MonsterEditorSpecial;
            }
        }

        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);

            FindViewById<Button>(Resource.Id.addButton).Click += (object sender, EventArgs e) => 
            {
                EditMonster.SpecialAbilitiesList.Add(new SpecialAbility());
                CreateSpecialList();
            };
            
            CreateSpecialList();
        }

        private void CreateSpecialList()
        {
            LinearLayout list = FindViewById<LinearLayout>(Resource.Id.specialLayout);

            list.RemoveAllViews();

            foreach (SpecialAbility ab in EditMonster.SpecialAbilitiesList)
            {
                View v = LayoutInflater.Inflate(Resource.Layout.SpecialAbility, list, false);

                SpecialAbility spec = ab;

                EditText nameText = 
                    v.FindViewById<EditText>(Resource.Id.nameText);

                nameText.Text = ab.Name;
                nameText.AfterTextChanged += (object sender, Android.Text.AfterTextChangedEventArgs e) => 
                {
                    spec.Name = nameText.Text;
                };

                EditText descriptionText = 
                    v.FindViewById<EditText>(Resource.Id.descriptionText);
                descriptionText.Text = ab.Text;
                descriptionText.AfterTextChanged += (object sender, Android.Text.AfterTextChangedEventArgs e) => 
                {
                    spec.Text = descriptionText.Text;
                };

                v.FindViewById<ImageButton>(Resource.Id.deleteButton).Click += (object sender, EventArgs e) => 
                {
                    EditMonster.SpecialAbilitiesList.Remove(spec);
                    CreateSpecialList();
                };

                UIUtils.AttachButtonStringList(v, ab, Resource.Id.typeButton, "Type", new List<string>()
                                               {"Ex", "Sp", "Su", "-"});

                list.AddView(v);

            }
        }


    }
}

