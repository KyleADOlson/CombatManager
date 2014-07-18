/*
 *  CombatListWindow.xaml.cs
 *
 *  Copyright (C) 2010-2012 Kyle Olson, kyle@kyleolson.com
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU General Public License
 *  as published by the Free Software Foundation; either version 2
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 * 
 *  You should have received a copy of the GNU General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 *
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Globalization;

namespace CombatManager
{
	/// <summary>
	/// Interaction logic for CombatListWindow.xaml
	/// </summary>
	public partial class CombatListWindow : Window
	{
		
	    private CombatState _CombatState;
		IInitiativeController _Controller;
        private double _Scale;
        private bool _SetupComplete;
		
		public CombatListWindow()
		{
            _Scale = 1.0;
			this.InitializeComponent();

            _Scale = UserSettings.Settings.InitiativeScale;
            PlayersComboBox.SelectedIndex = ComboIndex(UserSettings.Settings.InitiativeShowPlayers,
                                        UserSettings.Settings.InitiativeHidePlayerNames) ;

            MonstersComboBox.SelectedIndex = ComboIndex(UserSettings.Settings.InitiativeShowMonsters,
                                        UserSettings.Settings.InitiativeHideMonsterNames);
            TopmostCheckbox.IsChecked = UserSettings.Settings.InitiativeAlwaysOnTop;
            FlipButton.IsChecked = UserSettings.Settings.InitiativeFlip;
            ShowConditionsCheckBox.IsChecked = UserSettings.Settings.InitiativeShowConditions;
            ConditionSizeComboBox.SelectedIndex = UserSettings.Settings.InitiativeConditionsSize;


            UserSettings.Settings.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Settings_PropertyChanged);

			UpdateCombatListState();
            _SetupComplete = true;
		}

        void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdateCharacterName();
        }

        public int ComboIndex(bool show, bool hideNames)
        {
            if (!show)
            {
                return 2;
            }
            else if (hideNames)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }


        public CombatState CombatState
        {
            get { return _CombatState; }
            set
            {
                if (_CombatState != value)
                {
                    _CombatState = value;
					this.DataContext = _CombatState;
                    CombatList.CombatState = _CombatState;

                    _CombatState.CombatList.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(CombatList_CollectionChanged);
                    _CombatState.CharacterAdded += new CombatStateCharacterEvent(CombatState_CharacterAdded);
                    foreach (Character ch in _CombatState.CombatList)
                    {
                        ch.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Character_PropertyChanged);
                    }

                    _CombatState.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(CombatState_PropertyChanged);

                    UpdateCharacterName();
                
                }
            }
        }


        void UpdateCharacterName()
        {
            if (CombatState == null || CombatState.CurrentCharacter == null)
            {
                CharacterNameText.Text = "";
            }
            else if ((CombatState.CurrentCharacter.IsMonster && (UserSettings.Settings.InitiativeHideMonsterNames || !UserSettings.Settings.InitiativeShowMonsters))
                || (!CombatState.CurrentCharacter.IsMonster && (UserSettings.Settings.InitiativeHidePlayerNames || !UserSettings.Settings.InitiativeShowPlayers)))
            {
                CharacterNameText.Text = "??????";
            }
            else
            {
                CharacterNameText.Text = CombatState.CurrentCharacter.HiddenName;
            }

        }

        void CombatState_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentCharacter")
            {
                UpdateCharacterName();
            }
        }


        void CombatState_CharacterAdded(object sender, CombatStateCharacterEventArgs e)
        {
            e.Character.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Character_PropertyChanged);
        }

        void Character_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (((Character)sender) == _CombatState.CurrentCharacter && e.PropertyName == "HiddenName")
            {
                UpdateCharacterName();
            }
        }

        void CombatList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
        }
		
		public IInitiativeController Controller
		{
			get
			{
				return _Controller;
			}
			set
			{
				if (_Controller != value)
				{
					_Controller = value;
				}
			}
		}

		
		void UpdateCombatListState()
		{
            if (CombatList != null && PlayersComboBox != null && MonstersComboBox != null)
			{
                CombatList.ShowPlayers = UserSettings.Settings.InitiativeShowPlayers;
                CombatList.HidePlayerNames = UserSettings.Settings.InitiativeHidePlayerNames;
                CombatList.ShowMonsters = UserSettings.Settings.InitiativeShowMonsters;
                CombatList.HideMonsterNames = UserSettings.Settings.InitiativeHideMonsterNames;
                CombatList.ShowConditions = UserSettings.Settings.InitiativeShowConditions;
                CombatList.ConditionSize = UserSettings.Settings.InitiativeConditionsSizePercent;
			}
				
		}

		private void TopmostCheckbox_Checked(object sender, System.Windows.RoutedEventArgs e)
		{
			this.Topmost = (TopmostCheckbox.IsChecked == true);
            UpdateSettings();
		}

		private void IncreaseSizeButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
            _Scale += .1;
            if (_Scale > 2.0)
            {
                _Scale = 2.0;
            }
            UpdateSettings();
            AnimateToScale();
            
		}

        private void DecreaseSizeButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _Scale -= .1;
            if (_Scale < .5)
            {
                _Scale = .5;
            }
            UpdateSettings();
            AnimateToScale();

        }

        private void AnimateToScale()
        {


            DoubleAnimation animatex = new DoubleAnimation();
            DoubleAnimation animatey = new DoubleAnimation();
            animatex.To = _Scale;
            animatex.Duration = new Duration(TimeSpan.FromMilliseconds(100.0));
            animatey.To = _Scale;
            animatey.Duration = new Duration(TimeSpan.FromMilliseconds(100.0));


            RootScaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, animatex);
            RootScaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, animatey);
        }

        private void toggleButton_Checked(object sender, RoutedEventArgs e)
        {
            DoubleAnimation animate = new DoubleAnimation();
            animate.To = 180.0;
            animate.Duration = new Duration(TimeSpan.FromMilliseconds(100.0));

            LayoutRootRenderRotateTransform.BeginAnimation(RotateTransform.AngleProperty, animate);
			
            UpdateSettings();
        }

        private void toggleButton_Unchecked(object sender, RoutedEventArgs e)
        {

            DoubleAnimation animate = new DoubleAnimation();
            animate.To = 0;
            animate.Duration = new Duration(TimeSpan.FromMilliseconds(100.0));

            LayoutRootRenderRotateTransform.BeginAnimation(RotateTransform.AngleProperty, animate);
            UpdateSettings();
        }

        private void UpdateSettings()
        {
            if (_SetupComplete)
            {

                UserSettings.Settings.InitiativeScale = _Scale;

                UserSettings.Settings.InitiativeFlip = FlipButton.IsChecked == true;
                UserSettings.Settings.InitiativeShowPlayers = PlayersComboBox.SelectedIndex != 2;
                UserSettings.Settings.InitiativeShowMonsters = MonstersComboBox.SelectedIndex != 2;
                UserSettings.Settings.InitiativeHidePlayerNames = PlayersComboBox.SelectedIndex == 1;
                UserSettings.Settings.InitiativeHideMonsterNames = MonstersComboBox.SelectedIndex == 1;
                UserSettings.Settings.InitiativeAlwaysOnTop = TopmostCheckbox.IsChecked == true;
                UserSettings.Settings.InitiativeShowConditions = ShowConditionsCheckBox.IsChecked == true;
                UserSettings.Settings.InitiativeConditionsSize = ConditionSizeComboBox.SelectedIndex;

                UserSettings.Settings.SaveOptions(UserSettings.SettingsSaveSection.Initiative);
            }
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	if (DisplayControlPanel.Visibility == Visibility.Visible)
			{
				DisplayControlPanel.Visibility = Visibility.Collapsed;	
			}
			else
			{
				DisplayControlPanel.Visibility = Visibility.Visible;
			}
        }

        public bool HidingPlayerNames
        {
            get
            {
                return PlayersComboBox.SelectedIndex == 1;
            }
        }

        public bool HidingMonsterNames
        {
            get
            {
                return MonstersComboBox.SelectedIndex == 1;
            }
        }

        private void ComboBox_IndexChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateSettings();
			UpdateCombatListState();
        }

        private void ShowConditionsCheckBox_Checked(object sender, RoutedEventArgs e)
        {

            UpdateSettings();
            UpdateCombatListState();
        }

        private void ShowConditionsCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {

            UpdateSettings();
            UpdateCombatListState();

        }

        private void ConditionSizeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateSettings();
            UpdateCombatListState();
        }
        

		
	}

    public class CombatListWindowCharacterConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Character c = null;
            CombatListWindow list = null;

            foreach (object ob in values)
            {
                if (ob != null)
                {
                    if (ob.GetType() == typeof(Character))
                    {
                        c = (Character)ob;
                    }
                    else if (ob.GetType() == typeof(CombatListWindow))
                    {
                        list = (CombatListWindow)ob;
                    }
                }
            }

            if (c != null && list != null)
            {


                if (c.IsMonster ? !UserSettings.Settings.InitiativeShowMonsters : !UserSettings.Settings.InitiativeShowPlayers)
                {
                    return "";
                }

                if (c.IsMonster ? UserSettings.Settings.InitiativeHideMonsterNames : UserSettings.Settings.InitiativeHidePlayerNames)
                {
                    return "??????";
                }
                else
                {
                    return c.Name;
                }
            }

            return null;
        }


        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[targetTypes.Length];
        }



    }
}