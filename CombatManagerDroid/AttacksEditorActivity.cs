
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
using System.Collections.ObjectModel;

namespace CombatManagerDroid
{



    [Activity (Label = "Attacks Editor", MainLauncher = true, Theme="@android:style/Theme.Light.NoTitleBar")]           
    public class AttacksEditorActivity : Activity
    {
        
        private static Monster _Monster;

        private CharacterAttacks _Attacks;

        int visibleGroup = 0;

        protected override void OnCreate (Bundle bundle)
        {

            CoreContext.Context = Application.Context;
            
            base.OnCreate (bundle);
            
            SetContentView (Resource.Layout.AttacksEditor);

            var sets = new ObservableCollection<AttackSet>(_Monster.MeleeAttacks);
            var ranged = new ObservableCollection<Attack>(_Monster.RangedAttacks);
            _Attacks = new CharacterAttacks(sets, ranged);

            BuildMeleeTabs();
            BuildMeleeGroup();
            BuildRanged ();
            BuildNatural();

        }

        private void BuildMeleeTabs()
        {
            int i=0;
            LinearLayout l = FindViewById<LinearLayout>(Resource.Id.meleeGroupLayout);
            l.RemoveAllViews();
            foreach (var v in _Attacks.MeleeWeaponSets)
            {
                i++;
                Button b = new Button(this);
                b.Text = "Set #" + i;
                l.AddView(b);
                int item = i;
                b.Click += (object sender, EventArgs e) => 
                {
                    MeleeTabClicked(item);
                };
            }
            Button addButton = new Button(this);
            addButton.Text = "Add";
            addButton.Click += (object sender, EventArgs e) => 
            {
                AddGroupClicked();
            };
            l.AddView(addButton);
        }

        private void BuildMeleeGroup()
        {
            LinearLayout ml = FindViewById<LinearLayout>(Resource.Id.meleeLayout);
            ml.RemoveAllViews();
            if (visibleGroup < _Attacks.MeleeWeaponSets.Count)
            {
                var vs = _Attacks.MeleeWeaponSets[visibleGroup];
                foreach (var atk in vs)
                {
                    ml.AddView(CreateAttackView(atk, false));
                }

            }
        }

        private void BuildRanged()
        {
            LinearLayout rl = FindViewById<LinearLayout>(Resource.Id.rangedLayout);
            rl.RemoveAllViews();
            foreach (var atk in _Attacks.RangedWeapons)
            {
               
                rl.AddView(CreateAttackView(atk, false));
            }
        }

        private void BuildNatural()
        {
            LinearLayout nl = FindViewById<LinearLayout>(Resource.Id.naturalLayout); 
            foreach (var atk in _Attacks.NaturalAttacks)
            {
                
                TextView tv = new TextView(this);
                tv.Text = atk.FullName;
                nl.AddView(tv);
            }
        }


        private View CreateAttackView(WeaponItem atk, bool ranged)
        {
            LinearLayout baseLayout = new LinearLayout(this);
            baseLayout.Orientation = Orientation.Horizontal;
            
            baseLayout.SetGravity(GravityFlags.CenterVertical);
            baseLayout.SetPadding(4, 4, 4, 4);

            //hands
            LinearLayout handView = new LinearLayout(this);
            handView.Orientation = Orientation.Horizontal;
            TextView handsText = new TextView(this);
            handsText.SetTextSizeDip(18);
            handsText.SetPadding(4, 0, 4, 0);
            handView.AddView(handsText);
            if (ranged)
            {
                handsText.Text = "Ranged";
            }
            else
            {
                int hc = 1;
                if (atk.TwoHanded)
                {
                    handsText.Text = "";
                    hc = 2;
                    
                }
                else if (atk.MainHand)
                {
                    handsText.Text = "Main";
                }
                else
                {
                    handsText.Text = "Off";
                }

                for (int i=0; i<hc; i++)
                {
                    ImageView handImage = new ImageView(this);
                    handImage.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.hand16));
                    handView.AddView(handImage);
                }

            }
            handView.SetBackgroundColor(new Android.Graphics.Color(0xBF, 0x98, 0x97));
            handView.SetGravity(GravityFlags.CenterVertical);
            handView.SetPadding(4, 4, 4, 4);
            baseLayout.AddView(handView);


            //name
            LinearLayout nameView = new LinearLayout(this);
            nameView.Orientation = Orientation.Horizontal;
            nameView.SetPadding(4, 4, 4, 4);
            nameView.SetBackgroundColor(new Android.Graphics.Color(0xAF, 0xDB, 0xDD));

            TextView nameText = new TextView(this);
            nameText.SetTextSizeDip(18);
            nameText.Text = atk.Name;
            nameText.SetPadding(4, 0, 4, 0);
           
            nameView.AddView(nameText);


            baseLayout.AddView(nameView);

            //bonus
            Button b = new Button(this);
            SetBonusText(b, atk);
            baseLayout.AddView(b);
            b.Click += (object sender, EventArgs e) => 
            {
                BonusClicked(b, atk);
            };

            //magic
            Button magicButton = new Button(this);
            if (atk.SpecialAbilities == null || atk.SpecialAbilities == "")
            {
                magicButton.Text = "...";
            }
            else
            {
                magicButton.Text = atk.SpecialAbilities;
            }
            magicButton.SetMinimumWidth(120);
            baseLayout.AddView (magicButton);
            magicButton.Click += (object sender, EventArgs e) => 
            {
                SpecialClicked(magicButton, atk);
            };

            //delete
            ImageButton deleteButton = new ImageButton(this);
            deleteButton.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.redx));
            deleteButton.Click += (object sender, EventArgs e) => 
            {
                DeleteClicked(atk, ranged);
            };
            baseLayout.AddView(deleteButton);

            return baseLayout;
        }

        private void MeleeTabClicked(int tab)
        {
            if (tab != visibleGroup)
            {
                visibleGroup = tab;
                BuildMeleeGroup();
            }
        }

        private void BonusClicked(Button b, WeaponItem atk)
        {
            AlertDialog.Builder builderSingle = new AlertDialog.Builder(this);
            
            builderSingle.SetTitle("Magic Bonus");
            ArrayAdapter<String> arrayAdapter = new ArrayAdapter<String>(
                this,
                Android.Resource.Layout.SelectDialogSingleChoice);

            List<String> options = new List<string>();
            options.Add(((int)0).PlusFormat());
            options.Add("mwk");
            for(int i=1; i<11; i++)
            {
                options.Add(i.PlusFormat());
            }

            arrayAdapter.AddAll(options);
            
            
            builderSingle.SetAdapter (arrayAdapter, (se, ev)=> {
                if (ev.Which == 0)
                {
                    atk.MagicBonus =0;
                    atk.Masterwork = false;
                }
                else if (ev.Which == 1)
                {
                    atk.MagicBonus = 0;
                    atk.Masterwork = true;
                }
                else
                {
                    atk.MagicBonus = ev.Which - 1;
                    atk.Masterwork = false;
                }
                SetBonusText(b, atk);
            });

                
            
            builderSingle.Show();
        }

        void SetBonusText(Button b, WeaponItem atk)
        {
            if (atk.MagicBonus > 0)
            {
                b.Text = atk.MagicBonus.PlusFormat();
            }
            else if (atk.Masterwork)
            {
                b.Text = "mwk";
            }
            else
            {
                b.Text = ((int)0).PlusFormat();
            }
        }

        private void SpecialClicked(Button b, WeaponItem atk)
        {

        }

        private void DeleteClicked(WeaponItem atk, bool ranged)
        {
            if (ranged)
            {
                _Attacks.RangedWeapons.Remove(atk);
                BuildRanged();
            }
            else
            {
                _Attacks.MeleeWeaponSets[visibleGroup].Remove(atk);
                BuildMeleeGroup();
            }
        }

        private void AddGroupClicked()
        {
            _Attacks.MeleeWeaponSets.Add(new List<WeaponItem>());
            visibleGroup = _Attacks.MeleeWeaponSets.Count - 1;
            BuildMeleeGroup();
        }
        
        public static Monster Monster
        {
            get
            {
                return _Monster;
            }
            set
            {
                _Monster = value;
            }
        }
    }
}

