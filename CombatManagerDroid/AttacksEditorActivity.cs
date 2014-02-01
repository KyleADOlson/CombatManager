
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
using System.Text.RegularExpressions;

namespace CombatManagerDroid
{



    [Activity (Label = "Attacks Editor", Theme="@android:style/Theme.Light.NoTitleBar")]           
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

            Button b = FindViewById<Button>(Resource.Id.okButton);
            b.Click += (object sender, EventArgs e) => 
            {
                _Monster.Melee = _Monster.MeleeString(_Attacks);
                _Monster.Ranged = _Monster.RangedString(_Attacks);
                Finish();
            };

            b = FindViewById<Button>(Resource.Id.cancelButton);
            b.Click += (object sender, EventArgs e) => 
            {
                
                Finish();
            };
              

            b = FindViewById <Button>(Resource.Id.addMeleeButton);
            b.Click += (object sender, EventArgs e) => 
            {
                AddAttack(true, false, false);
            };

            b = FindViewById <Button>(Resource.Id.addRangedButton);
            b.Click += (object sender, EventArgs e) => 
            {
                AddAttack(false, true, false);
            };
            
            b = FindViewById <Button>(Resource.Id.addNaturalButton);
            b.Click += (object sender, EventArgs e) => 
            {
                AddAttack(false, false, true);
            };


        }

        private void BuildMeleeTabs()
        {
            int i=0;
            LinearLayout l = FindViewById<LinearLayout>(Resource.Id.meleeGroupLayout);
            l.RemoveAllViews();
            if (_Attacks.MeleeWeaponSets.Count == 0)
            {

                TextView view = new TextView(this);
                view.Text = "No Melee Sets";
                l.AddView(view);
            }
            foreach (var v in _Attacks.MeleeWeaponSets)
            {
                i++;
                Button b = new Button(this);
                b.Text = "Set #" + i;
                l.AddView(b);
                int item = i-1;
                b.Click += (object sender, EventArgs e) => 
                {
                    MeleeTabClicked(item);
                };
                b.SetBackgroundDrawable(Resources.GetDrawable(Resource.Drawable.init_button));

                if (item == visibleGroup)
                {
                    b.Selected = true;
                }
            }
            Button addButton = new Button(this);
            addButton.Text = "Add";
            addButton.Click += (object sender, EventArgs e) => 
            {
                AddGroupClicked();
            };
            l.AddView(addButton);
            Button addMeleeButton = FindViewById <Button>(Resource.Id.addMeleeButton);
            addMeleeButton.Enabled = _Attacks.MeleeWeaponSets.Count > 0;
        }

        private void BuildMeleeGroup()
        {
            LinearLayout ml = FindViewById<LinearLayout>(Resource.Id.meleeLayout);
            ml.RemoveAllViews();
            if (visibleGroup < _Attacks.MeleeWeaponSets.Count)
            {
                var vs = _Attacks.MeleeWeaponSets[visibleGroup];
                if (vs.Count == 0)
                {
                    TextView view = new TextView(this);
                    view.Text = "Empty";
                    ml.AddView(view);
                }

                foreach (var atk in vs)
                {
                    ml.AddView(CreateAttackView(atk, false, false));
                }
            }
        }

        private void BuildRanged()
        {
            LinearLayout rl = FindViewById<LinearLayout>(Resource.Id.rangedLayout);
            rl.RemoveAllViews();
            foreach (var atk in _Attacks.RangedWeapons)
            {
               
                rl.AddView(CreateAttackView(atk, true, false));
            }
        }

        private void BuildNatural()
        {
            LinearLayout nl = FindViewById<LinearLayout>(Resource.Id.naturalLayout); 
            nl.RemoveAllViews();
            foreach (var atk in _Attacks.NaturalAttacks)
            {
                
                
                nl.AddView(CreateAttackView(atk, false, true));
            }
        }


        private View CreateAttackView(WeaponItem atk, bool ranged, bool natural)
        {
            LinearLayout baseLayout = new LinearLayout(this);
            baseLayout.Orientation = Orientation.Horizontal;
            
            baseLayout.SetGravity(GravityFlags.CenterVertical);
            baseLayout.SetPadding(4, 4, 4, 4);

            //hands
            if (!natural)
            {
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
            }


            //name
            LinearLayout nameView = new LinearLayout(this);
            nameView.Orientation = Orientation.Horizontal;
            nameView.SetPadding(4, 4, 4, 4);
            nameView.SetBackgroundColor(new Android.Graphics.Color(0xAF, 0xDB, 0xDD));

            TextView nameText = new TextView(this);
            nameText.SetTextSizeDip(18);
            if (natural)
            {
                nameText.Text = atk.FullName;
            }
            else
            {
                nameText.Text = atk.Name;
            }
            nameText.SetPadding(4, 0, 4, 0);
           
            nameView.AddView(nameText);


            baseLayout.AddView(nameView);

            if (!natural)
            {
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
                baseLayout.AddView(magicButton);
                magicButton.Click += (object sender, EventArgs e) => 
                {
                    SpecialClicked(magicButton, atk, ranged);
                };
            }

            //delete
            ImageButton deleteButton = new ImageButton(this);
            deleteButton.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.redx));
            deleteButton.Click += (object sender, EventArgs e) => 
            {
                DeleteClicked(atk, ranged, natural);
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
                BuildMeleeTabs();
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

        private void SpecialClicked(Button b, WeaponItem atk, bool ranged)
        {
			AlertDialog.Builder builderSingle = new AlertDialog.Builder(this);

			builderSingle.SetTitle("Special");
		    

			List<String> options = new List<string>();
			List<WeaponSpecialAbility> specialList = ranged?WeaponSpecialAbility.RangedAbilities:WeaponSpecialAbility.MeleeAbilities;
			bool [] itemschecked = new bool[specialList.Count];
			int i = 0;
			foreach (WeaponSpecialAbility ab in specialList) 
			{
				options.Add (ab.Name);
				if (atk.SpecialAbilities != null && atk.SpecialAbilities.Contains (ab.Name)) 
				{
					itemschecked[i] = true;
				}
				i++;

			}

			builderSingle.SetMultiChoiceItems(options.ToArray(), itemschecked, (sender, args)=>
			{
				int index = args.Which;
				string special = options[index];
				SortedDictionary<string, string>  dict = new SortedDictionary<string, string>(atk.SpecialAbilitySet);
				if (args.IsChecked)
				{
					dict[special] = special;
				}
				else
				{
					dict.Remove(special);
				}
				atk.SpecialAbilitySet = dict;



			});

			builderSingle.ItemSelected += (object sender, AdapterView.ItemSelectedEventArgs e) => 
			{
				if (ranged)
                {
                    BuildRanged();
                }
                else
                {
                    BuildMeleeGroup();
                }
			};

			builderSingle.SetOnCancelListener(new SpecialListener(this, ranged));



			builderSingle.Show();
        }

		private class SpecialListener : Java.Lang.Object,  IDialogInterfaceOnCancelListener
		{
			AttacksEditorActivity at;
            bool ranged;
			public SpecialListener(AttacksEditorActivity at, bool ranged)
			{
				this.at = at;
                this.ranged = ranged;
			}

			public virtual void OnCancel(IDialogInterface s)
			{
                if (ranged)
                {
                    at.BuildRanged();    
                }
                else
                {

                    at.BuildMeleeGroup();
                }
			}
		}

	    private void AddAttack(bool melee, bool ranged, bool natural)
        {
            var weapons = from w in Weapon.Weapons.Values where TypeFilter(w, melee, ranged, natural) orderby w.Name select w.Name;

            AlertDialog.Builder builderSingle = new AlertDialog.Builder(this);

            builderSingle.SetTitle("Add Attack");
            ArrayAdapter<String> arrayAdapter = new ArrayAdapter<String>(
                this,
                Android.Resource.Layout.SelectDialogItem);
            arrayAdapter.AddAll(new List<string>(weapons));


            builderSingle.SetAdapter (arrayAdapter, (se, ev)=> {
                string name = arrayAdapter.GetItem(ev.Which);


                Weapon wp = Weapon.Weapons[name];
                WeaponItem item = new WeaponItem(wp);
               

                if (melee)
                {
                    _Attacks.MeleeWeaponSets[visibleGroup].Add(item);
                    BuildMeleeGroup();
                }
                else if (ranged)
                {
                    _Attacks.RangedWeapons.Add(item);
                    BuildRanged();
                }
                else if (natural)
                {
                    if (item != null)
                    {
                        bool bAdded = false;
                        foreach (WeaponItem wi in _Attacks.NaturalAttacks)
                        {
                            if (String.Compare(wi.Name, item.Name, true) == 0)
                            {
                                wi.Count++;
                                bAdded = true;
                                break;
                            }

                        }

                        if (!bAdded)
                        {
                            _Attacks.NaturalAttacks.Add(item);
                        }
                    }
                    BuildNatural();

                }

            });
            builderSingle.Show();
        }

        private bool TypeFilter(Weapon wp, bool melee, bool ranged, bool natural)
        {
            if (wp.Natural)
            {
                return natural;
            }
            else if (wp.Ranged)
            {
                if (!ranged)
                {
                    return false;
                }
                else
                {
                    return !(new Regex("(\\(( )?[0-9]+( )?\\))|(  Bolt)|(  Dart)|(  Bullets)", RegexOptions.IgnoreCase).Match(wp.Name).Success);                  
                }            
            }
            else 
            {
                if (wp.Throw)
                {
                    return melee || ranged;
                }

                return melee;
            }
        }
	

        private void DeleteClicked(WeaponItem atk, bool ranged, bool natural)
        {
            if (ranged)
            {
                _Attacks.RangedWeapons.Remove(atk);
                BuildRanged();
            }
            else if (!natural)
            {
                _Attacks.MeleeWeaponSets[visibleGroup].Remove(atk);
                BuildMeleeGroup();
            }
            else
            {
                _Attacks.NaturalAttacks.Remove(atk);
                BuildNatural();
            }
        }

        private void AddGroupClicked()
        {
            _Attacks.MeleeWeaponSets.Add(new List<WeaponItem>());
            visibleGroup = _Attacks.MeleeWeaponSets.Count - 1;
            BuildMeleeGroup();
            BuildMeleeTabs();
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

