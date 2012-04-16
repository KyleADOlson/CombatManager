using System;
using System.Data;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace CombatManager
{
	/// <summary>
	/// Interaction logic for WeaponSelectWindow.xaml
	/// </summary>
	public partial class WeaponSelectWindow : Window, INotifyPropertyChanged 
	{

        public event PropertyChangedEventHandler PropertyChanged;

		ICollectionView weaponsView;


        private int _Hands;
        private bool _Melee;
        private bool _Ranged;
        private bool _Natural;
        private MonsterSize _Size;

        
		
		public WeaponSelectWindow()
		{
			this.InitializeComponent();


            weaponsView = new ListCollectionView(new List<Weapon>(Weapon.Weapons.Values));
            weaponsView.Filter += WeaponFilter;
            weaponsView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));

            DataContext = weaponsView;
			
		}

        private bool WeaponFilter(object ob)
        {
            Weapon weapon = (Weapon)ob;

            return TypeFilter(weapon) && HandsFilter(weapon)  && NameFilter(weapon);

        }

        private bool TypeFilter(Weapon wp)
        {
            if (wp.Natural)
            {
                return Natural;
            }
            else if (wp.Ranged)
            {
                if (!Ranged)
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
                    return Melee || Ranged;
                }

                return Melee;
            }
        }

        private bool HandsFilter(Weapon wp)
        {
            if (wp.Natural || wp.Ranged)
            {
                return true;
            }
            else
            {
                return (Hands > 1 || !wp.TwoHanded);
            }

        }

        private bool SourceFilter(Weapon wp)
        {
            SourceType type = SourceInfo.GetSourceType(wp.Source);

            return (type == SourceType.Core || type == SourceType.APG);
        }
		
		private bool NameFilter(Weapon wp)
        {
            if (NameFilterBox.Text.Trim().Length == 0)
			{
				return true;
			}
			else
			{
				Regex regName = new Regex(Regex.Escape(NameFilterBox.Text.Trim()), RegexOptions.IgnoreCase);
			
				return regName.Match(wp.Name).Success;
			}
        }


        public Weapon Weapon
        {
            get
            {
                return (Weapon)weaponsView.CurrentItem;
            }
        }

		private void Window_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
		}

        public int Hands
        {
            get { return _Hands; }
            set
            {
                if (_Hands != value)
                {
                    _Hands = value;
                    weaponsView.Refresh();
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Hands")); }
                }
            }
        }
        public bool Melee
        {
            get { return _Melee; }
            set
            {
                if (_Melee != value)
                {
                    _Melee = value;
                    weaponsView.Refresh();
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Melee")); }
                }
            }
        }
        public bool Ranged
        {
            get { return _Ranged; }
            set
            {
                if (_Ranged != value)
                {
                    _Ranged = value;
                    weaponsView.Refresh();
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Ranged")); }
                }
            }
        }
        public bool Natural
        {
            get { return _Natural; }
            set
            {
                if (_Natural != value)
                {
                    _Natural = value;
                    weaponsView.Refresh();
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Natural")); }
                }
            }
        }
        public MonsterSize Size
        {
            get { return _Size; }
            set
            {
                if (_Size != value)
                {
                    _Size = value;
                    weaponsView.Refresh();
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Size")); }
                }
            }
        }


        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {

            DialogResult = true;
            Close();
        }

        private void WeaponListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	if (weaponsView.CurrentItem != null)
			{
                if (CMUIUtilities.ClickedListBoxItem((ListBox)sender, e) != null)
                {
                    DialogResult = true;
                    Close();
                }
			}
        }

        private void DamageText_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock box = (TextBlock)sender;

            Weapon wp = (Weapon)box.DataContext;

            box.Text = wp.SizeDamageText(Size);
        }

        private void NameFilterBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
        	weaponsView.Refresh();
        }

	}
}